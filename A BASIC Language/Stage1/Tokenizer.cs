using System.Text.RegularExpressions;

namespace A_BASIC_Language.Stage1;

public class Tokenizer
{
    public List<List<string>> TokenizedLines { get; set; }
    public List<string> TokenizedSource { get; set; }

    public List<string> TokenValues { get; set; }
    public List<TokenType> TokenTypes { get; set; }
    public Result Result { get; set; }

    readonly string _source;
    string _mutableSource;
    int _index = 0;
    bool _isNewLine = true;
    private bool _eof = false;
    private char _currentCharacter;
    readonly TokenizeStandard _tokenizationHelper;

    public Tokenizer(string source)
    {
        TokenizedLines = new List<List<string>>();
        TokenizedSource = new List<string>();

        TokenValues = new List<string>();
        TokenTypes = new List<TokenType>();


        _source = source;
        _mutableSource = _source;
        _currentCharacter = _source[_index];
        ReservedWords rw = new();
        var reservedWords = rw.Operators;
        reservedWords.AddRange(rw.Punctuation);
        reservedWords.AddRange(rw.Functions);
        reservedWords.AddRange(rw.Statements);

        Result = RegexTokenizer();

        _tokenizationHelper = new TokenizeStandard(_source, reservedWords);

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

    private Result RegexTokenizer()
    {
        /* The algorirthm here is:
         * Handle each case in turn.
         * Remove the matches so that they don't interfere in the later cases.
         *      For instance: If we leave strings around then if they contain
         *       numbers that will mess with later cases that deal with that.
         * Once all cases have been handled and the matches collected we use the
         *  collected indices and lengths to determine the order of tokens and
         *  we note the different matches to ascertain the token type.
         */

        //Note: Remove non rem comments.
        var nonRemComments = @"^ (?! \s* (\d+) \s* [A-Z]) .*";
        _mutableSource = Regex.Replace(_mutableSource, nonRemComments, string.Empty,
            RegexOptions.CultureInvariant |
            RegexOptions.IgnoreCase |
            RegexOptions.IgnorePatternWhitespace |
            RegexOptions.Multiline);
        RemoveBlankLines();

        List<TokenType> tokenTypes = new();
        List<int> tokenIndices = new();
        List<int> tokenLengths = new();
        List<string> tokenValues = new();

        //Note: Get (and remove) the labels.
        var labelPattern = @"^ \s* (?<label>\d+) \s* [A-Z] .*";
        var labelMatches = Regex.Matches(_mutableSource, labelPattern,
            RegexOptions.CultureInvariant |
            RegexOptions.IgnoreCase |
            RegexOptions.IgnorePatternWhitespace |
            RegexOptions.Multiline);
        foreach (Match ml in labelMatches)
        {
            foreach (Group g in ml.Groups)
            {
                if (g.Success && g.Name == "label")
                {
                    tokenTypes.Add(TokenType.Label);
                    tokenIndices.Add(g.Index);
                    tokenLengths.Add(g.Length);
                    tokenValues.Add(g.Value);

                    //Note: I choose '¤' as a placeholder character because it's not allowed in valid
                    // BASIC code. If we ever decide that it should be allowed in ABL code the
                    // choosen placeholder character must be replaced or the logic changed.
                    string placeholders = new('¤', g.Value.Length);
                    _mutableSource = _mutableSource.Remove(g.Index, g.Length).Insert(g.Index, placeholders);
                }
            }
        }

        //Note: Get (and remove) the strings.
        var stringPattern = "\" (?<string>.*?) \"";
        var stringMatches = Regex.Matches(_mutableSource, stringPattern,
            RegexOptions.CultureInvariant |
            RegexOptions.IgnorePatternWhitespace);
        foreach (Match ms in stringMatches)
        {
            foreach (Group g in ms.Groups)
            {
                if (ms.Success)
                {
                    if (g.Name == "string")
                    {
                        tokenTypes.Add(TokenType.String);
                        tokenIndices.Add(g.Index);
                        tokenLengths.Add(g.Length);
                        tokenValues.Add(g.Value);
                    }
                    else
                    {
                        string placeholders = new('¤', g.Value.Length);
                        _mutableSource = _mutableSource.Remove(g.Index, g.Length).Insert(g.Index, placeholders);
                    }
                }
            }
        }

        //Note: Get (and remove) the reserved words.
        ReservedWords rw = new();
        var (statementIndices, statementLengths, statements) = GetReservedWords(rw.Statements);
        tokenTypes.AddRange(Enumerable.Repeat(TokenType.Statement, statementIndices.Count).ToList());
        tokenIndices.AddRange(statementIndices);
        tokenLengths.AddRange(statementLengths);
        tokenValues.AddRange(statements);

        var (operatorIndices, operatorLengths, operators) = GetReservedWords(rw.Operators);
        tokenTypes.AddRange(Enumerable.Repeat(TokenType.Operator, operatorIndices.Count).ToList());
        tokenIndices.AddRange(operatorIndices);
        tokenLengths.AddRange(operatorLengths);
        tokenValues.AddRange(operators);

        var (functionIndices, functionLengths, functions) = GetReservedWords(rw.Functions);
        tokenTypes.AddRange(Enumerable.Repeat(TokenType.Function, functionIndices.Count).ToList());
        tokenIndices.AddRange(functionIndices);
        tokenLengths.AddRange(functionLengths);
        tokenValues.AddRange(functions);

        var (punctuationIndices, punctuationLengths, punctuations) = GetReservedWords(rw.Punctuation);
        foreach (string punctuation in punctuations)
        {
            switch (punctuation)
            {
                case "(":
                    tokenTypes.Add(TokenType.OpeningParenthesis);
                    break;
                case ")":
                    tokenTypes.Add(TokenType.ClosingParenthesis);
                    break;
                case "=":
                    tokenTypes.Add(TokenType.EqualityOrAssignment);
                    break;
                case ",":
                    tokenTypes.Add(TokenType.Comma);
                    break;
                case ":":
                    tokenTypes.Add(TokenType.Colon);
                    break;
                case ";":
                    tokenTypes.Add(TokenType.Semicolon);
                    break;
                case "$":
                    tokenTypes.Add(TokenType.StringSpecifier);
                    break;
                case "%":
                    tokenTypes.Add(TokenType.IntegerSpecifier);
                    break;
                default:
                    throw new Exception("Unpecified token type");
            }
        }
        tokenIndices.AddRange(punctuationIndices);
        tokenLengths.AddRange(punctuationLengths);
        tokenValues.AddRange(punctuations);

        //Note: Get (and remove) user defined names.
        var namePattern = @"[A-Z][A-Z0-9]*";
        var (nameIndices, nameLengths, names) = GetSimpleMatch(namePattern,
            RegexOptions.CultureInvariant |
            RegexOptions.IgnoreCase |
            RegexOptions.IgnorePatternWhitespace);
        tokenTypes.AddRange(Enumerable.Repeat(TokenType.UserDefinedName, nameIndices.Count).ToList());
        tokenIndices.AddRange(nameIndices);
        tokenLengths.AddRange(nameLengths);
        tokenValues.AddRange(names);

        //Note: Get (and remove) the floats.
        var floatPattern = @"\d+ \. \d+ | \.\d+";
        var (floatIndices, floatLengths, floats) = GetSimpleMatch(floatPattern,
            RegexOptions.CultureInvariant |
            RegexOptions.IgnorePatternWhitespace);
        tokenTypes.AddRange(Enumerable.Repeat(TokenType.Float, floatIndices.Count).ToList());
        tokenIndices.AddRange(floatIndices);
        tokenLengths.AddRange(floatLengths);
        tokenValues.AddRange(floats);

        //Note: Get (and remove) the numbers.
        // (there is no way to know at this point whether this is a float or an integer).
        var intPattern = @$"\d+";
        var (intIndices, intLengths, ints) = GetSimpleMatch(intPattern,
            RegexOptions.CultureInvariant |
            RegexOptions.IgnorePatternWhitespace);
        tokenTypes.AddRange(Enumerable.Repeat(TokenType.Number, intIndices.Count).ToList());
        tokenIndices.AddRange(intIndices);
        tokenLengths.AddRange(intLengths);
        tokenValues.AddRange(ints);


        //Note: Get (and remove) space characters.
        var spacePattern = @$"\s+";

        var spaceMatches = Regex.Matches(_mutableSource, spacePattern,
            RegexOptions.CultureInvariant |
            RegexOptions.IgnorePatternWhitespace);
        foreach (Match mi in spaceMatches)
        {
            if (mi.Success)
            {
                if (!mi.Value.Contains('\n'))
                {
                    tokenTypes.Add(TokenType.Space);
                    tokenIndices.Add(mi.Index);
                    tokenLengths.Add(mi.Length);
                    tokenValues.Add(mi.Value);
                }

                string placeholders = new('¤', mi.Value.Length);
                _mutableSource = _mutableSource.Remove(mi.Index, mi.Length).Insert(mi.Index, placeholders);
            }
        }

        //Note: Put the tokens in the correct order.
        SortedDictionary<int, (TokenType, int, string)> sortedTokens = new();
        for (int i = 0; i < tokenIndices.Count; i++)
        {
            var value = (tokenTypes[i], tokenLengths[i], tokenValues[i]);
            sortedTokens.Add(tokenIndices[i], value);
        }

        //Note: Put the lines in the correct order.
        var isBeginning = true;
        SortedDictionary<string, (List<string>, List<TokenType>)> sortedLines = new();
        List<string> lineValues = new();
        List<TokenType> lineTypes = new();
        foreach ((_, (TokenType type, _, string value)) in sortedTokens)
        {
            if (isBeginning)
            {
                if (type == TokenType.Space)
                    continue;
                else
                    isBeginning = false;
            }

            if (type == TokenType.Label)
            {
                if (lineValues.Count > 0)
                    sortedLines.Add(lineValues.First(), (lineValues, lineTypes));
                lineValues = new();
                lineTypes = new();
            }
            lineValues.Add(value);
            lineTypes.Add(type);
        }
        if (lineValues.Count > 0)
        {
            sortedLines.Add(lineValues.First(), (lineValues, lineTypes));
        }

        //Note: Put the lime in the coconut/ Populate the result.
        Result result = new();
        foreach ((_, (List<string> values, List<TokenType> types)) in sortedLines)
        {
            Line line = new()
            {
                TokenValues = values,
                TokenTypes = types
            };
            result.Add(line);
            TokenValues.AddRange(values);
            TokenTypes.AddRange(types);
        }
        return result;
    }

    private (List<int> indices, List<int> lengths, List<string> values) GetSimpleMatch(string pattern, RegexOptions regexOption)
    {
        var matches = Regex.Matches(_mutableSource, pattern, regexOption);
        List<int> indices = new();
        List<int> lengths = new();
        List<string> values = new();
        foreach (Match m in matches)
        {
            if (m.Success)
            {
                indices.Add(m.Index);
                lengths.Add(m.Length);
                values.Add(m.Value);

                //Note: I choose '¤' as a placeholder character because it's not allowed in valid
                // BASIC code. If we ever decide that it should be allowed in ABL code the
                // choosen placeholder character must be replaced or the logic changed.
                string placeholders = new('¤', m.Value.Length);
                _mutableSource = _mutableSource.Remove(m.Index, m.Length).Insert(m.Index, placeholders);
            }
        }
        return (indices, lengths, values);
    }

    private void RemoveBlankLines()
    {
        using StringReader reader = new(_mutableSource);
        using StringWriter writer = new();
        string line;
        while ((line = reader.ReadLine()!) != null)
        {
            if (line.Trim().Length > 0)
                writer.WriteLine(line);
        }
        _mutableSource = writer.ToString().Trim();
    }

    private (List<int> indices, List<int> lengths, List<string> values) GetReservedWords(List<string> words)
    {
        var aWord = string.Empty;
        foreach (var word in words)
        {
            //., +, *, ?, ^, $, (, ), [, ], {, }, |, \
            if ("()$+*^/".Contains(word))
            {
                aWord += "\\";
            }
            aWord += word + "|";
        }
        aWord = aWord.Remove(aWord.Length - 1);
        var matches = Regex.Matches(_mutableSource, aWord,
            RegexOptions.CultureInvariant |
            RegexOptions.IgnoreCase);
        List<int> indices = new();
        List<int> lengths = new();//not sure I need this.
        List<string> values = new();
        foreach (Match m in matches)
        {
            if (!m.Success || m.Captures.Count != 1)
                throw new Exception("this is a tokenizer bug, sorry.");
            indices.Add(m.Index);
            lengths.Add(m.Length);
            values.Add(m.Value);

            //Note: I choose '¤' as a placeholder character because it's not allowed in valid
            // BASIC code. If we ever decide that it should be allowed in ABL code the
            // choosen placeholder character must be replaced or the logic changed.
            string placeholders = new('¤', m.Value.Length);
            _mutableSource = _mutableSource.Remove(m.Index, m.Length).Insert(m.Index, placeholders);
        }
        return (indices, lengths, values);
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
        if (_index < _source.Length && _index >= 0)
            _currentCharacter = _source[_index];
    }
}