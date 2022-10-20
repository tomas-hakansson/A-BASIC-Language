using System.Text;
using A_BASIC_Language.Gui.WinForms.PseudoGraphics;

namespace A_BASIC_Language.Gui.WinForms;

public partial class TerminalEmulator : Form
{
    private readonly CharacterRenderer _characterRenderer;
    private bool? _isActive;
    private bool FullScreen { get; set; }
    private Rectangle OldPosition { get; set; }
    private FormWindowState OldWindowState { get; set; }
    private readonly int _pixelsWidth;
    private const int PixelsHeight = 200;
    private const int CharacterWidth = 8;
    private const int CharacterHeight = 8;
    private readonly char[,] _characters;
    private readonly List<GraphicalElement> _graphicalElements;
    private int CursorX { get; set; }
    private int CursorY { get; set; }
    private bool CursorBlink { get; set; }
    private int LineInputX { get; set; }
    private int LineInputY { get; set; }
    public static Pen VectorGraphicsPen { get; }
    public bool LineInputMode { get; set; }
    public const int RowCount = 25;
    public int ColumnCount;
    public string LineInputResult { get; private set; }
    public TerminalState State { get; set; }

    static TerminalEmulator()
    {
        VectorGraphicsPen = new Pen(Color.FromArgb(100, 100, 0));
    }

#pragma warning disable CS8618 // Initializes from method.
    public TerminalEmulator()
#pragma warning restore CS8618
    {
        InitializeComponent();

        var columnCountConfigValue = System.Configuration.ConfigurationManager.AppSettings["columnCount"];

        switch (columnCountConfigValue)
        {
            case "40":
                ColumnCount = 40;
                break;
            case "80":
                ColumnCount = 80;
                break;
            default:
                ColumnCount = 60;
                break;
        }

        _pixelsWidth = ColumnCount * 8;

        FullScreen = false;
        OldWindowState = FormWindowState.Normal;
        OldPosition = new Rectangle(50, 50, 200, 200);
        _characters = new char[ColumnCount, RowCount];
        _graphicalElements = new List<GraphicalElement>();
        Clear();

        _characterRenderer = new CharacterRenderer(_characters, RowCount, ColumnCount);
    }

    public void EndLineInput()
    {
        LineInputMode = false;
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
        LineInputX = 0;
        LineInputY = 0;
        LineInputResult = "";
        CursorX = 0;
        CursorY = 0;
        LineInputMode = false;
        _graphicalElements.Clear();

        for (var y = 0; y < RowCount; y++)
            for (var x = 0; x < ColumnCount; x++)
                _characters[x, y] = ' ';
    }

    public void ShowWelcome(string program)
    {
        var spaces = 28;
        switch (ColumnCount)
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
        
        WriteLine("Ready.");
        
        if (!string.IsNullOrWhiteSpace(program))
        {
            WriteLine();
            WriteLine("Loaded program:");
            WriteLine(program);
            WriteSeparator();
        }
    }

    private void TerminalEmulator_Shown(object sender, EventArgs e)
    {
        Width = (int)(Screen.PrimaryScreen.WorkingArea.Width / 2.1);
        Height = (int)(Screen.PrimaryScreen.WorkingArea.Height / 1.8);
        CenterToScreen();

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

        CursorX = 0;
        CursorY++;

        if (CursorY >= RowCount)
        {
            CursorY = RowCount - 1;
            ScrollUp();
        }

        OperationCompleted();
    }

    public void NextTab() =>
        NextTab("");

    public void NextTab(string text)
    {
        while (CursorX%8 != 0)
            Write(" ");

        Write(text);
    }

    public void WriteSeparator()
    {
        _graphicalElements.Add(new SeparatorGraphicalElement(CursorY));
        WriteLine();
    }

    public void BeginLineInput()
    {
        LineInputX = CursorX;
        LineInputY = CursorY;
        LineInputMode = true;
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
        _characters[CursorX, CursorY] = c;

        CursorX++;

        if (CursorX < ColumnCount)
            return;

        CursorX = 0;
        CursorY++;

        if (CursorY < RowCount)
            return;

        CursorY = RowCount - 1;
        ScrollUp();
    }

    private void ScrollUp()
    {
        for (var y = 1; y < RowCount; y++)
        {
            for (var x = 0; x < ColumnCount; x++)
            {
                _characters[x, y - 1] = _characters[x, y];
            }
        }

        var lastRow = RowCount - 1;
        var zeroChar = (char)0;

        for (var x = 0; x < ColumnCount; x++)
        {
            _characters[x, lastRow] = zeroChar;
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

        e.Graphics.FillRectangle(Brushes.Black, ClientRectangle);

        foreach (var graphicalElement in _graphicalElements)
            graphicalElement.Draw(e.Graphics, CharacterWidth, CharacterHeight, ClientRectangle.Width, ClientRectangle.Height);

        _characterRenderer.Render(e.Graphics, LineInputMode, active, CursorBlink, CursorX, CursorY, LineInputX, LineInputY);

        e.Graphics.ResetTransform();
        
        e.Graphics.FillRectangle(Brushes.White, 20, 20, 20, 20);

        if (!active)
        {
            const string inactiveMessage = "This window is not active.";
            using var blueBrush = new SolidBrush(Color.FromArgb(CursorBlink ? 90 : 170, 0, 0, 200));
            e.Graphics.FillRectangle(blueBrush, 0, 18, ClientRectangle.Width, Font.Height + 2);
            e.Graphics.DrawString(inactiveMessage, Font, Brushes.White, 20, 20);
        }
    }

    private void TerminalEmulator_Resize(object sender, EventArgs e)
    {
        Invalidate();
    }

    private void TerminalEmulator_KeyDown(object sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.F11:
                ToggleFullScreen();
                break;
            case Keys.Enter:
                if (LineInputMode)
                    SaveLineInput();
                else if (State == TerminalState.Empty || State == TerminalState.Ended)
                    SaveDirectModeInput();

                CursorY++;

                if (CursorY >= RowCount)
                {
                    CursorY = RowCount - 1;
                    ScrollUp();
                }

                CursorX = 0;

                KeyDownOperationCompleted(ref e);

                if (LineInputMode)
                    LineInputMode = false;

                break;
            case Keys.Up:
                CursorY--;

                if (CursorY < 0)
                    CursorY = 0;

                KeyDownOperationCompleted(ref e);
                break;
            case Keys.Down:
                CursorY++;

                if (CursorY >= RowCount)
                {
                    CursorY = RowCount - 1;
                    ScrollUp();
                }

                KeyDownOperationCompleted(ref e);
                break;
            case Keys.Left:
                CursorLeft();
                KeyDownOperationCompleted(ref e);
                break;
            case Keys.Right:
                CursorRight();
                KeyDownOperationCompleted(ref e);
                break;
            case Keys.PageUp:
                CursorY = 0;
                KeyDownOperationCompleted(ref e);
                break;
            case Keys.PageDown:
                if (CursorY == RowCount - 1)
                    ScrollUp();
                else
                    CursorY = RowCount - 1;
                KeyDownOperationCompleted(ref e);
                break;
            case Keys.Insert:
                if (CursorX == ColumnCount - 1 && CursorY == RowCount - 1)
                {
                    _characters[CursorX, CursorY] = ' ';
                    return;
                }
                InsertCharacterAt(CursorX, CursorY);
                KeyDownOperationCompleted(ref e);
                break;
            case Keys.Delete:
                if (CursorX == ColumnCount - 1 && CursorY == RowCount - 1)
                {
                    _characters[CursorX, CursorY] = ' ';
                    return;
                }
                DeleteCharacterAt(CursorX, CursorY);
                KeyDownOperationCompleted(ref e);
                break;
            case Keys.Home:
                while (CursorX > 0)
                    CursorLeft();
                KeyDownOperationCompleted(ref e);
                break;
            case Keys.End:
                while (CursorX < ColumnCount - 1)
                    CursorRight();
                KeyDownOperationCompleted(ref e);
                break;
            case Keys.Back:
                if (CursorX <= 0 && CursorY <= 0)
                    return;

                if (LineInputMode)
                    if (LineInputY > CursorY || (LineInputY == CursorY && LineInputX >= CursorX))
                        MoveLineInputLeft();

                CursorLeft();
                DeleteCharacterAt(CursorX, CursorY);
                KeyDownOperationCompleted(ref e);
                break;
        }
    }

    public void InsertCharacterAt(int posX, int posY)
    {
        for (var y = RowCount - 1; y > posY; y--)
        {
            for (var x = ColumnCount - 1; x >= 0; x--)
            {
                _characters[x, y] = GetPreviousCharacter(x, y);
            }
        }

        for (var x = ColumnCount - 1; x > posX; x--)
        {
            _characters[x, posY] = GetPreviousCharacter(x, posY);
        }

        _characters[posX, posY] = ' ';
    }

    private void DeleteCharacterAt(int posX, int posY)
    {
        for (var x = posX; x < ColumnCount; x++)
        {
            _characters[x, posY] = GetNextCharacter(x, posY);
        }

        posY++;

        if (posY >= RowCount - 1)
            return;

        for (var y = posY; y < RowCount; y++)
        {
            for (var x = 0; x < ColumnCount; x++)
            {
                _characters[x, y] = GetNextCharacter(x, y);
            }
        }
    }

    private char GetNextCharacter(int x, int y)
    {
        if (x >= ColumnCount - 1 && y >= RowCount - 1)
            return ' ';

        x++;

        if (x >= ColumnCount && y < RowCount - 1)
        {
            x = 0;
            y++;
        }
        else if (x >= ColumnCount)
        {
            x = 0;
            y = RowCount - 1;
        }

        return _characters[x, y];
    }

    private char GetPreviousCharacter(int x, int y)
    {
        if (x <= 0 && y <= 0)
            return ' ';

        x--;

        if (x < 0 && y > 0)
        {
            x = ColumnCount - 1;
            y--;
        }
        else if (x < 0)
        {
            x = 0;
        }

        return _characters[x, y];
    }

    private void CursorLeft()
    {
        CursorX--;

        if (CursorX < 0 && CursorY > 0)
        {
            CursorX = ColumnCount - 1;
            CursorY--;
        }
        else if (CursorX < 0)
        {
            CursorX = 0;
        }
    }

    private void CursorRight()
    {
        CursorX++;

        if (CursorX >= ColumnCount && CursorY < RowCount - 1)
        {
            CursorX = 0;
            CursorY++;
        }
        else if (CursorX >= ColumnCount)
        {
            CursorX = 0;
            CursorY = RowCount - 1;
        }
    }

    private void MoveLineInputLeft()
    {
        LineInputX--;

        if (LineInputX < 0 && LineInputY > 0)
        {
            LineInputX = ColumnCount - 1;
            LineInputY--;
        }
        else if (LineInputX < 0)
        {
            LineInputX = 0;
        }
    }

    private void SaveLineInput()
    {
        var result = new StringBuilder();

        for (var y = LineInputY; y <= CursorY; y++)
        {
            if (y == LineInputY && y == CursorY) // Only one row
            {
                for (var x = LineInputX; x < CursorX; x++)
                    result.Append(_characters[x, y] == (char)0 ? " " : _characters[x, y]);
            }
            else if (y == LineInputY) // First row
            {
                for (var x = LineInputX; x < ColumnCount; x++)
                    result.Append(_characters[x, y] == (char)0 ? " " : _characters[x, y]);
            }
            else if (y == CursorY) // Last row
            {
                for (var x = 0; x < CursorX; x++)
                    result.Append(_characters[x, y] == (char)0 ? " " : _characters[x, y]);
            }
            else // Row between
            {
                for (var x = 0; x < ColumnCount; x++)
                    result.Append(_characters[x, y] == (char)0 ? " " : _characters[x, y]);
            }
        }

        LineInputResult = result.ToString();
    }

    private void SaveDirectModeInput()
    {
        var result = new StringBuilder();

        for (var x = LineInputX; x < CursorX; x++)
            result.Append(_characters[x, CursorY] == (char)0 ? " " : _characters[x, CursorY]);

        LineInputResult = result.ToString();
    }

    private void TerminalEmulator_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (!"abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.:,;-*+-/!#$€&()=".Contains(e.KeyChar.ToString()))
            return;

        _characters[CursorX, CursorY] = e.KeyChar;

        CursorX++;

        if (CursorX >= ColumnCount && CursorY < RowCount - 1)
        {
            CursorX = 0;
            CursorY++;
        }
        else if (CursorX >= ColumnCount)
        {
            CursorX = 0;
            CursorY = RowCount - 1;
        }

        e.Handled = true;
        timer1.Enabled = false;
        CursorBlink = true;
        timer1.Enabled = true;
        Invalidate();
    }

    private void TerminalEmulator_FormClosed(object sender, FormClosedEventArgs e)
    {
        var forms = Application.OpenForms.Cast<Form>().ToList();
        
        MainWindow? m = forms.OfType<MainWindow>().FirstOrDefault();

        if (m != null)
            m.Quit();
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
}