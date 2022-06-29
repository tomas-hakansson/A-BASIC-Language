namespace A_BASIC_Language.Stage2;

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