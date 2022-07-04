namespace A_BASIC_Language.Stage1;

public class TokenizationResult
{
    public bool Success { get; set; }
    public string Token { get; private set; }
    public int NewIndex { get; set; }
    public bool IsNewLine { get; set; }
    public bool EOF { get; set; }

    public TokenizationResult(bool success, string token, int newIndex, bool isNewLine = false, bool eof = false)
    {
        Success = success;
        Token = token;
        NewIndex = newIndex;
        IsNewLine = isNewLine;
        EOF = eof;
    }
}