using ConsoleControlLibrary;
using ConsoleControlLibrary.Controls;

namespace A_BASIC_Language.Gui.WinForms;

public class ViewSourceForm : ConsoleForm
{
    private readonly string _sourceCode;
    private readonly string _filename;
    private readonly int _width;
    private readonly int _height;

    public ViewSourceForm(nint handle, ConsoleControl parentConsole, string sourceCode, string filename) : base(handle, parentConsole)
    {
        _sourceCode = sourceCode;
        _filename = filename;
        _width = parentConsole.Width;
        _height = parentConsole.Height;

        var labelText = string.IsNullOrWhiteSpace(_filename) ? "Current source code:" : $"Source: {_filename}";

        AddControl(new ConsoleControlLibrary.Controls.Label(this, 0, 0, _width, labelText));

        var rows = _sourceCode.Split(
            new [] { Environment.NewLine },
            StringSplitOptions.None
        );

        if (rows.Length <= 0)
            return;

        var listHeight = _height - 1;
        
        if (rows.Length < listHeight)
            listHeight = rows.Length;

        var list = new SimpleList(this, 0, 1, _width, listHeight);

        foreach (var row in rows)
            list.AddItem(row);

        AddControl(list);
    }
}