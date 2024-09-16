using A_BASIC_Language.Language;

namespace A_BASIC_Language.Stage2;

public enum PrintThe
{
    EvalValues,
    LabelIndex
}

public class ParseResult
{
    public List<ABL_EvalValue> EvalValues { get; set; } = new List<ABL_EvalValue>();
    public Dictionary<int, int> LabelIndex { get; set; } = new Dictionary<int, int>();

    public string ToString(PrintThe selection)
    {
        return selection switch
        {
            PrintThe.EvalValues => string.Join(" ", EvalValues.Select(x => x.ToString())),
            PrintThe.LabelIndex => throw new NotImplementedException(),
            _ => throw new ArgumentException("You know what you did..."),
        };
    }
}