using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace A_BASIC_Language.TerminalMatrix;

public class ProgramLineDictionary : SortedDictionary<int, ProgramLine>
{
    public void InsertProgramLine(ProgramLine programLine)
    {
        Remove(programLine.LineNumber);
        Add(programLine.LineNumber, programLine);
    }

    public void RemoveProgramLine(int number)
    {
        Remove(number);
    }

    public override string ToString()
    {
        var s = new StringBuilder();

        foreach (var l in this.OrderBy(x => x.Key))
            s.AppendLine($"{l.Key} {l.Value.Code}");

        return s.ToString();
    }
}
