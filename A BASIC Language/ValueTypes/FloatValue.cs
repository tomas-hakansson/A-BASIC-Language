using System.Globalization;

namespace A_BASIC_Language.ValueTypes;

public class FloatValue : ValueBase
{
    public const double CompareErrorTolerance = 0.00001;

    public double Value { get; set; }

    public FloatValue(double value) =>
        Value = value;

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

    public override bool TryGetAsFloatValue(out FloatValue value)
    {
        value = this;
        return true;
    }

    public override bool TryGetAsIntValue(out IntValue value)
    {
        value = new IntValue((int)Value);
        return true;
    }

    public override bool TryGetAsStringValue(out StringValue value)
    {
        value = new StringValue(Value.ToString(CultureInfo.InvariantCulture));
        return true;
    }

    public override bool CanActAsBool() =>
        true;

    public override bool GetBoolValue() =>
        Value != 0.0;

    public override string ToString() =>
        Value.ToString(CultureInfo.InvariantCulture);

    public override bool Equals(object? obj)
    {
        if (obj is not ValueBase b)
            return false;

        if (!b.CanGetAsType<FloatValue>())
            return false;

        var f = (double)b.GetValueAsType<FloatValue>();

        return Math.Abs(Value - f) < FloatValue.CompareErrorTolerance;
    }

    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        var bits = BitConverter.DoubleToUInt64Bits(Value);
        return (int)bits % int.MaxValue;
    }
}