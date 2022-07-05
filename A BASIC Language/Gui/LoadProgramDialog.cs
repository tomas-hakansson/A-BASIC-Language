namespace A_BASIC_Language.Gui;

public partial class LoadProgramDialog : Form
{
    public string? Filename { get; private set; }

    public LoadProgramDialog()
    {
        InitializeComponent();
    }

    private void LoadProgramDialog_Load(object sender, EventArgs e)
    {
        Add("23matches.bas"); //  1
        Add("3dplot.bas"); //     2
        Add("aceyducey.bas"); //  3
        Add("amazing.bas"); //    4
        Add("animal.bas"); //     5
        Add("awari.bas"); //      6
        Add("bagels.bas"); //     7
    }

    private void Add(string filename)
    {
        var li = listView1.Items.Add(filename);
        li.ImageIndex = 0;
        li.Tag = $@"https://raw.githubusercontent.com/GReaperEx/bcg/master/{filename}";
    }

    private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        var li = listView1.GetItemAt(e.X, e.Y);

        if (li == null)
            return;

        listView1.SelectedItems.Clear();
        li.Selected = true;

        btnOk_Click(sender, EventArgs.Empty);
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
        if (listView1.SelectedItems.Count <= 0)
        {
            MessageBox.Show(@"No game is selected.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        Filename = (string)listView1.SelectedItems[0].Tag;

        DialogResult = DialogResult.OK;
    }
}