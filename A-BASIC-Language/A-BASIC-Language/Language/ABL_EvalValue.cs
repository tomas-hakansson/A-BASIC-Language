namespace A_BASIC_Language.Language;

public class AblEvalValue
{
}

public sealed class ABL_FunctionDefinition : AblEvalValue
{
    public string Name { get; }
    public string Parameter { get; }
    public System.Collections.Generic.List<AblEvalValue> Body { get; }
    public ABL_FunctionDefinition(string name, string parameter, System.Collections.Generic.List<AblEvalValue> body)
    { Name = name; Parameter = parameter; Body = body; }
}

public sealed class ABL_FunctionCall : AblEvalValue
{
    public string Name { get; }
    public ABL_FunctionCall(string name) { Name = name; }
}
