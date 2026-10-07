using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using A_BASIC_Language.Games;
using A_BASIC_Language.Gui;
using A_BASIC_Language.Gui.Dialogs;
using A_BASIC_Language.Language;
using A_BASIC_Language.MainWindowControllers;
using A_BASIC_Language.StringManipulation;
using TerminalMatrixNetFramework;

namespace A_BASIC_Language;

public partial class MainWindow : Form
{
    private readonly BasicSession _session = new();
    private bool _logVisible;
    private readonly Log _log;
#if !DEBUG
    private bool _promptQuit;
#endif

    public MainWindow()
    {
        InitializeComponent();
#if !DEBUG
        _promptQuit = true;
#endif
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

    private void MainWindow_Load(object sender, EventArgs e) =>
        new WindowConfigurator(terminalMatrixControl1, _log).Configure(this, resolutionToolStripMenuItem);

    private void exitToolStripMenuItem_Click(object sender, EventArgs e) =>
        Close();

    private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
    {
#if !DEBUG
        if (_promptQuit && e.CloseReason == CloseReason.UserClosing && !MsgBox.Ask(this, @"Are you sure you want to close this application?"))
            e.Cancel = true;
#endif
    }

    public void debugOutputToolStripMenuItem_Click(object sender, EventArgs e)
    {
        debugOutputToolStripMenuItem.Checked = !debugOutputToolStripMenuItem.Checked;
        btnDebug.Checked = debugOutputToolStripMenuItem.Checked;
        _logVisible = debugOutputToolStripMenuItem.Checked;

        if (_logVisible)
            _log.Clear();

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

        if (_logVisible)
            _log.Write(text);

        fullscreenToolStripMenuItem_Click(sender, e);
        lblUserAction.Text = text.MaxLength(10, 30);
    }

    private void terminalMatrixControl1_TypedLine(object sender, TerminalMatrixNetFramework.Events.TypedLineEventArgs e)
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

        if (_logVisible)
            _log.Write(text);

        lblUserAction.Text = text;

        _session.Execute(
            e.InputValue,
            _log,
            new TerminalAdapter(terminalMatrixControl1),
            terminalMatrixControl1.GetProgramAsString,
            (first, last) =>
            {
                if (!first.HasValue && !last.HasValue)
                {
                    terminalMatrixControl1.List();
                    return;
                }
                foreach (var line in terminalMatrixControl1.ProgramLines
                             .Where(line => (!first.HasValue || line.Key >= first.Value) &&
                                            (!last.HasValue || line.Key <= last.Value))
                             .OrderBy(line => line.Key))
                    terminalMatrixControl1.WriteLine(line.Value.RawString);
            },
            terminalMatrixControl1.New);
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

        if (_logVisible)
            _log.Write(text);

        lblUserAction.Text = text;

        _session.Break();
    }

    private void terminalMatrixControl1_Paint(object sender, PaintEventArgs e)
    {
        if (_logVisible)
            _log.Paint(e.Graphics, terminalMatrixControl1);
    }

    private void MainWindow_Shown(object sender, EventArgs e)
    {
        terminalMatrixControl1.Focus();
        Refresh();
    }

    private void terminalMatrixControl1_Leave(object sender, EventArgs e) =>
        terminalMatrixControl1.Focus();

    private void terminalMatrixControl1_Tick(object sender, TerminalMatrixNetFramework.Events.TickEventArgs e) =>
        lblCursPos.Text = $@"Cursor: {e.CursorX}, {e.CursorY}";

    private void terminalMatrixControl1_InputCompleted(object sender, TerminalMatrixNetFramework.Events.TypedLineEventArgs e)
    {
        var text = $"{DateTime.Now:mm:HH:ss} - Input command completed.";

        if (_logVisible)
            _log.Write(text);

        lblUserAction.Text = text;
    }

    private void highQualityRenderingToolStripMenuItem_Click(object sender, EventArgs e)
    {
        highQualityRenderingToolStripMenuItem.Checked = !highQualityRenderingToolStripMenuItem.Checked;
        terminalMatrixControl1.RenderingMode = highQualityRenderingToolStripMenuItem.Checked ? RenderingMode.HighQuality : RenderingMode.HighSpeed;
        Refresh();
    }

    private void fullscreenToolStripMenuItem_Click(object sender, EventArgs e) =>
        ToggleFullscreen(!_logVisible && !fullscreenToolStripMenuItem.Checked);

    public void ToggleFullscreen(bool warn)
    {
        if (warn)
        {
            if (MessageBox.Show(this, @"Enter fullscreen mode? Use F11 to exit.", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                fullscreenToolStripMenuItem.Checked = !fullscreenToolStripMenuItem.Checked;
                return;
            }
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

    private void onlineHelpToolStripMenuItem_Click(object sender, EventArgs e)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "https://abl.winsoft.se/",
                UseShellExecute = true
            };
            Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, @"Failed to open online help", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var asm = Assembly.GetExecutingAssembly();
        var vInfo = FileVersionInfo.GetVersionInfo(asm.Location);
        var v = vInfo.ProductVersion!.Split(['.', '+']);

        MessageBox.Show(this, $@"ABL - A BASIC Language v{v[0]}.{v[1]}

An Altair BASIC player, written by Tomas Håkansson and Anders Hesselbom", @"About ABL", MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void versionHistoryToolStripMenuItem_Click(object sender, EventArgs e)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "https://github.com/tomas-hakansson/A-BASIC-Language/blob/master/README.md",
                UseShellExecute = true
            };
            Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, @"Failed to open version history", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using var x = new OptionsDialog();
        x.Resolution = terminalMatrixControl1.Resolution;

        if (x.ShowDialog(this) != DialogResult.OK)
            return;

        if (terminalMatrixControl1.Resolution != x.Resolution)
        {
            terminalMatrixControl1.SetResolution(x.Resolution);
            terminalMatrixControl1.WriteLine("Changed resolution.");
        }
    }

    private void btnOptions_Click(object sender, EventArgs e) =>
        optionsToolStripMenuItem_Click(sender, e);

    private void LoadProgram(string source)
    {
        terminalMatrixControl1.ProgramLines.Clear();
        terminalMatrixControl1.SetProgramLines(source);
        terminalMatrixControl1.List();
        terminalMatrixControl1.WriteLine("");
        terminalMatrixControl1.WriteLine("Ready.");
    }

    private void aceyDuceyToolStripMenuItem_Click(object sender, EventArgs e) =>
        LoadProgram(BasicPrograms.AceyDucey);

    private void amazinToolStripMenuItem_Click(object sender, EventArgs e) =>
        LoadProgram(BasicPrograms.Amazin);

    private void animalToolStripMenuItem_Click(object sender, EventArgs e) =>
        LoadProgram(BasicPrograms.Animal);
}
