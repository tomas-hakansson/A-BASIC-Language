namespace A_BASIC_Language.Gui.WinForms;

public class TerminalEmulatorStateStructure
{
    public int ColumnCount { get; }
    public int RowCount { get; }
    public bool LineInputMode { get; set; }
    public Point CursorPosition { get; }
    public TerminalState State { get; set; }
    public bool UserBreak { get; set; }
    public Point LineInputPosition { get; }

    public TerminalEmulatorStateStructure(int columnCount, int rowCount)
    {
        UserBreak = false;
        LineInputMode = false;
        CursorPosition = new Point();
        ColumnCount = columnCount;
        RowCount = rowCount;
        LineInputPosition = new Point();
    }

    public void Quit()
    {
        State = TerminalState.Ended;
    }

    public void CursorLeft() =>
        CursorPosition.MoveLeft(ColumnCount);

    public void CursorRight() =>
        CursorPosition.MoveRight(ColumnCount, RowCount);
}