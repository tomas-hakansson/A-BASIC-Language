#nullable enable
using System;
using System.Drawing;

namespace A_BASIC_Language.TerminalMatrix;

public static class ResolutionHelper
{
    public static Size GetPixelSize(Resolution resolution, int borderWidth, int borderHeight) =>
        resolution switch
        {
            Resolution.Pixels640x200Characters80x25 => new Size(640 + borderWidth * 2, 200 + borderHeight * 2),
            Resolution.Pixels480x200Characters60x25 => new Size(480 + borderWidth * 2, 200 + borderHeight * 2),
            Resolution.Pixels320x200Characters40x25 => new Size(320 + borderWidth * 2, 200 + borderHeight * 2),
            Resolution.Pixels160x200Characters20x25 => new Size(160 + borderWidth * 2, 200 + borderHeight * 2),
            _ => throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null)
        };

    public static Size GetCharacterMapSize(Resolution resolution) =>
        resolution switch
        {
            Resolution.Pixels640x200Characters80x25 => new Size(80, 25),
            Resolution.Pixels480x200Characters60x25 => new Size(60, 25),
            Resolution.Pixels320x200Characters40x25 => new Size(40, 25),
            Resolution.Pixels160x200Characters20x25 => new Size(20, 25),
            _ => throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null)
        };

    public static string ResolutionToString(Resolution resolution) =>
        resolution switch
        {
            Resolution.Pixels640x200Characters80x25 => "640*200 pixels, 80*25 characters",
            Resolution.Pixels480x200Characters60x25 => "480*200 pixels, 60*25 characters",
            Resolution.Pixels320x200Characters40x25 => "320*200 pixels, 40*25 characters",
            Resolution.Pixels160x200Characters20x25 => "160*200 pixels, 20*25 characters",
            _ => throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null)
        };
}
