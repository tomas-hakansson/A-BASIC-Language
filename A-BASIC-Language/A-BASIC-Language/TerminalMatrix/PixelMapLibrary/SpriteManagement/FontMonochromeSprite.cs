using System;
using System.Drawing;
using A_BASIC_Language.Language;
using A_BASIC_Language.TerminalMatrix.PixelMapLibrary.FontManagement;
using A_BASIC_Language.TerminalMatrix.PixelMapLibrary.FontManagement.Fonts;

namespace A_BASIC_Language.TerminalMatrix.PixelMapLibrary.SpriteManagement;

public class FontMonochromeSprite : MonochromeSprite
{
    internal const int GlyphSize = 8;
    internal const int GlyphCount = 256;
    private const int BlankPlane = GlyphCount;
    private readonly TerminalFont _font;

    private FontMonochromeSprite(TerminalFont font) : base(GlyphSize, GlyphSize, GlyphCount + 1)
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
            result.SetData(result.GetPlane(glyph.Key), glyph.Value);
        
        result.CurrentPlane = 0;
        return result;
    }

    internal void EnsureGlyph(char character)
    {
        if (Petscii.TryGetPlaceholderCode(character, out var code) && !_font.ContainsKey((char)code))
            _font.EnsureGlyph(character);
    }

    internal FontMonochromeSprite ChangeFont(CurrentFont currentFont)
    {
        var result = Create(currentFont);
        foreach (var character in _font.Keys)
            result.EnsureGlyph(character);

        return result;
    }

    private int GetPlane(char character)
    {
        if (Petscii.TryGetSwedishCode(character, out var swedishCode))
            return swedishCode;

        if (character < 128)
            return character;

        return Petscii.TryGetPlaceholderCode(character, out var code) ? code : BlankPlane;
    }

    internal void CopyToMemory(byte[] memory, int start)
    {
        Array.Clear(memory, start, GlyphCount * GlyphSize);
        foreach (var glyph in _font)
        {
            var code = GetPlane(glyph.Key);
            if (code == BlankPlane)
                continue;
            for (var y = 0; y < GlyphSize; y++)
            {
                byte row = 0;
                for (var x = 0; x < GlyphSize; x++)
                    if (glyph.Value.Pixels[x, y])
                        row |= (byte)(0x80 >> x);

                memory[start + code * GlyphSize + y] = row;
            }
        }
    }

    internal void SetGlyphRow(byte code, int y, byte row)
    {
        var character = code >= Petscii.SwedishFirstCode &&
            code < Petscii.SwedishFirstCode + Petscii.SwedishCharacters.Length
            ? Petscii.Decode(code) : (char)code;
        if (!_font.TryGetValue(character, out var glyph))
        {
            character = Petscii.Decode(code);
            _font.EnsureGlyph(character);
            glyph = _font[character];
        }

        var previousPlane = CurrentPlane;
        CurrentPlane = code;
        for (var x = 0; x < GlyphSize; x++)
        {
            var pixel = (row & (0x80 >> x)) != 0;
            glyph.Pixels[x, y] = pixel;
            Set(x, y, pixel);
        }
        CurrentPlane = previousPlane;
    }

    private void SetData(int plane, FontManagement.CharacterPixelMatrix character)
    {
        var previousPlane = CurrentPlane;
        CurrentPlane = plane;
        for (var y = 0; y < GlyphSize; y++)
        {
            for (var x = 0; x < GlyphSize; x++)
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

#if DEBUG
[Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
public class SwedishFontTests
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
    public void SwedishGlyphsRenderAndOccupyTheirOwnMemorySlotsInBothFonts()
    {
        foreach (var selectedFont in new[] { CurrentFont.C64, CurrentFont.Pet })
        {
            var sprite = FontMonochromeSprite.Create(selectedFont);
            TerminalFont font = selectedFont == CurrentFont.C64 ? new C64TerminalFont() : new PetTerminalFont();
            var memory = new byte[65536];
            sprite.CopyToMemory(memory, 53248);

            foreach (var character in "ÅÄÖåäöAa\\")
            {
                var code = Petscii.TryGetSwedishCode(character, out var mapped) ? mapped : (byte)character;
                using var bitmap = Pixelmap.CreateCompatibleBitmap(8, 8);
                var pixels = new Pixelmap(bitmap);
                pixels.LockBits();
                sprite.DrawOpaque(pixels, character, 0, 0, Color.White, Color.Black);
                pixels.UnlockBits();
                var litPixels = 0;
                
                for (var y = 0; y < 8; y++)
                {
                    byte expectedRow = 0;

                    for (var x = 0; x < 8; x++)
                    {
                        var expected = font[character].Pixels[x, y];

                        if (expected)
                        {
                            expectedRow |= (byte)(128 >> x); litPixels++;
                        }
                        
                        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected,
                            bitmap.GetPixel(x, y).R == 255, $"{selectedFont}: {character} ({x},{y})");
                    }

                    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expectedRow,
                        memory[53248 + code * 8 + y], $"{selectedFont}: {character} row {y}");
                }
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(litPixels > 0);
            }
        }
    }

    [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
    public void EditingSwedishGlyphRowsSurvivesMemoryCopyAndFontSwitchRestoresDefaults()
    {
        foreach (var selectedFont in new[] { CurrentFont.C64, CurrentFont.Pet })
        {
            var sprite = FontMonochromeSprite.Create(selectedFont);
            var memory = new byte[65536];
            sprite.CopyToMemory(memory, 53248);
            var original = (byte[])memory.Clone();

            for (byte code = 128; code <= 133; code++)
                sprite.SetGlyphRow(code, 0, 255);
            
            sprite.CopyToMemory(memory, 53248);
            
            for (var address = 53248; address <= 55295; address++)
            {
                var offset = address - 53248;
                var edited = offset / 8 >= 128 && offset / 8 <= 133 && offset % 8 == 0;
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(edited ? (byte)255 : original[address], memory[address]);
            }

            sprite = sprite.ChangeFont(selectedFont);
            sprite.CopyToMemory(memory, 53248);
            Microsoft.VisualStudio.TestTools.UnitTesting.CollectionAssert.AreEqual(original, memory);
        }
    }
}
#endif
