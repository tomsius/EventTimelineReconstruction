using System.Collections.Generic;
using System;
using EventTimelineReconstruction.ViewModels;
using EventTimelineReconstruction.Utils;

namespace EventTimelineReconstruction.ChainOfResponsibility.LowLevelEvents;

public class LowLnkEventHandler : ILowLnkEventHandler
{
    private readonly IHighLevelEventsAbstractorUtils _highLevelEventsAbstractorUtils;
    private readonly ILowLevelEventsAbstractorUtils _lowLevelEventsAbstractorUtils;

    public IHandler Next { get; set; }

    public LowLnkEventHandler(IHighLevelEventsAbstractorUtils highLevelEventsAbstractorUtils, ILowLevelEventsAbstractorUtils lowLevelEventsAbstractorUtils)
    {
        _highLevelEventsAbstractorUtils = highLevelEventsAbstractorUtils;
        _lowLevelEventsAbstractorUtils = lowLevelEventsAbstractorUtils;
    }

    public ISerializableLevel FormAbstractEvent(List<EventViewModel> events, List<ISerializableLevel> abstractionLevel, EventViewModel currentEvent)
    {
        if (currentEvent.Source == "LNK")
        {
            LowLevelEventViewModel _event = null;
            int index = events.IndexOf(currentEvent);

            if (!_lowLevelEventsAbstractorUtils.DoesNeedComposing(events, index - 1, currentEvent))
            {
                string shortValue = _highLevelEventsAbstractorUtils.GetShort(currentEvent.Description);
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
