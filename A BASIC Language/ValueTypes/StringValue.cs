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
        typeof(T) is StringValue;

    public override bool CanGetAsType<T>()
    {
        if (typeof(T) is StringValue)
            return true;

        if (typeof(T) is IntValue)
            return int.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture, out _);

        if (typeof(T) is FloatValue)
            return double.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture, out _);

        throw new SystemException("What?!");
    }

    public override object GetValueAsType<T>()
    {
        if (typeof(T) is StringValue)
            return true;

        if (typeof(T) is IntValue)
            return int.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture, out _);

        if (typeof(T) is FloatValue)
            return double.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture, out _);

        throw new SystemException("What?!");
    }

    public override bool CanActAsBool() =>
        false;

    public override bool GetBoolValue() =>
        false;
}