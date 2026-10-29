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
        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsedDouble))
            Value = parsedDouble;
        else
            throw new ArgumentException($@"Expected a double but got: {value}", nameof(value));
    }

    public override string ToString()
    {
        return $"N({Value.ToString(CultureInfo.InvariantCulture)})";
    }
}

public class AblString : AblEvalValue
{
    public string Value { get; }

    public AblString(string value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return $"S({Value})";
    }
}

public class AblDimCreation : AblEvalValue
{
    public string Symbol { get; }

    public AblDimCreation(string symbol)
    {
        Symbol = symbol.ToUpper();
    }

    public override string ToString()
    {
        return $"DC({Symbol})";
    }
}

public class AblVariable : AblEvalValue
{
    public string Symbol { get; }

    public AblVariable(string symbol)
    {
        Symbol = symbol.ToUpper();
    }

    public override string ToString()
    {
        return $"V({Symbol})";
    }
}

public class AblDimVariable : AblEvalValue
{
    public string Symbol { get; }

    public AblDimVariable(string symbol)
    {
        Symbol = symbol.ToUpper();
    }

    public override string ToString()
    {
        return $"DV({Symbol})";
    }
}

public class AblAssignment : AblEvalValue
{
    public string Symbol { get; }

    public AblAssignment(string symbol)
    {
        Symbol = symbol.ToUpper();
    }

    public override string ToString()
    {
        return $"=({Symbol})";
    }
}

public class AblDimAssignment : AblEvalValue
{
    public string Symbol { get; }

    public AblDimAssignment(string symbol)
    {
        Symbol = symbol.ToUpper();
    }

    public override string ToString()
    {
        return $"D=({Symbol})";
    }
}
