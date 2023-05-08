using System.Collections.Generic;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Abstractors;

public interface IHighLevelEventsAbstractor
{
    List<ISerializableLevel> FormHighLevelEvents(List<EventViewModel> events);
}