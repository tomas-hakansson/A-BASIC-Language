using System.Linq;
using System.Windows.Forms;
using A_BASIC_Language.Gui.Multimedia;

namespace A_BASIC_Language.Gui.UserControls;

public class ColorSchemeComboBox : ComboBox
{
    public ColorSchemeComboBox()
    {
        DropDownStyle = ComboBoxStyle.DropDownList;
        // ReSharper disable once CollectionNeverUpdated.Local
        var items = new ColorSchemeConfigurationList();

        foreach (var item in items)
        {
            Items.Add(item);
        }
    }

    public ColorSchemeConfiguration SelectedColorScheme
    {
        get => SelectedItem == null ? new ColorSchemeConfigurationList().First() : (ColorSchemeConfiguration)SelectedItem;
        set
        {
            for (var i = 0; i < Items.Count; i++)
            {
                if (((ColorSchemeConfiguration)Items[i]!).Id != value.Id)
                    continue;

                SelectedIndex = i;
                return;
            }
        }
    }

    public void SetSelectedColorScheme(ColorScheme scheme)
    {
        for (var i = 0; i < Items.Count; i++)
        {
            if (((ColorSchemeConfiguration)Items[i]!).Id != scheme)
                continue;

            SelectedIndex = i;
            return;
        }
    }
}
