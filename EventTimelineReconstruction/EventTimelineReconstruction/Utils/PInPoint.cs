using System.Windows;

namespace EventTimelineReconstruction.Utils;

public struct PInPoint
{
    public int X;
    public int Y;

    public PInPoint(int x, int y)
    {
        X = x; Y = y;
    }

    public PInPoint(double x, double y)
    {
        X = (int)x; Y = (int)y;
    }

    public Point GetPoint(double xOffset = 0, double yOffet = 0)
    {
        return new Point(X + xOffset, Y + yOffet);
    }

    public Point GetPoint(Point offset)
    {
        return new Point(X + offset.X, Y + offset.Y);
    }
}
