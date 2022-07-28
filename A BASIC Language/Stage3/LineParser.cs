namespace A_BASIC_Language.Stage3;

internal class LineParser
{
    public int Label { get; private set; }
    public Line Line { get; private set; }

    List<Stage2.Token> _tokens;
    List<string> _values;
    int _index;
    Stage2.Token _currentToken;
    string _currentValue;

    List<ABL_EvalValue> _expression;

    public LineParser(Stage2.Line tokenizedLine)
    {
        /*
        What is a line?
        It starts with a label and is followed by a (optional?) statement.
        In the case of = it amy actually start with a variable which is immedialy followed by =.
        Depending on the statement it can be followed by an expression or even another statement.
         */
        _tokens = tokenizedLine.Tokens;
        _values = tokenizedLine.TextValues;
        _index = 0;
        Next();

        _expression = new List<ABL_EvalValue>();
        Label = LineNumber();
        Line = Statement();
    }

    int LineNumber()
    {
        int result;
        if (_currentToken == Stage2.Token.Label)
        {
            result = int.Parse(_currentValue);
            Next();
        }
        else
            throw new Exception("placeholder");//todo: proper error handling.
        return result;
    }

    /// <summary>
    /// This will handle both statement and expression parsing to ensure proper ordering.
    /// </summary>
    /// <returns></returns>
    Line Statement()
    {
        /*
        as i'm iterating from left to right, I need to save the initial tokens if I am to
        add them to the data structure later (to get the proper order) but I still need to 
        match the proper syntax from the beginning.
        Perhaps I should start with figuring out the parsing first and then figure out how 
        to put the tokens in the proper order?
         */
        //there are two possibilities: beginning with a statement or a variable to be set.
        //assume the program is correct.
        //the first step is to identify the statement and then act accordingly
        if (_currentToken == Stage2.Token.Statement)
        {
            return _currentValue switch
            {
                "GOTO" => Goto(),
                "INPUT" => Input(),
                "PRINT" => Print(),
                _ => throw new NotImplementedException()//todo: proper error handling.
            };
        }
        else if (_currentToken == Stage2.Token.Other)//let's ignore assignment for now and look at the others.
            return Assignment();
        else
            throw new Exception("placeholder");//todo: proper error handling.
    }

    private Line Goto()
    {
        if (_tokens.Count > 3)
            throw new NotImplementedException("The current implementation of goto can't do that.");
        Next();
        //todo: validate the token types.
        return new Line(new List<ABL_EvalValue>() { new ABL_Number(int.Parse(_currentValue)), new ABL_Procedure("GOTO") });
    }

    private Line Input()
    {
        if (_tokens.Count > 3)
            throw new NotImplementedException("The current implementation of input can't do that.");
        Next();
        //todo: validate the token types.
        return new Line(new List<ABL_EvalValue>() { new ABL_Procedure("INPUT"), new ABL_Assignment(_currentValue) });
    }

    private Line Print()
    {
        if (_tokens.Count > 3)
            throw new NotImplementedException("The current implementation of print can't do that.");
        Next();
        //todo: validate the token types.
        return new Line(new List<ABL_EvalValue>() { new ABL_Variable(_currentValue), new ABL_Procedure("PRINT") });
    }

    Line Assignment()
    {
        var variableName = _currentValue;
        Next();
        Next();//Note: I call Next() twice to skip '='.
        /*var tokens = */
        Expression();
        _expression.Add(new ABL_Assignment(variableName));
        return new Line(_expression);
    }

    /// <summary>
    /// Calls the operator of the lowest precedence.
    /// </summary>
    private void Expression() => PrecedenceThreeOperator();

    private void PrecedenceThreeOperator()
    {//Note: This method is Expression in Crenshaw's book.
        PrecedenceTwoOperator();
        var temp = IsOneOf(_currentValue, "+ -");
        while (_currentToken == Stage2.Token.Operator && IsOneOf(_currentValue, "+ -"))
        {
            switch (_currentValue)
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

    private void Add()
    {
        Next();
        PrecedenceTwoOperator();
        _expression.Add(new ABL_Procedure("+"));
    }

    private void Subtract()
    {
        Next();
        PrecedenceTwoOperator();
        _expression.Add(new ABL_Procedure("-"));
    }

    private void PrecedenceTwoOperator()
    {//Note: This method is Term in Crenshaw's book.
        PrecedenceOneOperator();
        while (_currentToken == Stage2.Token.Operator && IsOneOf(_currentValue, "* /"))//todo: confirm that "/" is divide in Basic.
        {
            switch (_currentValue)
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

    private void Multiply()
    {
        Next();
        PrecedenceOneOperator();
        _expression.Add(new ABL_Procedure("*"));
    }

    private void Divide()
    {
        Next();
        PrecedenceOneOperator();
        _expression.Add(new ABL_Procedure("/"));
    }

    private void PrecedenceOneOperator()
    {
        Atom();
        while (_currentToken == Stage2.Token.Operator && _currentValue == "^")
        {
            //exponentiate
            Next();
            Atom();
            _expression.Add(new ABL_Procedure("^"));
        }
    }

    static bool IsOneOf(string value, string values)
    {
        string[] vs = values.Split();
        return vs.Contains(value);
    }

    private void Atom()
    {//Note: This method is Factor in Crenshaw's book.
        switch (_currentToken)
        {
            case Stage2.Token.OpeningParenthesis:
                Next();
                Expression();
                Match(Stage2.Token.ClosingParenthesis);
                break;
            case Stage2.Token.StandardFunction:
                //e.g. sqr(n)
                //get name of function then next.
                var functionName = _currentValue;
                Next();
                Match(Stage2.Token.OpeningParenthesis);
                Expression();
                Match(Stage2.Token.ClosingParenthesis);
                _expression.Add(new ABL_Procedure(functionName));
                break;
            case Stage2.Token.UserDefinedFunction:
                break;
            case Stage2.Token.Number:
                _expression.Add(new ABL_Number(_currentValue));
                Next();
                break;
            case Stage2.Token.Other:
                _expression.Add(new ABL_Variable(_currentValue));
                Next();
                break;
            default:
                break;
        }
    }

    void Match(Stage2.Token token)
    {
        if (token == _currentToken)
            Next();
        else
        {
            throw new Exception("placeholder");//todo: proper error handling.
        }
    }

    void Next()
    {
        if (_index == _tokens.Count)//Note: End of line (Now I want to watch Tron again).
            return;
        _currentToken = _tokens[_index];
        _currentValue = _values[_index];
        _index++;
    }
}