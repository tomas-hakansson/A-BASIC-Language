using System;
using System.Collections.Generic;
using System.Linq;

namespace A_BASIC_Language.Language.Stage2;

public enum PrintThe
{
    EvalValues,
    LabelIndex
}

public class ParseResult
{
    public List<AblEvalValue> EvalValues { get; set; } = new();
    public Dictionary<int, int> LabelIndex { get; set; } = new();

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
