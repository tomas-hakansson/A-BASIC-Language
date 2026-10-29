using System;
using System.Collections.Generic;
using System.Linq;

namespace A_BASIC_Language.Language;

public class ParseResult
{
    public List<string> Errors { get; } = [];
    public bool Success { get; set; } = false;
    public List<AblEvalValue> EvalValues { get; set; } = [];
    public Dictionary<int, int> LabelIndex { get; set; } = new();

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
