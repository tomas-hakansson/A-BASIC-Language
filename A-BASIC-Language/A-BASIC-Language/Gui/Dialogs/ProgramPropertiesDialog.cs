#nullable enable
using A_BASIC_Language.Language;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace A_BASIC_Language.Gui.Dialogs;

public partial class ProgramPropertiesDialog : Form
{
    public BasicSession? BasicSession { private get; set; }
    public bool AutoClose { private get; set; }
    private readonly MsgBox _msgBox;

    public ProgramPropertiesDialog()
    {
        InitializeComponent();
        _msgBox = new MsgBox(Owner);
    }

    private void ProgramPropertiesDialog_Load(object sender, EventArgs e)
    {
        if (BasicSession == null)
            throw new SystemException("BasicSession not initialized.");

        if (BasicSession.ProgramFile == null)
        {
            txtFilename.Text = @"Nothing here.";
            txtSourceFolder.Text = txtFilename.Text;
            btnOpenSourceFolder.Enabled = false;
        }
        else
        {
            txtFilename.Text = BasicSession.ProgramFile.FullName;
            txtSourceFolder.Text = BasicSession.ProgramFile.Directory?.FullName ?? "Nothing here.";
            btnOpenSourceFolder.Enabled = true;
        }

        if (BasicSession.DataDirectory == null)
        {
            txtDataFolder.Text = @"Nothing here.";
            btnOpenDataFolder.Enabled = false;
        }
        else
        {
            txtDataFolder.Text = BasicSession.DataDirectory.FullName;
            btnOpenDataFolder.Enabled = true;
        }

        var state = BasicSession.GetRuntimeState();

        foreach (var variable in state.Variables)
            lvVariables.Add(variable.Value, variable.Key, variable.Value.ToString());
        
        foreach (var array in state.Arrays)
            lvArrays.Add(array.Value, array.Key, array.Value.ToString());

        foreach (var function in state.Functions)
            lvFunctions.Add(function.Value, function.Key, function.Value.ToString());
    }

    private void ProgramPropertiesDialog_Shown(object sender, EventArgs e)
    {
        if (BasicSession == null)
            throw new SystemException("BasicSession not initialized.");

        Cursor = Cursors.WaitCursor;
        Refresh();
        WriteLine("Ensure that the 101 BASIC Games folder exists...");
        BasicSession.CreateDataDirectories(WriteLine);
        Cursor = Cursors.Default;
        Refresh();

        if (AutoClose)
            Close();
    }

    private void WriteLine(string text, bool withRefresh = true)
    {
        txtStatus.AppendText(Environment.NewLine + text);
        txtStatus.SelectionStart = txtStatus.Text.Length;
        txtStatus.SelectionLength = 0;

        if (withRefresh)
            Refresh();

        Application.DoEvents();
    }

    private void btnOpenSourceFolder_Click(object sender, EventArgs e)
    {
        var path = txtSourceFolder.Text;

        if (string.IsNullOrEmpty(path) || path.StartsWith(@"Nothing"))
        {
            _msgBox.Tell("No source folder to open.");
            return;
        }

        try
        {
            Process.Start("explorer.exe", path);
        }
        catch
        {
            _msgBox.Fail("Failed to open the source folder.");
        }
    }

    private void btnOpenDataFolder_Click(object sender, EventArgs e)
    {
        var path = txtDataFolder.Text;

        if (string.IsNullOrEmpty(path) || path.StartsWith(@"Nothing"))
        {
            _msgBox.Tell("No data folder to open.");
            return;
        }

        try
        {
            Process.Start("explorer.exe", path);
        }
        catch
        {
            _msgBox.Fail("Failed to open the source folder.");
        }
    }
}
