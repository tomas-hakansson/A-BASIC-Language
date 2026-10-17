using System;

namespace A_BASIC_Language.Language.LanguageParts;

public class AblLabel : AblEvalValue
{
    public int Value { get; private set; }

    public AblLabel(int value)
    {
        Value = value;
    }

    public AblLabel(string value)//DeleteMe: Once the new parser replaces the old.
    {
        if (int.TryParse(value, out int parsedInt))
            Value = parsedInt;
        else
            throw new ArgumentException($@"Expected an int but got: ({value})", nameof(value));
    }

    public override string ToString() =>
        $"L({Value})";
}
