namespace StageThree
{
    internal class LineParser
    {
        public int Label { get; private set; }
        public Line Line { get; private set; }

        List<StageTwo.Token> _tokens;
        List<string> _values;
        int _index;
        StageTwo.Token _currentToken;
        string _currentValue;

        List<Token> _expression;

        public LineParser(StageTwo.Line tokenizedLine)
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

            _expression = new List<Token>();
            Label = LineNumber();
            Line = Statement();
        }

        int LineNumber()
        {
            int result;
            if (_currentToken == StageTwo.Token.Label)
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
            if (_currentToken == StageTwo.Token.Statement)
            {
                return _currentValue switch
                {
                    "GOTO" => Goto(),
                    "INPUT" => Input(),
                    "PRINT" => Print(),
                    _ => throw new NotImplementedException()//todo: proper error handling.
                };
            }
            else if (_currentToken == StageTwo.Token.Other)//let's ignore assignment for now and look at the others.
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
            return new Line(new List<Token>() { new Number(int.Parse(_currentValue)), new Procedure("GOTO") });
        }

        private Line Input()
        {
            if (_tokens.Count > 3)
                throw new NotImplementedException("The current implementation of input can't do that.");
            Next();
            //todo: validate the token types.
            return new Line(new List<Token>() { new Procedure("INPUT"), new Assignment(_currentValue) });
        }

        private Line Print()
        {
            if (_tokens.Count > 3)
                throw new NotImplementedException("The current implementation of print can't do that.");
            Next();
            //todo: validate the token types.
            return new Line(new List<Token>() { new Variable(_currentValue), new Procedure("PRINT") });
        }

        Line Assignment()
        {
            var variableName = _currentValue;
            Next();
            Next();//Note: I call Next() twice to skip '='.
            /*var tokens = */
            Expression();
            _expression.Add(new Assignment(variableName));
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
            while (_currentToken == StageTwo.Token.Operator && IsOneOf(_currentValue, "+ -"))
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
            _expression.Add(new Procedure("+"));
        }

        private void Subtract()
        {
            Next();
            PrecedenceTwoOperator();
            _expression.Add(new Procedure("-"));
        }

        private void PrecedenceTwoOperator()
        {//Note: This method is Term in Crenshaw's book.
            PrecedenceOneOperator();
            while (_currentToken == StageTwo.Token.Operator && IsOneOf(_currentValue, "* /"))//todo: confirm that "/" is divide in Basic.
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
            _expression.Add(new Procedure("*"));
        }

        private void Divide()
        {
            Next();
            PrecedenceOneOperator();
            _expression.Add(new Procedure("/"));
        }

        private void PrecedenceOneOperator()
        {
            Atom();
            while (_currentToken == StageTwo.Token.Operator && _currentValue == "^")
            {
                //exponentiate
                Next();
                Atom();
                _expression.Add(new Procedure("^"));
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
                case StageTwo.Token.OpeningParenthesis:
                    Next();
                    Expression();
                    Match(StageTwo.Token.ClosingParenthesis);
                    break;
                case StageTwo.Token.StandardFunction:
                    //e.g. sqr(n)
                    //get name of function then next.
                    var functionName = _currentValue;
                    Next();
                    Match(StageTwo.Token.OpeningParenthesis);
                    Expression();
                    Match(StageTwo.Token.ClosingParenthesis);
                    _expression.Add(new Procedure(functionName));
                    break;
                case StageTwo.Token.UserDefinedFunction:
                    break;
                case StageTwo.Token.Number:
                    _expression.Add(new Number(_currentValue));
                    Next();
                    break;
                case StageTwo.Token.Other:
                    _expression.Add(new Variable(_currentValue));
                    Next();
                    break;
                default:
                    break;
            }
        }

        void Match(StageTwo.Token token)
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
}