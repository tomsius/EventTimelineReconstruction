using System.Collections;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Utils;

public class EventSorter : IComparer
{
    public int Compare(object x, object y)
    {
        EventViewModel eventX = x as EventViewModel;
        EventViewModel eventY = y as EventViewModel;

        return eventX.CompareTo(eventY);
    }
}
