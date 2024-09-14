using System.Globalization;

namespace A_BASIC_Language.Language;

public class ABL_Label : ABL_EvalValue
{
    public int Value { get; private set; }

    public ABL_Label(int value)
    {
        Value = value;
    }

    public ABL_Label(string value)//DeleteMe: Once the new parser replaces the old.
    {
        if (int.TryParse(value, out int parsedInt))
            Value = parsedInt;
        else
            throw new ArgumentException($@"Expected an int but got: ({value})", nameof(value));
    }

    public override string ToString()
    {
        return $"L({Value})";
    }
}

public class ABL_Number : ABL_EvalValue
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

public class ABL_String : ABL_EvalValue
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

public class ABL_DIM_Creation : ABL_EvalValue
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

public class ABL_Variable : ABL_EvalValue
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

public class ABL_DIM_Variable : ABL_EvalValue
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

public class ABL_Assignment : ABL_EvalValue
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

public class ABL_DIM_Assignment : ABL_EvalValue
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

public class ABL_Procedure : ABL_EvalValue
{
    public string Name { get; private set; }

    public ABL_Procedure(string name)
    {
        Name = name.ToUpper();
    }

    public override string ToString()
    {
        return $"P({Name})";
    }
}