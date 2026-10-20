#nullable enable
using System.Collections.Generic;
using System.Linq;

namespace TerminalMatrixNetFramework;

public class CoordinateList : List<Coordinate>
{
    public CoordinateList()
    {
    }

    public CoordinateList(int startX, int startY)
    {
        Add(new Coordinate(startX, startY));
    }

    public void Add(int x, int y) =>
        Add(new Coordinate(x, y));

    public bool HitTest(Coordinate c) =>
        this.Any(coordinate => coordinate.IsSame(c));

    public Coordinate? GetAt(Coordinate c) =>
        this.FirstOrDefault(coordinate => coordinate.IsSame(c));

    public void PushAt(Coordinate c)
    {
        var pushStart = GetAt(c);

        if (pushStart == null)
            return;

        DeleteAllAt(pushStart);

        if (pushStart.MoveNext())
            DeleteAllAt(pushStart);

        Add(pushStart.X, pushStart.Y);
    }

    private void DeleteAllAt(Coordinate c)
    {
        bool again;

        do
        {
            var x = GetAt(c);

            if (x == null)
            {
                again = false;
            }
            else
            {
                Remove(x);
                again = true;
            }
        } while (again);
    }
}
