namespace A_BASIC_Language.Language;

public sealed class ABL_For(string symbol) : ABL_EvalValue
{
    public string Symbol { get; } = symbol.ToUpperInvariant();
}

public sealed class ABL_Next(string symbol) : ABL_EvalValue
{
    public string Symbol { get; } = symbol.ToUpperInvariant();
}
