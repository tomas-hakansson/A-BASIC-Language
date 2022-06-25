// See https://aka.ms/new-console-template for more information


namespace StageTwo
{
    public class Result//todo: come up with better name.
    {
        public List<Line> Tokens { get; set; }

        public Result()
        {
            Tokens = new List<Line>();
        }

        public void Add(Line line)
        {
            if (line.Tokens != null && line.Tokens.Count >= 2 &&
               line.TextValue != null && line.TextValue.Count >= 2)
                Tokens.Add(line);
        }
    }

    public class Line
    {
        public List<string> TextValue { get; set; }
        public List<Token> Tokens { get; set; }

        public Line()
        {
            TextValue = new List<string>();
            Tokens = new List<Token>();
        }

        public void Add(string textValue, Token token)
        {
            if (textValue != null && Enum.IsDefined<Token>(token))
            {
                TextValue.Add(textValue);
                Tokens.Add(token);
            }
        }
    }

    public enum Token
    {
        Label,
        OpeningParenthesis,
        ClosingParenthesis,
        Statement,
        StandardFunction,
        Number,
        Other,//Note: variables, user defined functions etc.
    }
    public class Tokenizer2
    {
        public Result Result { get; private set; }

        List<List<string>> _tokenizedLines;
        List<string> _statements;
        List<string> _standardFunctions;

        public Tokenizer2(List<List<string>> tokenizedLines)
        {
            Result = new();
            _tokenizedLines = tokenizedLines;
            _statements = new List<string>() { "INPUT", "=", "PRINT", "GOTO" };
            _standardFunctions = new List<string>() { "SQR", "^" };
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
                else if (_statements.Contains(token))
                    currentToken = Token.Statement;
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