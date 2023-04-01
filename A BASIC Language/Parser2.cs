using A_BASIC_Language.Stage1;

namespace A_BASIC_Language;

public class Parser2
{
    public Parsing.ParseResult Result { get; }

    public Parser2(string source)
    {
        Tokenizer t = new(source);
        Parsing.Parser p = new(t.Result);
        Result = p.Result;
    }
}