using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace TerminalMatrixNetFramework.TerminalColor;

public class Palette : IEnumerable<Color>
{
    private Color[] Colors { get; } = new Color[32];

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
        // Extended
        Colors[16] = Color.FromArgb(80, 165, 148);
        Colors[17] = Color.FromArgb(215, 215, 215);
        Colors[18] = Color.FromArgb(96, 0, 0);
        Colors[19] = Color.FromArgb(130, 215, 198);
        Colors[20] = Color.FromArgb(164, 28, 164);
        Colors[21] = Color.FromArgb(0, 144, 0);
        Colors[22] = Color.FromArgb(0, 0, 130);
        Colors[23] = Color.FromArgb(198, 198, 79);
        Colors[24] = Color.FromArgb(181, 96, 45);
        Colors[25] = Color.FromArgb(62, 28, 0);
        Colors[26] = Color.FromArgb(232, 0, 0);
        Colors[27] = Color.FromArgb(21, 21, 21);
        Colors[28] = Color.FromArgb(79, 79, 79);
        Colors[29] = Color.FromArgb(0, 75, 0);
        Colors[30] = Color.FromArgb(0, 96, 215);
        Colors[31] = Color.FromArgb(37, 57, 40);
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
