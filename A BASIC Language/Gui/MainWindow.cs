using System.Diagnostics.Tracing;
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
            throw new SystemException();

        Terminal.Run(ProgramFilename);

        var pathToMain = Path.GetFullPath(ProgramFilename);

        var ioDispatcher = new Dispatcher();
        var io = ioDispatcher.GetIo(pathToMain);
        var source = io.Load();

        if (!source.Result)
        {
            // TODO: Warn and quit
        }

        if (source.IsEmpty)
        {
            // TODO: Handle empty code
        }

        Interpreter eval = new(source.DataAsList());
        eval.Run(Terminal);
    }

    private void btnLoad_Click(object sender, EventArgs e)
    {

    }

    private void Run()
    {

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

    }
}