using System.Globalization;

namespace A_BASIC_Language.ValueTypes;

public class StringValue : ValueBase
{
    public string Value { get; set; }

    public StringValue(string value) =>
        Value = value;

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

    public override bool TryGetAsFloatValue(out FloatValue value)
    {
        value = new FloatValue(-1);
        if (double.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double newValue))
        {
            value = new FloatValue(newValue);
            return true;
        }
        else
            return false;
    }

    public override bool TryGetAsIntValue(out IntValue value)
    {
        value = new IntValue(-1);
        if (int.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var newInt))
        {
            value = new IntValue(newInt);
            return true;
        }
        else if (double.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var newDouble))
        {
            value = new IntValue((int)newDouble);
            return true;
        }
        return false;
    }

    public override bool TryGetAsStringValue(out StringValue value)
    {
        value = this;
        return true;
    }

    public override bool CanActAsBool() =>
        false;

    public override bool GetBoolValue() =>
        false;

    public override string ToString() =>
        Value;

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((StringValue)obj);
    }

    protected bool Equals(StringValue other)
    {
        return Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}