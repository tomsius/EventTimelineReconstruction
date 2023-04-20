using System;
using System.Collections.Generic;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.ChainOfResponsibility.HighLevelEvents;

public class HighPeEventHandler : IHighPeEventHandler
{
    private readonly IHighLevelEventsAbstractorUtils _abstractorUtils;

    public IHandler Next { get; set; }

    public HighPeEventHandler(IHighLevelEventsAbstractorUtils abstractorUtils)
    {
        _abstractorUtils = abstractorUtils;
    }

    public ISerializableLevel FormAbstractEvent(List<EventViewModel> events, List<ISerializableLevel> abstractionLevel, EventViewModel currentEvent)
    {
        if (currentEvent.Source == "PE")
        {
            if (currentEvent.MACB.Contains('B'))
            {
                int lastPeEventIndex = _abstractorUtils.FindLastEventIndexOf(abstractionLevel, currentEvent.FullDate, "PE");

                if (lastPeEventIndex == -1 && _abstractorUtils.IsValidPeEvent(currentEvent))
                {
                    string shortValue = currentEvent.Filename;
                    return new HighLevelEventViewModel(
                        DateOnly.FromDateTime(currentEvent.FullDate),
                        TimeOnly.FromDateTime(currentEvent.FullDate),
                        currentEvent.Source,
                        shortValue,
                        "-",
                        currentEvent.SourceLine);
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
