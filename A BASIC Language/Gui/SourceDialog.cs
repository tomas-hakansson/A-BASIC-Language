namespace A_BASIC_Language.Gui;

public partial class SourceDialog : Form
{
    public string SourceCode { get; set; }
    public string Filename { get; set; }

    public SourceDialog()
    {
        InitializeComponent();
    }

    private void SourceDialog_Load(object sender, EventArgs e)
    {
        Text = Filename;
        textBox1.Text = SourceCode;
        
        if (textBox1.Text.Length > 0)
        {
            textBox1.SelectionStart = 0;
            textBox1.SelectionLength = 0;
        }
    }
}