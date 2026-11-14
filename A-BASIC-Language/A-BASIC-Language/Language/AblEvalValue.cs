#nullable enable
namespace A_BASIC_Language.Language;

public class AblEvalValue
{
}

public sealed class AblRead : AblEvalValue
{
    public string Symbol { get; }
    public bool IsArray { get; }

    public AblRead(string symbol, bool isArray)
    {
        Symbol = symbol;
        IsArray = isArray;
    }
}

public sealed class AblOnGoto : AblEvalValue
{
    public System.Collections.Generic.IReadOnlyList<int> Targets { get; }

    public bool IsSubroutine { get; }

    public AblOnGoto(System.Collections.Generic.IEnumerable<int> targets, bool isSubroutine = false)
    {
        Targets = new System.Collections.Generic.List<int>(targets).AsReadOnly();
        IsSubroutine = isSubroutine;
    }
}

public sealed class AblFunctionDefinition : AblEvalValue
{
    public string Name { get; }
    public string Parameter { get; }
    public AblEvalValueList Body { get; }

    public AblFunctionDefinition(string name, string parameter, AblEvalValueList body)
    {
        Name = name;
        Parameter = parameter;
        Body = body;
    }

    public override string ToString() =>
        $"{Name}({Parameter}) {Body}";
}

public sealed class AblFunctionCall : AblEvalValue
{
    public string Name { get; }

    public AblFunctionCall(string name)
    {
        Name = name;
    }
}
