using TerminalMatrix;

namespace A_BASIC_Language.Gui.Dialogs;

public partial class OptionsDialog : Form
{
    public OptionsDialog()
    {
        InitializeComponent();
    }

    public Resolution Resolution
    {
        get => terminalResolutionComboBox1.Resolution;
        set => terminalResolutionComboBox1.Resolution = value;
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.OK;
    }
}