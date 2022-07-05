namespace A_BASIC_Language;

internal class ReservedWords
{
    public List<string> Statements { get; set; }
    public List<string> Operators { get; set; }
    public List<string> Functions { get; set; }
    public List<string> Punctuation { get; set; }

    public ReservedWords()
    {
        Statements = new List<string> { "DATA", "DEF","DIM", "ELSE", "END", "FN",
            "FOR", "GOSUB", "GOTO", "IF", "INPUT", "LET", "NEXT", "ON", "PRINT",
            "READ", "REM", "RESTORE", "RETURN", "STEP", "STOP","THEN","TO" };
        Operators = new List<string> { "^", "+", "-", "*", "/" , "<", ">", "<=", ">=", "NOT", "AND", "OR", "XOR"};
        Functions = new List<string> { "SQR", "INT" };
        Punctuation = new List<string> { "(", ")", "=", ",", ":", ";" };
    }
}