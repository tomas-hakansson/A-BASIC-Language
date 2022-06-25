// See https://aka.ms/new-console-template for more information

internal class Parser
{
    public ParsedProgram Program { get; private set; }

    public Parser(string pathToMain)
    {
        var lines = File.ReadAllLines(pathToMain).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
        Tokenizer1 t1 = new(lines);
        Tokenizer2? t2 = new(t1.TokenizedLines);
        Program = t2.Program;
    }
}