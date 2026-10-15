using System.Windows.Forms;

namespace A_BASIC_Language.Gui;

public partial class SourceCodeDialog : Form
{
    public string SourceCode { get; set; }

    public SourceCodeDialog()
    {
        InitializeComponent();
    }

    private void SourceCodeDialog_Load(object sender, System.EventArgs e)
    {
        textBox1.Text = SourceCode;
    }

    private void btnOk_Click(object sender, System.EventArgs e)
    {
        SourceCode = textBox1.Text;
        DialogResult = DialogResult.OK;
    }
}
