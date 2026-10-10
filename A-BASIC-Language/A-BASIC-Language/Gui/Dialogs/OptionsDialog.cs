using System;
using System.Windows.Forms;
using TerminalMatrixNetFramework;

namespace A_BASIC_Language.Gui.Dialogs;

public partial class OptionsDialog : Form
{
    public bool HighQualityRendering { get; set; }

    public OptionsDialog()
    {
        InitializeComponent();
    }

    private void OptionsDialog_Load(object sender, EventArgs e)
    {
        chkHighQualityRendering.Checked = HighQualityRendering;
    }

    public Resolution Resolution
    {
        get => terminalResolutionComboBox1.Resolution;
        set => terminalResolutionComboBox1.Resolution = value;
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
        HighQualityRendering = chkHighQualityRendering.Checked;
        DialogResult = DialogResult.OK;
    }
}
