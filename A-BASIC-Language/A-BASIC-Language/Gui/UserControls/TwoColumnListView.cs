#nullable enable
using System.Collections;
using System.Windows.Forms;

namespace A_BASIC_Language.Gui.UserControls;

public class TwoColumnListView : ListView
{
    private readonly ListViewColumnSorter _lvwColumnSorter;

    public TwoColumnListView()
    {
        _lvwColumnSorter = new ListViewColumnSorter();
        View = View.Details;
        FullRowSelect = true;
        Columns.Add("Name", 130);
        Columns.Add("Value", 350);
    }

    public void Add(object o, string name, string value)
    {
        var item = new ListViewItem(name) { Tag = o };
        item.SubItems.Add(value);
        Items.Add(item);
    }

    protected override void OnColumnClick(ColumnClickEventArgs e)
    {
        if (e.Column == _lvwColumnSorter.SortColumn)
        {
            _lvwColumnSorter.Order = _lvwColumnSorter.Order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
        }
        else
        {
            _lvwColumnSorter.SortColumn = e.Column;
            _lvwColumnSorter.Order = SortOrder.Ascending;
        }

        Sort();
        base.OnColumnClick(e);
    }
}

internal class ListViewColumnSorter : IComparer
{
    private readonly CaseInsensitiveComparer _objectCompare;
    public SortOrder Order { set; get; }
    public int SortColumn { set; get; }

    public ListViewColumnSorter()
    {
        SortColumn = 0;
        Order = SortOrder.None;
        _objectCompare = new CaseInsensitiveComparer();
    }

    public int Compare(object? x, object? y)
    {
        if (x == null || y == null)
            return 0;

        var listviewX = (ListViewItem)x;
        var listviewY = (ListViewItem)y;
        var compareResult = _objectCompare.Compare(listviewX.SubItems[SortColumn].Text, listviewY.SubItems[SortColumn].Text);

        return Order switch
        {
            SortOrder.Ascending => compareResult,
            SortOrder.Descending => (-compareResult),
            _ => 0
        };
    }
}
