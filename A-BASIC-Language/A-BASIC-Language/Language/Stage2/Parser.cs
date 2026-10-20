using System;
using System.Collections.Generic;
using A_BASIC_Language.Language.LanguageParts;
using A_BASIC_Language.Stage1;

namespace A_BASIC_Language.Language.Stage2;

public class Parser
{
    public A_BASIC_Language.Stage2.ParseResult Result { get; set; } = new A_BASIC_Language.Stage2.ParseResult();

    private readonly TokenizeResult _tokenizeResult;
    private readonly List<AblEvalValue> _evalValues;
    private readonly Dictionary<int, int> _labelIndex;
    private int _index;
    private string _currentTokenValue = string.Empty;
    private TokenType _currentTokenType;
    private bool _parsingIf;
    private int _generatedLabel;

    public Parser(TokenizeResult tokenizeResult)
    {
        //Note: Initialisation:
        _tokenizeResult = tokenizeResult;
        _evalValues = [];
        _labelIndex = new Dictionary<int, int>();
        Next_wws();

        //Note: The parsing begins:
        try
        {
            AProgram();
            Result.EvalValues = _evalValues;
            Result.LabelIndex = _labelIndex;
        }
        catch //(Exception ex)
        {
            //this try catch is for development but might find use in final program.
            //var message = ex.Message;
        }
    }

    public Parser(Dictionary<int, int> labelIndex)
    {
        _labelIndex = labelIndex;
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

    private bool ALine()
    {
        //line => aLabel oneOrMoreStatements

        if (!ALabel())
            return false;

        OneOrMoreStatements();

        //Ponder: should these checks be done in AStatementPlusParts?
        if (_currentTokenType == TokenType.Label)
            return true;
        
        if (_currentTokenType == TokenType.EOF)
            return false;
        
        return true;
    }

    private bool ALabel()
    {
        if (_currentTokenType != TokenType.Label)
            return false;

        GenerateLabel();
        Next();
        return true;
    }

    private void AStatementPlusParts()
    {
        //This is the tricky part.
        if (_currentTokenType == TokenType.UserDefinedName)
        {
            Let();
        }
        else if (_currentTokenType == TokenType.Statement)
        {
            switch (_currentTokenValue)
            {
                case "DIM":
                    Dim();
                    break;
                case "END":
                    Next();
                    Generate(new AblProcedure("#END-PROGRAM"));
                    break;
                case "GOTO":
                    Goto();
                    break;
                case "GO":
                    Next();
                    if (_currentTokenValue == "TO")
                        Goto();
                    else
                        throw new ArgumentException("GO TO (note the space) went wrong");
                    break;
                case "IF":
                    //Note: This wont work for nested ifs but that will probably not cause problems.
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
                    Next();
                    Generate(new AblProcedure("#END-PROGRAM"));
                    break;
                default:
                    throw new NotImplementedException("the given statement has not been implemented yet.");
            }
        }
        else
            throw new InvalidOperationException("Syntax error");
    }

    void Dim()
    {
        //dim     => DIM unset-variable ("," unset-variable)*

        Next();
        UnsetVariable();
        while (MightMatch(TokenType.Comma))
            UnsetVariable();

        void UnsetVariable()
        {
            var (fullName, _, dimVariable) = ASetVariable(true);
            if (dimVariable)
                Generate(new ABL_DIM_Creation(fullName));
            else
                Generate(new ABL_Variable(fullName));
        }
    }

    void If()
    {
        //if         => IF boolean-expr THEN body (ELSE body)?
        //body       => numeric-expr | statements
        //statements => statement (: statement)*

        /*
            IF expr THEN this ELSE that
            Is compiled to the following (10 and 20 are placeholder values.):
            expr #10 #IF-FALSE-GOTO this #20 GOTO 10 that 20
         */

        var falseBranchLabel = GetGeneratedLabel();//Note: Made negative because all valid BASIC labels are positive so there won't be a conflict.
        Next();
        Expression();
        Generate(new AblNumber(falseBranchLabel), new AblProcedure("#IF-FALSE-GOTO"));
        MustMatch("THEN");
        Body();
        var endBranchLabel = GetGeneratedLabel();//Note: Se above.
        Generate(new AblNumber(endBranchLabel), new AblProcedure("GOTO"));
        GenerateLabel(falseBranchLabel);
        if (MightMatch("ELSE"))
            Body();
        GenerateLabel(endBranchLabel);

        void Body()
        {
            if (_currentTokenType == TokenType.Statement)
                OneOrMoreStatements();
            else if (_currentTokenType == TokenType.UserDefinedName)
            {
                //Note: if the value after the user defined name is an equals sign then this is a statement.
                var (_, type) = LookAhead();
                if (type == TokenType.EqualityOrAssignment)
                    OneOrMoreStatements();
                else
                {
                    Expression();
                    Generate(new AblProcedure("GOTO"));
                }
            }
            else
            {
                Expression();
                Generate(new AblProcedure("GOTO"));
            }
        }

        (string value, TokenType type) LookAhead()
        {
            if (_currentTokenType == TokenType.EOF)//Note: just in case we're at the end.
                return ("", TokenType.EOF);
            var result = ("", TokenType.Space);
            for (int i = _index + 1; i < _tokenizeResult.TokenValues.Count; i++)
            {
                var ctt = _tokenizeResult.TokenTypes[i];
                if (ctt == TokenType.EOF)
                    return ("", TokenType.EOF);
                if (ctt == TokenType.Space) continue;
                result = (_tokenizeResult.TokenValues[i], ctt);
                break;
            }
            return result;
        }
    }

    private int GetGeneratedLabel() => --_generatedLabel;

    private void OneOrMoreStatements()
    {
        //OneOrMoreStatements => aStatementWithParts (: aStatementWithParts)*

        AStatementPlusParts();
        while (MightMatch(TokenType.Colon))
            AStatementPlusParts();
    }

    private void GenerateLabel(int? value = null)
    {
        if (value == null)
        {
            Generate(new AblLabel(_currentTokenValue));
            value = int.Parse(_currentTokenValue);
        }
        else
            Generate(new AblLabel(value.Value));
        _labelIndex.Add(value.Value, _evalValues.Count - 1);
    }

    void Print()
    {//todo: not completely tested.
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
        while (_currentTokenType != TokenType.Label &&
            _currentTokenType != TokenType.EOF &&
            _currentTokenType != TokenType.Colon &&
            _currentTokenType != TokenType.Statement)//Note: checks for statement in case of ELSE statement in IF.
        {
            switch (_currentTokenType)
            {
                case TokenType.Comma:
                    Generate(new AblProcedure("#NEXT-TAB-POSITION"));
                    shouldPrintNewline = false;
                    Next();
                    continue;
                case TokenType.Semicolon:
                    shouldPrintNewline = false;
                    Next();
                    continue;
            }
            Expression();
            Generate(new AblProcedure("#WRITE"));
            shouldPrintNewline = true;
        }
        if (shouldPrintNewline)
            Generate(new AblProcedure("#NEXT-LINE"));
    }

    void Input()
    {
        //input      => INPUT (prompt-string [;])? unset-variable (, unset-variable)*

        Next();
        Prompt();
        Generate(new AblString("? "), new AblProcedure("#WRITE"));
        UnsetVariable();
        while (MightMatch(TokenType.Comma))
            UnsetVariable();

        void Prompt()
        {
            if (MightMatch(TokenType.String, out var prompt))
            {
                MustMatch(TokenType.Semicolon);
                Generate(new AblString(prompt), new AblProcedure("#WRITE"));
            }
        }

        void UnsetVariable()
        {
            var v = ASetVariable(true);
            var assignmentType = ASetVariable_Helper(v);
            switch (v.typeSpecifier)
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
        }
    }

    void Goto()
    {
        Next();
        Expression();
        Generate(new AblProcedure("GOTO"));
        if (!_parsingIf)
            SkipLine();
    }

    void Let()
    {
        //let => 'LET'? unset-variable = expression

        MightMatch("LET");
        var v = ASetVariable(true);
        MustMatch(TokenType.EqualityOrAssignment);
        Expression();
        Generate(ASetVariable_Helper(v));
    }

    static AblEvalValue ASetVariable_Helper((string fullName, string _, bool dimVariable) v)
    {
        var fullName = v.fullName;
        if (v.dimVariable)
            return new ABL_DIM_Assignment(fullName);
        else
            return new ABL_Assignment(fullName);
    }

    /// <summary>
    /// A variable with a value.
    /// </summary>
    (string fullName, string typeSpecifier, bool dimVariable) ASetVariable(bool assignmentMode = false)
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

        // set-variable => name index?
        // name         => user-defined-name type-specifier?
        // index        => '(' index-value ')'
        // index-value  => expression (',' expression)*

        string fullName, typeSpecifier;
        bool dimVariable = false;
        Name();
        Index();
        return (fullName, typeSpecifier, dimVariable);

        void Name()
        {
            MustMatch(TokenType.UserDefinedName, out var name);
            MightMatch(TokenType.TypeSpecifier, out typeSpecifier);
            fullName = name + typeSpecifier;
        }

        void Index()
        {
            if (!MightMatch(TokenType.OpeningParenthesis))
            {
                if (!assignmentMode)
                    Generate(new ABL_Variable(fullName));
                return;
            }
            dimVariable = true;
            IndexValue();
            MustMatch(TokenType.ClosingParenthesis);

            if (assignmentMode)
                return;
            
            Generate(new ABL_DIM_Variable(fullName));
            return;

            void IndexValue()
            {
                Expression();
                var dimensionCount = 1;
                while (MightMatch(TokenType.Comma))
                {
                    Expression();
                    dimensionCount++;
                }
                Generate(new AblNumber(dimensionCount));
            }
        }
    }

    /// <summary>
    /// Calls the operator of the lowest precedence.
    /// </summary>
    void Expression() => Precedence_5_Operator();

    void Precedence_5_Operator()
    {//Note: This method is Expression in Crenshaw's book.
        Precedence_4_Operator();
        while (_currentTokenType == TokenType.Operator || _currentTokenType == TokenType.EqualityOrAssignment)
        {
            switch (_currentTokenValue)
            {
                case "=":
                    Equal();
                    break;
                case "<>":
                    NotEqual();
                    break;
                case "!=":
                    NotEqual();
                    break;
                case "<":
                    LessThan();
                    break;
                case ">":
                    GreaterThan();
                    break;
                case "<=":
                    LessThanOrEqual();
                    break;
                case ">=":
                    GreaterThanOrEqual();
                    break;
                default:
                    goto END;
            }
        }
    END:;
    }

    private void Equal()
    {
        Next();
        Precedence_4_Operator();
        Generate(new AblProcedure("="));
    }

    private void NotEqual()
    {
        Next();
        Precedence_4_Operator();
        Generate(new AblProcedure("<>"));
    }

    private void LessThan()
    {
        Next();
        Precedence_4_Operator();
        Generate(new AblProcedure("<"));
    }

    private void GreaterThan()
    {
        Next();
        Precedence_4_Operator();
        Generate(new AblProcedure(">"));
    }

    private void LessThanOrEqual()
    {
        Next();
        Precedence_4_Operator();
        Generate(new AblProcedure("<="));
    }

    private void GreaterThanOrEqual()
    {
        Next();
        Precedence_4_Operator();
        Generate(new AblProcedure(">="));
    }

    void Precedence_4_Operator()
    {//Note: This method is Expression in Crenshaw's book.
        Precedence_3_Operator();
        while (_currentTokenType == TokenType.Operator)
        {
            switch (_currentTokenValue)
            {
                case "+":
                    Add();
                    break;
                case "-":
                    Subtract();
                    break;
                default:
                    goto END;
            }
        }
        END:;
    }

    void Add()
    {
        Next();
        Precedence_3_Operator();
        Generate(new AblProcedure("+"));
    }

    void Subtract()
    {
        Next();
        Precedence_3_Operator();
        Generate(new AblProcedure("-"));
    }

    void Precedence_3_Operator()
    {//Note: This method is Term in Crenshaw's book.
        Precedence_2_Operator();
        while (_currentTokenType == TokenType.Operator)
        {
            switch (_currentTokenValue)
            {
                case "*":
                    Multiply();
                    break;
                case "/":
                    Divide();
                    break;
                default:
                    goto END;
            }
        }
        END:;
    }

    void Multiply()
    {
        Next();
        Precedence_2_Operator();
        Generate(new AblProcedure("*"));
    }

    void Divide()
    {
        Next();
        Precedence_2_Operator();
        Generate(new AblProcedure("/"));
    }

    void Precedence_2_Operator()
    {//todo: negation
        Precedence_1_Operator();
    }

    void Precedence_1_Operator()
    {
        Atom();
        while (_currentTokenType == TokenType.Operator && _currentTokenValue == "^")
        {
            //exponentiate
            Next();
            Atom();
            Generate(new AblProcedure("^"));
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
                Generate(new AblProcedure(functionName));
                break;
            case TokenType.Float://todo: float tokens.
            case TokenType.Number:
                Generate(new AblNumber(_currentTokenValue));
                Next();
                break;
            case TokenType.String:
                Generate(new AblString(_currentTokenValue));
                Next();
                break;
            case TokenType.UserDefinedName:
                ASetVariable();
                break;
            case TokenType.Statement://Note: user defined function.
                if (MightMatch("FN"))//todo: test.
                {
                    Next();
                    MustMatch(TokenType.UserDefinedName);
                    //var name = _currentTokenValue;
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

    private void Generate(params AblEvalValue[] values)
    {
        foreach (AblEvalValue value in values)
            _evalValues.Add(value);
    }

    private void MustMatch_wws(TokenType tokenType)//deleteMe: if not in use once parser is finished.
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

    private void MustMatch(TokenType tokenType)//deleteMe: if not in use once parser is finished.
    {
        MustMatch_wws(tokenType);
        SkipWhitespace();
    }

    private void MustMatch_wws(TokenType tokenType, out string tokenValue)//deleteMe: if not in use once parser is finished.
    {
        if (tokenType == _currentTokenType)
        {
            tokenValue = _currentTokenValue;
            Next_wws();
        }
        else
        {
            throw new Exception("placeholder #3"); //todo: proper error handling.
        }
    }

    private void MustMatch(TokenType tokenType, out string tokenValue)//deleteMe: if not in use once parser is finished.
    {
        MustMatch_wws(tokenType, out tokenValue);
        SkipWhitespace();
    }

    private void MustMatch_wws(string tokenValue)//deleteMe: if not in use once parser is finished.
    {
        if (tokenValue == _currentTokenValue)
            Next_wws();
        else
            throw new Exception("placeholder #1");//todo: proper error handling.
    }

    private void MustMatch(string tokenValue)//deleteMe: if not in use once parser is finished.
    {
        MustMatch_wws(tokenValue);
        SkipWhitespace();
    }

    private bool MightMatch_wws(TokenType tokenType) => //deleteMe: if not in use once parser is finished.
        tokenType == _currentTokenType && Next_wws();

    bool MightMatch(TokenType tokenType)//deleteMe: if not in use once parser is finished.
    {
        if (!MightMatch_wws(tokenType))
            return false;

        SkipWhitespace();
        return true;
    }

    bool MightMatch_wws(TokenType tokenType, out string tokenValue)//deleteMe: if not in use once parser is finished.
    {
        tokenValue = string.Empty;

        if (tokenType != _currentTokenType)
            return false;

        tokenValue = _currentTokenValue;
        return Next_wws();
    }

    private bool MightMatch(TokenType tokenType, out string tokenValue) => //deleteMe: if not in use once parser is finished.
        MightMatch_wws(tokenType, out tokenValue) && SkipWhitespace();

    private bool MightMatch_wws(string tokenValue) =>//deleteMe: if not in use once parser is finished.
        tokenValue == _currentTokenValue && Next_wws();

    private bool MightMatch(string tokenValue) =>//deleteMe: if not in use once parser is finished.
        MightMatch_wws(tokenValue) && SkipWhitespace();

    private void Next()
    {
        Next_wws();
        SkipWhitespace();
    }

    private bool SkipWhitespace()
    {
        while (_currentTokenType == TokenType.Space)
            Next_wws();

        return true;
    }

    private void SkipLine()
    {
        while (_currentTokenType != TokenType.Label && _currentTokenType != TokenType.EOF)
            Next_wws();
    }

    private bool Next_wws()
    {
        if (_currentTokenType == TokenType.EOF)
            return true;

        _currentTokenType = _tokenizeResult.TokenTypes[_index];
        _currentTokenValue = _tokenizeResult.TokenValues[_index];
        _index++;
        return true;
    }
}
