namespace TerminalMatrixNetFramework;

public class ProgramLine
{
    public string RawString { get; }
    public int LineNumber { get; }
    public string Code { get; }

    public ProgramLine(string rawString, int lineNumber, string code)
    {
        RawString = rawString;
        LineNumber = lineNumber;
        Code = code;
    }
}
