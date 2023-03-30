using System.Globalization;

namespace A_BASIC_Language.ValueTypes2;

public class StringValue : ValueBase
{
    public string Value { get; set; }

    public StringValue(string value)
    {
        Value = value;
    }
    protected bool Equals(StringValue other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((StringValue)obj);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override bool FitsInVariable(string symbol)
    {
        if (VariableIsDeclaredAsString(symbol))
            return true;

        if (VariableIsDeclaredAsInt(symbol) && CanGetAsType<IntValue>())
            return true;

        if (CanGetAsType<FloatValue>())
            return true;

        return false;
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