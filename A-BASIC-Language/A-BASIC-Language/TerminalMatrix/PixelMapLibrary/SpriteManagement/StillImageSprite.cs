using System.Drawing;

namespace PixelmapLibraryNetFramework.SpriteManagement;

public class StillImageSprite : ISprite
{
    private readonly int[,] _pixels;
    public int Width { get; }
    public int Height { get; }
    public int PlaneCount { get; }

    public StillImageSprite(string filename)
    {
        using var bitmap = new Bitmap(filename);
        PlaneCount = 0;
        Width = bitmap.Width;
        Height = bitmap.Height;
        _pixels = new int[Width, Height];

        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
                _pixels[x, y] = bitmap.GetPixel(x, y).ToArgb();
    }

    public int Get(int x, int y) =>
        _pixels[x, y];

    public Color GetColor(int x, int y) =>
        Color.FromArgb(_pixels[x, y]);
}
