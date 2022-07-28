namespace A_BASIC_Language;

public class ABL_EvalValue
{
}

public class ABL_Label : ABL_EvalValue
{
    public int Value { get; private set; }

    public ABL_Label(int value)
    {
        Value = value;
    }

    public ABL_Label(string value)
    {
        Value = int.Parse(value);
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
        Value = double.Parse(value);
    }
}

public class ABL_String : ABL_EvalValue
{
    public string Value { get; private set; }

    public ABL_String(string value)
    {
        Value = value;
    }
}

public class ABL_Variable : ABL_EvalValue
{
    public string Symbol { get; private set; }

    public ABL_Variable(string symbol)
    {
        Symbol = symbol;
    }
}

public class ABL_Assignment : ABL_EvalValue
{
    public string Symbol { get; private set; }
    public ABL_Assignment(string symbol)
    {
        Symbol = symbol;
    }
}

public class ABL_Procedure : ABL_EvalValue
{
    public string Name { get; private set; }

    public ABL_Procedure(string name)
    {
        Name = name;
    }
}