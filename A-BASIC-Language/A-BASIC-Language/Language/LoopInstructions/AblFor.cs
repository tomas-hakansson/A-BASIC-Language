namespace A_BASIC_Language.Language.LoopInstructions;

public sealed class AblFor(string symbol) : AblEvalValue
{
    public string Symbol { get; } = symbol.ToUpperInvariant();
}
