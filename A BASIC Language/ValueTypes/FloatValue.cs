using System.Globalization;

namespace A_BASIC_Language.ValueTypes;

public class FloatValue : Value
{
    public double Value { get; set; }

    public FloatValue(double value)
    {
        Value = value;
    }

    public override bool IsOfType<T>() =>
        typeof(T) is FloatValue;

    public override bool CanGetAsType<T>() =>
        true;

    public override object GetValueAsType<T>()
    {
        if (typeof(T) is IntValue)
            return (int)Value;

        if (typeof(T) is FloatValue)
            return Value;

        if (typeof(T) is StringValue)
            return Value.ToString(CultureInfo.InvariantCulture);

        throw new SystemException("What?!");
    }

    public override bool CanActAsBool() =>
        true;

    public override bool GetBoolValue() =>
        Value != 0.0;
}