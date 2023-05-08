using System.Collections.Generic;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Abstractors;

public interface ILowLevelEventsAbstractor
{
    List<ISerializableLevel> FormLowLevelEvents(List<EventViewModel> events);
}