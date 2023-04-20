using System;
using System.Collections.Generic;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.ChainOfResponsibility.LowLevelEvents;

public class LowRegEventHandler : ILowRegEventHandler
{
    private readonly ILowLevelEventsAbstractorUtils _abstractorUtils;

    public IHandler Next { get; set; }

    public LowRegEventHandler(ILowLevelEventsAbstractorUtils abstractorUtils)
    {
        _abstractorUtils = abstractorUtils;
    }

    public ISerializableLevel FormAbstractEvent(List<EventViewModel> events, List<ISerializableLevel> abstractionLevel, EventViewModel currentEvent)
    {
        if (currentEvent.Source == "REG")
        {
            LowLevelEventViewModel _event = null;

            if (_abstractorUtils.IsValidRegEvent(currentEvent))
            {
                string shortValue = _abstractorUtils.GetSummaryFromShort(currentEvent.Description);
                string extraValue = _abstractorUtils.GetExtraTillSha256(currentEvent.Extra);

                _event = new(
                    DateOnly.FromDateTime(currentEvent.FullDate),
                    TimeOnly.FromDateTime(currentEvent.FullDate),
                    currentEvent.Source,
                    shortValue,
                    "-",
                    extraValue,
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
