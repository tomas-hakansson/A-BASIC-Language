namespace A_BASIC_Language.Gui;

public partial class MainWindow : Form
{
    public string? ProgramFilename { get; set; }
    private Terminal Terminal { get; }

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
        Interpreter eval = new(pathToMain);
        eval.Run();
    }
}