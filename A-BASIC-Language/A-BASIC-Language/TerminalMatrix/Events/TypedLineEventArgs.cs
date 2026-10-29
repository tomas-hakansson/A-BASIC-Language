#nullable enable
using System;

namespace A_BASIC_Language.TerminalMatrix.Events;

public class TypedLineEventArgs : EventArgs
{
    public string InputValue { get; }

    public TypedLineEventArgs(string inputValue)
    {
        InputValue = inputValue;
    }
}
