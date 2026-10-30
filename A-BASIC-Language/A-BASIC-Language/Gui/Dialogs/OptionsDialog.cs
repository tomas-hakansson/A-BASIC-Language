using System;
using System.Windows.Forms;
using A_BASIC_Language.Gui.Multimedia;
using A_BASIC_Language.TerminalMatrix;
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

    public ColorScheme ColorScheme
    {
        get => colorSchemeComboBox1.SelectedColorScheme.Id;
        set => colorSchemeComboBox1.SetSelectedColorScheme(value);
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
        HighQualityRendering = chkHighQualityRendering.Checked;
        DialogResult = DialogResult.OK;
    }
}
