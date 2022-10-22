using A_BASIC_Language.Gui.WinForms;

Application.SetCompatibleTextRenderingDefault(false);
Application.EnableVisualStyles();
var main = new TerminalEmulator();
main.ProgramFilename = args.FirstOrDefault() ?? "";

if (string.IsNullOrWhiteSpace(main.ProgramFilename))
    main.ShowEmptyTerminal();
else
    main.Run(true); 

Application.Run(main);