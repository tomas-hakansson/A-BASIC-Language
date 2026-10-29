using System;

namespace A_BASIC_Language.TerminalMatrix.Events;

public class TickEventArgs : EventArgs
{
    public int CursorX { get; }
    public int CursorY { get; }

    internal TickEventArgs(int cursorX, int cursorY)
    {
        CursorX = cursorX;
        CursorY = cursorY;
    }
}
