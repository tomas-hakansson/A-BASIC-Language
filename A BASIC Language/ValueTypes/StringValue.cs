namespace A_BASIC_Language.ValueTypes;

public class StringValue : Value
{
    public string Value { get; set; }

    public StringValue(string value)
    {
        Value = value;
    }

    public override bool IsOfType<T>() =>
        typeof(T) is StringValue;

    public override bool CanGetAsType<T>()
    {
        if (typeof(T) is StringValue)
            return true;

        if (typeof(T) is IntValue)
        {
            // TODO: Depends on content.
        }

        if (typeof(T) is FloatValue)
        {

        }

        throw new SystemException("What?!");
    }

    public override object GetValueAsType<T>()
    {
        return null;
    }

    public override bool CanActAsBool()
    {
        return false;
    }

    public override bool GetBoolValue()
    {
        return false;
    }
}