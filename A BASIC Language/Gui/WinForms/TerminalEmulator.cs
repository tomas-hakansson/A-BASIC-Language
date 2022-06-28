namespace A_BASIC_Language.Gui.WinForms;

public partial class TerminalEmulator : Form
{
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
    private bool LineInputMode { get; set; }

    public TerminalEmulator()
    {
        InitializeComponent();
        _characters = new char[ColumnCount, RowCount];
        CursorX = 0;
        CursorY = 0;
        LineInputMode = false;
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
        for (var y = RowCount - 1; y > 0; y--)
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

        for (var y = 0; y < RowCount; y++)
        {
            for (var x = 0; x < ColumnCount; x++)
            {
                if (CursorBlink && CursorX == x && CursorY == y)
                {
                    e.Graphics.FillRectangle(Brushes.Green, pixelX, pixelY, CharacterWidth, CharacterHeight);

                    if (_characters[x, y] > 0)
                        e.Graphics.DrawString(_characters[x, y].ToString(), Font, Brushes.Black, pixelX, pixelY);
                }
                else
                {
                    if (_characters[x, y] > 0)
                        e.Graphics.DrawString(_characters[x, y].ToString(), Font, Brushes.Green, pixelX, pixelY);
                }
                pixelX += CharacterWidth;
            }
            pixelX = 0;
            pixelY += CharacterHeight;
        }
    }

    private void TerminalEmulator_Resize(object sender, EventArgs e)
    {
        Invalidate();
    }

    private void TerminalEmulator_KeyDown(object sender, KeyEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine(e.KeyCode);
        switch (e.KeyCode)
        {
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

                KeyDownOperationCompleted(ref e);
                break;
            case Keys.Right:
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

                KeyDownOperationCompleted(ref e);
                break;
        }
    }

    private void TerminalEmulator_KeyPress(object sender, KeyPressEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine((int)e.KeyChar);
    }
}