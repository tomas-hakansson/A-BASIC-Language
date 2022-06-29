using A_BASIC_Language.Stage1;

namespace A_BASIC_Language;

internal class Parser
{
    public ParsedProgram Program { get; }

    public Parser(string pathToMain)
    {
        var lines = File.ReadAllLines(pathToMain).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
        Tokenizer t1 = new(lines);
        StageTwo.Tokenizer t2 = new(t1.TokenizedLines);
        Stage3.Parser p1 = new(t2.Result);
        Program = p1.Program;
    }
}