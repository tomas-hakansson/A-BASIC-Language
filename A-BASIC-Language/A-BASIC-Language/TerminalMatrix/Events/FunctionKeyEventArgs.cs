using System;
using A_BASIC_Language.TerminalMatrix.Events;

namespace TerminalMatrixNetFramework.Events;

public class FunctionKeyEventArgs : EventArgs
{
    public FunctionKey Key { get; }

    public FunctionKeyEventArgs(FunctionKey key)
    {
        Key = key;
    }
}
