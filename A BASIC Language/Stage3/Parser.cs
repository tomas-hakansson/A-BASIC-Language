using StageTwo;

namespace StageThree
{
    internal class Parser
    {
        public ParsedProgram Program { get; set; }

        public Parser(Result result)
        {
            Program = new();

            foreach (StageTwo.Line line in result.Tokens)
            {
                LineParser lp = new(line);
                Program.Add(lp.Label, lp.Line);
            }
        }

        
    }
}
