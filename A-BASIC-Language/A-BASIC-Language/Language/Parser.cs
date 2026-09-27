using A_BASIC_Language.Language.Parsing;

namespace A_BASIC_Language.Language;

public class Parser
{
    public ParseResult Result { get; }

    public Parser(string source, bool direct = false)
    {
        var p = new BasicParser(source, direct);
        Result = p.Result;
    }
}