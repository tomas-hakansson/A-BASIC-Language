namespace A_BASIC_Language.Language;

public class Parser
{
    public ParseResult Result { get; }

    public Parser(string source)
    {
        Parsing.BasicParser p = new(source);
        Result = p.Result;
    }
}