using System.Linq;
using A_BASIC_Language.Gui.Multimedia;
using System.Windows.Forms;
using A_BASIC_Language.TerminalMatrix.PixelMapLibrary.FontManagement;

namespace A_BASIC_Language.Gui.UserControls;

public class FontComboBox : ComboBox
{
    public FontComboBox()
    {
        DropDownStyle = ComboBoxStyle.DropDownList;
        // ReSharper disable once CollectionNeverUpdated.Local
        var items = new FontConfigurationList();

        foreach (var item in items)
        {
            Items.Add(item);
        }
    }

    public CurrentFont SelectedFont
    {
        get => SelectedItem == null ? new FontConfigurationList().First().Id : (SelectedItem is FontConfiguration configuration ? (FontConfiguration)SelectedItem : default)!.Id;
        set
        {
            for (var i = 0; i < Items.Count; i++)
            {
                if (((FontConfiguration)Items[i]).Id != value)
                    continue;

                SelectedIndex = i;
                return;
            }
        }
    }
}
