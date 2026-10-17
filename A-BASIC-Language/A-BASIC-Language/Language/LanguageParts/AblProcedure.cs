namespace A_BASIC_Language.Language.LanguageParts;

public class AblProcedure : ABL_EvalValue
{
    public string Name { get; private set; }

    public AblProcedure(string name)
    {
        Name = name.ToUpper();
    }

    public override string ToString() =>
        $"P({Name})";
}
