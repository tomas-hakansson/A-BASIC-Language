﻿using System.Globalization;

namespace A_BASIC_Language.ValueTypes;

public class IntValue : ValueBase
{
    public int Value { get; set; }

    public IntValue(int value)
    {
        Value = value;
    }

    public override bool FitsInVariable(string symbol) =>
        true;

    public override bool IsOfType<T>() =>
        typeof(T) == typeof(IntValue);

    public override bool CanGetAsType<T>() =>
        true;

    public override object GetValueAsType<T>()
    {
        if (typeof(T) == typeof(IntValue))
            return Value;

        if (typeof(T) == typeof(FloatValue))
            return (double)Value;

        if (typeof(T) == typeof(StringValue))
            return Value.ToString(CultureInfo.InvariantCulture);

        throw new SystemException("What?!");
    }

    public override bool CanActAsBool() =>
        true;

    public override bool GetBoolValue() =>
        Value != 0;

    public override string ToString() =>
        Value.ToString();
}