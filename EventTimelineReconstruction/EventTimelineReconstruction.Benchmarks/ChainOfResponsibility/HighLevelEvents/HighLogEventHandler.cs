using EventTimelineReconstruction.Benchmarks.Models;
using EventTimelineReconstruction.Benchmarks.Utils;

namespace EventTimelineReconstruction.Benchmarks.ChainOfResponsibility.HighLevelEvents;

public class HighLogEventHandler : IHighLogEventHandler
{
    private readonly IHighLevelEventsAbstractorUtils _abstractorUtils;

    public IHandler Next { get; set; }

    public HighLogEventHandler(IHighLevelEventsAbstractorUtils abstractorUtils)
    {
        _abstractorUtils = abstractorUtils;
    }

    public ISerializableLevel FormAbstractEvent(List<EventViewModel> events, List<ISerializableLevel> abstractionLevel, EventViewModel currentEvent)
    {
        if (currentEvent.Source == "LOG")
        {
            if (currentEvent.MACB.Contains('B'))
            {
                string macAddress = _abstractorUtils.GetMacAddress(currentEvent.Short);
                string origin = _abstractorUtils.GetOrigin(currentEvent.Short);
                string shortValue = $"MAC Address: {macAddress}. Origin: {origin}.";

                HighLevelEventViewModel _event = new(
                    DateOnly.FromDateTime(currentEvent.FullDate),
                    TimeOnly.FromDateTime(currentEvent.FullDate),
                    currentEvent.Source,
                    shortValue,
                    "-",
                    currentEvent.SourceLine);

                int lastLogEventIndex = _abstractorUtils.FindLastEventIndexOf(abstractionLevel, currentEvent.FullDate, "LOG");

                if (lastLogEventIndex != -1)
                {
                    abstractionLevel.RemoveAt(lastLogEventIndex);
                    abstractionLevel.Insert(lastLogEventIndex, _event);
                }
                else
                {
                    abstractionLevel.Add(_event);
                }
            }

            return null;
        }

        if (Next is not null)
        {
            return Next.FormAbstractEvent(events, abstractionLevel, currentEvent);
        }

        return null;
    }
}
