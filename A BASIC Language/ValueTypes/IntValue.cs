using System.Globalization;

namespace A_BASIC_Language.ValueTypes;

public class IntValue : ValueBase
{
    public int Value { get; set; }

    public IntValue(int value)
    {
        Value = value;
    }

    public override bool IsOfType<T>() =>
        typeof(T) is IntValue;

    public override bool CanGetAsType<T>() =>
        true;

    public override object GetValueAsType<T>()
    {
        if (typeof(T) is IntValue)
            return Value;

        if (typeof(T) is FloatValue)
            return (double)Value;

        if (typeof(T) is StringValue)
            return Value.ToString(CultureInfo.InvariantCulture);

        throw new SystemException("What?!");
    }

    public override bool CanActAsBool() =>
        true;

    public override bool GetBoolValue() =>
        Value != 0;
}