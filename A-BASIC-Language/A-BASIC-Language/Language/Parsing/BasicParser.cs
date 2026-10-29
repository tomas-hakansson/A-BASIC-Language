#nullable enable
using A_BASIC_Language.Language.LanguageParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using A_BASIC_Language.Language.LoopInstructions;

namespace A_BASIC_Language.Language.Parsing;

public class BasicParser
{
    private readonly string _source;
    private readonly bool _direct;
    private readonly SortedDictionary<int, List<AblEvalValue>> _lines;
    private int _currentLabel;
    private int _index;
    private readonly List<(int index, string message)> _parseErrors;
    private bool _parsingIf;
    private int? _onJumpKeywordIndex;
    private bool _parsingIfCondition;
    private int _generatedLabel;
    private List<AblEvalValue>? _expressionTarget;
    public ParseResult Result { get; }

    public BasicParser(string source, bool direct = false)
    {
        Result = new ParseResult();
        _direct = direct;
        //Note: Initialisation:
        // Normalize once: every parser routine sees a single newline convention.
        source = source.Replace("\r\n", "\n").Replace('\r', '\n');
        _source = direct ? "0 " + source : source;
        _lines = new SortedDictionary<int, List<AblEvalValue>>();
        _parseErrors = new List<(int index, string message)>();

        //Note: The parsing begins:
        try
        {
            AProgram();
            if (_parseErrors.Any())
            {
                Result.Success = false;
                Result.Errors.AddRange(_parseErrors.Select(e => e.message));
            }
            else
            {
                var evalValues = _lines.SelectMany(v => v.Value).ToList();
                Dictionary<int, int> labelIndex = new();
                for (int i = 0; i < evalValues.Count; i++)
                {
                    if (evalValues[i] is AblLabel label)
                        labelIndex.Add(label.Value, i);
                }

                Result.Success = true;
                Result.EvalValues = evalValues;
                Result.LabelIndex = labelIndex;
            }
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or FormatException or OverflowException or IndexOutOfRangeException or NotImplementedException)
        {
            Result.Success = false;
            Result.Errors.Add(FormatDiagnostic(ex.Message));
        }
    }

    //program => line*
    private void AProgram()
    {
        while (_index < _source.Length)
        {
            SkipWhitespace();
            if (Maybe('\n'))
                continue;
            if (_index >= _source.Length)
                break;
            if (!ALabel())
            {
                ParseError("Expected a numbered BASIC line");
                SkipLine();
                continue;
            }
            OneOrMoreStatements();
            SkipWhitespace();
            if (_index < _source.Length && _source[_index] != '\n')
            {
                ParseError("Unexpected text after statement");
                SkipLine();
            }
        }
    }

    private bool ALabel()
    {
        Regex isLabel = new(@"\G(?<label>\d+)\s*[A-Za-z]", RegexOptions.CultureInvariant);
        var match = isLabel.Match(_source, _index);

        if (!match.Success)
            return false;

        var matched = false;
        var mg = match.Groups.Cast<Group>().First(g => g.Name == "label");

        if (mg == null)
            return matched;

        matched = true;
        _index += mg.Length;
        _currentLabel = int.Parse(mg.Value);
        //Note: We prime the next line by adding a new label to _lines.
        _lines.Add(_currentLabel, new List<AblEvalValue>() { new AblLabel(_currentLabel) });
        SkipWhitespace();
        return matched;
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
                    case "DEF":
                        DefineFunction();
                        break;
                    case "FOR":
                        For();
                        break;
                    case "NEXT":
                        Next();
                        break;
                    case "DIM":
                        Dim();
                        break;
                    case "RETURN":
                        Generate(new AblProcedure("RETURN"));
                        break;
                    case "END":
                        Generate(new AblProcedure("#END-PROGRAM"));
                        break;
                    case "GO":
                        Goto();// Handles GOTO, GO TO, GOSUB and GO SUB.
                        break;
                    case "ON":
                        OnGoto();
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
                    case "WINDOW":
                        Window();
                        break;
                    case "COLOR":
                        Color();
                        break;
                    case "RANDOMIZE":
                        Generate(new AblProcedure("RANDOMIZE"));
                        break;
                    case "SAVE":
                        if (!_direct)
                        {
                            ParseError("SAVE is only allowed in direct mode");
                            SkipStatement();
                            break;
                        }
                        Expression();
                        Generate(new AblProcedure("SAVE"));
                        break;
                    case "REM":
                        SkipLine();
                        break;
                    case "STOP":
                        Generate(new AblProcedure("#END-PROGRAM"));
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
        else if (LooksLikeAssignment())
            Let();
        else
        {
            var name = VariableRegex().Match(_source, _index);
            ParseError(name.Success
                ? $"Unknown statement '{name.Value}'"
                : "Expected a BASIC statement or variable assignment");
            SkipStatement();
        }
    }

    // Look ahead without consuming input or generating instructions. LET may be
    // omitted only when a scalar or indexed variable is followed by '='.
    private bool LooksLikeAssignment()
    {
        var name = VariableRegex().Match(_source, _index);
        
        if (!name.Success)
            return false;
        
        var position = _index + name.Length;
        SkipSpaces();
        
        if (position < _source.Length && (_source[position] == '$' || _source[position] == '%'))
            position++;
        
        SkipSpaces();

        if (position < _source.Length && _source[position] == '(')
        {
            var depth = 0;
            var quoted = false;
            do
            {
                var c = _source[position++];
                if (c == '\n') return false;
                if (c == '"') quoted = !quoted;
                if (!quoted)
                {
                    if (c == ':') return false;
                    if (c == '(') depth++;
                    if (c == ')') depth--;
                }
            } while (position < _source.Length && depth > 0);
            
            if (depth != 0 || quoted)
                return false;
            
            SkipSpaces();
        }
        
        return position < _source.Length && _source[position] == '=';

        void SkipSpaces()
        {
            while (position < _source.Length && _source[position] != '\n' && char.IsWhiteSpace(_source[position]))
                position++;
        }
    }

    private void For()
    {
        var variable = ASetVariable(true);
        
        if (!variable.Success || variable.DimVariable || variable.TypeSpecifier == "$")
        {
            ParseError("FOR requires a numeric scalar variable");
            SkipStatement();
            return;
        }

        if (!Maybe('='))
        {
            ParseError("Expected = after FOR variable");
            SkipStatement();
            return;
        }
        
        Expression();

        if (!Maybe("TO"))
        {
            ParseError("Expected TO in FOR");
            SkipStatement();
            return;
        }
        
        Expression();
        
        if
            (Maybe("STEP")) Expression();
        else
            Generate(new AblNumber(1));
        
        Generate(new AblFor(variable.FullName));
    }

    private void Next()
    {
        do
        {
            var match = VariableRegex().Match(_source, _index);
            var name = "";

            if (match.Success)
            {
                name = match.Groups["var"].Value;
                _index += name.Length;
                
                if (Maybe('%'))
                    name += "%";

                SkipWhitespace();
            }

            Generate(new AblNext(name));
        } while (Maybe(','));
    }

    //dim => DIM unset-variable ("," unset-variable)*
    private void Dim()
    {
        UnsetVariable();

        while (Maybe(','))
            UnsetVariable();

        return;

        void UnsetVariable()
        {
            var result = ASetVariable(true);
            
            if (!result.Success)
                return;
            
            if (result.DimVariable)
                Generate(new AblDimCreation(result.FullName));
            else
                Generate(new AblVariable(result.FullName));
        }
    }

    private void Goto()
    {
        var gosub = JumpKeyword();
        Expression();
        Generate(new AblProcedure(gosub ? "GOSUB" : "GOTO"));
    }

    // Shared by ordinary jumps and ON dispatch, including the spaced spellings.
    private bool JumpKeyword()
    {
        if (Maybe("GOSUB"))
            return true;
        if (Maybe("GOTO"))
            return false;
        if (Maybe("GO"))
        {
            if (Maybe("SUB"))
                return true;
            if (Maybe("TO"))
                return false;
        }
        throw new InvalidOperationException("Expected GOTO, GO TO, GOSUB or GO SUB.");
    }
    // ON expression (GOTO | GOSUB) line-number (, line-number)*
    private void OnGoto()
    {
        if (_direct)
        {
            ParseError("ON GOTO/GOSUB is only allowed in numbered program lines");
            SkipStatement();
            return;
        }

        // Bound variable names in compact selectors such as IGOTO2300.
        var previousBoundary = _onJumpKeywordIndex;
        _onJumpKeywordIndex = FindOnJumpKeyword();

        try
        {
            Expression();
        }
        finally
        {
            _onJumpKeywordIndex = previousBoundary;
        }

        var gosub = JumpKeyword();
        var targets = new List<int>();
        
        do
        {
            var target = Regex.Match(_source.Substring(_index), @"^[0-9]+");
        
            if (!target.Success || !int.TryParse(target.Value, out var lineNumber))
                throw new InvalidOperationException("Expected a line number in ON jump list.");

            targets.Add(lineNumber);
            _index += target.Length;
            SkipWhitespace();
        } while (Maybe(','));

        Generate(new AblOnGoto(targets, gosub));
    }

    private int? FindOnJumpKeyword()
    {
        var depth = 0;
        var inString = false;

        var keyword = new Regex(@"\GGO[ \t]*(?:TO|SUB)(?=[ \t]*[0-9])",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        for (var i = _index; i < _source.Length; i++)
        {
            var c = _source[i];

            if (c == '\n')
                break;

            if (c == '"')
                inString = !inString;

            if (inString)
                continue;

            if (c == ':')
                break;

            if (c == '(')
                depth++;
            else if (c == ')')
                depth--;
            else if (depth == 0 && keyword.IsMatch(_source, i))
                return i;
        }

        return null;
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
        var previousCondition = _parsingIfCondition;
        _parsingIfCondition = true;
        try
        {
            Expression();
        }
        finally
        {
            _parsingIfCondition = previousCondition;
        }
        Generate(new AblNumber(falseBranchLabel), new AblProcedure("#IF-FALSE-GOTO"));
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
        Generate(new AblNumber(endBranchLabel), new AblProcedure("GOTO"));
        Generate(new AblLabel(falseBranchLabel));

        if (Maybe("ELSE"))
            Body();

        Generate(new AblLabel(endBranchLabel));
        return;

        void Body()
        {
            //Note: The body can contain either a statement, a variable or an expression.
            var isStatement = StatementRegex().Match(_source, _index);

            if (isStatement.Success)
            {
                OneOrMoreStatements(isStatement);
            }
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
                        Generate(new AblProcedure("GOTO"));
                    }
                    else
                    {
                        var index = GetNextNonWhitespaceIndex(startAt: _index + match.Length);

                        if (index == -1)
                        {
                            ParseError("Expected either a variable of assignment operator in 'IF' statement, got EOF");
                            return;
                        }

                        // An array reference may be an assignment or an implicit jump target.
                        if (_source[index] == '(')
                        {
                            var depth = 0;
                            var inString = false;
                            for (var i = index; i < _source.Length && _source[i] != '\n'; i++)
                            {
                                if (_source[i] == '"')
                                    inString = !inString;
                                if (inString)
                                    continue;
                                if (_source[i] == '(')
                                    depth++;
                                else if (_source[i] == ')' && --depth == 0)
                                {
                                    index = GetNextNonWhitespaceIndex(i + 1);
                                    break;
                                }
                            }
                        }

                        if (index >= 0 && _source[index] == '=')
                            OneOrMoreStatements();//Note: Is assignment.
                        else
                        {
                            Expression();
                            Generate(new AblProcedure("GOTO"));
                        }
                    }
                }
                else if (char.IsWhiteSpace(_source[_index]))
                {
                    ParseError("Expected an IF body");
                }
                else
                {
                    Expression();
                    Generate(new AblProcedure("GOTO"));
                }
            }
        }
    }

    void ParseError(string message) =>
        _parseErrors.Add((_index, FormatDiagnostic(message)));

    private string FormatDiagnostic(string message)
    {
        // Direct commands have a synthetic label used only for parsing.
        if (_direct) return message;
        var column = _index - _source.LastIndexOf('\n', Math.Max(0, _index - 1));
        return $"Line {_currentLabel}, column {column}: {message}";
    }

    //Note: Made negative because all valid BASIC labels are positive so there won't be a conflict.
    int GetGeneratedLabel() => --_generatedLabel;

    //input => INPUT (prompt-string [;])? unset-variable (, unset-variable)*
    private void Input()
    {
        Prompt();
        Generate(new AblString("? "), new AblProcedure("#WRITE"));
        
        if (!UnsetVariable())
            return;
        
        while (Maybe(','))
        {
            if (!UnsetVariable())
                return;
        }

        void Prompt()
        {
            if (!AString(out var newString))
                return;

            if (!Maybe(';'))
            {
                ParseError("Expected semicolon in 'INPUT' statement");
                SkipStatement();
                return;
            }

            Generate(new AblString(newString), new AblProcedure("#WRITE"));
        }

        bool UnsetVariable()
        {
            var v = ASetVariable(true);
            
            if (!v.Success)
                return false;
            
            var assignmentType = ASetVariable_Helper(v);
            
            switch (v.TypeSpecifier)
            {
                case "$":
                    Generate(new AblProcedure("#INPUT-STRING"), assignmentType);
                    break;
                case "%":
                    Generate(new AblProcedure("#INPUT-INT"), assignmentType);
                    break;
                default:
                    Generate(new AblProcedure("#INPUT-FLOAT"), assignmentType);
                    break;
            }
            return true;
        }
    }

    //let => 'LET'? unset-variable = expression
    private void Let()
    {
        var v = ASetVariable(true);

        if (!v.Success)
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

    static AblEvalValue ASetVariable_Helper(ParseResultSet r)
    {
        var fullName = r.FullName;

        if (r.DimVariable)
            return new AblDimAssignment(fullName);
        
        return new AblAssignment(fullName);
    }

    // set-variable => name index?
    // name         => user-defined-name type-specifier?
    // index        => '(' index-value ')'
    // index-value  => expression (',' expression)*
    /// <summary>
    /// A variable with a value.
    /// </summary>
    private ParseResultSet ASetVariable(bool assignmentMode = false)
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
        var dimVariable = false;

        if (!Name() || !Index())
            return new ParseResultSet(false, "", "", false);
        
        return new ParseResultSet(true, fullName, typeSpecifier, dimVariable);

        bool Name()
        {
            fullName = string.Empty;
            typeSpecifier = string.Empty;

            var isVariable = _onJumpKeywordIndex is { } boundary && boundary >= _index
                ? VariableRegex().Match(_source, _index, boundary - _index)
                : VariableRegex().Match(_source, _index);

            if (_parsingIfCondition && isVariable.Success)
            {
                var keyword = Regex.Match(isVariable.Value, "AND|OR|THEN",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

                if (keyword.Success)
                    isVariable = VariableRegex().Match(_source, _index, keyword.Index);
            }
            
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
                    Generate(new AblVariable(fullName));
                return true;
            }
            dimVariable = true;
            IndexValue();
            if (!Maybe(')'))
            {
                ParseError($"Expected closing parenthesis sign in {nameof(ASetVariable)}");
                SkipStatement();
                return false;
            }
            if (assignmentMode)
                return true;
            Generate(new AblDimVariable(fullName));
            return true;

            void IndexValue()
            {
                Expression();
                var dimensionCount = 1;

                while (Maybe(','))
                {
                    Expression();
                    dimensionCount++;
                }

                Generate(new AblNumber(dimensionCount));
            }
        }
    }

    // COLOR target, color: both arguments are required numeric expressions.
    void Color()
    {
        Expression();
        if (!Maybe(','))
            throw new InvalidOperationException("Expected comma between COLOR arguments.");
        Expression();
        Generate(new AblProcedure("COLOR"));
    }

    // WINDOW [expression]: the argument is the number of text rows at the bottom.
    void Window()
    {
        if (_index >= _source.Length || _source[_index] == '\n' ||
            Maybe(':', false) || (_parsingIf && Maybe("ELSE", incrementIfFound: false)))
            Generate(new AblNumber(25));
        else
            Expression();
        Generate(new AblProcedure("WINDOW"));
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
            var previous = _index;
            if (Maybe(','))
            {
                Generate(new AblProcedure("#NEXT-TAB-POSITION"));
                shouldPrintNewline = false;
            }
            else if (Maybe(';'))
                shouldPrintNewline = false;
            else if (_parsingIf && Maybe("ELSE", incrementIfFound: false) ||
                     _index >= _source.Length ||//EOF
                     _index < _source.Length//EOS
                        && _source[_index] == '\n'
                         ||
                     Maybe(':', false))//EOS
                break;
            else
            {
                Expression();
                Generate(new AblProcedure("#WRITE"));
                shouldPrintNewline = true;
            }
            if (_index == previous)
            {
                ParseError("Expected PRINT expression or separator");
                SkipStatement();
                break;
            }
            //If the end of the file is a whitespace, this pushes the _index over EOF.
            if (_index == _source.Length - 1 && char.IsWhiteSpace(_source[_index]))
                _index++;
        }
        if (shouldPrintNewline)
            Generate(new AblProcedure("#NEXT-LINE"));
    }

    void DefineFunction()
    {
        if (!Maybe("FN")) throw new InvalidOperationException("Expected FN after DEF.");
        var name = ReadFunctionSymbol();
        if (!Maybe('(')) throw new InvalidOperationException("Expected function parameter.");
        var parameter = ReadFunctionSymbol();
        if (!Maybe(')') || !Maybe('=')) throw new InvalidOperationException("Expected )= in DEF FN.");
        var body = new List<AblEvalValue>();
        // Compile the body once, using the ordinary expression grammar.
        _expressionTarget = body;
        try { Expression(); }
        finally { _expressionTarget = null; }
        Generate(new AblFunctionDefinition(name, parameter, body));
    }

    string ReadFunctionSymbol()
    {
        var match = VariableRegex().Match(_source, _index);
        if (!match.Success) throw new InvalidOperationException("Expected numeric function name or parameter.");
        _index += match.Length;
        SkipWhitespace();
        return match.Value.ToUpperInvariant();
    }

    /// <summary>Calls the operator of the lowest precedence.</summary>
    void Expression()
    {
        AndExpression();
        while (Maybe("OR"))
        {
            AndExpression();
            Generate(new AblProcedure("OR"));
        }
    }

    void AndExpression()
    {
        Precedence_5_Operator();

        while (Maybe("AND"))
        {
            Precedence_5_Operator();
            Generate(new AblProcedure("AND"));
        }
    }

    void Precedence_5_Operator()
    {
        Precedence_4_Operator();
        while (OneOf(out var op, "=", "<>", "!=", "<", ">", "<=", ">="))
        {
            Precedence_4_Operator();
            if (op == "!=")
                Generate(new AblProcedure("<>"));
            else
                Generate(new AblProcedure(op));
        }
    }

    void Precedence_4_Operator()
    {//Note: This method is Expression in Crenshaw's book.
        Precedence_3_Operator();
        while (OneOf(out var op, "+", "-"))
        {
            Precedence_3_Operator();
            Generate(new AblProcedure(op));
        }
    }

    void Precedence_3_Operator()
    {//Note: This method is Term in Crenshaw's book.
        Precedence_2_Operator();
        while (OneOf(out var op, "*", "/"))
        {
            Precedence_2_Operator();
            Generate(new AblProcedure(op));
        }
    }

    void Precedence_2_Operator()
    {
        if (Maybe('-'))
        {
            Precedence_2_Operator();
            Generate(new AblProcedure("#NEGATE"));
        }
        else if (Maybe('+'))
            Precedence_2_Operator();
        else
            Precedence_1_Operator();
    }

    void Precedence_1_Operator()
    {
        Atom();
        while (Maybe('^'))
        {
            // Exponentiation binds more tightly than unary minus.
            Precedence_2_Operator();
            Generate(new AblProcedure("^"));
        }
    }

    void Atom()
    {//Note: This method is Factor in Crenshaw's book.
        if (_index >= _source.Length || _source[_index] == '\n')
        {
            ParseError("Expected an expression");
            return;
        }
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
        else
        {
            if (Maybe("FN"))//Note: user defined function.
            {
                var name = ReadFunctionSymbol();
                if (!Maybe('(')) throw new InvalidOperationException("Expected ( in FN call.");
                Expression();
                if (!Maybe(')')) throw new InvalidOperationException("Expected ) in FN call.");
                Generate(new AblFunctionCall(name));
                return;
            }
            if (Maybe('"', false))
            {
                if (AString(out var newString))
                {
                    Generate(new AblString(newString));
                }
                else
                {
                    ParseError("Unterminated string");
                    SkipStatement();
                }

                return;
            }
        }

        //Note: If function
        Regex regex = new(@"\G(?<fun>ABS|ASC|ATN|CHR\$|COS|EXP|INT|LEFT\$|LEN|LOG|MID\$|RND|RIGHT\$|SGN|SIN|SQR|STR\$|TAB|TAN|VAL)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        var isFunction = regex.Match(_source, _index);

        if (isFunction.Success)
        {//e.g. sqr(n)
            var match = isFunction.Groups.Cast<Group>().First(g => g.Name == "fun");
            _index += match.Length;
            var functionName = match.Value.ToUpperInvariant();
            SkipWhitespace();

            if (!Maybe('('))
            {
                // Bare RND uses the same default argument as RND(1).
                if (functionName == "RND")
                {
                    Generate(new AblNumber(1), new AblProcedure("RND"));
                    return;
                }

                ParseError("Expected opening parenthesis sign in Atom");

                if (_parsingIf)
                    SkipLine();
                else
                    SkipStatement();
                return;
            }

            Expression();

            if (functionName == "LEFT$" || functionName == "RIGHT$" || functionName == "MID$")
            {
                if (!Maybe(','))
                    throw new InvalidOperationException($"Expected a second argument in {functionName}.");
                Expression();
                if (functionName == "MID$")
                {
                    if (Maybe(','))
                        Expression();
                    else
                        Generate(new AblNumber(int.MaxValue));
                }
            }

            if (!Maybe(')'))
            {
                ParseError("Expected closing parenthesis sign in Atom");
                if (_parsingIf)
                    SkipLine();
                else
                    SkipStatement();

                return;
            }

            Generate(new AblProcedure(functionName));
            return;
        }

        var isVariable = VariableRegex().Match(_source, _index);

        if (isVariable.Success)
        {
            if (ASetVariable().Success)
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
        }
        else
        {
            Generate(new AblNumber(maybeNumber));
        }
    }

    // ANumber => digit* '.'? digit+
    string ANumber()
    {
        var match = Regex.Match(_source.Substring(_index, _source.Length - _index), @"^(?:[0-9]+(?:\.[0-9]*)?|\.[0-9]+)(?:[Ee][+-]?[0-9]+)?");
        
        if (!match.Success)
            return "";
        
        _index += match.Length;
        SkipWhitespace();
        return match.Value;
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
        
        return false;

        //ToDo: Implement string parsing properly.
        //for (; _index < _source.Length; _index++)
        //{
        //    //ToDo: string parsing.
        //}

        //return result;
    }

    private void Generate(params AblEvalValue[] values)
    {
        if (_expressionTarget != null)
        {
            _expressionTarget.AddRange(values);
            return;
        }

        List<AblEvalValue> evalValues = [];

        foreach (AblEvalValue value in values)
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
        while (_index < _source.Length && _source[_index] != '\n' && char.IsWhiteSpace(_source[_index]))
            _index++;
    }

    int GetNextNonWhitespaceIndex(int? startAt = null)
    {
        var index = startAt ?? _index;
        while (index < _source.Length && _source[index] != '\n' && char.IsWhiteSpace(_source[index]))
            index++;
        return index < _source.Length ? index : -1;
    }

    void SkipStatement()
    {
        while (_index < _source.Length && _source[_index] != '\n' && _source[_index] != ':')
            _index++;
    }

    void SkipLine()
    {
        while (_index < _source.Length && _source[_index] != '\n')
            _index++;
    }

    /// <summary>
    /// Compares the source with the given string at the current index. Returns true and updates index if matching.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="skipWhitespace"></param>
    /// <param name="incrementIfFound"></param>
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
            if (skipWhitespace && incrementIfFound)
                SkipWhitespace();
            return match.Success;
        }
        return false;
    }

    /// <summary>
    /// Tries to parse source at the current index with the given value. Skips whitespace after match.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="incrementIfFound"></param>
    /// <returns></returns>
    bool Maybe(char value, bool incrementIfFound = true)
    {
        if (_index >= _source.Length)
            return false;
        if (_source[_index] == value)
        {
            if (incrementIfFound)
                _index++;
            if (incrementIfFound)
                SkipWhitespace();
            return true;
        }
        return false;
    }

    bool OneOf(out string outValue, params string[] values)
    {
        outValue = string.Empty;
        var orderedValues = values.OrderByDescending(v => v.Length);
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

    static Regex StatementRegex() =>
        new(@"\G(?<statement>DEF|FOR|NEXT|DIM|END|GO|ON|IF|INPUT|LET|PRINT|WINDOW|COLOR|RANDOMIZE|RETURN|REM|SAVE|STOP)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    static Regex VariableRegex() =>
        new(@"\G(?<var>[A-Za-z][A-Za-z0-9]*)", RegexOptions.CultureInvariant);
}
