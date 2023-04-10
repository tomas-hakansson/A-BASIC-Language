using A_BASIC_Language.Stage1;

namespace A_BASIC_Language;

public class Parser_Old
{
    public Stage2.ParseResult Result { get; }

    public Parser_Old(string source)
    {
        Tokenizer t = new(source);
        Stage2.Parser p = new(t.Result);
        Result = p.Result;
    }
}