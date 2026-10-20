namespace TerminalMatrixNetFramework;

public class CharacterPixelMatrix
{
    public readonly bool[,] Pixels;

    public CharacterPixelMatrix(string data)
    {
        Pixels = new bool[8, 8];

        var index = 0;

        for (var y = 0; y < 8; y++)
        {
            for (var x = 0; x < 8; x++)
            {
                Pixels[x, y] = data.Substring(index, 1) == "*";
                index++;
            }
        }
    }
}
