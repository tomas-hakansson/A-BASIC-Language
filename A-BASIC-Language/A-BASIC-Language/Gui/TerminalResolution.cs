using TerminalMatrix;

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

        switch (resolution)
        {
            case Resolution.Pixels640x200Characters80x25:
                BorderWidth = 10;
                BorderHeight = 4;
                break;
            case Resolution.Pixels480x200Characters60x25:
                BorderWidth = 6;
                BorderHeight = 4;
                break;
            case Resolution.Pixels320x200Characters40x25:
                BorderWidth = 4;
                BorderHeight = 4;
                break;
            case Resolution.Pixels160x200Characters20x25:
                BorderWidth = 2;
                BorderHeight = 4;
                break;
            default:
                throw new SystemException("Unsupported resolution.");
        }
    }

    public override string ToString() =>
        ResolutionDisplay;
}