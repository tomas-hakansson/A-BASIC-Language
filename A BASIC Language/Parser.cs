// See https://aka.ms/new-console-template for more information

internal class Parser
{
    public ParsedProgram Program { get; private set; }

    public Parser(string pathToMain)
    {
        var lines = File.ReadAllLines(pathToMain).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
        StageOne.Tokenizer t1 = new(lines);
        StageTwo.Tokenizer t2 = new(t1.TokenizedLines);
        StageThree.Parser p1 = new(t2.Result);
        Program = p1.Program;
    }
}