using System.Collections.Generic;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Abstractors;

public interface IHighLevelArtefactsAbstractor
{
    List<HighLevelArtefactViewModel> FormHighLevelArtefacts(List<EventViewModel> events);
}