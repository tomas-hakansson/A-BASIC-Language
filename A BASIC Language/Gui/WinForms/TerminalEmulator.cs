using System.Text;
using A_BASIC_Language.Gui.WinForms.PseudoGraphics;
using A_BASIC_Language.Gui.WinForms.TerminalSubControllers;
using A_BASIC_Language.IO;
using CharacterMatrix;
using ConsoleControlLibrary;

namespace A_BASIC_Language.Gui.WinForms;

public partial class TerminalEmulator : Form
{
    private delegate void DirectInputHandlerDelegate(string command);
    private readonly TerminalEmulatorStateStructure _ts;
    private string SourceCode { get; set; }
    private readonly CharacterRenderer _characterRenderer;
    private readonly OverlayRenderer _overlayRenderer;
    private readonly KeyboardController _keyboardController;
    private readonly ConsoleControl _consoleControl;
    private readonly ConsoleHook _consoleHook;
    private bool? _isActive;
    private bool FullScreen { get; set; }
    private Rectangle OldPosition { get; set; }
    private FormWindowState OldWindowState { get; set; }
    private readonly int _pixelsWidth;
    private const int PixelsHeight = 200;
    private const int CharacterWidth = 8;
    private const int CharacterHeight = 8;
    private readonly List<GraphicalElement> _graphicalElements;
    private bool CursorBlink { get; set; }
    private Terminal Terminal { get; }
    private readonly Matrix _characters;
    public static ProgramRepository ProgramRepository { get; }
    public string ProgramFilename { get; set; }
    public static Pen VectorGraphicsPen { get; }
    public string LineInputResult { get; private set; }
    public bool QuitFlag { get; set; }

    static TerminalEmulator()
    {
        VectorGraphicsPen = new Pen(Configuration.PrimaryVectorGraphicsColor);
        ProgramRepository = new ProgramRepository();
    }

    public TerminalEmulator()
    {
        InitializeComponent();
        LineInputResult = "";
        SourceCode = "";
        ProgramFilename = "";

        _consoleControl = new ConsoleControl();
        _consoleHook = new ConsoleHook(_consoleControl, this, tmrRestoreForm);

        var columnCountConfigValue = System.Configuration.ConfigurationManager.AppSettings["columnCount"];
        var columnCount = 60;

        switch (columnCountConfigValue)
        {
            case "40":
                columnCount = 40;
                break;
            case "80":
                columnCount = 80;
                break;
        }

        _ts = new TerminalEmulatorStateStructure(columnCount, 25);
        Terminal = new Terminal(this, _ts);

        _characters = new Matrix(columnCount);
        _graphicalElements = new List<GraphicalElement>();
        Clear();

        _pixelsWidth = columnCount * CharacterWidth;

        _characterRenderer = new CharacterRenderer(_characters);
        _overlayRenderer = new OverlayRenderer(imageList1);
        _keyboardController = new KeyboardController(_characters, KeyDownOperationCompleted, _ts, ToggleFullScreen, ScrollUp, SaveLineInput, SaveDirectModeInput, MoveLineInputLeft, Terminal.End);
    }

    private void TerminalEmulator_Load(object sender, EventArgs e)
    {
        var marginX = Size.Width - ClientRectangle.Width;
        var marginY = Size.Height - ClientRectangle.Height;

        var width = _pixelsWidth + marginX;
        var height = 25 * CharacterHeight + marginY;
        MinimumSize = new Size(width, height);
        _consoleHook.Hook(Handle, Controls, _ts);
    }

    public void EndLineInput() =>
        _ts.LineInputMode = false;

    public bool IsFullScreen() =>
        FullScreen;

    internal void ToggleFullScreen() =>
        SetFullScreen(!FullScreen);

    public void SetFullScreen(bool fullScreen)
    {
        if (fullScreen & !FullScreen)
            DoSetFullScreen();
        else if (!fullScreen & FullScreen)
            DoSetWindowMode();
    }

    private void DoSetFullScreen()
    {
        FullScreen = true;
        var s = GetScreen();
        OldWindowState = WindowState;
        OldPosition = new Rectangle(Left, Top, Width, Height);
        WindowState = FormWindowState.Normal;
        FormBorderStyle = FormBorderStyle.None;
        TopMost = true;
        Top = s.Bounds.Top;
        Left = s.Bounds.Left;
        Width = s.Bounds.Width;
        Height = s.Bounds.Height;
        Focus();
    }

    private void DoSetWindowMode()
    {
        FullScreen = false;
        FormBorderStyle = FormBorderStyle.Sizable;
        WindowState = OldWindowState;
        Width = OldPosition.Width;
        Height = OldPosition.Height;
        Top = OldPosition.Top;
        Left = OldPosition.Left;
        TopMost = false;
        Focus();
    }

    private Screen GetScreen() =>
        Screen.FromPoint(new System.Drawing.Point(Left + Width / 2, Top + Height / 2));

    public void Clear()
    {
        _ts.LineInputPosition.Clear();
        LineInputResult = "";
        _ts.CursorPosition.Clear();
        _ts.LineInputMode = false;
        _graphicalElements.Clear();
        _characters.Clear();
    }

    private void TerminalEmulator_Shown(object sender, EventArgs e)
    {
        FullScreen = false;
        OldWindowState = FormWindowState.Normal;
        OldPosition = new Rectangle(50, 50, 200, 200);
        Invalidate();
        timer1.Enabled = true;
    }

    public void ShowWelcome(string programFilename) =>
        new WelcomeScreen(_characters.ColumnCount, WriteLine, WriteLine, WriteSeparator).Show(programFilename);

    public async Task Write(string text)
    {
        if (text == null!)
            return;

        foreach (var c in text)
            WriteCharAndProgress(c);

        await OperationCompleted();
    }

    public async Task WriteLine() =>
        await WriteLine("");

    public async Task WriteLine(string text)
    {
        if (text == null!)
            return;

        foreach (var c in text)
            WriteCharAndProgress(c);

        _ts.CursorPosition.X = 0;
        _ts.CursorPosition.Y++;

        if (_ts.CursorPosition.Y >= _characters.RowCount)
        {
            _ts.CursorPosition.Y = _characters.RowCount - 1;
            ScrollUp();
        }

        await OperationCompleted();
    }

    public async Task NextTab() =>
        await NextTab("");

    public async Task NextTab(string text)
    {
        while (_ts.CursorPosition.X % 8 != 0)
            await Write(" ");

        await Write(text);
    }

    public async Task WriteSeparator()
    {
        _graphicalElements.Add(new SeparatorGraphicalElement(_characters.RowCount, _ts.CursorPosition.Y));
        await WriteLine();
    }

    public void BeginLineInput()
    {
        _ts.LineInputPosition.Set(_ts.CursorPosition);
        _ts.LineInputMode = true;
    }

    private async Task OperationCompleted()
    {
        timer1.Enabled = false;
        CursorBlink = true;

        if (QuitFlag)
            return;

        timer1.Enabled = true;
        Invalidate();
        Refresh();
        await Task.Run(() => Thread.Sleep(5));
    }

    private void KeyDownOperationCompleted(ref KeyEventArgs eventArgs)
    {
        eventArgs.SuppressKeyPress = true;
        eventArgs.Handled = true;
        timer1.Enabled = false;
        CursorBlink = true;
        timer1.Enabled = true;
        Invalidate();
    }

    private void WriteCharAndProgress(char c)
    {
        _characters.SetAt(_ts.CursorPosition.X, _ts.CursorPosition.Y, c);

        _ts.CursorPosition.X++;

        if (_ts.CursorPosition.X < _characters.ColumnCount)
            return;

        _ts.CursorPosition.X = 0;
        _ts.CursorPosition.Y++;

        if (_ts.CursorPosition.Y < _characters.RowCount)
            return;

        _ts.CursorPosition.Y = _characters.RowCount - 1;
        ScrollUp();
    }

    private void ScrollUp()
    {
        _characters.ScrollUp();

        if (_ts.LineInputMode)
        {
            _ts.LineInputPosition.Y--;

            if (_ts.LineInputPosition.Y < 0)
                _ts.LineInputPosition.Y = 0;
        }

        foreach (var graphicalElement in _graphicalElements)
            graphicalElement.ScrollUp();

        bool again;

        do
        {
            again = false;

            foreach (var graphicalElement in _graphicalElements.Where(graphicalElement => !graphicalElement.Visible))
            {
                _graphicalElements.Remove(graphicalElement);
                again = true;
                break;
            }
        } while (again);
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
        CursorBlink = !CursorBlink;
        Invalidate();
    }

    private void TerminalEmulator_Paint(object sender, PaintEventArgs e)
    {
        if (ClientRectangle.Width < 10 || ClientRectangle.Height < 10)
            return;

        var active = _isActive ?? true;

        float scaleX, scaleY;

        if (ClientRectangle.Width > _pixelsWidth && ClientRectangle.Height > PixelsHeight)
        {
            scaleX = (float)(ClientRectangle.Width / (double)_pixelsWidth);
            scaleY = (float)(ClientRectangle.Height / (double)PixelsHeight);
        }
        else if (ClientRectangle.Width > _pixelsWidth)
        {
            scaleX = (float)(ClientRectangle.Width / (double)_pixelsWidth);
            scaleY = 1f;
        }
        else if (ClientRectangle.Height > PixelsHeight)
        {
            scaleX = 1f;
            scaleY = (float)(ClientRectangle.Height / (double)PixelsHeight);
        }
        else
        {
            scaleX = 1f;
            scaleY = 1f;
        }

        e.Graphics.ScaleTransform(scaleX, scaleY);

        using var backgroundColor = new SolidBrush(Configuration.EmulatorBackgroundColor);
        e.Graphics.FillRectangle(backgroundColor, ClientRectangle);

        foreach (var graphicalElement in _graphicalElements)
            graphicalElement.Draw(e.Graphics, CharacterWidth, CharacterHeight, ClientRectangle.Width, ClientRectangle.Height);

        _characterRenderer.Render(e.Graphics, backgroundColor, _ts.LineInputMode, active, CursorBlink, _ts.CursorPosition, _ts.LineInputPosition.X, _ts.LineInputPosition.Y);

        e.Graphics.ResetTransform();

        _overlayRenderer.Render(e.Graphics, active, CursorBlink, ClientRectangle, _ts.State);
    }

    private void TerminalEmulator_Resize(object sender, EventArgs e) =>
        Invalidate();

    private async void TerminalEmulator_KeyDown(object sender, KeyEventArgs e)
    {
        var breakFlag = _keyboardController.HandleKeyDown(e, _ts);

        if (!breakFlag)
            return;

        if (_ts.CursorPosition.X > 0)
            await WriteLine();

        Console.WriteLine(_ts.LineInputMode);

        await WriteLine("?User break.");
    }

    private void MoveLineInputLeft() =>
        _ts.LineInputPosition.MoveLeft(_characters.ColumnCount);

    private void SaveLineInput()
    {
        var result = new StringBuilder();

        for (var y = _ts.LineInputPosition.Y; y <= _ts.CursorPosition.Y; y++)
        {
            if (y == _ts.LineInputPosition.Y && y == _ts.CursorPosition.Y) // Only one row
            {
                for (var x = _ts.LineInputPosition.X; x < _ts.CursorPosition.X; x++)
                    result.Append(_characters.GetAt(x, y) == (char)0 ? " " : _characters.GetAt(x, y));
            }
            else if (y == _ts.LineInputPosition.Y) // First row
            {
                for (var x = _ts.LineInputPosition.X; x < _characters.ColumnCount; x++)
                    result.Append(_characters.GetAt(x, y) == (char)0 ? " " : _characters.GetAt(x, y));
            }
            else if (y == _ts.CursorPosition.Y) // Last row
            {
                for (var x = 0; x < _ts.CursorPosition.X; x++)
                    result.Append(_characters.GetAt(x, y) == (char)0 ? " " : _characters.GetAt(x, y));
            }
            else // Row between
            {
                for (var x = 0; x < _characters.ColumnCount; x++)
                    result.Append(_characters.GetAt(x, y) == (char)0 ? " " : _characters.GetAt(x, y));
            }
        }

        LineInputResult = result.ToString();
    }

    private void SaveDirectModeInput()
    {
        var result = new StringBuilder();

        for (var x = 0; x < _ts.CursorPosition.X; x++)
            result.Append(_characters.GetAt(x, _ts.CursorPosition.Y) == (char)0 ? " " : _characters.GetAt(x, _ts.CursorPosition.Y));

        var directInput = result.ToString();

        var handler = new DirectInputHandlerDelegate(DirectInputHandler);

        Task.Run(() => handler(directInput));
    }

    private void DirectInputHandler(string command)
    {
        Thread.Sleep(50);
        Invoke(CarryOutSimpleImmediateCommand, command);
    }

    private async Task CarryOutSimpleImmediateCommand(string command)
    {
        switch (command.Trim().ToUpper())
        {
            case "":
                break;
            case "RESTART":
                if (_ts.State == TerminalState.Ended)
                {
                    _ts.State = TerminalState.Ended;
                    Application.DoEvents();
                    Run(false);
                }
                else
                {
                    await WriteLine("Invalid state for restart.");
                }
                break;
            case "SOURCE":
                if (_ts.State == TerminalState.Ended)
                    _consoleHook.ShowSourceForm(SourceCode, ProgramFilename);
                else
                    await WriteLine("Invalid state for source.");
                break;
            case "LOAD":
                {
                    using var x = new LoadProgramDialog();

                    if (x.ShowDialog(this) != DialogResult.OK)
                        return;

                    var f = x.Filename ?? "";

                    if (f.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase) || f.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase))
                        ProgramFilename = f;
                    else
                        ProgramFilename = Path.GetFullPath(x.Filename!);

                    Run(false);
                }
                break;
            case "QUIT":
                _ts.State = TerminalState.Ended;
                Application.DoEvents();
                Close();
                break;
            default:
                await WriteLine("Invalid simple direct mode input.");
                break;
        }
    }

    public async void Run(bool clear)
    {
        Cursor = Cursors.WaitCursor;

        var source = await ProgramRepository.GetProgram(this, ProgramFilename);

        if (string.IsNullOrWhiteSpace(source.SourceCode))
        {
            await Terminal.WriteLine("Load failed.");
            return;
        }

        SourceCode = source.SourceCode;

        Interpreter eval = new(SourceCode);
        Cursor = Cursors.Default;
        Terminal.Run(source.Filename, ProgramFilename, clear);
        await eval.Run(Terminal);
    }

    public async Task ShowEmptyTerminal()
    {
        Console.WriteLine(_ts.State);
        Interpreter eval = new(SourceCode);
        Terminal.Run("A BASIC Language", "", true);
        await eval.Run(Terminal);
    }

    private void TerminalEmulator_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (!"abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.:,;-*+-/!#$€&()=".Contains(e.KeyChar.ToString()))
            return;

        _characters.SetAt(_ts.CursorPosition.X, _ts.CursorPosition.Y, e.KeyChar);

        _ts.CursorPosition.X++;

        if (_ts.CursorPosition.X >= _characters.ColumnCount && _ts.CursorPosition.Y < _characters.RowCount - 1)
        {
            _ts.CursorPosition.X = 0;
            _ts.CursorPosition.Y++;
        }
        else if (_ts.CursorPosition.X >= _characters.ColumnCount)
        {
            _ts.CursorPosition.X = 0;
            _ts.CursorPosition.Y = _characters.RowCount - 1;
        }

        e.Handled = true;
        timer1.Enabled = false;
        CursorBlink = true;
        timer1.Enabled = true;
        Invalidate();
    }

    private void TerminalEmulator_Activated(object sender, EventArgs e)
    {
        _isActive = true;
        Invalidate();
    }

    private void TerminalEmulator_Deactivate(object sender, EventArgs e)
    {
        _isActive = false;
        Invalidate();
    }

    private void TerminalEmulator_FormClosed(object sender, FormClosedEventArgs e)
    {
        QuitFlag = true;
        _ts.Quit();
    }
}