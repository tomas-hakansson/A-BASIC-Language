namespace A_BASIC_Language.Gui.WinForms;

public class TerminalEmulatorStateStructure
{
    private readonly int _columnCount;
    private readonly int _rowCount;
    public bool LineInputMode { get; set; }
    public int CursorX { get; set; }
    public int CursorY { get; set; }
    public TerminalState State { get; set; }
    public int LineInputX { get; set; }
    public int LineInputY { get; set; }

    public TerminalEmulatorStateStructure(int columnCount, int rowCount)
    {
        LineInputMode = false;
        CursorX = 0;
        CursorY = 0;
        _columnCount = columnCount;
        _rowCount = rowCount;
        LineInputX = 0;
        LineInputY = 0;
    }

    public void Quit()
    {
        State = TerminalState.Ended;
    }

    public void CursorLeft()
    {
        CursorX--;

        if (CursorX < 0 && CursorY > 0)
        {
            CursorX = _columnCount - 1;
            CursorY--;
        }
        else if (CursorX < 0)
        {
            CursorX = 0;
        }
    }

    public void CursorRight()
    {
        CursorX++;

        if (CursorX >= _columnCount && CursorY < _rowCount - 1)
        {
            CursorX = 0;
            CursorY++;
        }
        else if (CursorX >= _columnCount)
        {
            CursorX = 0;
            CursorY = _rowCount - 1;
        }
    }
}