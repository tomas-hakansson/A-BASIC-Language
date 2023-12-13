using A_BASIC_Language.Gui;

Application.SetCompatibleTextRenderingDefault(false);
Application.EnableVisualStyles();
var main = new TerminalEmulator();
main.ProgramFilename = args.FirstOrDefault() ?? "";
main.Show();

try
{
    Application.Run(main);
}
catch (ObjectDisposedException e)
{
    // Not sure why it does this.
    Console.WriteLine(e.Message);
}