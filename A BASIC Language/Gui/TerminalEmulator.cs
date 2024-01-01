using A_BASIC_Language.IO;
using TerminalMatrix;
using TerminalMatrix.TerminalColor;

namespace A_BASIC_Language.Gui;

public partial class TerminalEmulator : Form
{
    private string SourceCode { get; set; }
    private bool FullScreen { get; set; }
    private Rectangle OldPosition { get; set; }
    private FormWindowState OldWindowState { get; set; }
    private Terminal Terminal { get; }
    public static ProgramRepository ProgramRepository { get; }
    public string ProgramFilename { get; set; }
    public string LineInputResult { get; private set; }

    static TerminalEmulator()
    {
        ProgramRepository = new ProgramRepository();
    }

    public TerminalEmulator()
    {
        InitializeComponent();
        LineInputResult = "";
        SourceCode = "";
        ProgramFilename = "";
        Terminal = new Terminal(this);
    }

    private void TerminalEmulator_Load(object sender, EventArgs e)
    {
        switch (Configuration.ColumnCount)
        {
            case 60:
                terminalMatrixControl1.SetResolution(Resolution.Pixels480x200Characters60x25);
                break;
            case 80:
                terminalMatrixControl1.SetResolution(Resolution.Pixels640x200Characters80x25);
                break;
        }

        terminalMatrixControl1.CurrentCursorColor = (int)ColorName.Orange;
        terminalMatrixControl1.BorderWidth = 10;
        terminalMatrixControl1.BorderHeight = 10;
    }

    public void Run(bool clear)
    {
        var source = ProgramRepository.GetProgram(this, ProgramFilename);

        if (string.IsNullOrWhiteSpace(source.SourceCode))
        {
            Terminal.WriteLine("Load failed.");
            return;
        }

        SourceCode = source.SourceCode;

        Interpreter eval = new(SourceCode);
        Text = ProgramFilename;
        eval.Run(Terminal);
    }

    public void ShowWelcome(string programName)
    {
        var spaces = 28;

        switch (terminalMatrixControl1.Resolution)
        {
            case Resolution.Pixels320x200Characters40x25:
                spaces = 8;
                break;
            case Resolution.Pixels480x200Characters60x25:
                spaces = 18;
                break;
        }

        terminalMatrixControl1.WriteLine("");
        terminalMatrixControl1.WriteLine($"{new string(' ', spaces)}*** A BASIC LANGUAGE ***");
        terminalMatrixControl1.WriteLine("");
        terminalMatrixControl1.WriteLine($"{new string(' ', spaces + 1)}Altair BASIC Emulator");

        if (string.IsNullOrWhiteSpace(programName))
        {
            terminalMatrixControl1.WriteLine($"{new string(' ', spaces - 1)}written by Tomas Hakansson");
            terminalMatrixControl1.WriteLine($"{new string(' ', spaces + 2)}and Anders Hesselbom");
            terminalMatrixControl1.WriteLine("");
        }
        else
        {
            terminalMatrixControl1.WriteLine("");
        }

        if (Configuration.ConfigurationMessages.Count > 0)
        {
            foreach (var configurationMessage in Configuration.ConfigurationMessages)
            {
                terminalMatrixControl1.WriteLine(configurationMessage);
                terminalMatrixControl1.WriteLine("");
            }
        }

        if (!string.IsNullOrWhiteSpace(programName))
        {
            terminalMatrixControl1.WriteLine("Ready.");
            terminalMatrixControl1.WriteLine("");
            terminalMatrixControl1.WriteLine("Loaded programFilename:");
            terminalMatrixControl1.WriteLine(programName);
            terminalMatrixControl1.WriteLine("");
        }
        else
        {
            terminalMatrixControl1.WriteLine("Ready. Type LOAD or QUIT.");
        }
    }

    public void Clear()
    {
        terminalMatrixControl1.Clear();
    }

    public bool IsFullScreen() =>
        FullScreen;

    public void SetFullScreen(bool fullScreen)
    {
        if (fullScreen & !FullScreen)
            DoSetFullScreen();
        else if (!fullScreen & FullScreen)
            DoSetWindowMode();
    }

    private void DoSetFullScreen()
    {
        FullScreen = true;
        var s = GetScreen();
        OldWindowState = WindowState;
        OldPosition = new Rectangle(Left, Top, Width, Height);
        WindowState = FormWindowState.Normal;
        FormBorderStyle = FormBorderStyle.None;
        Top = s.Bounds.Top;
        Left = s.Bounds.Left;
        Width = s.Bounds.Width;
        Height = s.Bounds.Height;
        Focus();
    }

    private void DoSetWindowMode()
    {
        FullScreen = false;
        FormBorderStyle = FormBorderStyle.Sizable;
        WindowState = OldWindowState;
        Width = OldPosition.Width;
        Height = OldPosition.Height;
        Top = OldPosition.Top;
        Left = OldPosition.Left;
        TopMost = false;
        Focus();
    }

    private Screen GetScreen() =>
        Screen.FromPoint(new Point(Left + Width / 2, Top + Height / 2));

    private void TerminalEmulator_Shown(object sender, EventArgs e)
    {
        FullScreen = false;
        OldWindowState = FormWindowState.Normal;
        OldPosition = new Rectangle(50, 50, 200, 200);
        Invalidate();
        ShowWelcome(ProgramFilename);
    }

    public void WriteLine(string text)
    {
        terminalMatrixControl1.WriteLine(text);
    }

    public void WriteLine()
    {
        terminalMatrixControl1.WriteLine("");
    }

    public void Write(string text)
    {
        terminalMatrixControl1.Write(text);
    }

    private void TerminalEmulator_Activated(object sender, EventArgs e)
    {
    }

    private void TerminalEmulator_Deactivate(object sender, EventArgs e)
    {
    }

    private void TerminalEmulator_FormClosed(object sender, FormClosedEventArgs e)
    {
        terminalMatrixControl1.Quit();
    }

    private void terminalMatrixControl1_TypedLine(object sender, TerminalMatrix.Events.TypedLineEventArgs e)
    {
        if (Terminal.Runtime)
            return;

        switch (e.InputValue.Trim().ToUpper())
        {
            case "":
                break;
            case "RESTART":
                if (!Terminal.Runtime)
                {
                    Application.DoEvents();
                    Run(false);
                }
                else
                {
                    WriteLine("Invalid state for restart.");
                }
                break;
            case "SOURCE":
                //if (_ts.State == TerminalState.Ended)
                //    _consoleHook.ShowSourceForm(SourceCode, ProgramFilename);
                //else
                //    await WriteLine("Invalid state for source.");
                break;
            case "LOAD":
                {
                    using var x = new LoadProgramDialog();

                    if (x.ShowDialog(this) != DialogResult.OK)
                        return;

                    var f = x.Filename ?? "";

                    if (f.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase) || f.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase))
                        ProgramFilename = f;
                    else
                        ProgramFilename = Path.GetFullPath(x.Filename!);

                    Run(false);
                }
                break;
            case "QUIT":
                Terminal.QuitFlag = true;
                Application.DoEvents();
                Close();
                break;
            default:
                WriteLine("Invalid simple direct mode input.");
                break;
        }
    }

    private void terminalMatrixControl1_RequestToggleFullscreen(object sender, EventArgs e) =>
        SetFullScreen(!FullScreen);

    private void terminalMatrixControl1_UserBreak(object sender, EventArgs e)
    {
        Terminal.UserBreak = true;
    }
}