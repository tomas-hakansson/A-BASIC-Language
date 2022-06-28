// See https://aka.ms/new-console-template for more information


internal class Token
{
}

internal class Number : Token
{
    public double Value { get; private set; }

    public Number(double value)
    {
        Value = value;
    }

    public Number(string value)
    {
        Value = double.Parse(value);
    }
}

internal class Variable : Token
{
    public string Symbol { get; private set; }

    public Variable(string symbol)
    {
        Symbol = symbol;
    }
}

internal class Assignment : Token
{
    public string Symbol { get; private set; }
    public Assignment(string symbol)
    {
        Symbol = symbol;
    }
}

internal class Procedure : Token
{
    public string Name { get; private set; }

    public Procedure(string name)
    {
        Name = name;
    }
}