namespace A_BASIC_Language;

internal class Line
{
    readonly List<ABL_EvalValue> _tokens;
    public int Count { get; private set; }

    public Line(List<ABL_EvalValue> tokens)
    {
        _tokens = tokens;
        Count = tokens.Count;
    }

    public ABL_EvalValue this[int index]
    {
        get { return _tokens[index]; }
        set { _tokens[index] = value; }
    }
}