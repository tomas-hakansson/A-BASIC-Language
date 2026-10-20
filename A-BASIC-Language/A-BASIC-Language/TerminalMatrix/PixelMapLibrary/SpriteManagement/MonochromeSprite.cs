namespace PixelmapLibraryNetFramework.SpriteManagement;

public class MonochromeSprite : ISprite
{
    private readonly bool[,,] _pixels;
    public int Width { get; }
    public int Height { get; }
    public int PlaneCount { get; }
    public int CurrentPlane { get; set; }

    public MonochromeSprite(int width, int height, int planeCount)
    {
        CurrentPlane = 0;
        Width = width;
        Height = height;
        PlaneCount = planeCount;
        _pixels = new bool[width, height, planeCount];
    }

    public void Set(int x, int y) =>
        _pixels[x, y, CurrentPlane] = true;

    public void Unset(int x, int y) =>
        _pixels[x, y, CurrentPlane] = false;

    public void Set(int x, int y, bool set) =>
        _pixels[x, y, CurrentPlane] = set;

    public bool Get(int x, int y) =>
        _pixels[x, y, CurrentPlane];
}