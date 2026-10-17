namespace A_BASIC_Language.Language.LoopInstructions;

public sealed class AblFor(string symbol) : ABL_EvalValue
{
    public string Symbol { get; } = symbol.ToUpperInvariant();
}
