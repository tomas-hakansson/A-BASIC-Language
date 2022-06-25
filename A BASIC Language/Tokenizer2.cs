// See https://aka.ms/new-console-template for more information


public class Tokenizer2
{
    public ParsedProgram Program { get; private set; }

    List<List<string>> _tokenizedLines;

    public Tokenizer2(List<List<string>> tokenizedLines)
    {
        //_lines = new List<Line>
        //{
        //    new Line(new List<Token>() { new Procedure("INPUT"), new Assignment("N") }),
        //    new Line(new List<Token>() { new Variable("N"), new Procedure("SQR"), new Procedure("SQR"), new Assignment("I") }),
        //    new Line(new List<Token>() { new Variable("I"), new Number(2), new Procedure("^"), 
        //                                                    new Number(2), new Procedure("^"), new Assignment("J") }),
        //    new Line(new List<Token>() { new Variable("N"), new Procedure("PRINT") }),
        //    new Line(new List<Token>() { new Variable("J"), new Procedure("PRINT") }),
        //    new Line(new List<Token>() { new Number(20), new Procedure("GOTO") }),
        //};
        //_labelIndex = new Dictionary<int, int>
        //{
        //    { 20, 0 },
        //    { 25, 1 },
        //    { 30, 2 },
        //    { 35, 3 },
        //    { 38, 4 },
        //    { 40, 5 },
        //};
        Program = new ParsedProgram();
        _tokenizedLines = tokenizedLines;
        foreach (var line in _tokenizedLines)
        {
            Line(line);
        }
    }

    private void Line(List<string> line)
    {
        List<Token> tokens = new();
        Program.Add(Label(), new global::Line(tokens));

        int Label()
        {
            if (int.TryParse(line[0], out var parsedInt))
            {
                return parsedInt;
            }
            throw new ArgumentException("todo: proper error handling here.");
        }
    }

}