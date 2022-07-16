namespace A_BASIC_Language.Stage1;

public class Result//todo: come up with better name.
{
    public List<Line> Tokens { get; set; }

    public Result()
    {
        Tokens = new List<Line>();
    }

    public void Add(Line line)
    {
        if (line.TokenTypes != null && line.TokenTypes.Count >= 2 &&
            line.TokenValues != null && line.TokenValues.Count >= 2)
            Tokens.Add(line);
    }
}
