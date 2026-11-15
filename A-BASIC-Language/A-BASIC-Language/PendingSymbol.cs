#nullable enable
namespace A_BASIC_Language;

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
