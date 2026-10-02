using System.Windows.Forms;
using TerminalMatrixNetFramework;

namespace A_BASIC_Language.Gui.UserControls;

public class TerminalResolutionComboBox : ComboBox
{
    public TerminalResolutionComboBox()
    {
        DropDownStyle = ComboBoxStyle.DropDownList;
        // ReSharper disable once CollectionNeverUpdated.Local
        var items = new TerminalResolutionList();

        foreach (var item in items)
        {
            Items.Add(item);
        }
    }

    public Resolution Resolution
    {
        get => SelectedItem == null ? Resolution.Pixels480x200Characters60x25 : (SelectedItem as TerminalResolution)!.Resolution;
        set
        {
            for (var i = 0; i < Items.Count; i++)
            {
                if (((TerminalResolution)Items[i]!).Resolution != value)
                    continue;

                SelectedIndex = i;
                return;
            }
        }
    }
}