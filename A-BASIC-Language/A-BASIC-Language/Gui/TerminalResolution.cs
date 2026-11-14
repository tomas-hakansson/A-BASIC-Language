#nullable enable
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
            Resolution.Pixels640x200Characters80x25 => 10,
            Resolution.Pixels480x200Characters60x25 => 6,
            Resolution.Pixels320x200Characters40x25 => 4,
            Resolution.Pixels160x200Characters20x25 => 2,
            _ => throw new SystemException("Unsupported resolution.")
        };

        BorderHeight = 4;
    }

    public override string ToString() =>
        ResolutionDisplay;
}
