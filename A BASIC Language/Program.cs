using A_BASIC_Language.Gui;

Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);
var main = new MainWindow();
main.ProgramFilename = args.FirstOrDefault() ?? "";
Application.Run(main);