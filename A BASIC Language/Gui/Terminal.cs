using A_BASIC_Language.Gui.WinForms;

namespace A_BASIC_Language.Gui;

public class Terminal : IDisposable
{
    private TerminalEmulator Emu { get; }
    private readonly TerminalEmulatorStateStructure _ts;

    public Terminal(TerminalEmulator emu, TerminalEmulatorStateStructure ts)
    {
        Emu = emu;
        _ts = ts;
    }

    public void Run(string title, string programName, bool clear)
    {
        Title = title;
        Emu.BringToFront();

        if (clear)
        {
            Emu.Clear();
            Emu.ShowWelcome(programName);
        }

        _ts.State = TerminalState.Running;
    }

    public bool QuitFlag =>
        Emu.QuitFlag;

    public TerminalState State
    {
        get => _ts.State;
        set => _ts.State = value;
    }

    public void End()
    {
        Emu.EndLineInput();
        _ts.State = TerminalState.Ended;
    }

    public bool UserBreak
    {
        get => _ts.UserBreak;
        set => _ts.UserBreak = value;
    }

    public bool FullScreen
    {
        get => Emu.IsFullScreen();
        set => Emu.SetFullScreen(value);
    }

    public string Title
    {
        get => Emu.Text;
        set => Emu.Text = value;
    }

    public async Task NextTab(string s) =>
        await Emu.NextTab(s);

    public async Task NextTab() =>
        await Emu.NextTab();

    public async Task Write(string s) =>
        await Emu.Write(s);

    public async Task WriteLine() =>
        await Emu.WriteLine();

    public async Task WriteLine(string s) =>
        await Emu.WriteLine(s);

    public async Task<string> ReadLine(string prompt)
    {
        await Write(prompt);
        return ReadLine();
    }

    public string ReadLine()
    {
        Emu.BeginLineInput();

        do
        {
            Application.DoEvents();

            if (_ts.State != TerminalState.Running || QuitFlag)
                return "";

        } while (_ts.LineInputMode);

        return Emu.LineInputResult;
    }

    public void Dispose()
    {
        Emu.Close();
        Emu.Dispose();
    }
}