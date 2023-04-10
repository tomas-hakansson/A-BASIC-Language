using ConsoleControlLibrary;
using ConsoleControlLibrary.Controls;

namespace A_BASIC_Language.Gui.WinForms;

public class ViewSourceForm : ConsoleForm
{
    private readonly SimpleList? _list;

    public ViewSourceForm(nint handle, ConsoleControl parentConsole, string sourceCode, string filename) : base(handle, parentConsole)
    {
        var labelText = string.IsNullOrWhiteSpace(filename) ? "Current source code:" : $"Source: {filename}";

        AddControl(new ConsoleControlLibrary.Controls.Label(this, 0, 0, parentConsole.Width, labelText));

        var rows = sourceCode.Split(
            new [] { Environment.NewLine },
            StringSplitOptions.None
        );

        if (rows.Length <= 0)
            return;

        var listHeight = parentConsole.Height - 1;
        
        if (rows.Length < listHeight)
            listHeight = rows.Length;

        _list = new SimpleList(this, 0, 1, parentConsole.Width, listHeight);

        foreach (var row in rows)
            _list.AddItem(row);

        AddControl(_list);
    }

    public void SetFocusToList() =>
        SetFocus(_list!);
}