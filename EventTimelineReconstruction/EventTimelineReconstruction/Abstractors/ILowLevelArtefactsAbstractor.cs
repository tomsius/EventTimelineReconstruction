using System.Collections.Generic;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Abstractors;

public interface ILowLevelArtefactsAbstractor
{
    int LinesNeglected { get; }
    int LinesSkipped { get; }

    List<LowLevelArtefactViewModel> FormLowLevelArtefacts(List<EventViewModel> events, double periodInMinutes = 1);
}