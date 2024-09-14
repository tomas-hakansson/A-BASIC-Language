using TerminalMatrix;

namespace A_BASIC_Language.Gui;

public class TerminalResolutionList : List<TerminalResolution>
{
    public TerminalResolutionList()
    {
        Add(new TerminalResolution(Resolution.Pixels640x200Characters80x25));
        Add(new TerminalResolution(Resolution.Pixels480x200Characters60x25));
        Add(new TerminalResolution(Resolution.Pixels320x200Characters40x25));
        Add(new TerminalResolution(Resolution.Pixels160x200Characters20x25));
    }

    public TerminalResolution Get(Resolution resolution)
    {
        foreach (var r in this.Where(r => r.Resolution == resolution))
            return r;

        throw new SystemException("Not supported.");
    }
}