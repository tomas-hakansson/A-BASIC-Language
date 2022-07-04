namespace A_BASIC_Language.Stage0;

public class TokenizeStandard
{
    readonly string _source;
    readonly List<string> _standardTokens;

    string _token;
    List<string> _potentialMatches;
    int _matchIndex;

    public TokenizeStandard(string source, List<string> standardTokens)
    {
        _source = source;
        _standardTokens = standardTokens;

        //Note: the number of potential matches can never be greater
        // than the number of standard tokens.
        _token = string.Empty;
        _potentialMatches = new List<string>(_standardTokens.Count);
    }

    public TokenizationResult Read(int index)
    {
        _potentialMatches = _standardTokens;
        _matchIndex = 0;
        _token = String.Empty;

        var initialIndex = index;
        int indexIncrement = -1;
        bool done = false;
        bool isNewLine = false;
        for (; index < _source.Length; index++)
        {
            indexIncrement++;
            var cc = char.ToUpper(_source[index]);

            if (cc == Environment.NewLine[0])
                isNewLine = true;
            if (char.IsWhiteSpace(cc))
                continue;
            if (done)
                break;
            if (!done)
            {
                if (FindMatch(cc))
                {
                    done = true;
                    continue;
                }

                if (_potentialMatches.Count == 0)
                    return new(false, string.Empty, index);
            }
        }
        if (index == _source.Length)
            return new(true, _token, -1, eof: true);
        return new(true, _token, initialIndex + indexIncrement, isNewLine: isNewLine);
    }

    private bool FindMatch(char tokenFragment)
    {
        List<string> matches = new();
        foreach (var token in _potentialMatches)
        {
            if (_matchIndex >= token.Length)
                continue;
            if (tokenFragment == token[_matchIndex])
                matches.Add(token);
        }
        _potentialMatches = matches;
        if (_potentialMatches.Count > 0)
            _token += tokenFragment;
        if (_potentialMatches.Count == 1 && _token.Length == _potentialMatches.First().Length)
            return true;
        _matchIndex++;
        return false;
    }
}

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