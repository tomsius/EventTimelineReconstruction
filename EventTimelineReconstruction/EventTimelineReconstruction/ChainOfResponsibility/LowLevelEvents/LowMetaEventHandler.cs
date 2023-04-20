using System.Collections.Generic;
using System;
using EventTimelineReconstruction.ViewModels;
using EventTimelineReconstruction.Utils;

namespace EventTimelineReconstruction.ChainOfResponsibility.LowLevelEvents;

public class LowMetaEventHandler : ILowMetaEventHandler
{
    private readonly ILowLevelEventsAbstractorUtils _abstractorUtils;

    public IHandler Next { get; set; }

    public LowMetaEventHandler(ILowLevelEventsAbstractorUtils abstractorUtils)
    {
        _abstractorUtils = abstractorUtils;
    }

    public ISerializableLevel FormAbstractEvent(List<EventViewModel> events, List<ISerializableLevel> abstractionLevel, EventViewModel currentEvent)
    {
        if (currentEvent.Source == "META")
        {
            string shortValue = currentEvent.Filename;
            string extraValue = _abstractorUtils.GetKeywordsFromExtra(currentEvent.Extra, currentEvent.Short);

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
