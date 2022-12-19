using System.Collections.Generic;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Abstractors;

public interface IHighLevelEventsAbstractor
{
    List<HighLevelEventViewModel> FormHighLevelEvents(List<EventViewModel> events);
}