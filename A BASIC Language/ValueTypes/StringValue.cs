using System.Globalization;

namespace A_BASIC_Language.ValueTypes;

public class StringValue : ValueBase
{
    public string Value { get; set; }

    public StringValue(string value)
    {
        Value = value;
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

    public static bool operator ==(StringValue? left, StringValue? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return string.Compare(left.Value, right.Value, StringComparison.CurrentCultureIgnoreCase) == 0;
    }

    public static bool operator !=(StringValue? left, StringValue? right) =>
        !(left == right);

    public static bool operator ==(StringValue? left, IntValue? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        if (!right.CanGetAsType<StringValue>())
            return false;

        var s = (string)right.GetValueAsType<StringValue>();

        return string.Compare(left.Value, s, StringComparison.CurrentCultureIgnoreCase) == 0;
    }

    public static bool operator !=(StringValue? left, IntValue? right) =>
        !(left == right);

    public static bool operator ==(StringValue? left, FloatValue? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        if (!right.CanGetAsType<StringValue>())
            return false;

        var s = (string)right.GetValueAsType<StringValue>();

        return string.Compare(left.Value, s, StringComparison.CurrentCultureIgnoreCase) == 0;
    }

    public static bool operator !=(StringValue? left, FloatValue? right) =>
        !(left == right);

    public static bool operator >(StringValue? left, StringValue? right) =>
        false; // Cannot compare.

    public static bool operator <(StringValue? left, StringValue? right) =>
        false; // Cannot compare.

    public static bool operator >(StringValue? left, IntValue? right) =>
        false; // Cannot compare.

    public static bool operator <(StringValue? left, IntValue? right) =>
        false; // Cannot compare.

    public static bool operator >(StringValue? left, FloatValue? right) =>
        false; // Cannot compare.

    public static bool operator <(StringValue? left, FloatValue? right) =>
        false; // Cannot compare.

    public static bool operator >=(StringValue? left, StringValue? right) =>
    false; // Cannot compare.

    public static bool operator <=(StringValue? left, StringValue? right) =>
        false; // Cannot compare.

    public static bool operator >=(StringValue? left, IntValue? right) =>
        false; // Cannot compare.

    public static bool operator <=(StringValue? left, IntValue? right) =>
        false; // Cannot compare.

    public static bool operator >=(StringValue? left, FloatValue? right) =>
        false; // Cannot compare.

    public static bool operator <=(StringValue? left, FloatValue? right) =>
        false; // Cannot compare.
}