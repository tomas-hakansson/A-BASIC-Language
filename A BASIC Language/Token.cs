// See https://aka.ms/new-console-template for more information


public class Token
{
}

public class Number : Token
{
    public double Value { get; private set; }

    public Number(double value)
    {
        Value = value;
    }
}

public class Variable : Token
{
    public string Symbol { get; private set; }

    public Variable(string symbol)
    {
        Symbol = symbol;
    }
}

public class Assignment : Token
{
    public string Symbol { get; private set; }
    public Assignment(string symbol)
    {
        Symbol = symbol;
    }
}

public class Procedure : Token
{
    public string Name { get; private set; }

    public Procedure(string name)
    {
        Name = name;
    }
}