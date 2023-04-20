using System.Collections.Generic;
using System;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.ChainOfResponsibility.HighLevelEvents;

public class HighOlecfEventHandler : IHighOlecfEventHandler
{
    public IHandler Next { get; set; }

    public ISerializableLevel FormAbstractEvent(List<EventViewModel> events, List<ISerializableLevel> abstractionLevel, EventViewModel currentEvent)
    {
        if (currentEvent.Source == "OLECF")
        {
            string shortValue = currentEvent.Filename;
            HighLevelEventViewModel _event = new(
                DateOnly.FromDateTime(currentEvent.FullDate),
                TimeOnly.FromDateTime(currentEvent.FullDate),
                currentEvent.Source,
                shortValue,
                "-",
                currentEvent.SourceLine);

            if (currentEvent.MACB.Contains('B') && IsOlecfEventValid((HighLevelEventViewModel)abstractionLevel[^1], _event))
            {
                return _event;
            }
            else
            {
                return null;
            }
        }

        if (Next is not null)
        {
            return Next.FormAbstractEvent(events, abstractionLevel, currentEvent);
        }

        return null;
    }

    private static bool IsOlecfEventValid(HighLevelEventViewModel lastHighLevelEvent, HighLevelEventViewModel newOlecfEvent)
    {
        bool isSameSource = lastHighLevelEvent.Source == newOlecfEvent.Source;
        bool isSameShortValue = lastHighLevelEvent.Short == newOlecfEvent.Short;

        return isSameSource == false || isSameShortValue == false;
    }
}
