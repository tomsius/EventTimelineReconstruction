using EventTimelineReconstruction.Benchmarks.Models;
using EventTimelineReconstruction.Benchmarks.Utils;

namespace EventTimelineReconstruction.Benchmarks.ChainOfResponsibility.HighLevelEvents;

public class HighMetaEventHandler : IHighMetaEventHandler
{
    private readonly IHighLevelEventsAbstractorUtils _abstractorUtils;

    public IHandler Next { get; set; }

    public HighMetaEventHandler(IHighLevelEventsAbstractorUtils abstractorUtils)
    {
        _abstractorUtils = abstractorUtils;
    }

    public ISerializableLevel FormAbstractEvent(List<EventViewModel> events, List<ISerializableLevel> abstractionLevel, EventViewModel currentEvent)
    {
        if (currentEvent.Source == "META")
        {
            HighLevelEventViewModel _event = null;

            if (currentEvent.MACB.Contains('B'))
            {
                int lastMetaEventIndex = _abstractorUtils.FindLastEventIndexOf(abstractionLevel, currentEvent.FullDate, "META");

                if (lastMetaEventIndex == -1)
                {
                    string shortValue = currentEvent.Filename;
                    _event = new(
                        DateOnly.FromDateTime(currentEvent.FullDate),
                        TimeOnly.FromDateTime(currentEvent.FullDate),
                        currentEvent.Source,
                        shortValue,
                        "-",
                        currentEvent.SourceLine);
                }
            }

            return _event;
        }

        if (Next is not null)
        {
            return Next.FormAbstractEvent(events, abstractionLevel, currentEvent);
        }

        return null;
    }
}
