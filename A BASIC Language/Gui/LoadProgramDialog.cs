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
        Add("23matches.bas"); //   1
        Add("3dplot.bas"); //      2
        Add("aceyducey.bas"); //   3
        Add("amazing.bas"); //     4
        Add("animal.bas"); //      5
        Add("awari.bas"); //       6
        Add("bagels.bas"); //      7
        Add("banner.bas"); //      8
        Add("basketball.bas"); //  9
        Add("batnum.bas"); //      10
        Add("battle.bas"); //      11
        Add("blackjack.bas"); //   12
        Add("bombardment.bas"); // 13
        Add("bombsaway.bas"); //   14
        Add("bounce.bas"); //      15
        Add("bowling.bas"); //     16
        Add("boxing.bas"); //      17
        Add("bug.bas"); //         18
        Add("bullfight.bas"); //   19
        Add("bullseye.bas"); //    20
        Add("bunny.bas"); //       21
        Add("buzzword.bas"); //    22
        Add("calendar.bas"); //    23
        Add("change.bas"); //      24
        Add("checkers.bas"); //    25
        Add("chemist.bas"); //     26
        Add("chief.bas"); //       27
        Add("chomp.bas"); //       28
        Add("civilwar.bas"); //    29
        Add("combat.bas"); //      30
        Add("craps.bas"); //       31
        Add("cube.bas"); //        32
        Add("depthcharge.bas"); // 33
        Add("diamond.bas"); //     34
        Add("dice.bas"); //        35
        Add("digits.bas"); //      36
        Add("evenwins.bas"); //    37
        Add("flipflop.bas"); //    38
        Add("football.bas"); //    39
        Add("ftball.bas"); //      40
        Add("furtrader.bas"); //   41
        Add("gameofevenwins.bas"); // 42
        Add("golf.bas"); //        43
        Add("gomoko.bas"); //      44
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