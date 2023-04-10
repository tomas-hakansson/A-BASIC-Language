namespace A_BASIC_Language;

public class Parser
{
    public Parsing.ParseResult Result { get; }

    public Parser(string source)
    {
        Parsing.Parser p = new(source);
        Result = p.Result;
    }
}