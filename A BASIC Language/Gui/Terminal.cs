using A_BASIC_Language.Gui.WinForms;

namespace A_BASIC_Language.Gui;

public class Terminal : IDisposable
{
    private TerminalEmulator Emu { get; }
    private Thread? Thread { get; set; }
    public bool Running { get; set; }

    public Terminal()
    {
        Emu = new TerminalEmulator();
    }

    public void Run(string title)
    {
        Emu.Text = title;
        Emu.Show();
        Emu.BringToFront();
        Running = true;
    }
    
    public void Write(string s) =>
        Emu.Write(s);

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
        } while (Emu.LineInputMode);

        return Emu.LineInputResult;
    }

    public void Dispose()
    {
        Emu.Close();
        Emu.Dispose();
    }
}