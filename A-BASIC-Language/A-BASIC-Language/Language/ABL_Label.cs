using System;
using System.Globalization;

namespace A_BASIC_Language.Language;

public class ABL_Number : AblEvalValue
{
    public double Value { get; private set; }

    public ABL_Number(double value)
    {
        Value = value;
    }

    public ABL_Number(string value)
    {
        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsedDouble))
            Value = parsedDouble;
        else
            throw new ArgumentException($"Expected a double but got: ({value})", nameof(value));
    }

    public override string ToString()
    {
        return $"N({Value.ToString(CultureInfo.InvariantCulture)})";
    }
}

public class ABL_String : AblEvalValue
{
    public string Value { get; private set; }

    public ABL_String(string value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return $"S({Value})";
    }
}

public class ABL_DIM_Creation : AblEvalValue
{
    public string Symbol { get; private set; }

    public ABL_DIM_Creation(string symbol)
    {
        Symbol = symbol.ToUpper();
    }

    public override string ToString()
    {
        return $"DC({Symbol})";
    }
}

public class ABL_Variable : AblEvalValue
{
    public string Symbol { get; private set; }

    public ABL_Variable(string symbol)
    {
        Symbol = symbol.ToUpper();
    }

    public override string ToString()
    {
        return $"V({Symbol})";
    }
}

public class ABL_DIM_Variable : AblEvalValue
{
    public string Symbol { get; private set; }

    public ABL_DIM_Variable(string symbol)
    {
        Symbol = symbol.ToUpper();
    }

    public override string ToString()
    {
        return $"DV({Symbol})";
    }
}

public class ABL_Assignment : AblEvalValue
{
    public string Symbol { get; private set; }
    public ABL_Assignment(string symbol)
    {
        Symbol = symbol.ToUpper();
    }

    public override string ToString()
    {
        return $"=({Symbol})";
    }
}

public class ABL_DIM_Assignment : AblEvalValue
{
    public string Symbol { get; private set; }

    public ABL_DIM_Assignment(string symbol)
    {
        Symbol = symbol.ToUpper();
    }

    public override string ToString()
    {
        return $"D=({Symbol})";
    }
}


