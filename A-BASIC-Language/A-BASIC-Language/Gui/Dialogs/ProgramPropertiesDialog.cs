#nullable enable
using System;
using System.Windows.Forms;
using A_BASIC_Language.Language;

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

    private void WriteLine(string text)
    {
        txtStatus.AppendText(Environment.NewLine + text);
        txtStatus.SelectionStart = txtStatus.Text.Length;
        txtStatus.SelectionLength = 0;
        Refresh();
        Application.DoEvents();
    }

    private void btnOpenSourceFolder_Click(object sender, EventArgs e)
    {
        try
        {

        }
        catch
        {
            _msgBox.Fail("Failed to open the source folder.");
        }
    }
}
