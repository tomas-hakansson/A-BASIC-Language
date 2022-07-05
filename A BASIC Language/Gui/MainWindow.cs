using A_BASIC_Language.IO;

namespace A_BASIC_Language.Gui;

public partial class MainWindow : Form
{
    private Terminal Terminal { get; }
    private Interpreter? Interpreter { get; }
    public string? ProgramFilename { get; set; }

    public MainWindow()
    {
        InitializeComponent();
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

            ProgramFilename = x.Filename;
        }

        Run(Path.GetFullPath(ProgramFilename!));
    }

    private void btnLoad_Click(object sender, EventArgs e)
    {
        using var x = new LoadProgramDialog();

        if (x.ShowDialog(this) != DialogResult.OK)
            return;

        Run(x.Filename!);
    }

    private void Run(string fullPath)
    {
        Terminal.Run(ProgramFilename!);

        var ioDispatcher = new Dispatcher();
        var io = ioDispatcher.GetIo(fullPath);
        var source = io.Load();

        if (!source.Result)
        {
            // TODO: Warn and quit
        }

        if (source.IsEmpty)
        {
            // TODO: Handle empty code
        }

        Interpreter eval = new(source.Data);
        eval.Run(Terminal);
    }

    private void btnPause_Click(object sender, EventArgs e)
    {

    }

    private void btnRestart_Click(object sender, EventArgs e)
    {

    }

    private void btnSource_Click(object sender, EventArgs e)
    {

    }

    private void btnQuit_Click(object sender, EventArgs e)
    {
        Terminal.Running = false;

        var formsToClose = Application.OpenForms.Cast<Form>().ToList();

        foreach (var x in formsToClose)
        {
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
    }
}