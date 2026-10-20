using System;
using System.Drawing;
using PixelmapLibraryNetFramework.FontManagement;

namespace PixelmapLibraryNetFramework.SpriteManagement;

public class FontMonochromeSprite : MonochromeSprite
{
    private readonly TerminalCodePage _codePage;

    private FontMonochromeSprite() : base(8, 8, 128)
    {
        _codePage = new TerminalCodePage();
    }

    public static FontMonochromeSprite Create()
    {
        var terminalFont = new TerminalFont();
        var result = new FontMonochromeSprite();
        result.SetData(terminalFont);
        result.CurrentPlane = 0;
        return result;
    }

    private void SetData(TerminalFont terminalFont)
    {
        foreach (var terminalFontKey in terminalFont.Keys)
        {
            SetData(terminalFontKey, terminalFont);
        }
    }

    private void SetData(char c, TerminalFont font)
    {
        CurrentPlane = c;

        if (!font.TryGetValue(c, out var character))
            throw new SystemException();

        for (var y = 0; y < 8; y++)
        {
            for (var x = 0; x < 8; x++)
            {
                Set(x, y, character.Pixels[x, y]);
            }
        }
    }

    public void Draw(Pixelmap p, int plane, int x, int y, Color color) =>
        p.DrawSprite(this, plane, x, y, color);

    public void Draw(Pixelmap p, char c, int x, int y, Color color) =>
        p.DrawSprite(this, _codePage.Asc[c], x, y, color);

    public void Draw(Pixelmap p, string text, int x, int y, Color color)
    {
        foreach (var c in text)
        {
            p.DrawSprite(this, _codePage.Asc[c], x, y, color);
            x += 8;
        }
    }

    public void DrawOpaque(Pixelmap p, int plane, int x, int y, Color foreColor, Color backColor) =>
        p.DrawSpriteOpaque(this, plane, x, y, foreColor, backColor);

    public void DrawOpaque(Pixelmap p, char c, int x, int y, Color foreColor, Color backColor) =>
        p.DrawSpriteOpaque(this, _codePage.Asc[c], x, y, foreColor, backColor);

    public void DrawOpaque(Pixelmap p, string text, int x, int y, Color foreColor, Color backColor)
    {
        foreach (var c in text)
        {
            p.DrawSpriteOpaque(this, _codePage.Asc[c], x, y, foreColor, backColor);
            x += 8;
        }
    }
}
