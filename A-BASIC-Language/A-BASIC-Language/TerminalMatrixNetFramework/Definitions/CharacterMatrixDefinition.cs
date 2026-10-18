using System;

namespace TerminalMatrixNetFramework.Definitions;

public static class CharacterMatrixDefinition
{
    public static int Width;
    public static int Height;
    public const int CharacterEmpty = 32;
    public static int TextRenderLimit { get; set; }

    static CharacterMatrixDefinition()
    {
        Width = 40;
        Height = 25;
        TextRenderLimit = 0;
    }

    public static byte[,] Create(Resolution resolution)
    {
        switch (resolution)
        {
            case Resolution.Pixels640x200Characters80x25:
                Width = 80;
                Height = 25;
                break;
            case Resolution.Pixels480x200Characters60x25:
                Width = 60;
                Height = 25;
                break;
            case Resolution.Pixels320x200Characters40x25:
                Width = 40;
                Height = 25;
                break;
            case Resolution.Pixels160x200Characters20x25:
                Width = 20;
                Height = 25;
                break;
            case Resolution.LogPixels640x80Characters80x10:
                Width = 80;
                Height = 10;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null);
        }

        return new byte[Width, Height];
    }

    public static bool[] CreateTerminations(Resolution resolution) =>
        resolution switch
        {
            Resolution.Pixels640x200Characters80x25 or Resolution.Pixels480x200Characters60x25 or Resolution.Pixels320x200Characters40x25 or Resolution.Pixels160x200Characters20x25 => new bool[25],
            Resolution.LogPixels640x80Characters80x10 => new bool[10],
            _ => throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null)
        };
}
