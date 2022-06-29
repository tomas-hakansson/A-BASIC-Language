using StageTwo;

namespace A_BASIC_Language.Stage3
{
    internal class Parser
    {
        public ParsedProgram Program { get; set; }

        public Parser(Result result)
        {
            Program = new();

            foreach (Stage2.Line line in result.Tokens)
            {
                LineParser lp = new(line);
                Program.Add(lp.Label, lp.Line);
            }
        }

        
    }
}
