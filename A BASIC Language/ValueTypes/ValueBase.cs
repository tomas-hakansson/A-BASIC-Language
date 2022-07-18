using System.Globalization;

namespace A_BASIC_Language.ValueTypes;

public abstract class ValueBase
{
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

    public abstract bool FitsInVariable(string symbol);

    internal bool VariableIsDeclaredAsString(string symbol) =>
        symbol.EndsWith("$");

    internal bool VariableIsDeclaredAsInt(string symbol) =>
        symbol.EndsWith("%");

    public abstract bool IsOfType<T>() where T : ValueBase;

    public abstract bool CanGetAsType<T>() where T : ValueBase;

    public abstract object GetValueAsType<T>() where T : ValueBase;

    public abstract bool CanActAsBool();

    public abstract bool GetBoolValue();
}