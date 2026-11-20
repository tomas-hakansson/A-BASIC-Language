using System;
using A_BASIC_Language.TerminalMatrix;

namespace A_BASIC_Language.Gui;

public class TerminalResolution
{
    public Resolution Resolution { get; }
    public string ResolutionDisplay { get; }
    public int BorderWidth { get; }
    public int BorderHeight { get; }

    public TerminalResolution(Resolution resolution)
    {
        Resolution = resolution;
        ResolutionDisplay = ResolutionHelper.ResolutionToString(resolution);

        BorderWidth = resolution switch
        {
            Resolution.Pixels640X200Characters80X25 => 10,
            Resolution.Pixels480X200Characters60X25 => 6,
            Resolution.Pixels320X200Characters40X25 => 4,
            Resolution.Pixels160X200Characters20X25 => 2,
            _ => throw new SystemException("Unsupported resolution.")
        };

        BorderHeight = 4;
    }

    public override string ToString() =>
        ResolutionDisplay;
}
