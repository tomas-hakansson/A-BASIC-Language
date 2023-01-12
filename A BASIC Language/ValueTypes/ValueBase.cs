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
        IsName(symbol, out _) && symbol.EndsWith("$");

    internal static bool VariableIsDeclaredAsInt(string symbol) =>
        IsName(symbol, out _) && symbol.EndsWith("%");

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

    public static bool operator ==(ValueBase? left, ValueBase? right)
    {
        if (left is IntValue iLeft)
        {
            if (right is IntValue iRight)
                return iLeft == iRight;

            if (right is FloatValue fRight)
                return iLeft == fRight;

            if (right is StringValue sRight)
                return iLeft == sRight;
        }

        if (left is FloatValue fLeft)
        {
            if (right is IntValue iRight)
                return fLeft == iRight;

            if (right is FloatValue fRight)
                return fLeft == fRight;

            if (right is StringValue sRight)
                return fLeft == sRight;
        }

        if (left is StringValue sLeft)
        {
            if (right is IntValue iRight)
                return sLeft == iRight;

            if (right is FloatValue fRight)
                return sLeft == fRight;

            if (right is StringValue sRight)
                return sLeft == sRight;
        }

        throw new SystemException("What?");
    }

    public static bool operator !=(ValueBase? left, ValueBase? right) =>
        !(left == right);

    public static bool operator >(ValueBase? left, ValueBase? right)
    {
        if (left is IntValue iLeft)
        {
            if (right is IntValue iRight)
                return iLeft > iRight;

            if (right is FloatValue fRight)
                return iLeft > fRight;

            if (right is StringValue sRight)
                return iLeft > sRight;
        }

        if (left is FloatValue fLeft)
        {
            if (right is IntValue iRight)
                return fLeft > iRight;

            if (right is FloatValue fRight)
                return fLeft > fRight;

            if (right is StringValue sRight)
                return fLeft > sRight;
        }

        if (left is StringValue sLeft)
        {
            if (right is IntValue iRight)
                return sLeft > iRight;

            if (right is FloatValue fRight)
                return sLeft > fRight;

            if (right is StringValue sRight)
                return sLeft > sRight;
        }

        throw new SystemException("What?");
    }

    public static bool operator <(ValueBase? left, ValueBase? right)
    {
        if (left is IntValue iLeft)
        {
            if (right is IntValue iRight)
                return iLeft < iRight;

            if (right is FloatValue fRight)
                return iLeft < fRight;

            if (right is StringValue sRight)
                return iLeft < sRight;
        }

        if (left is FloatValue fLeft)
        {
            if (right is IntValue iRight)
                return fLeft < iRight;

            if (right is FloatValue fRight)
                return fLeft < fRight;

            if (right is StringValue sRight)
                return fLeft < sRight;
        }

        if (left is StringValue sLeft)
        {
            if (right is IntValue iRight)
                return sLeft < iRight;

            if (right is FloatValue fRight)
                return sLeft < fRight;

            if (right is StringValue sRight)
                return sLeft < sRight;
        }

        throw new SystemException("What?");
    }

    public static bool operator >=(ValueBase? left, ValueBase? right)
    {
        if (left is IntValue iLeft)
        {
            if (right is IntValue iRight)
                return iLeft >= iRight;

            if (right is FloatValue fRight)
                return iLeft >= fRight;

            if (right is StringValue sRight)
                return iLeft >= sRight;
        }

        if (left is FloatValue fLeft)
        {
            if (right is IntValue iRight)
                return fLeft >= iRight;

            if (right is FloatValue fRight)
                return fLeft >= fRight;

            if (right is StringValue sRight)
                return fLeft >= sRight;
        }

        if (left is StringValue sLeft)
        {
            if (right is IntValue iRight)
                return sLeft >= iRight;

            if (right is FloatValue fRight)
                return sLeft >= fRight;

            if (right is StringValue sRight)
                return sLeft >= sRight;
        }

        throw new SystemException("What?");
    }

    public static bool operator <=(ValueBase? left, ValueBase? right)
    {
        if (left is IntValue iLeft)
        {
            if (right is IntValue iRight)
                return iLeft <= iRight;

            if (right is FloatValue fRight)
                return iLeft <= fRight;

            if (right is StringValue sRight)
                return iLeft <= sRight;
        }

        if (left is FloatValue fLeft)
        {
            if (right is IntValue iRight)
                return fLeft <= iRight;

            if (right is FloatValue fRight)
                return fLeft <= fRight;

            if (right is StringValue sRight)
                return fLeft <= sRight;
        }

        if (left is StringValue sLeft)
        {
            if (right is IntValue iRight)
                return sLeft <= iRight;

            if (right is FloatValue fRight)
                return sLeft <= fRight;

            if (right is StringValue sRight)
                return sLeft <= sRight;
        }

        throw new SystemException("What?");
    }

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

    public abstract bool IsOfType<T>() where T : ValueBase;

    public abstract bool CanGetAsType<T>() where T : ValueBase;

    public abstract object GetValueAsType<T>() where T : ValueBase;

    public abstract bool CanActAsBool();

    public abstract bool GetBoolValue();
}