namespace A_BASIC_Language.Gui.WinForms.PseudoGraphics;

public class SeparatorGraphicalElement : GraphicalElement
{
    public int CharacterY { get; set; }

    public SeparatorGraphicalElement(int rowCount, int characterY) : base(rowCount)
    {
        CharacterY = characterY;
    }

    public override void ScrollUp() =>
        CharacterY--;

    public override bool Visible =>
        CharacterY >= 0 && CharacterY < RowCount;

    public override void Draw(Graphics g, int characterWidth, int characterHeight, int totalPixelWidth, int totalPixelHeight)
    {
        var halfHeight = (int)(characterHeight / 2);
        g.DrawRectangle(TerminalEmulator.VectorGraphicsPen, 0, CharacterY * characterHeight + halfHeight, totalPixelWidth, 1);
    }
}