using System.Globalization;

namespace A_BASIC_Language.ValueTypes;

public abstract class ValueBase
{
    private const string FirstCharacterOfName = "abcdefghijklmnopqrstuvwxyz";
    private const string MiddleCharacterOfName = FirstCharacterOfName + "0123456789";
    private const string LastCharacterOfName = MiddleCharacterOfName + "$%";

    public static bool IsFloat(string value) =>
        double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _);

    public static bool IsInt(string value) =>
        int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _);

    public static ValueBase GetValueType(string value)
    {
        if (value.Contains(".") && double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var f))
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

    public abstract bool FitsInVariable(string symbol);

    internal static bool VariableIsDeclaredAsString(string symbol) =>
        IsName(symbol, out var n) && symbol.EndsWith("$");

    internal static bool VariableIsDeclaredAsInt(string symbol) =>
        IsName(symbol, out var n) && symbol.EndsWith("%");

    internal static bool VariableIsDeclaredAsFloat(string symbol) =>
        IsName(symbol, out var n) && MiddleCharacterOfName.Contains(n[^1]);

    private static bool IsName(string symbol, out string symbolName)
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

    public abstract bool IsOfType<T>() where T : ValueBase;

    public abstract bool CanGetAsType<T>() where T : ValueBase;

    public abstract object GetValueAsType<T>() where T : ValueBase;

    public abstract bool CanActAsBool();

    public abstract bool GetBoolValue();
}