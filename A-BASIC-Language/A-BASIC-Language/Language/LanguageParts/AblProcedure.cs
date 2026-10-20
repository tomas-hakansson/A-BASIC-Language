namespace A_BASIC_Language.Language.LanguageParts;

public class AblProcedure : AblEvalValue
{
    public string Name { get; }

    public AblProcedure(string name)
    {
        Name = name.ToUpper();
    }

    public override string ToString() =>
        $"P({Name})";
}
