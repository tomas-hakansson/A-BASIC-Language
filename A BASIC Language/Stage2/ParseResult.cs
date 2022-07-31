namespace A_BASIC_Language.Stage2;

public class ParseResult
{
    public List<ABL_EvalValue> EvalValues { get; set; } = new List<ABL_EvalValue>();
    public Dictionary<int, int> LabelIndex { get; set; } = new Dictionary<int, int>();
}