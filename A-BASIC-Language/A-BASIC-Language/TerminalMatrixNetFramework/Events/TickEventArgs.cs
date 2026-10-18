using System;

namespace TerminalMatrixNetFramework.Events;

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
