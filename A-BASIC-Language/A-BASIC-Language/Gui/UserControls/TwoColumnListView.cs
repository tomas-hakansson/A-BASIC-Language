using System.Windows.Forms;

namespace A_BASIC_Language.Gui.UserControls;

public class TwoColumnListView : ListView
{
    public TwoColumnListView()
    {
        View = View.Details;
        FullRowSelect = true;
        Columns.Add("Name", 130);
        Columns.Add("Value", 350);
    }
}
