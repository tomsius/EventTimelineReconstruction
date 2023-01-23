using System.Collections.Generic;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Models;

public sealed class LoadedWork
{
    public List<EventViewModel> Events { get; set; }
    public List<HighLevelEventViewModel> HighLevelEvents { get; set; }
    public List<LowLevelEventViewModel> LowLevelEvents { get; set; }
    public List<HighLevelArtefactViewModel> HighLevelArtefacts { get; set; }
    public List<LowLevelArtefactViewModel> LowLevelArtefacts { get; set; }
}
