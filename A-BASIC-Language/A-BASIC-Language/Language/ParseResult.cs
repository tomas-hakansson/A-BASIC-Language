using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using A_BASIC_Language.Language.ValueTypes;

namespace A_BASIC_Language.Language;

public class ParseResult
{
    public List<string> Errors { get; } = [];
    public bool Success { get; set; } = false;
    public InstructionList EvalValues { get; set; } = [];
    public List<ValueBase> DataValues { get; } = [];
    public Dictionary<int, int> LabelIndex { get; set; } = new();
    public IReadOnlyDictionary<string, int> NamedLabels { get; internal set; } =
        new ReadOnlyDictionary<string, int>(new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase));

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
