namespace StageTwo
{
    internal class Tokenizer
    {
        public Result Result { get; private set; }

        List<List<string>> _tokenizedLines;
        List<string> _statementTokens;
        List<string> _Operators;
        List<string> _standardFunctions;

        public Tokenizer(List<List<string>> tokenizedLines)
        {
            Result = new();
            _tokenizedLines = tokenizedLines;
            _statementTokens = new List<string>() { "INPUT", "PRINT", "GOTO", "DEF", 
                "DATA", "DIM", "END", "FOR", "TO", "STEP", "GOSUB", "IF", "THEN", 
                "ELSE", "LET", "NEXT", "ON", "READ", "REM", "RESTORE", "RETURN", "STOP" };
            _Operators = new List<string>() { "^", "+", "-", "*", "/" };
            _standardFunctions = new List<string>() { "SQR", "INT" };
            foreach (var line in _tokenizedLines)
            {
                Result.Add(Line(line));
            }
        }

        private Line Line(List<string> line)
        {
            Line result = new();
            var first = line.First();
            if (int.TryParse(first, out _))
            {
                //todo: use a different data structure in the future to not waste the parsed result.
                result.Add(first, Token.Label);
            }
            else
            {
                //todo:error handling.
            }

            foreach (string token in line.Skip(1))//skips the first because it's already handled above.
            {
                Token currentToken;
                if (token == "(")
                    currentToken = Token.OpeningParenthesis;
                else if (token == ")")
                    currentToken = Token.ClosingParenthesis;
                else if (token == "=")
                    currentToken = Token.EqualityOrAssignment;
                else if (token == ",")
                    currentToken = Token.Comma;
                else if (token == ":")
                    currentToken = Token.Colon;
                else if (token == ";")
                    currentToken = Token.Semicolon;
                else if (token.StartsWith("FN"))
                    currentToken = Token.UserDefinedFunction;
                else if (_statementTokens.Contains(token))
                    currentToken = Token.Statement;
                else if (_Operators.Contains(token))
                    currentToken = Token.Operator;
                else if (_standardFunctions.Contains(token))
                    currentToken = Token.StandardFunction;
                else if (double.TryParse(token, out _))//Ponder: we might want our own number parser.
                    currentToken = Token.Number;
                else
                    currentToken = Token.Other;
                result.Add(token, currentToken);
            }
            return result;
        }
    }
}