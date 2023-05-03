using CharacterMatrix;

namespace A_BASIC_Language.Gui.WinForms;

public class CharacterRenderer : IDisposable
{
    private readonly Matrix _characters;
    private Brush OutputBrush { get; }
    private Brush InputBrush { get; }
    private Brush InputBackgroundBrush { get; }
    private const int CharacterWidth = 8;
    private const int CharacterHeight = 8;
    private Font Font { get; }

    public CharacterRenderer(Matrix characters)
    {
        _characters = characters;
        OutputBrush = new SolidBrush(Color.FromArgb(0, 255, 0));
        InputBrush = new SolidBrush(Color.FromArgb(0, 255, 255));
        InputBackgroundBrush = new SolidBrush(Color.FromArgb(0, 42, 0));
        Font = new Font("Courier New", 6.0f);
    }

    public void Render(Graphics g, SolidBrush backgroundBrush, bool lineInputMode, bool showCursor, bool cursorBlink, Point cursorPosition, int lineInputX, int lineInputY)
    {
        var pixelX = 0;
        var pixelY = 0;

        if (lineInputMode)
        {
            for (var y = 0; y < _characters.RowCount; y++)
            {
                for (var x = 0; x < _characters.ColumnCount; x++)
                {
                    var isInsideInputZone = IsInsideInputZone(x, y, lineInputX, lineInputY, cursorPosition);
                    var b = isInsideInputZone ? InputBrush : OutputBrush;

                    if (showCursor && cursorBlink && cursorPosition.Is(x, y))
                    {
                        g.FillRectangle(b, pixelX, pixelY, CharacterWidth, CharacterHeight);

                        if (_characters.GetAt(x, y) > 0)
                            g.DrawString(_characters.GetAt(x, y).ToString(), Font, backgroundBrush, pixelX, pixelY);
                    }
                    else
                    {
                        if (isInsideInputZone)
                            g.FillRectangle(InputBackgroundBrush, pixelX, pixelY, CharacterWidth, CharacterHeight);

                        if (_characters.GetAt(x, y) > 0)
                            g.DrawString(_characters.GetAt(x, y).ToString(), Font, b, pixelX, pixelY);
                    }
                    pixelX += CharacterWidth;
                }
                pixelX = 0;
                pixelY += CharacterHeight;
            }
        }
        else
        {
            for (var y = 0; y < _characters.RowCount; y++)
            {
                for (var x = 0; x < _characters.ColumnCount; x++)
                {
                    if (showCursor && cursorBlink && cursorPosition.Is(x, y))
                    {
                        g.FillRectangle(OutputBrush, pixelX, pixelY, CharacterWidth, CharacterHeight);

                        if (_characters.GetAt(x, y) > 0)
                            g.DrawString(_characters.GetAt(x, y).ToString(), Font, backgroundBrush, pixelX, pixelY);
                    }
                    else
                    {
                        if (_characters.GetAt(x, y) > 0)
                            g.DrawString(_characters.GetAt(x, y).ToString(), Font, OutputBrush, pixelX, pixelY);
                    }
                    pixelX += CharacterWidth;
                }
                pixelX = 0;
                pixelY += CharacterHeight;
            }
        }
    }

    private static bool IsInsideInputZone(int x, int y, int lineInputX, int lineInputY, Point cursorPosition)
    {
        if (y < lineInputY)
            return false;

        if (y == lineInputY && x < lineInputX)
            return false;

        if (y > cursorPosition.Y)
            return false;

        if (y == cursorPosition.Y && x >= cursorPosition.X)
            return false;

        return true;
    }

    public void Dispose()
    {
        OutputBrush.Dispose();
        InputBrush.Dispose();
        InputBackgroundBrush.Dispose();
        Font.Dispose();
    }
}