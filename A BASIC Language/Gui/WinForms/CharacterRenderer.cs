using System;

namespace A_BASIC_Language.Gui.WinForms;

public class CharacterRenderer : IDisposable
{
    private readonly char[,] _characters;
    private Brush OutputBrush { get; }
    private Brush InputBrush { get; }
    private Brush InputBackgroundBrush { get; }
    private int RowCount { get; }
    private int ColumnCount { get; }
    private const int CharacterWidth = 8;
    private const int CharacterHeight = 8;
    private Font Font { get; }


    public CharacterRenderer(char[,] characters, int rowCount, int columnCount)
    {
        _characters = characters;
        OutputBrush = new SolidBrush(Color.FromArgb(0, 255, 0));
        InputBrush = new SolidBrush(Color.FromArgb(0, 255, 255));
        InputBackgroundBrush = new SolidBrush(Color.FromArgb(0, 42, 0));
        RowCount = rowCount;
        ColumnCount = columnCount;
        Font = new Font(FontFamily.GenericSerif, 20);
    }

    public void Render(Graphics g, bool lineInputMode, bool showCursor, bool cursorBlink, int cursorX, int cursorY, int lineInputX, int lintInputY)
    {
        var pixelX = 0;
        var pixelY = 0;

        if (lineInputMode)
        {
            for (var y = 0; y < RowCount; y++)
            {
                for (var x = 0; x < ColumnCount; x++)
                {
                    var isInsideInputZone = IsInsideInputZone(x, y);
                    var b = isInsideInputZone ? InputBrush : OutputBrush;

                    if (showCursor && cursorBlink && cursorX == x && cursorY == y)
                    {
                        g.FillRectangle(b, pixelX, pixelY, CharacterWidth, CharacterHeight);

                        if (_characters[x, y] > 0)
                            g.DrawString(_characters[x, y].ToString(), Font, Brushes.Black, pixelX, pixelY);
                    }
                    else
                    {
                        if (isInsideInputZone)
                            g.FillRectangle(InputBackgroundBrush, pixelX, pixelY, CharacterWidth, CharacterHeight);

                        if (_characters[x, y] > 0)
                            g.DrawString(_characters[x, y].ToString(), Font, b, pixelX, pixelY);
                    }
                    pixelX += CharacterWidth;
                }
                pixelX = 0;
                pixelY += CharacterHeight;
            }
        }
        else
        {
            for (var y = 0; y < RowCount; y++)
            {
                for (var x = 0; x < ColumnCount; x++)
                {
                    if (showCursor && cursorBlink && cursorX == x && cursorY == y)
                    {
                        g.FillRectangle(OutputBrush, pixelX, pixelY, CharacterWidth, CharacterHeight);

                        if (_characters[x, y] > 0)
                            g.DrawString(_characters[x, y].ToString(), Font, Brushes.Black, pixelX, pixelY);
                    }
                    else
                    {
                        if (_characters[x, y] > 0)
                            g.DrawString(_characters[x, y].ToString(), Font, OutputBrush, pixelX, pixelY);
                    }
                    pixelX += CharacterWidth;
                }
                pixelX = 0;
                pixelY += CharacterHeight;
            }
        }
    }

    private bool IsInsideInputZone(int x, int y, int lineInputX, int lineInputY)
    {
        if (y < lineInputY)
            return false;

        if (y == lineInputY && x < lineInputX)
            return false;

        if (y > cursorY)
            return false;

        if (y == cursorY && x >= cursorX)
            return false;

        return true;
    }

    public void Dispose()
    {
        OutputBrush.Dispose();
        InputBrush.Dispose();
        InputBackgroundBrush.Dispose();
    }
}