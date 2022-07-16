namespace A_BASIC_Language.Stage1;

public class Line
{
    public List<string> TokenValues { get; set; }
    public List<TokenType> TokenTypes { get; set; }

    public Line()
    {
        TokenValues = new List<string>();
        TokenTypes = new List<TokenType>();
    }
}
