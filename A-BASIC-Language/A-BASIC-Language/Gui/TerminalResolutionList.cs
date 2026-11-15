#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using A_BASIC_Language.TerminalMatrix;

namespace A_BASIC_Language.Gui;

public class TerminalResolutionList : List<TerminalResolution>
{
    public TerminalResolutionList()
    {
        Add(new TerminalResolution(Resolution.Pixels640X200Characters80X25));
        Add(new TerminalResolution(Resolution.Pixels480X200Characters60X25));
        Add(new TerminalResolution(Resolution.Pixels320X200Characters40X25));
        Add(new TerminalResolution(Resolution.Pixels160X200Characters20X25));
    }

    public TerminalResolution Get(Resolution resolution)
    {
        foreach (var r in this.Where(r => r.Resolution == resolution))
            return r;

        throw new SystemException("Not supported.");
    }
}
