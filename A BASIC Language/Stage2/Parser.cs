using A_BASIC_Language.Stage1;

namespace A_BASIC_Language.Stage2;

public class Parser
{
    public Dictionary<int, int> LabelIndex { get; set; } = new Dictionary<int, int>();
    public List<ABL_EvalValue> ABL_EvalValues { get; set; } = new List<ABL_EvalValue>();
    public ParseResult Result { get; set; } = new ParseResult();

    readonly TokenizeResult _tokenizeResult;

    int _index = 0;
    string _currentTokenValue = string.Empty;
    TokenType _currentTokenType = default;

    public Parser(TokenizeResult tokenizeResult)
    {
        //Note: Initialisation:
        _tokenizeResult = tokenizeResult;
        Next_wws();

        //Note: The parsing begins:
        try
        {
            AProgram();
            Result.EvalValues = ABL_EvalValues;
            Result.LabelIndex = LabelIndex;
        }
        catch (Exception ex)
        {
            //this try catch is for development but might find use in final program.
            var message = ex.Message;
        }
    }

    private void AProgram()
    {
        /* A program consists of zero or more lines of code.
         * Each line begins with a numeric label.
         * It's then followed by a statement or (in the case of let) a user defined name.
         * The statement can optionally be followed by:
         *  Control symbols (e.g. ',', ';' and maybe others).
         *  Other portions of the statement in the cases where it consists of multiple parts:
         *   If is followed by then (which itself can be followed by else) or in some versions of BASIC by goto.
         *   Def is followed by fn.
         *   On is followed by gosub or goto.
         *   For is followed by to (which itself can be followed by step).
         *   There might be others.
         *  Mathematical and logical operations and user defined functions.
         *  If can be followed by other statements.
         *  Finally it can be followed by ':' which is then followed by another statement.
         * 
         * It follows from this that a valid line looks something like this:
         *  aLabel aStatementWithParts (: aStatementWithParts)* //where * means zero or more.
         *  We therefore need a parser that first parses the number and then the statement with
         *   its parts.
         *  The statement parser itself identifies the current statement and calls its parser 
         *   (each statement with parts has its own parser)
         *  After the statement parser is done, the line parser (which contains all this) 
         *   checks if ':' follows and if so calls the statement parser again.
         *  This is repeated until the next label is found then the line parser returns and the
         *   program parser loops
         *  The line parser returns true if it finds a valid program line, false otherwise.
         *   
         */

        //Ponder: long term we probably want to keep parsing even if a line is erroneous and simply
        // collect the errors so that we can maximise the error information we can give the user.
        bool more;
        do more = ALine();
        while (more);
    }

    bool ALine()
    {
        //aLabel aStatementWithParts (: aStatementWithParts)*

        if (!ALabel())
            return false;

        AStatementPlusParts();

        while (MightMatch(TokenType.Colon))
            AStatementPlusParts();

        //Ponder: should these checks be done in AStatementPlusParts?
        if (_currentTokenType == TokenType.Label)
            return true;
        else if (_currentTokenType == TokenType.EOF)
            return false;
        else
            return true;
    }

    bool ALabel()
    {
        if (_currentTokenType != TokenType.Label)
            return false;
        Generate(new ABL_Label(_currentTokenValue));
        LabelIndex.Add(int.Parse(_currentTokenValue), _index);
        Next();
        return true;
    }

    void AStatementPlusParts()
    {
        //This is the tricky part.
        if (_currentTokenType == TokenType.UserDefinedName)
            Let();
        else if (_currentTokenType == TokenType.Statement)
        {
            switch (_currentTokenValue)
            {
                case "GOTO":
                    Goto();
                    break;
                case "GO":
                    Next();
                    MustMatch("TO");
                    Goto();
                    break;
                case "IF"://todo
                    //IF F(N)*6+K=INT(F(I)/6^(7-C)+.1) THEN Q=Q-2
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
                default:
                    throw new NotImplementedException("the given statement has not been implemented yet.");
            }
        }
        else
            throw new InvalidOperationException("Syntax error");
    }

    void Print()
    {
        //print => PRINT (value | ; | ,)*
        //value => literal | variable | expression

        /* Examples:
         * PRINT "IS IT A ";RIGHT$(A$(K),LEN(A$(K))-2);
         * PRINT "I WIN BY";-D;"POINTS"
         * PRINT 4 2: REM returns 42
         * PRINT 4;2: REM returns 4  2 (note two spaces, first from 4 the second from potential minus).
         */
        Next();
        bool shouldPrintNewline = true;
        while (_currentTokenType != TokenType.Comma &&
            _currentTokenType != TokenType.Label &&
            _currentTokenType != TokenType.EOF)
        {
            switch (_currentTokenType)
            {
                case TokenType.Comma:
                    Generate(new ABL_Procedure("NEXT-TAB-POSITION"));
                    shouldPrintNewline = false;
                    Next();
                    continue;
                case TokenType.Semicolon:
                    shouldPrintNewline = false;
                    Next();
                    continue;
            }
            Expression();
            Generate(new ABL_Procedure("WRITE"));
            shouldPrintNewline = true;
        }
        if (shouldPrintNewline)
            Generate(new ABL_Procedure("NEXT-LINE"));
    }

    void Input()
    {
        //input      => INPUT (prompt-string [;])? assignable (, assignable)*
        //assignable => user-defined-name (type-specifier  ('(' index ')')? )?

        Next();
        Prompt();
        Generate(new ABL_String("? "), new ABL_Procedure("WRITE"));
        Variable();
        while (MightMatch(TokenType.Comma))
            Variable();

        void Prompt()
        {
            if (MightMatch(TokenType.String, out var prompt))
            {
                MustMatch(TokenType.Semicolon);
                Generate(new ABL_String(prompt), new ABL_Procedure("WRITE"));
            }
        }

        void Variable()
        {
            MustMatch(TokenType.UserDefinedName, out var newVariable);
            if (MightMatch(TokenType.TypeSpecifier, out var typeSpecifier))
            {
                switch (typeSpecifier)
                {
                    case "$":
                        Generate(new ABL_Procedure("INPUT-STRING"), new ABL_Assignment(newVariable));
                        break;
                    case "%":
                        Generate(new ABL_Procedure("INPUT-INT"), new ABL_Assignment(newVariable));
                        break;
                }
            }
            else
                Generate(new ABL_Procedure("INPUT-FLOAT"), new ABL_Assignment(newVariable));

            //todo: check for dim accessing (e.g. a(i)).
        }
    }

    void Goto()
    {
        Next();
        Expression();
        Generate(new ABL_Procedure("GOTO"));
        SkipLine();
    }

    void Let()
    {
        //let => 'LET'? N = Expr
        MightMatch("LET");
        MustMatch(TokenType.UserDefinedName, out var name);
        MustMatch(TokenType.EqualityOrAssignment);
        Expression();
        Generate(new ABL_Assignment(name));
    }

    /// <summary>
    /// Calls the operator of the lowest precedence.
    /// </summary>
    void Expression() => PrecedenceThreeOperator();

    void PrecedenceThreeOperator()
    {//Note: This method is Expression in Crenshaw's book.
        PrecedenceTwoOperator();
        while (_currentTokenType == TokenType.Operator && IsOneOf(_currentTokenValue, "+ -"))
        {
            switch (_currentTokenValue)
            {
                case "+":
                    Add();
                    break;
                case "-":
                    Subtract();
                    break;
            }
        }
    }

    void Add()
    {
        Next();
        PrecedenceTwoOperator();
        Generate(new ABL_Procedure("+"));
    }

    void Subtract()
    {
        Next();
        PrecedenceTwoOperator();
        Generate(new ABL_Procedure("-"));
    }

    void PrecedenceTwoOperator()
    {//Note: This method is Term in Crenshaw's book.
        PrecedenceOneOperator();
        while (_currentTokenType == TokenType.Operator && IsOneOf(_currentTokenValue, "* /"))
        {
            switch (_currentTokenValue)
            {
                case "*":
                    Multiply();
                    break;
                case "/":
                    Divide();
                    break;
            }
        }
    }

    void Multiply()
    {
        Next();
        PrecedenceOneOperator();
        Generate(new ABL_Procedure("*"));
    }

    void Divide()
    {
        Next();
        PrecedenceOneOperator();
        Generate(new ABL_Procedure("/"));
    }

    void PrecedenceOneOperator()
    {
        Atom();
        while (_currentTokenType == TokenType.Operator && _currentTokenValue == "^")
        {
            //exponentiate
            Next();
            Atom();
            Generate(new ABL_Procedure("^"));
        }
    }

    void Atom()
    {//Note: This method is Factor in Crenshaw's book.
        switch (_currentTokenType)
        {
            case TokenType.OpeningParenthesis:
                Next();
                Expression();
                MustMatch(TokenType.ClosingParenthesis);
                break;
            case TokenType.Function:
                //e.g. sqr(n)
                var functionName = _currentTokenValue;
                Next();
                MustMatch(TokenType.OpeningParenthesis);
                Expression();
                MustMatch(TokenType.ClosingParenthesis);
                Generate(new ABL_Procedure(functionName));
                break;
            case TokenType.Float:
                Generate(new ABL_Number(_currentTokenValue));//todo: float tokens.
                Next();
                break;
            case TokenType.Number:
                Generate(new ABL_Number(_currentTokenValue));
                Next();
                break;
            case TokenType.String:
                Generate(new ABL_String(_currentTokenValue));
                Next();
                break;
            case TokenType.UserDefinedName:
                Generate(new ABL_Variable(_currentTokenValue));
                Next();
                break;
            case TokenType.Statement://Note: user defined function.
                if (MightMatch("FN"))//todo: test.
                {
                    Next();
                    MustMatch(TokenType.UserDefinedName);
                    var name = _currentTokenValue;
                    MustMatch(TokenType.OpeningParenthesis);
                    Expression();
                    MustMatch(TokenType.ClosingParenthesis);
                    //todo: generate for user defined functions.
                    //Generate($"userDefinedFunction({name})");
                }
                break;
            default:
                return;
        }
    }


    void Generate(params ABL_EvalValue[] values)
    {
        foreach (ABL_EvalValue value in values)
            ABL_EvalValues.Add(value);
    }

    static bool IsOneOf(string value, string values)
    {
        string[] vs = values.Split();
        return vs.Contains(value);
    }

    void MustMatch_wws(TokenType tokenType)//deleteMe: if not in use once parser is finished.
    {
        if (tokenType == _currentTokenType)
        {
            Next_wws();
        }
        else
        {
            throw new Exception("placeholder #2");//todo: proper error handling.
        }
    }

    void MustMatch(TokenType tokenType)//deleteMe: if not in use once parser is finished.
    {
        MustMatch_wws(tokenType);
        SkipWhitespace();
    }

    void MustMatch_wws(TokenType tokenType, out string tokenValue)//deleteMe: if not in use once parser is finished.
    {
        if (tokenType == _currentTokenType)
        {
            tokenValue = _currentTokenValue;
            Next_wws();
        }
        else
            throw new Exception("placeholder #3");//todo: proper error handling.
    }

    void MustMatch(TokenType tokenType, out string tokenValue)//deleteMe: if not in use once parser is finished.
    {
        MustMatch_wws(tokenType, out tokenValue);
        SkipWhitespace();
    }

    void MustMatch_wws(string tokenValue)//deleteMe: if not in use once parser is finished.
    {
        if (tokenValue == _currentTokenValue)
        {
            Next_wws();
        }
        else
            throw new Exception("placeholder #1");//todo: proper error handling.
    }

    void MustMatch(string tokenValue)//deleteMe: if not in use once parser is finished.
    {
        MustMatch_wws(tokenValue);
        SkipWhitespace();
    }

    bool MightMatch_wws(TokenType tokenType)//deleteMe: if not in use once parser is finished.
    {
        if (tokenType == _currentTokenType)
        {
            Next_wws();
            return true;
        }
        return false;
    }

    bool MightMatch(TokenType tokenType)//deleteMe: if not in use once parser is finished.
    {
        if (MightMatch_wws(tokenType))
        {
            SkipWhitespace();
            return true;
        }
        return false;
    }

    bool MightMatch_wws(TokenType tokenType, out string tokenValue)//deleteMe: if not in use once parser is finished.
    {
        tokenValue = string.Empty;
        if (tokenType == _currentTokenType)
        {
            tokenValue = _currentTokenValue;
            Next_wws();
            return true;
        }
        return false;
    }

    bool MightMatch(TokenType tokenType, out string tokenValue)//deleteMe: if not in use once parser is finished.
    {
        if (MightMatch_wws(tokenType, out tokenValue))
        {
            SkipWhitespace();
            return true;
        }
        return false;
    }

    bool MightMatch_wws(string tokenValue)//deleteMe: if not in use once parser is finished.
    {
        if (tokenValue == _currentTokenValue)
        {
            Next_wws();
            return true;
        }
        return false;
    }

    bool MightMatch(string tokenValue)//deleteMe: if not in use once parser is finished.
    {
        if (MightMatch_wws(tokenValue))
        {
            SkipWhitespace();
            return true;
        }
        return false;
    }

    void Next()
    {
        Next_wws();
        SkipWhitespace();
    }

    void SkipWhitespace()
    {
        while (_currentTokenType == TokenType.Space)
            Next_wws();
    }

    void SkipLine()
    {
        while (_currentTokenType != TokenType.Label &&
            _currentTokenType != TokenType.EOF)
            Next_wws();
    }

    void Next_wws()
    {
        if (_currentTokenType == TokenType.EOF)
            return;
        _currentTokenType = _tokenizeResult.TokenTypes[_index];
        _currentTokenValue = _tokenizeResult.TokenValues[_index];
        _index++;
    }
}
