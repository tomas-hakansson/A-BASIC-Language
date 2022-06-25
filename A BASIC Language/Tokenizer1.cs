// See https://aka.ms/new-console-template for more information

internal class Tokenizer1
{
    public List<List<string>> TokenizedLines { get; private set; }
    public Tokenizer1(List<string> lines)
    {
        TokenizedLines = new List<List<string>>();
        
        foreach (var line in lines)
        {
            BasicLine(line);
        }
    }

    private void BasicLine(string line)
    {
        var tokens = Tokenize(line);
        if (tokens.Count > 0)
            TokenizedLines.Add(tokens);
    }
    enum TokenType
    {
        Letter,
        Number,
        Other,
    }

    private List<string> Tokenize(string line)
    {
        List<string> result = new();
        int index = 0;
        char cc;
        for (; index < line.Length;)
        {
            cc = line[index];
            if (char.IsWhiteSpace(cc))
            {
                index++;
                continue;
            }

            if (!TokenSpecialiser()) break;
        }
        return result;

        bool TokenSpecialiser()
        {
            if (cc == '"')
                return String();
            else
                return Token();
        }

        bool String()
        {
            throw new NotImplementedException();
        }

        bool Token()
        {//Returns false if comment token is found.
            string token = string.Empty;

            TokenType type;
            if (char.IsLetter(cc))
                type = TokenType.Letter;
            else if (char.IsDigit(cc))
                type = TokenType.Number;
            else
                type = TokenType.Other;

            for (; index < line.Length; index++)
            {
                cc = line[index];
                if (cc == ' ') break;
                if (type == TokenType.Letter && !char.IsLetter(cc)) break;
                if (type == TokenType.Number && !char.IsDigit(cc)) break;//todo: only works for ints.
                token += cc;
                //Note: working under the assumption that non alphanumerics can only be one character long.
                if (type == TokenType.Other)
                {
                    index++;
                    break;
                }
            }
            token = token.ToUpper();//Note: this is for case insensitivity.
            if (token == "REM")//todo: This may not be how comments work, consult with colleague.
                return false;
            result.Add(token);
            return true;
        }
    }

}