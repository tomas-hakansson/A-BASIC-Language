namespace A_BASIC_Language.Language;

public class ABL_EvalValue
{
}

public sealed class ABL_FunctionDefinition : ABL_EvalValue
{
    public string Name { get; }
    public string Parameter { get; }
    public System.Collections.Generic.List<ABL_EvalValue> Body { get; }
    public ABL_FunctionDefinition(string name, string parameter, System.Collections.Generic.List<ABL_EvalValue> body)
    { Name = name; Parameter = parameter; Body = body; }
}

public sealed class ABL_FunctionCall : ABL_EvalValue
{
    public string Name { get; }
    public ABL_FunctionCall(string name) { Name = name; }
}
