using System;

namespace TerminalMatrixNetFramework.Definitions;

public static class PixelMatrixDefinition
{
    public static int Width;
    public static int Height;

    static PixelMatrixDefinition()
    {
        Width = 320;
        Height = 200;
    }

    public static byte[,] Create(Resolution resolution)
    {
        switch (resolution)
        {
            case Resolution.Pixels640x200Characters80x25:
                Width = 640;
                Height = 200;
                break;
            case Resolution.Pixels480x200Characters60x25:
                Width = 480;
                Height = 200;
                break;
            case Resolution.Pixels320x200Characters40x25:
                Width = 320;
                Height = 200;
                break;
            case Resolution.Pixels160x200Characters20x25:
                Width = 160;
                Height = 200;
                break;
            case Resolution.LogPixels640x80Characters80x10:
                Width = 640;
                Height = 80;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null);
        }

        return new byte[Width, Height];
    }
    
    public static int[] CreateBitmap() =>
        new int[Width * Height];
}
