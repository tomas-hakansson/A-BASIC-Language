namespace A_BASIC_Language.Gui.WinForms;

public class WelcomeScreen
{
    private readonly int _columnCount;
    private readonly Func<Task> _writeEmptyLine;
    private readonly Func<string, Task> _writeLine;
    private readonly Func<Task> _writeSeparator;

    public WelcomeScreen(int columnCount, Func<Task> writeEmptyLine, Func<string, Task> writeLine, Func<Task> writeSeparator)
    {
        _columnCount = columnCount;
        _writeEmptyLine = writeEmptyLine;
        _writeLine = writeLine;
        _writeSeparator = writeSeparator;
    }

    public void Show(string programFilename)
    {
        var spaces = 28;
        switch (_columnCount)
        {
            case 40:
                spaces = 8;
                break;
            case 60:
                spaces = 18;
                break;
        }

        _writeEmptyLine();
        _writeLine($"{new string(' ', spaces)}*** A BASIC LANGUAGE ***");
        _writeEmptyLine();
        _writeLine($"{new string(' ', spaces + 1)}Altair BASIC Emulator.");

        if (string.IsNullOrWhiteSpace(programFilename))
        {
            _writeLine($"{new string(' ', spaces - 1)}written by Tomas Hakansson");
            _writeLine($"{new string(' ', spaces + 2)}and Anders Hesselbom");
            _writeEmptyLine();
        }
        else
        {
            _writeEmptyLine();
        }

        if (!string.IsNullOrWhiteSpace(programFilename))
        {
            _writeLine("Ready.");
            _writeEmptyLine();
            _writeLine("Loaded programFilename:");
            _writeLine(programFilename);
            _writeSeparator();
        }
        else
        {
            _writeLine("Ready. Type LOAD or QUIT.");
        }
    }
}