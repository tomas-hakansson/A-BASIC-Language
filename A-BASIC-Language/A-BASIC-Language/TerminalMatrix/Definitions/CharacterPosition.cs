namespace TerminalMatrixNetFramework.Definitions;

public class CharacterPosition
{
    public int X { get; }
    public int Y { get; }

    public CharacterPosition(int x , int y)
    {
        X = x;
        Y = y;
    }

    public static bool operator >(CharacterPosition a, CharacterPosition b)
    {
        if (a.Y < b.Y)
            return false;

        if (a.Y > b.Y)
            return true;

        return a.X > b.X;
    }

    public static bool operator <(CharacterPosition a, CharacterPosition b)
    {
        if (a.Y > b.Y)
            return false;

        if (a.Y < b.Y)
            return true;

        return a.X < b.X;
    }
}
