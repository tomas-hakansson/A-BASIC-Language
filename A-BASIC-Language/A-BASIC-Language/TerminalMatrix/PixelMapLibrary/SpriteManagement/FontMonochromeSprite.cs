using System;
using System.Collections.Generic;
using System.Drawing;
using A_BASIC_Language.TerminalMatrix.PixelMapLibrary.FontManagement;
using A_BASIC_Language.TerminalMatrix.PixelMapLibrary.FontManagement.Fonts;
using PixelmapLibraryNetFramework.SpriteManagement;

namespace A_BASIC_Language.TerminalMatrix.PixelMapLibrary.SpriteManagement;

public class FontMonochromeSprite : MonochromeSprite
{
    private const int BlankPlane = 128;
    private readonly TerminalFont _font;
    private readonly Dictionary<char, int> _extraPlanes = new();

    private FontMonochromeSprite(TerminalFont font) : base(8, 8, BlankPlane + 1 + 256)
    {
        _font = font;
    }

    public static FontMonochromeSprite Create(CurrentFont currentFont)
    {
        TerminalFont terminalFont = currentFont switch
        {
            CurrentFont.C64 => new C64TerminalFont(),
            CurrentFont.Pet => new PetTerminalFont(),
            _ => throw new SystemException("Unsupported font.")
        };  

        var result = new FontMonochromeSprite(terminalFont);
        foreach (var glyph in terminalFont)
            result.SetData(glyph.Key, glyph.Value);
        result.CurrentPlane = 0;
        return result;
    }

    internal void EnsureGlyph(char character)
    {
        if (character < BlankPlane || _extraPlanes.ContainsKey(character))
            return;

        _font.EnsureGlyph(character);
        var plane = BlankPlane + 1 + _extraPlanes.Count;
        SetData(plane, _font[character]);
        _extraPlanes.Add(character, plane);
    }

    internal FontMonochromeSprite ChangeFont(CurrentFont currentFont)
    {
        var result = Create(currentFont);
        foreach (var character in _extraPlanes.Keys)
            result.EnsureGlyph(character);

        return result;
    }

    private int GetPlane(char character)
    {
        if (character < BlankPlane)
            return character;

        return _extraPlanes.TryGetValue(character, out var plane) ? plane : BlankPlane;
    }

    private void SetData(int plane, FontManagement.CharacterPixelMatrix character)
    {
        var previousPlane = CurrentPlane;
        CurrentPlane = plane;
        for (var y = 0; y < 8; y++)
        {
            for (var x = 0; x < 8; x++)
            {
                Set(x, y, character.Pixels[x, y]);
            }
        }
        CurrentPlane = previousPlane;
    }

    public void Draw(Pixelmap p, int plane, int x, int y, Color color) =>
        p.DrawSprite(this, plane, x, y, color);

    public void Draw(Pixelmap p, char c, int x, int y, Color color) =>
        p.DrawSprite(this, GetPlane(c), x, y, color);

    public void Draw(Pixelmap p, string text, int x, int y, Color color)
    {
        foreach (var c in text)
        {
            Draw(p, c, x, y, color);
            x += 8;
        }
    }

    public void DrawOpaque(Pixelmap p, int plane, int x, int y, Color foreColor, Color backColor) =>
        p.DrawSpriteOpaque(this, plane, x, y, foreColor, backColor);

    public void DrawOpaque(Pixelmap p, char c, int x, int y, Color foreColor, Color backColor) =>
        p.DrawSpriteOpaque(this, GetPlane(c), x, y, foreColor, backColor);

    public void DrawOpaque(Pixelmap p, string text, int x, int y, Color foreColor, Color backColor)
    {
        foreach (var c in text)
        {
            DrawOpaque(p, c, x, y, foreColor, backColor);
            x += 8;
        }
    }
}
