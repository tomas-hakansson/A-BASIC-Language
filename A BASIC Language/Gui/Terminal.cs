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

    public void NextTab(string s) =>
        Emu.NextTab(s);

    public void NextTab() =>
        Emu.NextTab();

    public void Write(string s) =>
        Emu.Write(s);

    public void WriteLine() =>
        Emu.WriteLine();

    public void WriteLine(string s) =>
        Emu.WriteLine(s);

    public string ReadLine(string prompt)
    {
        Write(prompt);
        return ReadLine();
    }

    public string ReadLine()
    {
        Emu.BeginLineInput();

        do
        {
            Application.DoEvents();

            if (_ts.State != TerminalState.Running)
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