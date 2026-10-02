#nullable enable
using System.Collections.Generic;

namespace A_BASIC_Language;

public class PendingSymbolsStack : Stack<(int, string)>
{
    public bool TryPop(out PendingSymbol value)
    {
        if (Count > 0)
        {
            var result = Pop();
            value = new PendingSymbol(result.Item1, result.Item2);
            return true;
        }

        value = PendingSymbol.Empty;
        return false;
    }
}

public class PendingSymbol
{
    public int Index { get; }
    public string Symbol { get; }

    public PendingSymbol(int index, string symbol)
    {
        Index = index;
        Symbol = symbol;
    }

    public static PendingSymbol Empty =>
        new(-1, string.Empty);
}
