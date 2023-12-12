using A_BASIC_Language.Gui;

Application.SetCompatibleTextRenderingDefault(false);
Application.EnableVisualStyles();
var main = new TerminalEmulator();
main.ProgramFilename = args.FirstOrDefault() ?? "";

if (string.IsNullOrWhiteSpace(main.ProgramFilename))
    await main.ShowEmptyTerminal();
else
{
    main.Show();
    main.Run(true);
}

try
{
    Application.Run(main);
}
catch (ObjectDisposedException e)
{
    // Not sure why it does this.
    Console.WriteLine(e.Message);
}