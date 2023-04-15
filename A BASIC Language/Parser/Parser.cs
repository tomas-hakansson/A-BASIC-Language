using System.Text.RegularExpressions;

namespace A_BASIC_Language.Parsing;

partial class Parser
{
    public ParseResult Result { get; private set; } = new ParseResult();

    readonly string _source;
    readonly SortedDictionary<int, List<ABL_EvalValue>> _lines;
    int _currentLabel;
    int _index = 0;
    readonly List<(int index, string message)> _parseErrors;

    bool _parsingIf = false;
    int _generatedLabel = 0;

    public Parser(string source)
    {
        //Note: Initialisation:
        _source = source;
        _lines = new SortedDictionary<int, List<ABL_EvalValue>>();
        _parseErrors = new List<(int index, string message)>();

        //Note: The parsing begins:
        try
        {
            AProgram();
            if (_parseErrors.Any())
            {
                //ToDo: Handle parse errors
                Result.Success = false;
            }
            else
            {
                var evalValues = _lines.SelectMany(v => v.Value).ToList();
                Dictionary<int, int> labelIndex = new();
                for (int i = 0; i < evalValues.Count; i++)
                {
                    if (evalValues[i] is ABL_Label label)
                        labelIndex.Add(label.Value, i);
                }

                Result.Success = true;
                Result.EvalValues = evalValues;
                Result.LabelIndex = labelIndex;
            }
        }
        catch (Exception ex)
        {
            //this is a bug.
            var message = ex.Message;
        }
    }

    //program => line*
    private void AProgram()
    {
        bool more;
        do more = ALine();
        while (more);
    }

    //line     => aLabel oneOrMoreStatements
    //line     => aComment
    //aComment => A valid line of code starts with a positive integer
    //              followed by a letter, anything else is a comment.
    bool ALine()
    {
        if (!ALabel())
        {
            //this is a comment line. unless last line, skip to next.
            SkipLine();
            return _index < _source.Length - 1;
        }

        OneOrMoreStatements();

        return _index < _source.Length - 1;
    }

    bool ALabel()
    {
        var match = LabelRegex().Match(_source, _index);
        if (match.Success)
        {
            var matched = false;
            var mg = match.Groups.Cast<Group>().First(g => g.Name == "label");
            if (mg != null)
            {
                matched = true;
                _index += mg.Length;
                _currentLabel = int.Parse(mg.Value);
                //Note: We prime the next line by adding a new label to _lines.
                _lines.Add(_currentLabel, new List<ABL_EvalValue>() { new ABL_Label(_currentLabel) });
                SkipWhitespace();
            }
            return matched;
        }
        else return false;
    }

    //OneOrMoreStatements => aStatementWithParts (: aStatementWithParts)*
    void OneOrMoreStatements(Match? match = null)
    {
        AStatementPlusParts(match);
        while (Maybe(':'))
            AStatementPlusParts();
    }

    void AStatementPlusParts(Match? isStatement = null)
    {
        //This is the tricky part.

        isStatement ??= StatementRegex().Match(_source, _index);
        if (isStatement.Success)
        {
            var match = isStatement.Groups.Cast<Group>().First(g => g.Name == "statement");
            if (match != null)
            {
                var statement = match.Value.ToUpper();
                if (statement != "GO")
                {
                    //Note: Since 'GO' is ambiguous and can be either Go or GOTO we handle this differently.
                    _index += match.Length;
                    SkipWhitespace();
                }
                switch (statement)
                {
                    case "DIM":
                        Dim();
                        break;
                    case "END":
                        Generate(new ABL_Procedure("#END-PROGRAM"));
                        break;
                    case "GO":
                        Goto();//Note: Handles both GOTO and GO TO.
                        break;
                    case "IF":
                        //Note: Since if statements can have statements as branches their parsing must handled differently
                        //          so these flags allow the relevant code to handle these special cases.
                        //Note: This won't work for nested ifs but that will probably not cause problems.
                        _parsingIf = true;
                        If();
                        _parsingIf = false;
                        break;
                    case "INPUT":
                        Input();
                        break;
                    case "LET":
                        Let();
                        break;
                    case "PRINT":
                        Print();
                        break;
                    case "REM":
                        SkipLine();
                        break;
                    case "STOP":
                        Generate(new ABL_Procedure("#END-PROGRAM"));
                        break;
                    default:
                        throw new NotImplementedException("the given statement has not been implemented yet.");
                }
            }
            else
            {
                //something went wrong.
            }
        }
        else
            Let();
    }

    //dim => DIM unset-variable ("," unset-variable)*
    void Dim()
    {
        UnsetVariable();
        while (Maybe(','))
            UnsetVariable();

        void UnsetVariable()
        {
            var (success, fullName, _, dimVariable) = ASetVariable(true);
            if (!success)
                return;
            if (dimVariable)
                Generate(new ABL_DIM_Creation(fullName));
            else
                Generate(new ABL_Variable(fullName));
        }
    }

    void Goto()
    {
        if (!Maybe("GOTO"))
        {//Note: GO TO
            _index += 2;//Note; It would be wasteful to reparse GO just to get the length.
            SkipWhitespace();
            if (!Maybe("TO"))
            {
                ParseError("Expected 'TO' in GO TO");
                if (_parsingIf)
                    SkipStatement();
                else
                    SkipLine();
                return;
            }
        }
        Expression();
        Generate(new ABL_Procedure("GOTO"));
        if (!_parsingIf)
            SkipLine();
    }

    //if         => IF boolean-expr THEN body (ELSE body)?
    //body       => numeric-expr | statements
    //statements => statement (: statement)*
    void If()
    {
        /*
            IF expr THEN this ELSE that
            Is compiled to the following (10 and 20 are placeholder values.):
            expr #10 #IF-FALSE-GOTO this #20 GOTO 10 that 20
         */

        var falseBranchLabel = GetGeneratedLabel();
        Expression();
        Generate(new ABL_Number(falseBranchLabel), new ABL_Procedure("#IF-FALSE-GOTO"));
        if (!Maybe("THEN"))
        {
            ParseError("Expected 'THEN' statement in 'IF' statement");
            //Note: Since an if statements can have multiple statements in its bodies,
            //  each separated by ':', and since we're in the test and haven't even
            //  reached either body, SkipStatement() is the wrong choice as it could
            //  skip further into the if statement even though we've return from it.
            SkipLine();
            return;
        }
        Body();
        var endBranchLabel = GetGeneratedLabel();
        Generate(new ABL_Number(endBranchLabel), new ABL_Procedure("GOTO"));
        Generate(new ABL_Label(falseBranchLabel));

        if (Maybe("ELSE"))
            Body();

        Generate(new ABL_Label(endBranchLabel));

        void Body()
        {
            var cc = _source[_index];
            //Note: The body can contain either a statement, a variable or an expression.
            var isStatement = StatementRegex().Match(_source, _index);
            if (isStatement.Success)
                OneOrMoreStatements(isStatement);
            else
            {
                var isVariable = VariableRegex().Match(_source, _index);
                if (isVariable.Success)
                {
                    var match = isVariable.Groups.Cast<Group>().First(g => g.Name == "var");
                    if (match.Value.StartsWith("FN", StringComparison.InvariantCultureIgnoreCase))
                    {
                        //Note: this is a user defined function.
                        Expression();
                        Generate(new ABL_Procedure("GOTO"));
                    }
                    else
                    {
                        var index = GetNextNonWhitespaceIndex(startAt: _index + match.Length);

                        if (index == -1)
                        {
                            ParseError("Expected either a variable of assignment operator in 'IF' statement, got EOF");
                            return;
                        }

                        if (_source[index] == '=')
                            OneOrMoreStatements();//Note: Is assignment.
                        else
                        {
                            Expression();
                            Generate(new ABL_Procedure("GOTO"));
                        }
                    }
                }
                else if (char.IsWhiteSpace(_source[_index]))
                {
                    ParseError("Expected an IF body");
                    return;
                }
                else
                {
                    Expression();
                    Generate(new ABL_Procedure("GOTO"));
                }
            }
        }
    }

    void ParseError(string message) =>
        _parseErrors.Add((_index, message));

    //Note: Made negative because all valid BASIC labels are positive so there won't be a conflict.
    int GetGeneratedLabel() => --_generatedLabel;

    //input => INPUT (prompt-string [;])? unset-variable (, unset-variable)*
    void Input()
    {
        Prompt();
        Generate(new ABL_String("? "), new ABL_Procedure("#WRITE"));
        if (!UnsetVariable())
            return;
        while (Maybe(','))
            if (!UnsetVariable())
                return;

        void Prompt()
        {
            if (AString(out var newString))
            {
                if (!Maybe(';'))
                {
                    ParseError("Expected semicolon in 'INPUT' statement");
                    SkipStatement();
                    return;
                }
                Generate(new ABL_String(newString), new ABL_Procedure("#WRITE"));
            }
        }

        bool UnsetVariable()
        {
            var v = ASetVariable(true);
            if (!v.success)
                return false;
            var assignmentType = ASetVariable_Helper(v);
            switch (v.typeSpecifier)
            {
                case "$":
                    Generate(new ABL_Procedure("#INPUT-STRING"), assignmentType);
                    break;
                case "%":
                    Generate(new ABL_Procedure("#INPUT-INT"), assignmentType);
                    break;
                default:
                    Generate(new ABL_Procedure("#INPUT-FLOAT"), assignmentType);
                    break;
            }
            return true;
        }
    }

    //let => 'LET'? unset-variable = expression
    void Let()
    {
        Maybe("LET");
        var v = ASetVariable(true);
        if (!v.success)
            return;
        if (!Maybe('='))
        {
            ParseError("Expected equal sign in 'LET' statement");
            SkipStatement();
            return;
        }
        Expression();
        Generate(ASetVariable_Helper(v));
    }

    static ABL_EvalValue ASetVariable_Helper((bool _1, string fullName, string _2, bool dimVariable) v)
    {
        var fullName = v.fullName;
        if (v.dimVariable)
            return new ABL_DIM_Assignment(fullName);
        else
            return new ABL_Assignment(fullName);
    }

    // set-variable => name index?
    // name         => user-defined-name type-specifier?
    // index        => '(' index-value ')'
    // index-value  => expression (',' expression)*
    /// <summary>
    /// A variable with a value.
    /// </summary>
    (bool success, string fullName, string typeSpecifier, bool dimVariable) ASetVariable(bool assignmentMode = false)
    {
        /*
        What's a set variable? It's a variable that has a value, together with an optional index.
        Examples:
        x
        y(4)
        z(2,9)
        A set variable can have set variables in its indices:
        a(x, y)
        b(4, s)
        These set variables can have indices in turn:
        b(1, c(x, y))
        Naturally the depth is arbitrary.
        indices have the type int and floats that can be ints. This means that any expression which produces that can be found in the index position:
        x(1+2)
        Naturally this to can have arbitrary depth and complexity.
         */


        string fullName, typeSpecifier;
        bool dimVariable = false;
        if (!Name())
            return (false, "", "", false);
        if (!Index())
            return (false, "", "", false);
        return (true, fullName, typeSpecifier, dimVariable);

        bool Name()
        {
            fullName = string.Empty;
            typeSpecifier = string.Empty;
            var isVariable = VariableRegex().Match(_source, _index);
            if (!isVariable.Success)
            {
                ParseError($"Expected variable in {nameof(ASetVariable)}");
                SkipStatement();
                return false;
            }
            var varMatch = isVariable.Groups.Cast<Group>().First(g => g.Name == "var");
            _index += varMatch.Length;
            SkipWhitespace();

            if (OneOf(out var ts, "$", "%"))
                typeSpecifier = ts;
            fullName = varMatch.Value + typeSpecifier;
            SkipWhitespace();
            return true;
        }

        bool Index()
        {
            if (!Maybe('('))
            {
                if (!assignmentMode)
                    Generate(new ABL_Variable(fullName));
                return true;
            }
            dimVariable = true;
            Index_Value();
            if (!Maybe(')'))
            {
                ParseError($"Expected closing parenthesis sign in {nameof(ASetVariable)}");
                SkipStatement();
                return false;
            }
            if (assignmentMode)
                return true;
            Generate(new ABL_DIM_Variable(fullName));
            return true;

            void Index_Value()
            {
                Expression();
                int dimensionCount = 1;
                while (Maybe(','))
                {
                    Expression();
                    dimensionCount++;
                }
                Generate(new ABL_Number(dimensionCount));
            }
        }
    }

    //print => PRINT (, | ; | expression)*
    void Print()
    {//todo: not completely tested.

        /* Examples:
         * PRINT "IS IT A ";RIGHT$(A$(K),LEN(A$(K))-2);
         * PRINT "I WIN BY";-D;"POINTS"
         * PRINT 4 2: REM returns 42
         * PRINT 4;2: REM returns 4  2 (note two spaces, first from 4 the second from potential minus).
         */

        var shouldPrintNewline = true;
        while (true)
        {
            if (Maybe(','))
            {
                Generate(new ABL_Procedure("#NEXT-TAB-POSITION"));
                shouldPrintNewline = false;
            }
            else if (Maybe(';'))
                shouldPrintNewline = false;
            else if (_parsingIf && Maybe("ELSE", incrementIfFound: false) ||
                     _index >= _source.Length ||//EOF
                     _index < _source.Length//EOS
                        && _source[_index] == Environment.NewLine[0]
                        && Maybe(Environment.NewLine) ||
                     Maybe(':', false))//EOS
                break;
            else
            {
                Expression();
                Generate(new ABL_Procedure("#WRITE"));
                shouldPrintNewline = true;
            }
            //If the end of the file is a whitespace, this pushes the _index over EOF.
            if (_index == _source.Length - 1 && char.IsWhiteSpace(_source[_index]))
                _index++;
        }
        if (shouldPrintNewline)
            Generate(new ABL_Procedure("#NEXT-LINE"));
    }

    /// <summary>
    /// Calls the operator of the lowest precedence.
    /// </summary>
    void Expression() => Precedence_5_Operator();

    void Precedence_5_Operator()
    {
        Precedence_4_Operator();
        while (OneOf(out var op, "=", "<>", "!=", "<", ">", "<=", ">="))
        {
            Precedence_4_Operator();
            if (op == "!=")
                Generate(new ABL_Procedure("<>"));
            else
                Generate(new ABL_Procedure(op));
        }
    }

    void Precedence_4_Operator()
    {//Note: This method is Expression in Crenshaw's book.
        Precedence_3_Operator();
        while (OneOf(out var op, "+", "-"))
        {
            Precedence_3_Operator();
            Generate(new ABL_Procedure(op));
        }
    }

    void Precedence_3_Operator()
    {//Note: This method is Term in Crenshaw's book.
        Precedence_2_Operator();
        while (OneOf(out var op, "*", "/"))
        {
            Precedence_2_Operator();
            Generate(new ABL_Procedure(op));
        }
    }

    void Precedence_2_Operator()
    {//todo: negation
        Precedence_1_Operator();
    }

    void Precedence_1_Operator()
    {
        Atom();
        while (Maybe('^'))
        {
            //exponentiate
            Atom();
            Generate(new ABL_Procedure("^"));
        }
    }

    void Atom()
    {//Note: This method is Factor in Crenshaw's book.
        if (Maybe('('))
        {
            Expression();
            if (!Maybe(')'))
            {
                ParseError("Expected closing parenthesis sign in Atom");
                if (_parsingIf)
                    SkipLine();
                else
                    SkipStatement();
            }
            return;
        }
        else if (Maybe("FN"))//Note: user defined function.
        {//todo: Implement.
            var isVar = VariableRegex().Match(_source, _index);
            if (isVar.Success)
            {
                var match = isVar.Groups.Cast<Group>().First(g => g.Name == "var");
                var userDefinedFunctionName = ("FN" + match.Value);
                if (!Maybe('('))
                {
                    ParseError("Expected opening parenthesis sign in Atom");
                    if (_parsingIf)
                        SkipLine();
                    else
                        SkipStatement();
                    return;
                }
                Expression();
                if (!Maybe(')'))
                {
                    ParseError("Expected closing parenthesis sign in Atom");
                    if (_parsingIf)
                        SkipLine();
                    else
                        SkipStatement();
                    return;
                }
                //todo: generate for user defined functions.
                //Generate($"userDefinedFunction({name})");
                return;
            }
            else
            {
                ParseError("Expected user defined function in Atom");
                if (_parsingIf)
                    SkipLine();
                else
                    SkipStatement();
                return;
            }
        }
        else if (Maybe('"', false))
        {
            if (AString(out var newString))
            {
                Generate(new ABL_String(newString));
            }
            else
            {
                //ToDo: Handle error.
            }
            return;
        }

        //Note: If function
        Regex regex = new(@"\G(?<fun>ABS|ASC|ATN|CHR\$|COS|EXP|INT|LEFT\$|LEN|LOG|MID\$|RND|RIGHT\$|SGN|SIN|SQR|STR\$|TAB|TAN|VAL)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        var isFunction = regex.Match(_source, _index);
        if (isFunction.Success)
        {//e.g. sqr(n)
            var match = isFunction.Groups.Cast<Group>().First(g => g.Name == "fun");
            _index += match.Length;
            if (!Maybe('('))
            {
                ParseError("Expected opening parenthesis sign in Atom");
                if (_parsingIf)
                    SkipLine();
                else
                    SkipStatement();
                return;
            }
            Expression();
            if (!Maybe(')'))
            {
                ParseError("Expected closing parenthesis sign in Atom");
                if (_parsingIf)
                    SkipLine();
                else
                    SkipStatement();
                return;
            }
            Generate(new ABL_Procedure(match.Value));
            return;
        }

        var isVariable = VariableRegex().Match(_source, _index);
        if (isVariable.Success)
        {
            if (ASetVariable().success)
            {
                //Note: We do nothing because ASetVariable() handles everything.
            }
            else
            {
                //ToDo: Handle error.
            }
            return;
        }

        var maybeNumber = ANumber();
        if (maybeNumber == string.Empty)
        {
            ParseError("Expected a number Atom");
            if (_parsingIf)
                SkipLine();
            else
                SkipStatement();
            return;
        }
        else
            Generate(new ABL_Number(maybeNumber));
    }

    // ANumber => digit* '.'? digit+
    string ANumber()
    {
        var result = string.Empty;
        var cc = _source[_index];

        while (char.IsDigit(cc))
        {
            result += cc;
            _index++;

            // Note: Checks for eos.
            if (_index >= _source.Length)
                break;
            cc = _source[_index];
        }

        SkipWhitespace();

        if (cc == '.')
        {
            result += cc;
            _index++;

            SkipWhitespace();

            // Note: Checks for eos.
            if (_index >= _source.Length)
                return string.Empty;
            cc = _source[_index];
            if (!char.IsDigit(cc))
                return string.Empty;
            while (char.IsDigit(cc))
            {
                result += cc;
                _index++;

                // Note: Checks for eos.
                if (_index >= _source.Length)
                    break;
                cc = _source[_index];
            }
        }
        return result;
    }

    bool AString(out string value)
    {
        value = string.Empty;

        Regex regex = new(@"\G""(?<string>.*?)""", RegexOptions.CultureInvariant);
        var isString = regex.Match(_source, _index);
        if (isString.Success)
        {
            var match = isString.Groups.Cast<Group>().First(g => g.Name == "string");
            value = match.Value;
            _index += match.Length + 2;
            SkipWhitespace();
            return true;
        }
        else
            return false;

        //ToDo: Implement string parsing properly.
        //for (; _index < _source.Length; _index++)
        //{
        //    //ToDo: string parsing.
        //}

        //return result;
    }

    void Generate(params ABL_EvalValue[] values)
    {
        List<ABL_EvalValue> evalValues = new();
        foreach (ABL_EvalValue value in values)
            evalValues.Add(value);
        var v = _lines[_currentLabel];
        v.AddRange(evalValues);
        _lines[_currentLabel] = v;
    }

    /// <summary>
    /// Skips all whitespace but newline.
    /// </summary>
    void SkipWhitespace()
    {
        var index = GetNextNonWhitespaceIndex();
        if (index != -1)
            _index = index;
    }

    int GetNextNonWhitespaceIndex(int? startAt = null)
    {
        startAt ??= _index;
        if (startAt >= _source.Length - 1)
            return -1;
        for (; startAt.Value < _source.Length; startAt++)
        {
            var cc = _source[startAt.Value];

            if (!char.IsWhiteSpace(cc) ||
                cc == Environment.NewLine[0] && Maybe(Environment.NewLine, false))
                return startAt.Value;
        }
        return -1;
    }

    void SkipStatement()
    {
        for (; _index < _source.Length; _index++)
        {
            if (_index >= _source.Length - 1 ||//EOF.
                _source[_index] == Environment.NewLine[0] && Maybe(Environment.NewLine) ||//Just after newline.
                Maybe(':', false))//New statement.
                break;
        }
    }

    void SkipLine()
    {
        for (; _index < _source.Length; _index++)
        {
            if (_index >= _source.Length - 1)
                break;
            var cc = _source[_index];
            if (cc == Environment.NewLine[0] && Maybe(Environment.NewLine))
                break;
        }
    }

    /// <summary>
    /// Compares the source with the given string at the current index. Returns true and updates index if matching.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool Maybe(string value, bool skipWhitespace = true, bool incrementIfFound = true)
    {
        var rsl = _source.Length - _index; //Note: Remaining source length.
        if (rsl < value.Length) return false; //Note: If the remaining source is too short to fit the given string.

        Regex regex = new($@"\G(?<value>{value})", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        var isValue = regex.Match(_source, _index);
        if (isValue.Success)
        {
            var match = isValue.Groups.Cast<Group>().First(g => g.Name == "value");
            if (incrementIfFound)
                _index += match.Length;
            if (skipWhitespace)
                SkipWhitespace();
            return match.Success;
        }
        return false;
    }

    /// <summary>
    /// Tries to parse source at the current index with the given value. Skips whitespace after match.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool Maybe(char value, bool incrementIfFound = true)
    {
        if (_index >= _source.Length)
            return false;
        if (_source[_index] == value)
        {
            if (incrementIfFound)
                _index++;
            SkipWhitespace();
            return true;
        }
        return false;
    }

    bool OneOf(out string outValue, params string[] values)
    {
        outValue = string.Empty;
        var orderedValues = values.OrderByDescending(v => v);
        foreach (var value in orderedValues)
        {
            if (value.Length == 1)
            {
                if (Maybe(value[0]))
                {
                    outValue = value;
                    return true;
                }
            }
            else if (Maybe(value))
            {
                outValue = value;
                return true;
            }
        }
        return false;
    }

    [GeneratedRegex("\\G(?<label>\\d+)\\s*[A-Z]", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex LabelRegex();

    [GeneratedRegex("\\G(?<statement>DIM|END|GO|IF|INPUT|LET|PRINT|REM|STOP)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex StatementRegex();
    [GeneratedRegex("\\G(?<var>[A-Z][A-Z0-9]*)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex VariableRegex();
}
