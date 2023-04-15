using System.Collections.Generic;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Abstractors;

public interface IHighLevelArtefactsAbstractor
{
    List<ISerializableLevel> FormHighLevelArtefacts(List<EventViewModel> events);
}