namespace A_BASIC_Language.Gui.WinForms;

public class WelcomeScreen
{
    private readonly int _columnCount;
    private readonly Action _writeEmptyLine;
    private readonly Action<string> _writeLine;
    private readonly Action _writeSeparator;

    public WelcomeScreen(int columnCount, Action writeEmptyLine, Action<string> writeLine, Action writeSeparator)
    {
        _columnCount = columnCount;
        _writeEmptyLine = writeEmptyLine;
        _writeLine = writeLine;
        _writeSeparator = writeSeparator;
    }

    public void Show(string program)
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

        if (string.IsNullOrWhiteSpace(program))
        {
            _writeLine($"{new string(' ', spaces - 1)}written by Tomas Hakansson");
            _writeLine($"{new string(' ', spaces + 2)}and Anders Hesselbom");
            _writeEmptyLine();
        }
        else
        {
            _writeEmptyLine();
        }

        if (!string.IsNullOrWhiteSpace(program))
        {
            _writeLine("Ready.");
            _writeEmptyLine();
            _writeLine("Loaded program:");
            _writeLine(program);
            _writeSeparator();
        }
        else
        {
            _writeLine("Ready. Type LOAD or QUIT.");
        }
    }
}