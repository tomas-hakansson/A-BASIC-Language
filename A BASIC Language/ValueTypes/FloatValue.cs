using System.Globalization;

namespace A_BASIC_Language.ValueTypes;

public class FloatValue : ValueBase
{
    public double Value { get; set; }

    public FloatValue(double value)
    {
        Value = value;
    }

    public override bool FitsInVariable(string symbol) =>
        true;

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

    public static bool operator ==(FloatValue? left, FloatValue? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return Math.Abs(left.Value - right.Value) < 0.001;
    }

    public static bool operator !=(FloatValue? left, FloatValue? right) =>
        !(left == right);

    public static bool operator ==(FloatValue? left, IntValue? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        if (!right.CanGetAsType<FloatValue>())
            return false;

        var f = (double)right.GetValueAsType<FloatValue>();

        return Math.Abs(left.Value - f) < 0.001;
    }

    public static bool operator !=(FloatValue? left, IntValue? right) =>
        !(left == right);

    public static bool operator ==(FloatValue? left, StringValue? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        if (!right.CanGetAsType<FloatValue>())
            return false;

        var f = (double)right.GetValueAsType<FloatValue>();

        return Math.Abs(left.Value - f) < 0.001;
    }

    public static bool operator !=(FloatValue? left, StringValue? right) =>
        !(left == right);

    public static bool operator >(FloatValue? left, FloatValue? right)
    {
        if (left is null || right is null)
            return false;

        return left.Value > right.Value;
    }

    public static bool operator <(FloatValue? left, FloatValue? right)
    {
        if (left is null || right is null)
            return false;

        return left.Value < right.Value;
    }

    public static bool operator >(FloatValue? left, IntValue? right)
    {
        if (left is null || right is null)
            return false;

        return left.Value > right.Value;
    }

    public static bool operator <(FloatValue? left, IntValue? right)
    {
        if (left is null || right is null)
            return false;

        return left.Value < right.Value;
    }

    public static bool operator >(FloatValue? left, StringValue? right)
    {
        if (left is null || right is null)
            return false;

        if (!right.CanGetAsType<FloatValue>())
            return false;

        var f = (double)right.GetValueAsType<FloatValue>();

        return left.Value > f;
    }

    public static bool operator <(FloatValue? left, StringValue? right)
    {
        if (left is null || right is null)
            return false;

        if (!right.CanGetAsType<FloatValue>())
            return false;

        var f = (double)right.GetValueAsType<FloatValue>();

        return left.Value < f;
    }

    public static bool operator >=(FloatValue? left, FloatValue? right)
    {
        if (left is null || right is null)
            return false;

        return left.Value > right.Value;
    }

    public static bool operator <=(FloatValue? left, FloatValue? right)
    {
        if (left is null || right is null)
            return false;

        return left.Value < right.Value;
    }

    public static bool operator >=(FloatValue? left, IntValue? right)
    {
        if (left is null || right is null)
            return false;

        return left.Value > right.Value;
    }

    public static bool operator <=(FloatValue? left, IntValue? right)
    {
        if (left is null || right is null)
            return false;

        return left.Value < right.Value;
    }

    public static bool operator >=(FloatValue? left, StringValue? right)
    {
        if (left is null || right is null)
            return false;

        if (!right.CanGetAsType<FloatValue>())
            return false;

        var f = (double)right.GetValueAsType<FloatValue>();

        return left.Value > f;
    }

    public static bool operator <=(FloatValue? left, StringValue? right)
    {
        if (left is null || right is null)
            return false;

        if (!right.CanGetAsType<FloatValue>())
            return false;

        var f = (double)right.GetValueAsType<FloatValue>();

        return left.Value < f;
    }
}