using A_BASIC_Language.Gui;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace A_BASIC_Language;

public class VersionService
{
    private readonly MsgBox _msgBox;

    public VersionService(Form owner)
    {
        _msgBox = new MsgBox(owner);
    }

    public string GetCurrentVersion()
    {
        var asm = Assembly.GetExecutingAssembly();
        var vInfo = FileVersionInfo.GetVersionInfo(asm.Location);
        var v = vInfo.ProductVersion!.Split(['.', '+']);
        return $"{v[0]}.{v[1]}";
    }

    public string GetAboutBoxText()
    {
        var v = GetCurrentVersion();
        return $@"ABL - A BASIC Language v{v}

An Altair BASIC player, written by Tomas Håkansson and Anders Hesselbom";
    }

    public void GetOnlineHelp()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "https://abl.winsoft.se/",
                UseShellExecute = true
            };
            Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            _msgBox.Fail(ex.Message, @"Failed to open online help");
        }
    }

    public void GetVersionHistory()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "https://github.com/tomas-hakansson/A-BASIC-Language/blob/master/README.md",
                UseShellExecute = true
            };
            Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            _msgBox.Fail(ex.Message, @"Failed to open version history");
        }
    }
}
