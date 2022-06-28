// See https://aka.ms/new-console-template for more information


namespace StageTwo
{
    internal class Result//todo: come up with better name.
    {
        public List<Line> Tokens { get; set; }

        public Result()
        {
            Tokens = new List<Line>();
        }

        public void Add(Line line)
        {
            if (line.Tokens != null && line.Tokens.Count >= 2 &&
               line.TextValues != null && line.TextValues.Count >= 2)
                Tokens.Add(line);
        }
    }
}