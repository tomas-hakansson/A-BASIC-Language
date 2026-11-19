#nullable enable
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
using A_BASIC_Language.TerminalMatrix;
using A_BASIC_Language.TerminalMatrix.PixelMapLibrary.FontManagement;

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
            if (fullscreenToolStripMenuItem.Checked)
                return;

            if (!_msgBox.Ask("Enter fullscreen mode? Use F11 to exit."))
                return;

            new FullScreenController(this, menuStrip1, toolStrip1, statusStrip1).Set(true, terminalMatrixControl1);
            fullscreenToolStripMenuItem.Checked = !fullscreenToolStripMenuItem.Checked;
            _log.Write("Enter fullscreen (F11 to exit)");
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

    public bool LoadProgram(string source, bool list = true, bool showReady = true)
    {
        try
        {
            terminalMatrixControl1.ProgramLines.Clear();
            terminalMatrixControl1.SetProgramLines(source);

            if (list)
                terminalMatrixControl1.List();

            if (showReady)
                terminalMatrixControl1.WriteLine(true, "Ready.");

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
            var file = Paths.GetUserProgramFile(filename);
            File.WriteAllText(file.FullName, terminalMatrixControl1.GetProgramAsString());
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
        x.ProgramSource = terminalMatrixControl1.GetProgramAsString();
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

    private void newToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (!_msgBox.Ask("Are you sure you want to clear the current program from memory?"))
            return;

        terminalMatrixControl1.New();
    }

    private void btnNew_Click(object sender, EventArgs e) =>
        newToolStripMenuItem_Click(sender, e);

    private void openToolStripMenuItem_Click(object sender, EventArgs e)
    {
        try
        {
            var path = Paths.GetUserProgramsPath();
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

    private void btnOpen_Click(object sender, EventArgs e) =>
        openToolStripMenuItem_Click(sender, e);

    private void saveToolStripMenuItem_Click(object sender, EventArgs e)
    {
        try
        {
            var path = Paths.GetUserProgramsPath() ?? throw new IOException("Could not open the user programs folder.");

            using var dialog = new SaveFileDialog();
            dialog.InitialDirectory = path.FullName;
            dialog.FileName = _session.ProgramFile?.Name ?? string.Empty;
            dialog.Filter = @"BASIC source files (*.bas)|*.bas|All files (*.*)|*.*";
            dialog.DefaultExt = "bas";

            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;

            var file = new FileInfo(dialog.FileName);
            File.WriteAllText(file.FullName, terminalMatrixControl1.GetProgramAsString());
            SessionSetFolders(file);
        }
        catch (Exception exception)
        {
            _msgBox.Fail(exception.Message, @"Failed to save BASIC source file");
        }
    }

    private void btnSave_Click(object sender, EventArgs e) =>
        saveToolStripMenuItem_Click(sender, e);

    private void spaceFlightToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using var x = new SourceCodeDialog();
        x.SourceCode = @"10 REM SPACEFLIGHT - SCROLL AND BLOCKSPRITE DEMO
20 REM WHITE SHIP, STARFIELD AND GREEN SCROLLING TERRAIN
30 REM RUNS FOR ABOUT 30 SECONDS; CHANGE FRAMES OR DELAY
40 FRAMES=600:DELAY=50
50 DIM OLD(7,7),DX(4),DY(4)
60 COLOR 0,1:COLOR 1,2:COLOR 2,1
70 WINDOW:CLS 40:SCROLL 0:DELETEBLOCKSPRITE
80 RANDOMIZE
90 REM SAVE EIGHT GLYPHS; DEFINE SHIP, FLAME AND GROUND
100 FOR G=0 TO 7
110 FOR R=0 TO 7
120 A=53248+8*(240+G)+R
130 OLD(G,R)=PEEK(A):READ B:POKE A,B
140 NEXT R,G
150 FOR N=0 TO 4:READ DX(N),DY(N):NEXT N
160 REM GENERATE THE INITIAL LANDSCAPE, ONE COLUMN AT A TIME
165 H=3
170 FOR X=0 TO 39
180 GOSUB 1200
200 NEXT X
210 REM FIVE WHITE TILES FORM A SHIP POINTING RIGHT
220 X=8:Y=11
230 FOR N=0 TO 4
240 CREATEBLOCKSPRITE N+1,X+DX(N),Y+DY(N),240+N,2
250 NEXT N
260 CREATEBLOCKSPRITE 6,X-1,Y+1,245,8
270 REM SCROLL ONLY THE BACKGROUND; SPRITES KEEP THEIR POSITIONS
280 FOR T=1 TO FRAMES
290 SCROLL 5
300 X=39
310 GOSUB 1200
320 REM A GENTLE FLIGHT PATH, INDEPENDENT OF THE STARFIELD
330 X=8+INT(3*SIN(T/45))
340 Y=10+INT(4*SIN(T/65))
350 FOR N=0 TO 4
360 MOVEBLOCKSPRITE N+1,X+DX(N),Y+DY(N)
370 NEXT N
380 F=T-2*INT(T/2)
390 MOVEBLOCKSPRITE 6,X-1,Y+1,245+F
400 SLEEP DELAY
410 NEXT T
420 REM RESTORE THE GLYPHS AFTER NORMAL COMPLETION
430 DELETEBLOCKSPRITE
440 FOR G=0 TO 7
450 FOR R=0 TO 7
460 POKE 53248+8*(240+G)+R,OLD(G,R)
470 NEXT R,G
480 SCROLL 1:COLOR 1,2:CLS
490 PRINT ""SPACEFLIGHT COMPLETE""
500 END
1000 REM ADD A STAR TO ONE BACKGROUND CELL
1010 IF RND(1)>.07 THEN RETURN
1020 C=13:S=ASC(""."")
1030 IF RND(1)<.25 THEN C=16:S=ASC(""+"")
1040 IF RND(1)<.08 THEN C=2:S=ASC(""*"")
1050 PRINTAT X,Y,S,C
1060 RETURN
1200 REM GROUND HEIGHT CHANGES GENTLY BETWEEN 1 AND 5 ROWS
1210 H=H+SRND(-1,2)
1220 IF H<1 THEN H=1
1230 IF H>5 THEN H=5
1240 FOR Y=0 TO 24-H:GOSUB 1000:NEXT Y
1250 REM EXPLICIT CELL COLORS KEEP THE GROUND GREEN
1260 FOR Y=25-H TO 24:PRINTAT X,Y,247,6:NEXT Y
1270 RETURN
2000 REM SHIP: TOP FIN, REAR HULL, CENTRE, NOSE, BOTTOM FIN
2010 DATA 0,192,224,240,248,252,254,255
2020 DATA 63,127,255,255,255,127,63,0
2030 DATA 255,255,255,255,255,255,255,255
2040 DATA 128,224,248,254,248,224,128,0
2050 DATA 254,252,248,240,224,192,0,0
2060 REM TWO ENGINE FLAME FRAMES
2070 DATA 0,3,15,63,15,3,0,0
2080 DATA 0,7,31,255,31,7,0,0
2085 DATA 255,255,255,255,255,255,255,255
2090 REM SHIP TILE OFFSETS X,Y
2100 DATA 1,0,0,1,1,1,2,1,1,2
";

        if (x.ShowDialog(this) == DialogResult.OK)
            terminalMatrixControl1.SetProgramLines(x.SourceCode);
    }
}
