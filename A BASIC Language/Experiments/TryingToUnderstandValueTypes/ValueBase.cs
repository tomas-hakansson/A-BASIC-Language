using System.Globalization;

namespace A_BASIC_Language.Experiments.TryingToUnderstandValueTypes;

public abstract class ValueBase
{
    public const double FloatValueCompareErrorTolerance = 0.00001;

    //https://www.codeproject.com/Articles/242749/Multiple-Dispatch-and-Double-Dispatch
    //Note: Multimethod -> Compare(ValueBase x, ValueBase y);
    static bool Equal(FloatValue f1, FloatValue f2) => Math.Abs(f1.Value - f2.Value) < FloatValueCompareErrorTolerance;
    static bool Equal(FloatValue f, IntValue i) => Math.Abs(f.Value - i.Value) < FloatValueCompareErrorTolerance;
    static bool Equal(FloatValue f, StringValue s) => f.Value.ToString(CultureInfo.InvariantCulture) == s.Value;
    static bool Equal(FloatValue f, NullValue n) => false;

    static bool Equal(IntValue i1, IntValue i2) => i1.Value == i2.Value;
    static bool Equal(IntValue i, FloatValue f) => Math.Abs(i.Value - f.Value) < FloatValueCompareErrorTolerance;
    static bool Equal(IntValue i, StringValue s) => i.Value.ToString() == s.Value;
    static bool Equal(IntValue i, NullValue s) => false;

    static bool Equal(StringValue s1, StringValue s2) => s1.Value == s2.Value;
    static bool Equal(StringValue s, FloatValue f) => s.Value == f.Value.ToString(CultureInfo.InvariantCulture);
    static bool Equal(StringValue s, IntValue i) => s.Value == i.Value.ToString();
    static bool Equal(StringValue s, NullValue n) => false;

    static bool Equal(NullValue n1, NullValue n2) => true;
    static bool Equal(NullValue n1, FloatValue n2) => false;
    static bool Equal(NullValue n1, IntValue n2) => false;
    static bool Equal(NullValue n1, StringValue n2) => false;

    public static bool operator ==(ValueBase left, ValueBase right) => Equal((dynamic)left, (dynamic)right);
    public static bool operator !=(ValueBase left, ValueBase right) => !(left == right);

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (ReferenceEquals(this, obj))
            return true;
        
        return obj is ValueBase @base && this == @base;
    }
    public abstract int GetHash();
    public override int GetHashCode() => GetHash();
}

public class FloatValue : ValueBase
{
    public double Value { get; set; }

    public FloatValue(double value)
    {
        Value = value;
    }

    public override int GetHash() => Value.GetHashCode();
}

public class IntValue : ValueBase
{
    public int Value { get; set; }

    public IntValue(int value)
    {
        Value = value;
    }

    public override int GetHash() => Value.GetHashCode();
}

public class StringValue : ValueBase
{
    public string Value { get; set; }

    public StringValue(string value)
    {
        Value = value;
    }

    public override int GetHash() => Value.GetHashCode();
}

public class NullValue : ValueBase
{
    public override int GetHash() => 42;
}