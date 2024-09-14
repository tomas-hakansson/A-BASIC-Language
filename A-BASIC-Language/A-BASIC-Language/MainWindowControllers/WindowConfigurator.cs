using A_BASIC_Language.Gui;
using TerminalMatrix;

namespace A_BASIC_Language.MainWindowControllers;

public class WindowConfigurator
{
    private readonly TerminalMatrixControl _terminalMatrixControl;
    private readonly Log _log;

    public WindowConfigurator(TerminalMatrixControl terminalMatrixControl, Log log)
    {
        _terminalMatrixControl = terminalMatrixControl;
        _log = log;
    }

    public void Configure(Form owner, ToolStripMenuItem resolutionToolStripMenuItem)
    {
        _terminalMatrixControl.CurrentCursorColor = 5;

        var resolutions = new TerminalResolutionList();
        var resolution = resolutions.Get(Resolution.Pixels480x200Characters60x25);
        _terminalMatrixControl.BorderWidth = resolution.BorderWidth;
        _terminalMatrixControl.BorderHeight = resolution.BorderHeight;
        _terminalMatrixControl.SetResolution(resolution.Resolution);
        _log.Write($@"Start resolution {resolution.ResolutionDisplay}");
        new StartScreen(resolution.Resolution).Write(_terminalMatrixControl);

        foreach (var r in resolutions)
        {
            var resItem = new ToolStripMenuItem(r.ResolutionDisplay);
            resItem.Tag = r;
            resolutionToolStripMenuItem.DropDownItems.Add(resItem);

            if (r.Resolution == Resolution.Pixels480x200Characters60x25)
                resItem.Checked = true;

            resItem.Click += (s, _) =>
            {
                foreach (ToolStripMenuItem i in resolutionToolStripMenuItem.DropDownItems)
                    i.Checked = false;

                foreach (ToolStripMenuItem i in resolutionToolStripMenuItem.DropDownItems)
                {
                    var currentTag = i.Tag as TerminalResolution;
                    var clickedTag = ((ToolStripMenuItem)s!).Tag as TerminalResolution;

                    if (currentTag!.Resolution != clickedTag!.Resolution)
                        continue;

                    if (_terminalMatrixControl.Resolution == clickedTag.Resolution)
                        return;

                    i.Checked = true;
                    var iTag = (TerminalResolution)i.Tag!;
                    _terminalMatrixControl.SetResolution(iTag.Resolution);
                    _terminalMatrixControl.BorderWidth = iTag.BorderWidth;
                    _terminalMatrixControl.BorderHeight = iTag.BorderHeight;
                    Application.DoEvents();
                    owner.Refresh();
                    Application.DoEvents();
                    new StartScreen(iTag.Resolution).Write(_terminalMatrixControl);
                    owner.Refresh();
                    _log.Write($@"Changed resolution {iTag.ResolutionDisplay}");
                    break;
                }
            };
        }
    }
}