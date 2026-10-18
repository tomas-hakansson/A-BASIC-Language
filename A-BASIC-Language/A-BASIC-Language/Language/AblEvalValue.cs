namespace A_BASIC_Language.Language;

public class AblEvalValue
{
}

public sealed class AblOnGoto : AblEvalValue
{
    public System.Collections.Generic.IReadOnlyList<int> Targets { get; }

    public AblOnGoto(System.Collections.Generic.IEnumerable<int> targets)
    {
        Targets = new System.Collections.Generic.List<int>(targets).AsReadOnly();
    }
}

public sealed class AblFunctionDefinition : AblEvalValue
{
    public string Name { get; }
    public string Parameter { get; }
    public System.Collections.Generic.List<AblEvalValue> Body { get; }

    public AblFunctionDefinition(string name, string parameter, System.Collections.Generic.List<AblEvalValue> body)
    {
        Name = name;
        Parameter = parameter;
        Body = body;
    }
}

public sealed class AblFunctionCall : AblEvalValue
{
    public string Name { get; }

    public AblFunctionCall(string name)
    {
        Name = name;
    }
}
