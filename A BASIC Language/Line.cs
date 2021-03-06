namespace A_BASIC_Language;

internal class Line
{
    readonly List<Token> _tokens;
    public int Count { get; private set; }

    public Line(List<Token> tokens)
    {
        _tokens = tokens;
        Count = tokens.Count;
    }

    public Token this[int index]
    {
        get { return _tokens[index]; }
        set { _tokens[index] = value; }
    }
}