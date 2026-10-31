#nullable enable
using System;
using System.Windows.Forms;
using A_BASIC_Language.Language;

namespace A_BASIC_Language.Gui.Dialogs;

public partial class ProgramPropertiesDialog : Form
{
    public BasicSession? BasicSession { private get; set; }

    public ProgramPropertiesDialog()
    {
        InitializeComponent();
    }

    private void ProgramPropertiesDialog_Load(object sender, System.EventArgs e)
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
    }
}
