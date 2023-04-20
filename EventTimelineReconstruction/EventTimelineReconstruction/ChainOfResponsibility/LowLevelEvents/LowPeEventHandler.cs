using System.Collections.Generic;
using System;
using EventTimelineReconstruction.ViewModels;
using EventTimelineReconstruction.Utils;

namespace EventTimelineReconstruction.ChainOfResponsibility.LowLevelEvents;

public class LowPeEventHandler : ILowPeEventHandler
{
    private readonly IHighLevelEventsAbstractorUtils _abstractorUtils;

    public IHandler Next { get; set; }

    public LowPeEventHandler(IHighLevelEventsAbstractorUtils abstractorUtils)
    {
        _abstractorUtils = abstractorUtils;
    }

    public ISerializableLevel FormAbstractEvent(List<EventViewModel> events, List<ISerializableLevel> abstractionLevel, EventViewModel currentEvent)
    {
        if (currentEvent.Source == "PE")
        {
            LowLevelEventViewModel _event = null;

            if (_abstractorUtils.IsValidPeEvent(currentEvent))
            {
                string shortValue = currentEvent.Filename;

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
