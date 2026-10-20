using System.Collections.Generic;
using System.Text;

namespace TerminalMatrixNetFramework;

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


        return s.ToString();
    }
}
