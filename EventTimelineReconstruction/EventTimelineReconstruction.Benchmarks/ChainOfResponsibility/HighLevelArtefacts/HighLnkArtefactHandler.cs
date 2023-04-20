using EventTimelineReconstruction.Benchmarks.Models;
using EventTimelineReconstruction.Benchmarks.Utils;

namespace EventTimelineReconstruction.Benchmarks.ChainOfResponsibility.HighLevelArtefacts;

public class HighLnkArtefactHandler : IHighLnkArtefactHandler
{
    private readonly IHighLevelEventsAbstractorUtils _abstractorUtils;

    public IHandler Next
    {
        get; set;
    }

    public HighLnkArtefactHandler(IHighLevelEventsAbstractorUtils abstractorUtils)
    {
        _abstractorUtils = abstractorUtils;
    }

    public ISerializableLevel FormAbstractEvent(List<EventViewModel> events, List<ISerializableLevel> abstractionLevel, EventViewModel currentEvent)
    {
        if (currentEvent.Source == "LNK")
        {
            string shortValue = _abstractorUtils.GetShort(currentEvent.Description);
            HighLevelArtefactViewModel artefact = new(
                DateOnly.FromDateTime(currentEvent.FullDate),
                TimeOnly.FromDateTime(currentEvent.FullDate),
                currentEvent.Source,
                shortValue,
                "-",
                "-",
                currentEvent.SourceLine,
                currentEvent.MACB,
                currentEvent.SourceType,
                "-");

            if (!IsLnkEventValid(abstractionLevel, artefact))
            {
                artefact = null;
            }

            return artefact;
        }

        if (Next is not null)
        {
            return Next.FormAbstractEvent(events, abstractionLevel, currentEvent);
        }

        return null;
    }

    private static bool IsLnkEventValid(List<ISerializableLevel> highLevelArtefacts, HighLevelArtefactViewModel current)
    {
        for (int i = highLevelArtefacts.Count - 1; i >= 0; i--)
        {
            HighLevelArtefactViewModel previous = (HighLevelArtefactViewModel)highLevelArtefacts[i];

            if (previous.Short == current.Short && previous.Source == "LNK")
            {
                return false;
            }
        }

        return true;
    }
}
