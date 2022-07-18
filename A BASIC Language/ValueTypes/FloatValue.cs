using System.Globalization;

namespace A_BASIC_Language.ValueTypes;

public class FloatValue : ValueBase
{
    public double Value { get; set; }

    public FloatValue(double value)
    {
        Value = value;
    }

    public override bool IsOfType<T>() =>
        typeof(T) == typeof(FloatValue);

    public override bool CanGetAsType<T>() =>
        true;

    public override object GetValueAsType<T>()
    {
        if (typeof(T) == typeof(IntValue))
            return (int)Value;

        if (typeof(T) == typeof(FloatValue))
            return Value;

        if (typeof(T) == typeof(StringValue))
            return Value.ToString(CultureInfo.InvariantCulture);

        throw new SystemException("What?!");
    }

    public override bool CanActAsBool() =>
        true;

    public override bool GetBoolValue() =>
        Value != 0.0;

    public override string ToString() =>
        Value.ToString();
}