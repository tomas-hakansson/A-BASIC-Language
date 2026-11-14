#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace A_BASIC_Language.TerminalMatrix.TerminalColor;

public class Palette : IEnumerable<Color>
{
    private Color[] Colors { get; } = new Color[16];

    public Palette()
    {
        Colors[0] = Color.FromArgb(0, 0, 0);
        Colors[1] = Color.FromArgb(255, 255, 255);
        Colors[2] = Color.FromArgb(136, 0, 0);
        Colors[3] = Color.FromArgb(170, 255, 238);
        Colors[4] = Color.FromArgb(204, 68, 204);
        Colors[5] = Color.FromArgb(0, 184, 5);
        Colors[6] = Color.FromArgb(0, 0, 170);
        Colors[7] = Color.FromArgb(238, 238, 119);
        Colors[8] = Color.FromArgb(221, 136, 85);
        Colors[9] = Color.FromArgb(102, 68, 0);
        Colors[10] = Color.FromArgb(255, 119, 119);
        Colors[11] = Color.FromArgb(51, 51, 51);
        Colors[12] = Color.FromArgb(119, 119, 119);
        Colors[13] = Color.FromArgb(50, 215, 42);
        Colors[14] = Color.FromArgb(0, 136, 255);
        Colors[15] = Color.FromArgb(187, 187, 187);
    }

    public Color GetColor(int index) =>
        Colors[index];

    public Color GetColor(ColorName color) =>
        Colors[(int)color];

    public IEnumerator<Color> GetEnumerator() =>
        ((IEnumerable<Color>)Colors).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    public byte SearchColor(Color c)
    {
        for (var i = 0; i < Colors.Length; i++)
        {
            if (ColorsAreClose(i, c))
                return (byte)i;
        }

        throw new InvalidOperationException("GIF contains colors outside the allowed palette.");
    }

    private bool ColorsAreClose(int targetColorIndex, Color sampleColor)
    {
        var targetColor = Colors[targetColorIndex];
        const int threshold = 3;

        return Math.Max(targetColor.R, sampleColor.R) - Math.Min(targetColor.R, sampleColor.R) < threshold
               && Math.Max(targetColor.G, sampleColor.G) - Math.Min(targetColor.G, sampleColor.G) < threshold
               && Math.Max(targetColor.B, sampleColor.B) - Math.Min(targetColor.B, sampleColor.B) < threshold;
    }
}
