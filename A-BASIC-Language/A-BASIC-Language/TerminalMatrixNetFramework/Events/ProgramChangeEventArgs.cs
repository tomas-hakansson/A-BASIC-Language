using System;

namespace TerminalMatrixNetFramework.Events;

public class ProgramChangeEventArgs : EventArgs
{
    public bool IsAdd { get; }
    public bool IsOverwrite { get; }
    public bool IsDelete { get; }

    public ProgramChangeEventArgs(bool isAdd, bool isOverwrite, bool isDelete)
    {
        IsAdd = isAdd;
        IsOverwrite = isOverwrite;
        IsDelete = isDelete;
    }
}
