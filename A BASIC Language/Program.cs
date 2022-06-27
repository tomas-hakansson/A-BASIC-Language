using A_BASIC_Language.Gui;

namespace A_BASIC_Language;

public static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        var main = new MainWindow();
        main.ProgramFilename = args.FirstOrDefault();
        Application.Run(main);
    }
}