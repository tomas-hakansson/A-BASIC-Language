namespace A_BASIC_Language.Gui;

public class Terminal
{
    private readonly TerminalEmulator _terminalEmulator;
    public bool QuitFlag { get; set; } //TODO!
    public bool UserBreak { get; set; } //TODO!
    public bool Runtime { get; set; }

    public Terminal(TerminalEmulator terminalEmulator)
    {
        _terminalEmulator = terminalEmulator;
        Runtime = false;
    }


    public void Run(string title, string programName, bool clear)
    {
        Title = title;

        if (clear)
        {
            _terminalEmulator.Clear();
            _terminalEmulator.ShowWelcome(programName);
        }

        _terminalEmulator.Run(clear);
    }

    public bool FullScreen
    {
        get => _terminalEmulator.IsFullScreen();
        set => _terminalEmulator.SetFullScreen(value);
    }

    public string Title
    {
        get => _terminalEmulator.Text;
        set => _terminalEmulator.Text = value;
    }

    public void WriteLine(string text)
    {
        _terminalEmulator.WriteLine(text);
    }

    public void WriteLine()
    {
        _terminalEmulator.WriteLine();
    }

    public void Write(string text)
    {
        _terminalEmulator.Write(text);
    }

    public string ReadLine()
    {
        return "Hello"; // TODO
    }

    public void NextTab()
    {
        // TODO
    }

    public void End()
    {
        Runtime = false;
    }
}