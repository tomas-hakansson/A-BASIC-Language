using A_BASIC_Language.Gui;
using A_BASIC_Language.Gui.Dialogs;
using A_BASIC_Language.Gui.Multimedia;
using A_BASIC_Language.IO;
using A_BASIC_Language.Language;
using A_BASIC_Language.MainWindowControllers;
using A_BASIC_Language.Properties;
using A_BASIC_Language.StringManipulation;
using A_BASIC_Language.TerminalMatrix.Events;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using A_BASIC_Language.TerminalMatrix.PixelMapLibrary.FontManagement;
using TerminalMatrixNetFramework;

namespace A_BASIC_Language;

public partial class MainWindow : Form
{
    public const string CantWriteToDisk = "?Permission to write to disk is denied by Windows.";
    private readonly IMsgBox _msgBox;
    private readonly VersionService _versionService;
    private readonly BasicSession _session = new();
    private bool _logVisible;
    private readonly Log _log;
    public static string UserDataPath { get; set; }

    static MainWindow()
    {
        UserDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "A-BASIC-Language");
        var d = new DirectoryInfo(UserDataPath);

        if (!d.Exists)
            d.Create();
    }

    public MainWindow()
    {
        InitializeComponent();
        terminalMatrixControl1.AttachFontMemory(_session.GetRuntimeState().Memory);
#if !DEBUG
        _promptQuit = true;
#endif
        _msgBox = new MsgBox(this);
        _versionService = new VersionService(this);
        _logVisible = false;
        _log = new Log(lblUserAction, new Font(FontFamily.GenericMonospace, 9, FontStyle.Regular));
    }

    

    internal void DisableQuitPrompt()
    {
#if !DEBUG
        _promptQuit = false;
#endif
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _session.Break();
        terminalMatrixControl1.Quit();
        base.OnFormClosed(e);
    }

    private void MainWindow_Load(object sender, EventArgs e)
    {
        var highQualityRendering = Settings.Default.HighQualityRendering;
        highQualityRenderingToolStripMenuItem.Checked = highQualityRendering;
        terminalMatrixControl1.RenderingMode = highQualityRendering ? RenderingMode.HighQuality : RenderingMode.HighSpeed;
        new WindowConfigurator(terminalMatrixControl1, _log).Configure(this, resolutionToolStripMenuItem, Settings.Default.Resolution);
        ApplyColorScheme((ColorScheme)Settings.Default.ColorScheme);
        
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e) =>
        Close();

    private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
    {
#if !DEBUG
        if (e.CloseReason == CloseReason.UserClosing && !_msgBox.Ask("Are you sure you want to close this application?"))
            e.Cancel = true;
#endif
    }

    public void debugOutputToolStripMenuItem_Click(object sender, EventArgs e)
    {
        debugOutputToolStripMenuItem.Checked = !debugOutputToolStripMenuItem.Checked;
        btnDebug.Checked = debugOutputToolStripMenuItem.Checked;
        _logVisible = debugOutputToolStripMenuItem.Checked;
        _log.TraceEnabled = _logVisible;
        _log.ClearIf(_logVisible);
        var text = $"{DateTime.Now:mm:HH:ss} - Logging {(debugOutputToolStripMenuItem.Checked ? "enabled" : "disabled")}.";
        _log.Write(text);
        lblUserAction.Text = text;
        terminalMatrixControl1.Invalidate();
    }

    private void btnDebug_Click(object sender, EventArgs e) =>
        debugOutputToolStripMenuItem_Click(sender, e);

    private void terminalMatrixControl1_RequestToggleFullscreen(object sender, EventArgs e)
    {
        var text = $"{DateTime.Now:mm:HH:ss} - Toggle full screen";
        _log.WriteIf(_logVisible, text);
        fullscreenToolStripMenuItem_Click(sender, e);
        lblUserAction.Text = text.MaxLength(10, 30);
    }

    private void terminalMatrixControl1_TypedLine(object sender, TypedLineEventArgs e)
    {
        if (e.InputValue.IsEmpty())
            return;

        var preProcessor = e.InputValue.Trim();

        if (preProcessor.StartsWith("!"))
        {
            new PreProcessorController(terminalMatrixControl1, this).Run(preProcessor, out var quitFlag);

            if (quitFlag)
                Close();

            return;
        }

        var text = e.InputValue.IsEmpty()
            ? $"{DateTime.Now:mm:HH:ss} - Press enter"
            : $"{DateTime.Now:mm:HH:ss} - Typed: {e.InputValue}";

        _log.WriteIf(_logVisible, text);
        lblUserAction.Text = text;

        _session.Execute(
            e.InputValue,
            _log,
            new TerminalAdapter(this, terminalMatrixControl1),
            terminalMatrixControl1.GetProgramAsString,
            (first, last) =>
            {
                if (!first.HasValue && !last.HasValue)
                {
                    terminalMatrixControl1.List();
                    return;
                }

                var lines = terminalMatrixControl1.ProgramLines
                    .Where(line => (!first.HasValue || line.Key >= first.Value) && (!last.HasValue || line.Key <= last.Value))
                    .OrderBy(line => line.Key);

                foreach (var line in lines)
                    terminalMatrixControl1.WriteLine(line.Value.RawString);

            },
            terminalMatrixControl1.New,
            source => terminalMatrixControl1.SetProgramLines(source));
    }

    public void CheckResolutionBox()
    {
        var resolution = terminalMatrixControl1.Resolution;

        foreach (ToolStripMenuItem item in resolutionToolStripMenuItem.DropDownItems)
            item.Checked = ((TerminalResolution)item.Tag!).Resolution == resolution;
    }

    private void terminalMatrixControl1_UserBreak(object sender, EventArgs e)
    {
        var text = $"{DateTime.Now:mm:HH:ss} - User break";
        _log.WriteIf(_logVisible, text);
        lblUserAction.Text = text;
        _session.Break();
    }

    private void terminalMatrixControl1_Paint(object sender, PaintEventArgs e) =>
        _log.PaintIf(_logVisible, e.Graphics, terminalMatrixControl1);

    private void MainWindow_Shown(object sender, EventArgs e)
    {
        terminalMatrixControl1.Focus();
        Refresh();
        CheckResolutionBox();

        using var x = new ProgramPropertiesDialog();
        x.AutoClose = true;
        x.BasicSession = _session;
        x.ShowDialog(this);
    }

    private void terminalMatrixControl1_Leave(object sender, EventArgs e) =>
        terminalMatrixControl1.Focus();

    private void terminalMatrixControl1_Tick(object sender, TickEventArgs e) =>
        lblCursPos.Text = $@"Cursor: {e.CursorX}, {e.CursorY}";

    private void terminalMatrixControl1_InputCompleted(object sender, TypedLineEventArgs e)
    {
        var text = $"{DateTime.Now:mm:HH:ss} - Input command completed.";
        _log.WriteIf(_logVisible, text);
        lblUserAction.Text = text;
    }

    public void highQualityRenderingToolStripMenuItem_Click(object sender, EventArgs e)
    {
        highQualityRenderingToolStripMenuItem.Checked = !highQualityRenderingToolStripMenuItem.Checked;
        terminalMatrixControl1.RenderingMode = highQualityRenderingToolStripMenuItem.Checked ? RenderingMode.HighQuality : RenderingMode.HighSpeed;
        Settings.Default.HighQualityRendering = highQualityRenderingToolStripMenuItem.Checked;
        Refresh();
    }

    private void fullscreenToolStripMenuItem_Click(object sender, EventArgs e) =>
        ToggleFullscreen(!_logVisible && !fullscreenToolStripMenuItem.Checked);

    public void ToggleFullscreen(bool warn)
    {
        if (warn)
        {
            if (!fullscreenToolStripMenuItem.Checked)
            {
                if (_msgBox.Ask("Enter fullscreen mode? Use F11 to exit."))
                {
                    new FullScreenController(this, menuStrip1, toolStrip1, statusStrip1).Set(true, terminalMatrixControl1);
                    fullscreenToolStripMenuItem.Checked = !fullscreenToolStripMenuItem.Checked;
                    _log.Write("Enter fullscreen (F11 to exit)");
                }
            }

            return;
        }

        fullscreenToolStripMenuItem.Checked = !fullscreenToolStripMenuItem.Checked;

        if (fullscreenToolStripMenuItem.Checked)
        {
            new FullScreenController(this, menuStrip1, toolStrip1, statusStrip1).Set(true, terminalMatrixControl1);
            _log.Write("Enter fullscreen (F11 to exit)");
        }
        else
        {
            new FullScreenController(this, menuStrip1, toolStrip1, statusStrip1).Set(false, terminalMatrixControl1);
            _log.Write("Exit fullscreen (F11 to re-enter)");
        }
    }

    private void onlineHelpToolStripMenuItem_Click(object sender, EventArgs e) =>
        _versionService.GetOnlineHelp();

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e) =>
        _msgBox.Tell(_versionService.GetAboutBoxText(), @"About ABL");

    private void versionHistoryToolStripMenuItem_Click(object sender, EventArgs e) =>
        _versionService.GetVersionHistory();

    private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using var x = new OptionsDialog();
        x.Resolution = terminalMatrixControl1.Resolution;
        x.HighQualityRendering = highQualityRenderingToolStripMenuItem.Checked;
        x.ColorScheme = (ColorScheme)Settings.Default.ColorScheme;
        x.TerminalFont = (CurrentFont)Settings.Default.Font;
        
        if (x.ShowDialog(this) != DialogResult.OK)
            return;

        if (terminalMatrixControl1.Resolution != x.Resolution)
        {
            terminalMatrixControl1.SetResolution(x.Resolution);
            CheckResolutionBox();
            terminalMatrixControl1.WriteLine("Changed resolution.");
        }

        Settings.Default.Resolution = x.Resolution;

        if ((ColorScheme)Settings.Default.ColorScheme != x.ColorScheme)
        {
            Settings.Default.ColorScheme = (int)x.ColorScheme;
            terminalMatrixControl1.WriteLine("Default color scheme changed.");
            ApplyColorScheme(x.ColorScheme);
        }

        if ((CurrentFont)Settings.Default.Font != x.TerminalFont)
        {
            Settings.Default.Font = (int)x.TerminalFont;
            terminalMatrixControl1.WriteLine("Font changed.");
            ApplyTerminalFont(x.TerminalFont);
        }

        if (x.HighQualityRendering != highQualityRenderingToolStripMenuItem.Checked)
        {
            highQualityRenderingToolStripMenuItem_Click(sender, e);
        }
    }

    private void ApplyColorScheme(ColorScheme scheme)
    {
        var colors = new ColorSchemeConfigurationList().Get(scheme);
        terminalMatrixControl1.CurrentCursorColor = (byte)colors.Text;
        terminalMatrixControl1.ClearColorMap();
        terminalMatrixControl1.SetBackgroundColor((byte)colors.Background);
        terminalMatrixControl1.SetBorderColor((byte)colors.Border);
    }

    private void ApplyTerminalFont(CurrentFont font)
    {
        var fontConfiguration = new FontConfigurationList().Get(font);
        var fontConfig = new FontConfigurationList().Get(fontConfiguration);
        terminalMatrixControl1.CurrentFont = fontConfig;
    }

    private void btnOptions_Click(object sender, EventArgs e) =>
        optionsToolStripMenuItem_Click(sender, e);

    public bool LoadProgram(string source, bool list = true)
    {
        try
        {
            terminalMatrixControl1.ProgramLines.Clear();
            terminalMatrixControl1.SetProgramLines(source);

            if (list)
                terminalMatrixControl1.List();

            terminalMatrixControl1.WriteLine("");
            terminalMatrixControl1.WriteLine("Ready.");
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool SaveProgram(string filename)
    {
        try
        {
            var path = Paths.GetUserProgramsRootPath();
            var fullPath = Path.Combine(path.FullName, filename);
            File.WriteAllText(fullPath, terminalMatrixControl1.GetProgramAsString());
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void SessionSetFolders(FileInfo basicFileInfo)
    {
        var success = false;

        try
        {
            if (_session.SetFolders(basicFileInfo))
                success = true;
        }
        catch
        {
            success = false;
        }

        if (!success)
        {
            lblUserAction.Text = CantWriteToDisk;
            terminalMatrixControl1.WriteLine(CantWriteToDisk);
        }
    }

    private void openFrom101BASICComputerGamesToolStripMenuItem_Click(object sender, EventArgs e)
    {
        try
        {
            var path = Paths.GetUser101GamesPath();
            using var x = new OpenFileDialog();
            x.InitialDirectory = path!.FullName;
            x.Filter = @"BASIC source files (*.bas)|*.bas|All files (*.*)|*.*";

            if (x.ShowDialog(this) != DialogResult.OK)
                return;

            var fileInfo = new FileInfo(x.FileName);
            var source = File.ReadAllText(fileInfo.FullName);
            LoadProgram(source);
            SessionSetFolders(fileInfo);
        }
        catch (Exception exception)
        {
            _msgBox.Fail(exception.Message, @"Failed to open BASIC source files");
        }
    }

    internal void RunProgram()
    {
        terminalMatrixControl1.WriteLine("RUN");
        terminalMatrixControl1_TypedLine(terminalMatrixControl1, new TypedLineEventArgs("RUN"));
    }

    private void runToolStripMenuItem_Click(object sender, EventArgs e) =>
        RunProgram();

    private void btnPlay_Click(object sender, EventArgs e) =>
        runToolStripMenuItem_Click(sender, e);

    private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
    {
        try
        {
            Settings.Default.Save();
        }
        catch
        {
            // ignored
        }
    }

    private void textEditorToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using var x = new SourceCodeDialog();
        x.SourceCode = terminalMatrixControl1.GetProgramAsString();
        
        if (x.ShowDialog(this) == DialogResult.OK)
            terminalMatrixControl1.SetProgramLines(x.SourceCode);
    }

    private void toolStripButton1_Click(object sender, EventArgs e) =>
        textEditorToolStripMenuItem_Click(sender, e);

    private void MainWindow_Enter(object sender, EventArgs e) =>
        terminalMatrixControl1.HasFocus = true;

    private void MainWindow_Leave(object sender, EventArgs e) =>
        terminalMatrixControl1.HasFocus = false;

    private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using var x = new ProgramPropertiesDialog();
        x.AutoClose = false;
        x.BasicSession = _session;
        x.ShowDialog(this);
    }

    private void btnProgramProperties_Click(object sender, EventArgs e) =>
        propertiesToolStripMenuItem_Click(sender, e);

    private void programDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
    {
        try
        {
            Process.Start("explorer.exe", Paths.GetProgramBasePath()!.FullName);
        }
        catch
        {
            _msgBox.Fail("Failed to open the source folder.");
        }
    }

    private void gamesDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
    {
        try
        {
            Process.Start("explorer.exe", Paths.GetUser101GamesPath()!.FullName);
        }
        catch
        {
            _msgBox.Fail("Failed to open the source folder.");
        }
    }

    private void userProgramsDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
    {
        try
        {
            Process.Start("explorer.exe", Paths.GetUserProgramsPath()!.FullName);
        }
        catch
        {
            _msgBox.Fail("Failed to open the source folder.");
        }
    }
}
