using System.Text;

namespace A_BASIC_Language.Gui.WinForms;

public partial class TerminalEmulator : Form
{
    private Brush OutputBrush { get; }
    private Brush InputBrush { get; }
    private Brush InputBackgroundBrush { get; }
    private const int RowCount = 25;
    private const int ColumnCount = 40;
    private const int PixelsWidth = 320;
    private const int PixelsHeight = 200;
    private const int CharacterWidth = 8;
    private const int CharacterHeight = 8;
    private readonly char[,] _characters;
    private int CursorX { get; set; }
    private int CursorY { get; set; }
    private bool CursorBlink { get; set; }
    private int LineInputX { get; set; }
    private int LineInputY { get; set; }
    public bool LineInputMode { get; set; }
    public string LineInputResult { get; private set; }

#pragma warning disable CS8618 // Initializes from method.
    public TerminalEmulator()
#pragma warning restore CS8618
    {
        InitializeComponent();
        OutputBrush = new SolidBrush(Color.FromArgb(0, 255, 0));
        InputBrush = new SolidBrush(Color.FromArgb(0, 255, 255));
        InputBackgroundBrush = new SolidBrush(Color.FromArgb(0, 42, 0));
        _characters = new char[ColumnCount, RowCount];
        Clear();
    }

    public void EndLineInput()
    {
        LineInputMode = false;
    }

    public void Clear()
    {
        LineInputX = 0;
        LineInputY = 0;
        LineInputResult = "";
        CursorX = 0;
        CursorY = 0;
        LineInputMode = false;

        for (var y = 0; y < RowCount; y++)
            for (var x = 0; x < ColumnCount; x++)
                _characters[x, y] = ' ';
    }

    public void ShowWelcome(string program)
    {
        WriteLine();
        WriteLine("        *** A BASIC LANGUAGE ***");
        WriteLine();
        WriteLine("         Altair BASIC Emulator.");

        if (string.IsNullOrWhiteSpace(program))
        {
            WriteLine("       written by Tomas Hakansson");
            WriteLine("          and Anders Hesselbom");
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
        }
    }

    private void TerminalEmulator_Shown(object sender, EventArgs e)
    {
        Font = new Font("Courier New", 6.0f);
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

        if (ClientRectangle.Width > PixelsWidth && ClientRectangle.Height > PixelsHeight)
        {
            var scaleX = ClientRectangle.Width / (double)PixelsWidth;
            var scaleY = ClientRectangle.Height / (double)PixelsHeight;
            e.Graphics.ScaleTransform((float)scaleX, (float)scaleY);
        }
        else if (ClientRectangle.Width > PixelsWidth)
        {
            var scaleX = ClientRectangle.Width / (double)PixelsWidth;
            e.Graphics.ScaleTransform((float)scaleX, 1.0f);
        }
        else if (ClientRectangle.Height > PixelsHeight)
        {
            var scaleY = ClientRectangle.Height / (double)PixelsHeight;
            e.Graphics.ScaleTransform(1.0f, (float)scaleY);
        }
        else
        {
            e.Graphics.ScaleTransform(1.0f, 1.0f);
        }

        e.Graphics.FillRectangle(Brushes.Black, ClientRectangle);

        var pixelX = 0;
        var pixelY = 0;

        if (LineInputMode)
        {
            for (var y = 0; y < RowCount; y++)
            {
                for (var x = 0; x < ColumnCount; x++)
                {
                    var isInsideInputZone = IsInsideInputZone(x, y);
                    var b = isInsideInputZone ? InputBrush : OutputBrush;

                    if (CursorBlink && CursorX == x && CursorY == y)
                    {
                        e.Graphics.FillRectangle(b, pixelX, pixelY, CharacterWidth, CharacterHeight);

                        if (_characters[x, y] > 0)
                            e.Graphics.DrawString(_characters[x, y].ToString(), Font, Brushes.Black, pixelX, pixelY);
                    }
                    else
                    {
                        if (isInsideInputZone)
                            e.Graphics.FillRectangle(InputBackgroundBrush, pixelX, pixelY, CharacterWidth, CharacterHeight);

                        if (_characters[x, y] > 0)
                            e.Graphics.DrawString(_characters[x, y].ToString(), Font, b, pixelX, pixelY);
                    }
                    pixelX += CharacterWidth;
                }
                pixelX = 0;
                pixelY += CharacterHeight;
            }
        }
        else
        {
            for (var y = 0; y < RowCount; y++)
            {
                for (var x = 0; x < ColumnCount; x++)
                {
                    if (CursorBlink && CursorX == x && CursorY == y)
                    {
                        e.Graphics.FillRectangle(OutputBrush, pixelX, pixelY, CharacterWidth, CharacterHeight);

                        if (_characters[x, y] > 0)
                            e.Graphics.DrawString(_characters[x, y].ToString(), Font, Brushes.Black, pixelX, pixelY);
                    }
                    else
                    {
                        if (_characters[x, y] > 0)
                            e.Graphics.DrawString(_characters[x, y].ToString(), Font, OutputBrush, pixelX, pixelY);
                    }
                    pixelX += CharacterWidth;
                }
                pixelX = 0;
                pixelY += CharacterHeight;
            }
        }
    }

    private bool IsInsideInputZone(int x, int y)
    {
        if (y < LineInputY)
            return false;

        if (y == LineInputY && x < LineInputX)
            return false;

        if (y > CursorY)
            return false;

        if (y == CursorY && x >= CursorX)
            return false;

        return true;
    }

    private void TerminalEmulator_Resize(object sender, EventArgs e)
    {
        Invalidate();
    }

    private void TerminalEmulator_KeyDown(object sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Enter:
                if (LineInputMode)
                    SaveLineInput();

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
}