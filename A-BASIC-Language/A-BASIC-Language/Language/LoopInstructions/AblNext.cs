namespace A_BASIC_Language.Language.LoopInstructions;

public sealed class AblNext(string symbol) : AblEvalValue
{
    public string Symbol { get; } = symbol.ToUpperInvariant();
}
