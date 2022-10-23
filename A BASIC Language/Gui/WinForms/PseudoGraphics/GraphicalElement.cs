namespace A_BASIC_Language.Gui.WinForms.PseudoGraphics;

public abstract class GraphicalElement
{
    protected readonly int RowCount;

    protected GraphicalElement(int rowCount)
    {
        RowCount = rowCount;
    }

    public abstract void ScrollUp();
    public abstract bool Visible { get; }
    public abstract void Draw(Graphics g, int characterWidth, int characterHeight, int totalPixelWidth, int totalPixelHeight);
}