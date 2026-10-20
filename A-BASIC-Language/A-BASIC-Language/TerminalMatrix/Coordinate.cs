#nullable enable
using TerminalMatrixNetFramework.Definitions;

namespace TerminalMatrixNetFramework;

public class Coordinate
{
    private readonly int? _width;
    private readonly int? _height;
    private int Width => _width ?? CharacterMatrixDefinition.Width;
    private int Height => _height ?? CharacterMatrixDefinition.Height;

    public int X { get; set; }
    public int Y { get; set; }

    public static Coordinate Home() =>
        new(0, 0);

    public Coordinate(int x, int y)
    {
        X = x;
        Y = y;
    }

    internal Coordinate(int x, int y, int width, int height) : this(x, y)
    {
        _width = width;
        _height = height;
    }

    public Coordinate Copy() =>
        _width.HasValue ? new Coordinate(X, Y, Width, Height) : new Coordinate(X, Y);

    public bool IsSame(int x, int y) =>
        X == x && Y == y;

    public bool IsSame(Coordinate c) =>
        X == c.X && Y == c.Y;

    public Coordinate Set(int x, int y)
    {
        X = x;
        Y = y;
        return this;
    }

    public Coordinate AlsoSet(Coordinate c)
    {
        X = c.X;
        Y = c.Y;
        return this;
    }

    public void Scroll() =>
        Y--;

    public void NextRow()
    {
        X = 0;
        Y++;
    }

    public bool MoveNext()
    {
        if (X < Width - 1)
        {
            X++;
            return true;
        }

        if (Y < Height - 1)
        {
            Y++;
            X = 0;
            return true;
        }

        X = Width - 1;
        Y = Height - 1;
        return false;
    }

    public bool MovePrevious()
    {
        if (X > 0)
        {
            X--;
            return true;
        }

        if (Y > 0)
        {
            Y--;
            X = Width - 1;
            return true;
        }

        X = 0;
        Y = 0;
        return false;
    }

    public static bool operator >(Coordinate? a, Coordinate? b)
    {
        if (a == null || b == null)
            return false;

        if (a.Y > b.Y)
            return true;

        if (a.Y < b.Y)
            return false;

        return a.X > b.X;
    }

    public static bool operator <(Coordinate? a, Coordinate? b)
    {
        if (a == null || b == null)
            return false;

        if (a.Y < b.Y)
            return true;

        if (a.Y > b.Y)
            return false;

        return a.X < b.X;
    }

    public static bool operator >=(Coordinate? a, Coordinate? b)
    {
        if (a == null || b == null)
            return false;

        if (a.IsSame(b))
            return true;

        if (a.Y > b.Y)
            return true;

        if (a.Y < b.Y)
            return false;

        return a.X > b.X;
    }

    public static bool operator <=(Coordinate? a, Coordinate? b)
    {
        if (a == null || b == null)
            return false;

        if (a.IsSame(b))
            return true;

        if (a.Y < b.Y)
            return true;

        if (a.Y > b.Y)
            return false;

        return a.X < b.X;
    }

    public bool CanMoveDown() =>
        Y < Height - 1;


    public bool AtEnd()
    {
        if (Y < Height - 1)
            return false;

        return X >= Width - 1;
    }

    public bool AtEnd(CoordinateList earlyStop)
    {
        if (Y < Height - 1)
            return false;

        return X >= Width - 1;
    }

    public override string ToString() =>
        $"({X}, {Y})";
}
