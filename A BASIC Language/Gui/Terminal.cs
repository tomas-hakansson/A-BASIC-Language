using A_BASIC_Language.Gui.WinForms;

namespace A_BASIC_Language.Gui;

public class Terminal : IDisposable
{
    private TerminalEmulator Emu { get; }
    private Thread? Thread { get; set; }

    public Terminal()
    {
        Emu = new TerminalEmulator();
    }

    public void Run(string title)
    {
        Emu.Text = title;
        Emu.Show();
        Emu.BringToFront();
    }
    
    public void Write(string s) =>
        Emu.Write(s);

    public void WriteLine(string s) =>
        Emu.WriteLine(s);

    public void Dispose()
    {
        Emu.Close();
        Emu.Dispose();
    }
}