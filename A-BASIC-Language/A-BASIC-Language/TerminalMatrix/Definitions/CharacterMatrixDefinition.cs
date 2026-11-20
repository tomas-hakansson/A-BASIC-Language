using System;

namespace A_BASIC_Language.TerminalMatrix.Definitions;

public static class CharacterMatrixDefinition
{
    public static int Width;
    public static int Height;
    public const char CharacterEmpty = ' ';
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
            case Resolution.Pixels640X200Characters80X25:
                Width = 80;
                Height = 25;
                break;
            case Resolution.Pixels480X200Characters60X25:
                Width = 60;
                Height = 25;
                break;
            case Resolution.Pixels320X200Characters40X25:
                Width = 40;
                Height = 25;
                break;
            case Resolution.Pixels160X200Characters20X25:
                Width = 20;
                Height = 25;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null);
        }

        return new byte[Width, Height];
    }

    public static bool[] CreateTerminations(Resolution resolution) =>
        resolution switch
        {
            Resolution.Pixels640X200Characters80X25 or Resolution.Pixels480X200Characters60X25 or Resolution.Pixels320X200Characters40X25 or Resolution.Pixels160X200Characters20X25 => new bool[25],
            _ => throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null)
        };
}
