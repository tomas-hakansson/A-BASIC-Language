using A_BASIC_Language.Gui.WinForms;

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
#if DEBUG
        var d = new DirectoryInfo(@"..\..\..\testPrograms\");
        foreach (var f in d.GetFiles())
            if (f.Name.ToLower().EndsWith(".abl"))
                Add(f.Name, 1);
#endif

        foreach (var g in new GamesList())
            Add(g);
    }

    private void Add(string filename, int iconIndex = 0)
    {
        var li = listView1.AddItem(iconIndex, filename);

        li.Tag = iconIndex == 1
            ? $@"..\..\..\testPrograms\{filename}"
            : $@"https://raw.githubusercontent.com/GReaperEx/bcg/master/{filename}";
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

    private void listView1_MouseClick(object sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right)
            return;

        var item = listView1.GetItemAt(e.X, e.Y);

        if (item == null)
            return;

        contextMenuStrip1.Show(listView1, e.X, e.Y);
    }

    private void viewSourceToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (listView1.SelectedItem == null)
            return;

        var n = (string)listView1.SelectedItem.Tag;

        var source = TerminalEmulator.ProgramRepository.GetProgram(this, n, out _);

        using var x = new SourceDialog();
        x.Filename = listView1.SelectedItems[0].Text;
        x.SourceCode = source;
        x.ShowDialog(this);
    }

    private void runToolStripMenuItem_Click(object sender, EventArgs e) =>
        btnOk_Click(sender, e);

    private void listView1_ItemSelected(object sender, SelectListLibrary.ItemSelectedEventArgs eventArgs) =>
        btnOk_Click(sender, EventArgs.Empty);
}