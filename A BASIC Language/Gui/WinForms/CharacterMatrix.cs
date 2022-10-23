namespace A_BASIC_Language.Gui.WinForms;

public class CharacterMatrix
{
    public readonly int RowCount = 25;
    public int ColumnCount;
    public readonly char[,] Characters;

    public CharacterMatrix(int columnCount)
    {
        ColumnCount = columnCount;
        Characters = new char[ColumnCount, RowCount];
    }
}