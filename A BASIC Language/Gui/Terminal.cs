using A_BASIC_Language.Gui.WinForms;

namespace A_BASIC_Language.Gui;

public class Terminal : IDisposable
{
    private TerminalEmulator Emu { get; }
    
    public Terminal(TerminalEmulator emu)
    {
        Emu = emu;
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

        Emu.State = TerminalState.Running;
    }

    public TerminalState State
    {
        get => Emu.State;
        set => Emu.State = value;
    }

    public void End()
    {
        Emu.EndLineInput();
        Emu.State = TerminalState.Ended;
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

            if (Emu.State != TerminalState.Running)
                return "";

        } while (Emu.LineInputMode);

        return Emu.LineInputResult;
    }

    public void Dispose()
    {
        Emu.Close();
        Emu.Dispose();
    }
}