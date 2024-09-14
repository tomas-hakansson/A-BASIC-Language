namespace A_BASIC_Language.Language;

public class ParseResult
{
    public bool Success { get; set; } = false;
    public List<ABL_EvalValue> EvalValues { get; set; } = new List<ABL_EvalValue>();
    public Dictionary<int, int> LabelIndex { get; set; } = new Dictionary<int, int>();

    public string ToString(PrintThe selection)
    {
        return selection switch
        {
            PrintThe.EvalValues => string.Join(" ", EvalValues.Select(x => x.ToString())),
            PrintThe.LabelIndex => string.Join(" ++ ", LabelIndex.OrderBy(x => x.Key).Select(li => $"({li.Key}, {li.Value})")),
            _ => throw new ArgumentException("You know what you did..."),
        };
    }
}