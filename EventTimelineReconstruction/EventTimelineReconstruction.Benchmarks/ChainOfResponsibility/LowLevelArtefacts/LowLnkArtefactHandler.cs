using EventTimelineReconstruction.Benchmarks.Models;
using EventTimelineReconstruction.Benchmarks.Utils;

namespace EventTimelineReconstruction.Benchmarks.ChainOfResponsibility.LowLevelArtefacts;

public class LowLnkArtefactHandler : ILowLnkArtefactHandler
{
    private readonly ILowLevelArtefactsAbstractorUtils _abstractorUtils;

    public IHandler Next { get; set; }

    public LowLnkArtefactHandler(ILowLevelArtefactsAbstractorUtils abstractorUtils)
    {
        _abstractorUtils = abstractorUtils;
    }

    public ISerializableLevel FormAbstractEvent(List<EventViewModel> events, List<ISerializableLevel> abstractionLevel, EventViewModel currentEvent)
    {
        if (currentEvent.Source == "LNK")
        {
            string extraValue = _abstractorUtils.GetExtraValue(currentEvent.Extra);
            return new LowLevelArtefactViewModel(
                DateOnly.FromDateTime(currentEvent.FullDate),
                TimeOnly.FromDateTime(currentEvent.FullDate),
                currentEvent.Timezone.DisplayName,
                currentEvent.MACB,
                currentEvent.Source,
                currentEvent.SourceType,
                currentEvent.Type,
                currentEvent.User,
                currentEvent.Host,
                currentEvent.Short,
                currentEvent.Description,
                currentEvent.Version.ToString(),
                currentEvent.Filename,
                currentEvent.INode,
                currentEvent.Notes,
                currentEvent.Format,
                extraValue,
                currentEvent.SourceLine);
        }

        if (Next is not null)
        {
            return Next.FormAbstractEvent(events, abstractionLevel, currentEvent);
        }

        return null;
    }
}
