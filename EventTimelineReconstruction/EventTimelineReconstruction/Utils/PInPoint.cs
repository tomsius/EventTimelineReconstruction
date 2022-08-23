using System.Windows;

namespace EventTimelineReconstruction.Utils;

public struct PInPoint
{
    public int X;
    public int Y;

    public Point GetPoint(Point offset)
    {
        return new Point(X + offset.X, Y + offset.Y);
    }
}
