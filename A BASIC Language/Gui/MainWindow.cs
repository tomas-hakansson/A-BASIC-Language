using A_BASIC_Language.IO;

namespace A_BASIC_Language.Gui;

public partial class MainWindow : Form
{
    private string SourceCode { get; set; }
    private Terminal Terminal { get; }
    public string ProgramFilename { get; set; }

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

        ProgramFilename = Path.GetFullPath(x.Filename!);

        Run();
    }

    private void Run()
    {
        Cursor = Cursors.WaitCursor;
        Terminal.Run(ProgramFilename);

        var ioDispatcher = new Dispatcher();
        var io = ioDispatcher.GetIo(ProgramFilename);
        var source = io.Load();

        if (!source.Result)
        {
            Cursor = Cursors.Default;
            MessageBox.Show(@"Failed to load source code.", @"Run failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        SourceCode = source.Data;

        if (source.IsEmpty)
        {
            Cursor = Cursors.Default;
            MessageBox.Show(@"Empty file loaded.", @"Run failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Interpreter eval = new(SourceCode);
        Cursor = Cursors.Default;
        eval.Run(Terminal);
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