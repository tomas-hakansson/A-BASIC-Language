using A_BASIC_Language.IO;

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

    }

    public async void ShowEmptyTerminal()
    {
        Interpreter eval = new(SourceCode);
        Terminal.Run("A BASIC Language", "", true);
        await eval.Run(Terminal);
    }

    public async void Run(bool clear)
    {
        Cursor = Cursors.WaitCursor;

        var source = await ProgramRepository.GetProgram(this, ProgramFilename);

        if (string.IsNullOrWhiteSpace(source.SourceCode))
        {
            Terminal.WriteLine("Load failed.");
            return;
        }

        SourceCode = source.SourceCode;

        Interpreter eval = new(SourceCode);
        Cursor = Cursors.Default;
        Terminal.Run(source.Filename, ProgramFilename, clear);
        await eval.Run(Terminal);
    }

    public void ShowWelcome(string programName)
    {
        // TODO: Write something on screen
    }

    public void Clear()
    {
        // TODO
    }

    public bool IsFullScreen() =>
        FullScreen;

    internal void ToggleFullScreen() =>
        SetFullScreen(!FullScreen);

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
        TopMost = true;
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
    }
}