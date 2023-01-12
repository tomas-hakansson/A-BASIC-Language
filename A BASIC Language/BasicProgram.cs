using System.Windows.Forms.VisualStyles;

namespace A_BASIC_Language;

public class BasicProgram
{
    public string SourceCode { get; set; }
    public string Filename { get; set; }

    public BasicProgram() : this("", "")
    {
    }

    public BasicProgram(string sourceCode, string filename)
    {
        SourceCode = sourceCode;
        Filename = filename;
    }

    public static BasicProgram Empty() =>
        new();
}