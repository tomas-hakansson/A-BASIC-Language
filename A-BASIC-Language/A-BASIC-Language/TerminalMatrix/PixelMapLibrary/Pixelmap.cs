#nullable enable
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using PixelmapLibraryNetFramework.SpriteManagement;

namespace PixelmapLibraryNetFramework;

public class Pixelmap
{
    private readonly Bitmap _bitmap;
    private BitmapData? _data;
    private byte[]? _pixels;
    internal const PixelFormat Format = PixelFormat.Format24bppRgb;
    private const int BytesPerPixel = 3;
    public int Width { get; }
    public int Height { get; }

    public Pixelmap(Bitmap bitmap)
    {
        if (bitmap == null)
            throw new ArgumentNullException(nameof(bitmap));
        if (bitmap.PixelFormat != Format)
            throw new ArgumentException("Bitmap must use Format24bppRgb. Use CreateCompatibleBitmap to convert images.", nameof(bitmap));

        _bitmap = bitmap;
        _pixels = null;
        _data = null;
        Width = _bitmap.Width;
        Height = _bitmap.Height;
    }

    public static Bitmap CreateCompatibleBitmap(int width, int height) =>
        new(width, height, Format);

    public static Bitmap CreateCompatibleBitmap(string filename)
    {
        using var bitmap = new Bitmap(filename);
        var result = new Bitmap(bitmap.Width, bitmap.Height, Format);
        try
        {
            using var graphics = Graphics.FromImage(result);
            graphics.Clear(Color.Black);
            graphics.DrawImageUnscaled(bitmap, 0, 0);
            return result;
        }
        catch
        {
            result.Dispose();
            throw;
        }
    }

    public void LockBits()
    {
        if (_data != null)
            throw new InvalidOperationException("Bitmap is already locked.");

        var rowBytes = checked(Width * BytesPerPixel);
        var pixels = new byte[checked(rowBytes * Height)];
        var rect = new Rectangle(0, 0, Width, Height);
        var data = _bitmap.LockBits(rect, ImageLockMode.ReadWrite, Format);
        try
        {
            for (var y = 0; y < Height; y++)
                Marshal.Copy(IntPtr.Add(data.Scan0, checked(y * data.Stride)), pixels, y * rowBytes, rowBytes);
            _pixels = pixels;
            _data = data;
        }
        catch
        {
            _bitmap.UnlockBits(data);
            throw;
        }
    }

    public void UnlockBits()
    {
        if (_data == null)
            throw new InvalidOperationException("Bitmap is not locked.");

        var data = _data;
        try
        {
            var rowBytes = checked(Width * BytesPerPixel);
            for (var y = 0; y < Height; y++)
                Marshal.Copy(_pixels!, y * rowBytes, IntPtr.Add(data.Scan0, checked(y * data.Stride)), rowBytes);
        }
        finally
        {
            _bitmap.UnlockBits(data);
            _data = null;
            _pixels = null;
        }
    }

    public Color GetPixel(int x, int y)
    {
        GetRgb(x, y, out var r, out var g, out var b);
        return Color.FromArgb(r, g, b);
    }

    public Color RangeSafeGetPixel(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return Color.Black;

        GetRgb(x, y, out var r, out var g, out var b);
        return Color.FromArgb(r, g, b);
    }

    public void GetRgb(int x, int y, out int r, out int g, out int b)
    {
        var i = (y * Width + x) * BytesPerPixel;

        if (i > _pixels!.Length - BytesPerPixel)
            throw new IndexOutOfRangeException();

        b = _pixels[i];
        g = _pixels[i + 1];
        r = _pixels[i + 2];
    }

    public void RangeSafeGetRgb(int x, int y, out int r, out int g, out int b)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
        {
            r = 0;
            g = 0;
            b = 0;
            return;
        }

        var i = (y * Width + x) * BytesPerPixel;

        if (i > _pixels!.Length - BytesPerPixel)
            throw new IndexOutOfRangeException();

        b = _pixels[i];
        g = _pixels[i + 1];
        r = _pixels[i + 2];
    }

    public void SetPixel(int x, int y, Color color)
    {
        var i = (y * Width + x) * BytesPerPixel;
        _pixels![i] = color.B;
        _pixels[i + 1] = color.G;
        _pixels[i + 2] = color.R;
    }

    public void RangeSafeSetPixel(int x, int y, Color color)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return;

        var i = (y * Width + x) * BytesPerPixel;
        _pixels![i] = color.B;
        _pixels[i + 1] = color.G;
        _pixels[i + 2] = color.R;
    }

    public void SetPixel(int x, int y, int r, int g, int b)
    {
        var i = (y * Width + x) * BytesPerPixel;
        _pixels![i] = (byte)b;
        _pixels[i + 1] = (byte)g;
        _pixels[i + 2] = (byte)r;
    }

    public void Clear(Color color)
    {
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
                SetPixel(x, y, color);
    }

    public void RangeSafeSetPixel(int x, int y, int r, int g, int b)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return;

        var i = (y * Width + x) * BytesPerPixel;
        _pixels![i] = (byte)b;
        _pixels[i + 1] = (byte)g;
        _pixels[i + 2] = (byte)r;
    }

    public void AddColor(int x, int y, int r, int g, int b)
    {
        GetRgb(x, y, out var sr, out var sg, out var sb);
        sr += r;
        sg += g;
        sb += b;

        if (sr < 0)
            sr = 0;
        else if (sr > 255)
            sr = 255;

        if (sg < 0)
            sg = 0;
        else if (sg > 255)
            sg = 255;

        if (sb < 0)
            sb = 0;
        else if (sb > 255)
            sb = 255;

        SetPixel(x, y, sr, sg, sb);
    }

    public void RangeSafeAddColor(int x, int y, int r, int g, int b)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return;

        AddColor(x, y, r, g, b);
    }

    public void DrawSprite(StillImageSprite s, int x, int y)
    {
        for (var yp = 0; yp < s.Height; yp++)
            for (var xp = 0; xp < s.Width; xp++)
                    RangeSafeSetPixel(xp + x, yp + y, s.GetColor(xp, yp));
    }

    public void DrawSprite(MonochromeSprite s, int plane, int x, int y, Color color)
    {
        s.CurrentPlane = plane;

        for (var yp = 0; yp < s.Height; yp++)
        {
            for (var xp = 0; xp < s.Width; xp++)
            {
                if (s.Get(xp, yp))
                    RangeSafeSetPixel(xp + x, yp + y, color);
            }
        }
    }

    public void DrawSpriteOpaque(MonochromeSprite s, int plane, int x, int y, Color foreColor, Color backColor)
    {
        s.CurrentPlane = plane;

        for (var yp = 0; yp < s.Height; yp++)
        {
            for (var xp = 0; xp < s.Width; xp++)
            {
                RangeSafeSetPixel(xp + x, yp + y, s.Get(xp, yp) ? foreColor : backColor);
            }
        }
    }
}
