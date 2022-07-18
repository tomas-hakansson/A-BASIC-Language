using System.Globalization;

namespace A_BASIC_Language.ValueTypes;

public class StringValue : ValueBase
{
    public string Value { get; set; }

    public StringValue(string value)
    {
        Value = value;
    }

    public override bool IsOfType<T>() =>
        typeof(T) == typeof(StringValue);

    public override bool CanGetAsType<T>()
    {
        if (typeof(T) == typeof(StringValue))
            return true;

        if (typeof(T) == typeof(IntValue))
            return int.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture, out _) || double.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture, out _);

        if (typeof(T) == typeof(FloatValue))
            return double.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture, out _);

        throw new SystemException("What?!");
    }

    public override object GetValueAsType<T>()
    {
        if (typeof(T) == typeof(StringValue))
            return Value;

        if (typeof(T) == typeof(IntValue))
        {
            if (int.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var i))
                return i;

            if (double.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var d1))
                return (int)d1;
        }

        if (typeof(T) == typeof(FloatValue) && double.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var d2))
            return d2;

        throw new SystemException("What?!");
    }

    public override bool CanActAsBool() =>
        false;

    public override bool GetBoolValue() =>
        false;

    public override string ToString() =>
        Value;
}