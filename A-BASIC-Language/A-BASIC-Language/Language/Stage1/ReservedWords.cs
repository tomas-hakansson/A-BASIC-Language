using System.Collections.Generic;

namespace A_BASIC_Language.Language.Stage1;

internal class ReservedWords
{
    public List<string> Statements { get; set; }
    public List<string> Operators { get; set; }
    public List<string> Functions { get; set; }
    public List<string> Punctuation { get; set; }

    public ReservedWords()
    {
        Statements =
        [
            "DATA", "DEF", "DIM", "ELSE", "END", "FN",
            "FOR", "GOSUB", "GOTO", "GO", "IF", "INPUT", "LET", "NEXT", "ON", "PRINT",
            "READ", "REM", "RESTORE", "RETURN", "STEP", "STOP", "THEN", "TO"
        ];
        
        Operators = ["^", "+", "-", "*", "/", "<=", ">=", "<>", "!=", "<", ">", "NOT", "AND", "OR", "XOR"];
        
        Functions =
        [
            "ABS", "ASC", "ATN", "CHR$", "COS", "EXP", "INT", "LEFT$", "LEN", "LOG", "MID$", "RND", "RIGHT$", "SGN",
            "SIN", "SQR", "STR$", "TAB", "TAN", "VAL"
        ];

        Punctuation = ["(", ")", "=", ",", ":", ";", "$", "%"];
    }
}
