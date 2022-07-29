using A_BASIC_Language.IO;

namespace A_BASIC_Language.Gui;

public partial class MainWindow : Form
{
    public static ProgramRepository ProgramRepository { get; }
    private string SourceCode { get; set; }
    private Terminal Terminal { get; }
    public string ProgramFilename { get; set; }

    static MainWindow()
    {
        ProgramRepository = new ProgramRepository();
    }

    public MainWindow()
    {
        InitializeComponent();
        SourceCode = "";
        ProgramFilename = "";
        Terminal = new Terminal();
    }

    private void MainWindow_Shown(object sender, EventArgs e)
    {
        Refresh();

        if (string.IsNullOrWhiteSpace(ProgramFilename))
        {
            using var x = new LoadProgramDialog();

            if (x.ShowDialog(this) != DialogResult.OK)
            {
                btnQuit_Click(this, EventArgs.Empty);
                return;
            }

            ProgramFilename = Path.GetFullPath(x.Filename ?? "");
        }

        Run();
    }

    private void btnLoad_Click(object sender, EventArgs e)
    {
        using var x = new LoadProgramDialog();

        if (x.ShowDialog(this) != DialogResult.OK)
            return;

        var f = x.Filename ?? "";

        if (f.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase) || f.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase))
            ProgramFilename = f;
        else
            ProgramFilename = Path.GetFullPath(x.Filename!);

        Run();
    }

    private void Run()
    {
        Cursor = Cursors.WaitCursor;

        var source = ProgramRepository.GetProgram(this, ProgramFilename, out var nameOnly);

        if (string.IsNullOrWhiteSpace(source))
        {
            Terminal.WriteLine("Load failed.");
            return;
        }

        SourceCode = source;

        Interpreter eval = new(SourceCode);
        Cursor = Cursors.Default;
        eval.Run(Terminal);
        Terminal.Run(nameOnly, ProgramFilename);
    }

    private void btnRestart_Click(object sender, EventArgs e)
    {
        Terminal.Running = false;
        Application.DoEvents();

        Run();
    }

    private void btnSource_Click(object sender, EventArgs e)
    {
        using var x = new SourceDialog();
        x.Filename = ProgramFilename;
        x.SourceCode = SourceCode;
        x.ShowDialog(this);
    }

    private void btnQuit_Click(object sender, EventArgs e)
    {
        Quit();
    }

    public void Quit()
    {
        Terminal.Running = false;
        Application.DoEvents();

        var formsToClose = Application.OpenForms.Cast<Form>().ToList();

        foreach (var x in formsToClose)
        {
            Application.DoEvents();
            try
            {
                x.Close();
            }
            catch
            {
                // ignored
            }
            try
            {
                x.Dispose();
            }
            catch
            {
                // ignored
            }
        }
        Application.Exit();
    }

    private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
    {
        Quit();
    }
}