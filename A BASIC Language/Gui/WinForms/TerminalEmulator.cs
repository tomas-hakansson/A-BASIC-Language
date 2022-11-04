using System.Text;
using A_BASIC_Language.Gui.WinForms.PseudoGraphics;
using A_BASIC_Language.IO;
using CharacterMatrix;

namespace A_BASIC_Language.Gui.WinForms;

public partial class TerminalEmulator : Form
{
    private readonly TerminalEmulatorStateStructure _ts;
    private string SourceCode { get; set; }
    private delegate void DirectInputHandlerDelegate(string command);
    private readonly CharacterRenderer _characterRenderer;
    private readonly OverlayRenderer _overlayRenderer;
    private readonly KeyboardController _keyboardController;
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
    public static ProgramRepository ProgramRepository { get; }
    public string ProgramFilename { get; set; }
    public static Pen VectorGraphicsPen { get; }
    public string LineInputResult { get; private set; }
    private readonly Matrix _characters;

    static TerminalEmulator()
    {
        VectorGraphicsPen = new Pen(Color.FromArgb(100, 100, 0));
        ProgramRepository = new ProgramRepository();
    }

#pragma warning disable CS8618 // Initializes from method.
    public TerminalEmulator()
#pragma warning restore CS8618
    {
        InitializeComponent();
        SourceCode = "";
        ProgramFilename = "";

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

        _pixelsWidth = columnCount * 8;

        _characterRenderer = new CharacterRenderer(_characters);
        _overlayRenderer = new OverlayRenderer(imageList1);
        _keyboardController = new KeyboardController(_characters, KeyDownOperationCompleted, _ts, ToggleFullScreen, ScrollUp, SaveLineInput, SaveDirectModeInput, MoveLineInputLeft);
    }

    public void EndLineInput()
    {
        _ts.LineInputMode = false;
    }

    public bool IsFullScreen() =>
        FullScreen;

    private void ToggleFullScreen() =>
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
        Screen.FromPoint(new Point(Left + Width / 2, Top + Height / 2));

    public void Clear()
    {
        _ts.LineInputX = 0;
        _ts.LineInputY = 0;
        LineInputResult = "";
        _ts.CursorX = 0;
        _ts.CursorY = 0;
        _ts.LineInputMode = false;
        _graphicalElements.Clear();
        _characters.Clear();
    }

    public void ShowWelcome(string program)
    {
        var spaces = 28;
        switch (_characters.ColumnCount)
        {
            case 40:
                spaces = 8;
                break;
            case 60:
                spaces = 18;
                break;
        }

        WriteLine();
        WriteLine($"{new string(' ', spaces)}*** A BASIC LANGUAGE ***");
        WriteLine();
        WriteLine($"{new string(' ', spaces + 1)}Altair BASIC Emulator.");

        if (string.IsNullOrWhiteSpace(program))
        {
            WriteLine($"{new string(' ', spaces - 1)}written by Tomas Hakansson");
            WriteLine($"{new string(' ', spaces + 2)}and Anders Hesselbom");
            WriteLine();
        }
        else
        {
            WriteLine();
        }
        
        if (!string.IsNullOrWhiteSpace(program))
        {
            WriteLine("Ready.");
            WriteLine();
            WriteLine("Loaded program:");
            WriteLine(program);
            WriteSeparator();
        }
        else
        {
            WriteLine("Ready. Type LOAD or QUIT.");
        }
    }

    private void TerminalEmulator_Shown(object sender, EventArgs e)
    {
        FullScreen = false;
        OldWindowState = FormWindowState.Normal;
        OldPosition = new Rectangle(50, 50, 200, 200);

        Invalidate();
        timer1.Enabled = true;
    }

    public void Write(string text)
    {
        if (text == null!)
            return;

        foreach (var c in text)
            WriteCharAndProgress(c);

        OperationCompleted();
    }

    public void WriteLine() =>
        WriteLine("");

    public void WriteLine(string text)
    {
        if (text == null!)
            return;

        foreach (var c in text)
            WriteCharAndProgress(c);

        _ts.CursorX = 0;
        _ts.CursorY++;

        if (_ts.CursorY >= _characters.RowCount)
        {
            _ts.CursorY = _characters.RowCount - 1;
            ScrollUp();
        }

        OperationCompleted();
    }

    public void NextTab() =>
        NextTab("");

    public void NextTab(string text)
    {
        while (_ts.CursorX%8 != 0)
            Write(" ");

        Write(text);
    }

    public void WriteSeparator()
    {
        _graphicalElements.Add(new SeparatorGraphicalElement(_characters.RowCount, _ts.CursorY));
        WriteLine();
    }

    public void BeginLineInput()
    {
        _ts.LineInputX = _ts.CursorX;
        _ts.LineInputY = _ts.CursorY;
        _ts.LineInputMode = true;
    }

    private void OperationCompleted()
    {
        timer1.Enabled = false;
        CursorBlink = true;
        timer1.Enabled = true;
        Invalidate();
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
        _characters.SetAt(_ts.CursorX, _ts.CursorY, c);

        _ts.CursorX++;

        if (_ts.CursorX < _characters.ColumnCount)
            return;

        _ts.CursorX = 0;
        _ts.CursorY++;

        if (_ts.CursorY < _characters.RowCount)
            return;

        _ts.CursorY = _characters.RowCount - 1;
        ScrollUp();
    }

    private void ScrollUp()
    {
        _characters.ScrollUp();

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

        e.Graphics.FillRectangle(Brushes.Black, ClientRectangle);

        foreach (var graphicalElement in _graphicalElements)
            graphicalElement.Draw(e.Graphics, CharacterWidth, CharacterHeight, ClientRectangle.Width, ClientRectangle.Height);

        _characterRenderer.Render(e.Graphics, _ts.LineInputMode, active, CursorBlink, _ts.CursorX, _ts.CursorY, _ts.LineInputX, _ts.LineInputY);

        e.Graphics.ResetTransform();
        
        _overlayRenderer.Render(e.Graphics, active, CursorBlink, ClientRectangle, _ts.State);
    }

    private void TerminalEmulator_Resize(object sender, EventArgs e)
    {
        Invalidate();
    }

    private void TerminalEmulator_KeyDown(object sender, KeyEventArgs e) =>
        _keyboardController.HandleKeyDown(e, _ts);

    private void MoveLineInputLeft()
    {
        _ts.LineInputX--;

        if (_ts.LineInputX < 0 && _ts.LineInputY > 0)
        {
            _ts.LineInputX = _characters.ColumnCount - 1;
            _ts.LineInputY--;
        }
        else if (_ts.LineInputX < 0)
        {
            _ts.LineInputX = 0;
        }
    }

    private void SaveLineInput()
    {
        var result = new StringBuilder();

        for (var y = _ts.LineInputY; y <= _ts.CursorY; y++)
        {
            if (y == _ts.LineInputY && y == _ts.CursorY) // Only one row
            {
                for (var x = _ts.LineInputX; x < _ts.CursorX; x++)
                    result.Append(_characters.GetAt(x, y) == (char)0 ? " " : _characters.GetAt(x, y));
            }
            else if (y == _ts.LineInputY) // First row
            {
                for (var x = _ts.LineInputX; x < _characters.ColumnCount; x++)
                    result.Append(_characters.GetAt(x, y) == (char)0 ? " " : _characters.GetAt(x, y));
            }
            else if (y == _ts.CursorY) // Last row
            {
                for (var x = 0; x < _ts.CursorX; x++)
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

        for (var x = 0; x < _ts.CursorX; x++)
            result.Append(_characters.GetAt(x, _ts.CursorY) == (char)0 ? " " : _characters.GetAt(x, _ts.CursorY));

        var directInput = result.ToString();

        var handler = new DirectInputHandlerDelegate(DirectInputHandler);

        Task.Run(() => handler(directInput));
    }

    private void DirectInputHandler(string command)
    {
        Thread.Sleep(50);
        Invoke(CarryOutSimpleImmediateCommand, command);
    }

    private void CarryOutSimpleImmediateCommand(string command)
    {
        switch (command.Trim().ToUpper())
        {
            case "RESTART":
                if (_ts.State == TerminalState.Ended)
                {
                    _ts.State = TerminalState.Ended;
                    Application.DoEvents();
                    Run(false);
                }
                else
                {
                    WriteLine("Invalid state for restart.");
                }
                break;
            case "SOURCE":
                if (_ts.State == TerminalState.Ended)
                {
                    using var x = new SourceDialog();
                    x.Filename = ProgramFilename;
                    x.SourceCode = SourceCode;
                    x.ShowDialog(this);
                }
                else
                {
                    WriteLine("Invalid state for source.");
                }
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
                WriteLine("Invalid simple direct mode input.");
                break;
        }
    }

    public void Run(bool clear)
    {
        Cursor = Cursors.WaitCursor;

        var source = ProgramRepository.GetProgram(this, ProgramFilename, out var nameOnly);

        if (string.IsNullOrWhiteSpace(source))
        {
            Terminal.WriteLine("Load failed.");
            return;
        }

        SourceCode = source;

        Interpreter eval = new(SourceCode);
        Cursor = Cursors.Default;
        Terminal.Run(nameOnly, ProgramFilename, clear);
        eval.Run(Terminal);
    }

    public void ShowEmptyTerminal()
    {
        Console.WriteLine(_ts.State);
        Interpreter eval = new(SourceCode);
        Terminal.Run("A BASIC Language", "", true);
        eval.Run(Terminal);
    }

    private void TerminalEmulator_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (!"abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.:,;-*+-/!#$€&()=".Contains(e.KeyChar.ToString()))
            return;

        _characters.SetAt(_ts.CursorX, _ts.CursorY, e.KeyChar);

        _ts.CursorX++;

        if (_ts.CursorX >= _characters.ColumnCount && _ts.CursorY < _characters.RowCount - 1)
        {
            _ts.CursorX = 0;
            _ts.CursorY++;
        }
        else if (_ts.CursorX >= _characters.ColumnCount)
        {
            _ts.CursorX = 0;
            _ts.CursorY = _characters.RowCount - 1;
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
        _ts.Quit();
    }
}