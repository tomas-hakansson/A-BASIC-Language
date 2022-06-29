// See https://aka.ms/new-console-template for more information

using A_BASIC_Language;

internal class ParsedProgram
{
    public Dictionary<int, int> LabelIndex { get; set; }
    public List<Line> Lines { get; private set; }

    public ParsedProgram()
    {
        LabelIndex = new Dictionary<int, int>();
        Lines = new List<Line>();
    }

    public void Add(int label, Line line)
    {
        LabelIndex[label] = Lines.Count;
        Lines.Add(line);
    }
}