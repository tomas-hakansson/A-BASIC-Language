using System.Globalization;

namespace A_BASIC_Language.ValueTypes;

public abstract class ValueBase
{
    //Note: [a-z][A-Z0-9]*[$%]+
    const string FirstCharacterOfName = "abcdefghijklmnopqrstuvwxyz";
    const string MiddleCharacterOfName = FirstCharacterOfName + "0123456789";
    const string LastCharacterOfName = MiddleCharacterOfName + "$%";

    public static bool IsFloat(string value) =>
        double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _);

    public static bool IsInt(string value) =>
        int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _);

    public static ValueBase GetValueType(string value)
    {
        if (value.Contains('.') && double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var f))
            return GetValueType(f);

        if (int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var i))
            return GetValueType(i);

        return new StringValue(value);
    }

    public static ValueBase GetValueType(double value) =>
        value % 1 == 0
            ? new IntValue((int)value)
            : new FloatValue(value);

    public static ValueBase GetDefaultValueFor(string symbol)
    {
        if (VariableIsDeclaredAsInt(symbol))
            return new IntValue(0);

        if (VariableIsDeclaredAsString(symbol))
            return new StringValue("");

        if (VariableIsDeclaredAsFloat(symbol))
            return new FloatValue(0.0);

        throw new SystemException("This is not good...");
    }

    internal static bool VariableIsDeclaredAsString(string symbol) =>
        IsName(symbol, out _) && symbol.EndsWith("$");

    internal static bool VariableIsDeclaredAsInt(string symbol) =>
        IsName(symbol, out _) && symbol.EndsWith("%");

    internal static bool VariableIsDeclaredAsFloat(string symbol) =>
        IsName(symbol, out var n) && MiddleCharacterOfName.Contains(n[^1]);

    static bool IsName(string symbol, out string symbolName)
    {
        symbolName = symbol.Trim().ToLower();

        if (symbolName.Length < 1)
            return false;

        if (!FirstCharacterOfName.Contains(symbolName[0]))
            return false;

        if (!LastCharacterOfName.Contains(symbolName[^1]))
            return false;

        return true;
    }


    //Note: Overriding '==' and '!=' operators:
    static bool Equal(FloatValue x, FloatValue y) => Math.Abs(x.Value - y.Value) < FloatValue.CompareErrorTolerance;
    static bool Equal(FloatValue x, IntValue y) => Math.Abs(x.Value - y.Value) < FloatValue.CompareErrorTolerance;
    static bool Equal(FloatValue x, StringValue y) => y.TryGetAsFloatValue(out var ny) && Equal(x, ny);

    static bool Equal(IntValue x, IntValue y) => x.Value == y.Value;
    static bool Equal(IntValue x, FloatValue y) => Equal(y, x);
    static bool Equal(IntValue x, StringValue y) => y.TryGetAsIntValue(out var ny) && Equal(x, ny);

    static bool Equal(StringValue x, StringValue y) => x.Value == y.Value;
    static bool Equal(StringValue x, FloatValue y) => Equal(y, x);
    static bool Equal(StringValue x, IntValue y) => Equal(y, x);

    public static bool operator ==(ValueBase? x, ValueBase? y)
    {
        if (x is null && y is null)
            return true;

        if (x is null || y is null)
            return false;

        return Equal((dynamic)x, (dynamic)y);
    }
    public static bool operator !=(ValueBase? x, ValueBase? y) => !(x == y);


    //Note: Overriding '>' and '<' operators:
    static bool GreaterThan(FloatValue x, FloatValue y) => x.Value > y.Value;
    static bool GreaterThan(FloatValue x, IntValue y) => x.Value > y.Value;
    static bool GreaterThan(FloatValue x, StringValue y) => y.TryGetAsFloatValue(out var ny) && GreaterThan(x, ny);

    static bool GreaterThan(IntValue x, IntValue y) => x.Value > y.Value;
    static bool GreaterThan(IntValue x, FloatValue y) => x.Value > y.Value;
    static bool GreaterThan(IntValue x, StringValue y) => y.TryGetAsIntValue(out var ny) && GreaterThan(x, ny);

    static bool GreaterThan(StringValue x, StringValue y) =>
        throw new NotImplementedException();//ToDo: Check how this works in BASIC.
    static bool GreaterThan(StringValue x, FloatValue y) => x.TryGetAsFloatValue(out var nx) && GreaterThan(nx, y);
    static bool GreaterThan(StringValue x, IntValue y) => x.TryGetAsIntValue(out var nx) && GreaterThan(nx, y);

    public static bool operator >(ValueBase? x, ValueBase? y)
    {
        if (x is null || y is null)
            return false;

        return GreaterThan((dynamic)x, (dynamic)y);
    }
    public static bool operator <(ValueBase? x, ValueBase? y) => !(x >= y);


    //Note: Overriding '>=' and '<=' operators:
    public static bool operator >=(ValueBase? x, ValueBase? y)
    {
        if (x is null || y is null)
            return false;
        return x == y || x > y;
    }
    public static bool operator <=(ValueBase? x, ValueBase? y) => !(x > y);


    // ReSharper disable once RedundantOverriddenMember
    public override bool Equals(object? obj) // todo
    {
        // ReSharper disable once BaseObjectEqualsIsObjectEquals
        return base.Equals(obj);
    }

    // ReSharper disable once RedundantOverriddenMember
    public override int GetHashCode() // todo
    {
        // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
        return base.GetHashCode();
    }

    public abstract bool FitsInVariable(string symbol);

    public abstract bool IsOfType<T>() where T : ValueBase;

    public abstract bool CanGetAsType<T>() where T : ValueBase;

    public abstract object GetValueAsType<T>() where T : ValueBase;

    public abstract bool TryGetAsFloatValue(out FloatValue value);
    public abstract bool TryGetAsIntValue(out IntValue value);
    public abstract bool TryGetAsStringValue(out StringValue value);

    public abstract bool CanActAsBool();

    public abstract bool GetBoolValue();
}
