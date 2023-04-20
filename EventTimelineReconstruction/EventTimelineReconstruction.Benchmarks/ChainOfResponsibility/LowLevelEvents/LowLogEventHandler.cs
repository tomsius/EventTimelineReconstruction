using EventTimelineReconstruction.Benchmarks.Models;
using EventTimelineReconstruction.Benchmarks.Utils;

namespace EventTimelineReconstruction.Benchmarks.ChainOfResponsibility.LowLevelEvents;

public class LowLogEventHandler : ILowLogEventHandler
{
    private readonly ILowLevelEventsAbstractorUtils _abstractorUtils;

    public IHandler Next { get; set; }

    public LowLogEventHandler(ILowLevelEventsAbstractorUtils abstractorUtils)
    {
        _abstractorUtils = abstractorUtils;
    }

    public ISerializableLevel FormAbstractEvent(List<EventViewModel> events, List<ISerializableLevel> abstractionLevel, EventViewModel currentEvent)
    {
        if (currentEvent.Source == "LOG")
        {
            LowLevelEventViewModel _event = null;
            int index = events.IndexOf(currentEvent);

            if (!_abstractorUtils.DoesNeedComposing(events, index - 1, currentEvent))
            {
                string shortValue = _abstractorUtils.GetShort(currentEvent.Description);
                _event = new(
                    DateOnly.FromDateTime(currentEvent.FullDate),
                    TimeOnly.FromDateTime(currentEvent.FullDate),
                    currentEvent.Source,
                    shortValue,
                    "-",
                    "-",
                    currentEvent.SourceLine);
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
