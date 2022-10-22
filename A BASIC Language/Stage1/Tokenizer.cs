using System.Text.RegularExpressions;

namespace A_BASIC_Language.Stage1;

class Tokenizer
{
    public TokenizeResult Result { get; set; }

    string _source;

    public Tokenizer(string source)
    {
        Result = new TokenizeResult();
        _source = source;

        RegexTokenizer();
    }

    private void RegexTokenizer()
    {
        /* The algorithm here is:
         * Handle each case in turn.
         * Remove the matches so that they don't interfere in the later cases.
         *      For instance: If we leave strings around then if they contain
         *       numbers that will mess with later cases that deal with that.
         * Once all cases have been handled and the matches collected we use the
         *  collected indices and lengths to determine the order of tokens and
         *  we note the different matches to ascertain the token type.
         */

        //Note: Remove non rem comments.
        //todo: modify to allow for correct line and column numbers.
        var nonRemComments = @"^ (?! \s* (\d+) \s* [A-Z]) .*";
        _source = Regex.Replace(_source, nonRemComments, string.Empty,
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
        var labelMatches = Regex.Matches(_source, labelPattern,
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
                    _source = _source.Remove(g.Index, g.Length).Insert(g.Index, placeholders);
                }
            }
        }

        //Note: Get (and remove) the strings.
        var stringPattern = "\" (?<string>.*?) \"";
        var stringMatches = Regex.Matches(_source, stringPattern,
            RegexOptions.CultureInvariant |
            RegexOptions.IgnorePatternWhitespace);
        foreach (Match ms in stringMatches)
        {
            foreach (Group g in ms.Groups)
            {
                if (!ms.Success)
                    continue;

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
                    _source = _source.Remove(g.Index, g.Length).Insert(g.Index, placeholders);
                }
            }
        }

        //Note: Get (and remove) the reserved words.
        ReservedWords rw = new();
        var (statementIndices, statementLengths, statements) = GetReservedWords(rw.Statements);
        tokenTypes.AddRange(Enumerable.Repeat(TokenType.Statement, statementIndices.Count).ToList());
        tokenIndices.AddRange(statementIndices);
        tokenLengths.AddRange(statementLengths);
        tokenValues.AddRange(statements.Select(s => s.ToUpper()));

        var (operatorIndices, operatorLengths, operators) = GetReservedWords(rw.Operators);
        tokenTypes.AddRange(Enumerable.Repeat(TokenType.Operator, operatorIndices.Count).ToList());
        tokenIndices.AddRange(operatorIndices);
        tokenLengths.AddRange(operatorLengths);
        tokenValues.AddRange(operators.Select(o => o.ToUpper()));

        var (functionIndices, functionLengths, functions) = GetReservedWords(rw.Functions);
        tokenTypes.AddRange(Enumerable.Repeat(TokenType.Function, functionIndices.Count).ToList());
        tokenIndices.AddRange(functionIndices);
        tokenLengths.AddRange(functionLengths);
        tokenValues.AddRange(functions.Select(f => f.ToUpper()));

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
                    tokenTypes.Add(TokenType.TypeSpecifier);
                    break;
                case "%":
                    tokenTypes.Add(TokenType.TypeSpecifier);
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
        tokenValues.AddRange(names.Select(n => n.ToUpper()));

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

        /* todo: if the numbers are seperated solely by spaces then merge them (examples below):
             4 2 == 42
             4.3 3 == 4.33
            not these ones:
             4.3 2.9 != 4.32.9
             (can't think of other examples)
            3 .2, 3. 2 and 3 . 2 should also be merged but that would require more changes.
         */

        //Note: Get (and remove) space characters.
        var spacePattern = @$"\s+";

        var spaceMatches = Regex.Matches(_source, spacePattern,
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
                _source = _source.Remove(mi.Index, mi.Length).Insert(mi.Index, placeholders);
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
        //Note: Label is parsed to int for correct ordering.
        var isBeginning = true;
        SortedDictionary<int, (List<string>, List<TokenType>)> sortedLines = new();
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
                    sortedLines.Add(int.Parse(lineValues.First()), (lineValues, lineTypes));
                lineValues = new();
                lineTypes = new();
            }
            lineValues.Add(value);
            lineTypes.Add(type);
        }
        if (lineValues.Count > 0)
        {
            sortedLines.Add(int.Parse(lineValues.First()), (lineValues, lineTypes));
        }

        //Note: Put the lime in the coconut/ Populate the result.
        foreach ((_, (List<string> values, List<TokenType> types)) in sortedLines)
        {
            Result.AddMany(values, types);
        }

        //Note: add end of line token for parsing:
        Result.Add(string.Empty, TokenType.EOF);
    }

    private (List<int> indices, List<int> lengths, List<string> values) GetSimpleMatch(string pattern, RegexOptions regexOption)
    {
        var matches = Regex.Matches(_source, pattern, regexOption);
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
                _source = _source.Remove(m.Index, m.Length).Insert(m.Index, placeholders);
            }
        }
        return (indices, lengths, values);
    }

    private void RemoveBlankLines()
    {
        using StringReader reader = new(_source);
        using StringWriter writer = new();
        string line;
        while ((line = reader.ReadLine()!) != null)
        {
            if (line.Trim().Length > 0)
                writer.WriteLine(line);
        }
        _source = writer.ToString().Trim();
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

        var matches = Regex.Matches(_source, aWord,
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
            _source = _source.Remove(m.Index, m.Length).Insert(m.Index, placeholders);
        }

        return (indices, lengths, values);
    }
}