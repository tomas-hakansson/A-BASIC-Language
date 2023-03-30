namespace A_BASIC_Language.Gui;

public class Point
{
    public int X { get; set; }
    public int Y { get; set; }

    public Point() : this(0, 0)
    {
    }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void Clear()
    {
        X = 0;
        Y = 0;
    }

    public bool Is(int x, int y) =>
        X == x && Y == y;

    public bool Is(Point p) =>
        X == p.X && Y == p.Y;

    public void Set(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void Set(Point point)
    {
        X = point.X;
        Y = point.Y;
    }

    public void MoveLeft(int limit)
    {
        X--;

        if (X < 0 && Y > 0)
        {
            X = limit - 1;
            Y--;
        }
        else if (X < 0)
        {
            X = 0;
        }
    }

    public void MoveRight(int limitX, int limitY)
    {
        {
            X++;

            if (X >= limitX && Y < limitY - 1)
            {
                X = 0;
                Y++;
            }
            else if (X >= limitX)
            {
                X = 0;
                Y = limitY - 1;
            }
        }
    }
}