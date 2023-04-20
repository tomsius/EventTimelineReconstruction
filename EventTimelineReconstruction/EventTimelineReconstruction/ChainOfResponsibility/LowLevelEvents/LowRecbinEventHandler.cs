using System.Collections.Generic;
using System;
using EventTimelineReconstruction.ViewModels;
using EventTimelineReconstruction.Utils;

namespace EventTimelineReconstruction.ChainOfResponsibility.LowLevelEvents;

public class LowRecbinEventHandler : ILowRecbinEventHandler
{
    private readonly ILowLevelEventsAbstractorUtils _abstractorUtils;

    public IHandler Next { get; set; }

    public LowRecbinEventHandler(ILowLevelEventsAbstractorUtils abstractorUtils)
    {
        _abstractorUtils = abstractorUtils;
    }

    public ISerializableLevel FormAbstractEvent(List<EventViewModel> events, List<ISerializableLevel> abstractionLevel, EventViewModel currentEvent)
    {
        if (currentEvent.Source == "RECBIN")
        {
            string shortValue = currentEvent.Short;
            string extraValue = _abstractorUtils.GetExtraTillSha256(currentEvent.Extra);

            return new LowLevelEventViewModel(
                DateOnly.FromDateTime(currentEvent.FullDate),
                TimeOnly.FromDateTime(currentEvent.FullDate),
                currentEvent.Source,
                shortValue,
                "-",
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
