﻿namespace A_BASIC_Language.Gui.WinForms;

public class TerminalEmulatorStateStructure
{
    private readonly int _columnCount;
    private readonly int _rowCount;
    public bool LineInputMode { get; set; }
    public Point CursorPosition { get; }
    public TerminalState State { get; set; }
    public bool UserBreak { get; set; }
    public int LineInputX { get; set; }
    public int LineInputY { get; set; }

    public TerminalEmulatorStateStructure(int columnCount, int rowCount)
    {
        UserBreak = false;
        LineInputMode = false;
        CursorPosition = new Point();
        _columnCount = columnCount;
        _rowCount = rowCount;
        LineInputX = 0;
        LineInputY = 0;
    }

    public void Quit()
    {
        State = TerminalState.Ended;
    }

    public void CursorLeft() =>
        CursorPosition.MoveLeft(_columnCount);

    public void CursorRight() =>
        CursorPosition.MoveRight(_columnCount, _rowCount);
}