namespace A_BASIC_Language.Stage1;

public class Tokenizer
{
    public List<List<string>> TokenizedLines { get; set; }
    public List<string> TokenizedSource { get; set; }

    readonly string _source;
    int _index = 0;
    bool _isNewLine = true;
    private bool _eof = false;
    private char _currentCharacter;
    readonly TokenizeStandard _tokenizationHelper;

    public Tokenizer(string source)
    {
        TokenizedLines = new List<List<string>>();
        TokenizedSource = new List<string>();

        _source = source;
        _currentCharacter = _source[_index];
        //todo: get the standard tokens from somewhere else.
        List<string> standardTokens = new()
        { "=", "^", "(", ")", "GOTO", "INPUT", "PRINT", "REM", "SQR" };
        _tokenizationHelper = new TokenizeStandard(_source, standardTokens);

        Tokenize();
        List<string> tokenizedLine = new();
        foreach (var token in TokenizedSource)
        {
            if (token == "\n")
            {
                TokenizedLines.Add(tokenizedLine.ToList());
                tokenizedLine.Clear();
                continue;
            }
            tokenizedLine.Add(token);
        }
        if (tokenizedLine.Count > 0)
            TokenizedLines.Add(tokenizedLine);
    }

    private void Tokenize()
    {
        SkipWhitespace();
        while (!_eof)
        {
            Line();
        }
    }

    private void Line()
    {
        if (char.IsDigit(_currentCharacter))
        {
            //this is probably a label but if the full number
            // is followed by a dot (.) then it's a comment.
            var initialIndex = _index;
            Label();
            if (_currentCharacter != '.')
            {
                _isNewLine = false;
                TheRest();//FixMe: poorly named method.
            }
            else
            {
                _index = initialIndex;
                TokenizedSource.RemoveAt(TokenizedSource.Count - 1);
                SkipLine();
            }
        }
        else
            SkipLine();
    }

    private void TheRest()
    {
        while (!_isNewLine && !_eof)
        {
            if (_currentCharacter == '"')
            {
                //todo: string tokenization.
            }
            //Note: I will for now assume that everything else is an other.
            else
            {
                Others();
            }
        }
    }

    private void Others()
    {
        var token = string.Empty;
        for (; _index < _source.Length; _index++)
        {
            var cc = char.ToUpper(_source[_index]);

            if (cc == Environment.NewLine[0])
            {
                _isNewLine = true;
                AddNewToken();
                TokenizedSource.Add("\n");
                break;
            }

            if (char.IsWhiteSpace(cc))
            {
                AddNewToken();
                break;
            }

            var readResult = _tokenizationHelper.Read(_index);
            if (readResult.Success)
            {
                _eof = readResult.EOF;
                AddNewToken();
                _isNewLine = readResult.IsNewLine;
                var standardToken = readResult.Token;
                if (standardToken == "REM")
                    SkipLine();
                TokenizedSource.Add(standardToken);
                if (_isNewLine)
                    TokenizedSource.Add("\n");
                _index = readResult.NewIndex;
                break;
            }
            else
                token += cc;
        }

        AddNewToken();
        SetCurrentCharacter();
        SkipWhitespace();

        void AddNewToken()
        {
            if (token!.Length > 0)
            {
                TokenizedSource.Add(token);
                token = string.Empty;
            }
        }
    }

    private void Label()
    {
        string label = string.Empty;
        var done = false;
        for (; _index < _source.Length; _index++)
        {
            var cc = _source[_index];
            if (!done)
            {
                if (char.IsDigit(cc))
                {
                    label += cc;
                    continue;
                }
                else
                {
                    done = true;
                    TokenizedSource.Add(label);
                }
            }

            if (cc == Environment.NewLine[0])
            {
                throw new ArgumentException("Invalid syntax: A label must be followed by something");
            }
            if (char.IsWhiteSpace(cc)) continue;
            if (done) break;
        }
        SetCurrentCharacter();
    }

    private void SkipLine()
    {
        for (; _index < _source.Length; _index++)
        {
            var cc = (_source[_index]);
            if (Environment.NewLine.Contains(cc))
                break;
        }
        _isNewLine = true;
        SkipWhitespace();
    }

    private void SkipWhitespace()
    {
        for (; _index < _source.Length; _index++)
        {
            SetCurrentCharacter();
            if (!char.IsWhiteSpace(_currentCharacter))
                break;
        }
        if (_index == _source.Length)
            _eof = true;
    }

    private void SetCurrentCharacter()
    {
        if (_index < _source.Length)
            _currentCharacter = _source[_index];
    }
}


