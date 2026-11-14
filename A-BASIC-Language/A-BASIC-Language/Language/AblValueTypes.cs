using System;
using System.Globalization;

namespace A_BASIC_Language.Language;

public class AblNumber : AblEvalValue
{
    public double Value { get; }

    public AblNumber(double value)
    {
        Value = value;
    }

    public AblNumber(string value)
    {
        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedDouble))
            Value = parsedDouble;
        else
            throw new ArgumentException($@"Expected a double but got: {value}", nameof(value));
    }

    public override string ToString() =>
        $"N({Value.ToString(CultureInfo.InvariantCulture)})";
}

public class AblString(string value) : AblEvalValue
{
    public string Value { get; } = value;

    public override string ToString() =>
         $"S({Value})";
}

public class AblDimCreation(string symbol) : AblEvalValue
{
    public string Symbol { get; } = symbol.ToUpper();

    public override string ToString() =>
        $"DC({Symbol})";
}

public class AblVariable(string symbol) : AblEvalValue
{
    public string Symbol { get; } = symbol.ToUpper();

    public override string ToString() =>
        $"V({Symbol})";
}

public class AblDimVariable(string symbol) : AblEvalValue
{
    public string Symbol { get; } = symbol.ToUpper();

    public override string ToString() =>
        $"DV({Symbol})";
}

public class AblAssignment(string symbol) : AblEvalValue
{
    public string Symbol { get; } = symbol.ToUpper();

    public override string ToString() =>
        $"=({Symbol})";
}

public class AblDimAssignment(string symbol) : AblEvalValue
{
    public string Symbol { get; } = symbol.ToUpper();

    public override string ToString() =>
        $"D=({Symbol})";
}
