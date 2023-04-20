using System.Collections.Generic;
using System;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.ChainOfResponsibility.HighLevelEvents;

public class HighLnkEventHandler : IHighLnkEventHandler
{
    private readonly IHighLevelEventsAbstractorUtils _abstractorUtils;

    public IHandler Next { get; set; }

    public HighLnkEventHandler(IHighLevelEventsAbstractorUtils abstractorUtils)
    {
        _abstractorUtils = abstractorUtils;
    }

    public ISerializableLevel FormAbstractEvent(List<EventViewModel> events, List<ISerializableLevel> abstractionLevel, EventViewModel currentEvent)
    {
        if (currentEvent.Source == "LNK")
        {
            if (currentEvent.MACB.Contains('B'))
            {
                string shortValue = _abstractorUtils.GetShort(currentEvent.Short);

                HighLevelEventViewModel _event = new(
                    DateOnly.FromDateTime(currentEvent.FullDate),
                    TimeOnly.FromDateTime(currentEvent.FullDate),
                    currentEvent.Source,
                    shortValue,
                    "-",
                    currentEvent.SourceLine);

                int lastLnkEventIndex = _abstractorUtils.FindLastEventIndexOf(abstractionLevel, currentEvent.FullDate, "LNK");

                if (lastLnkEventIndex != -1)
                {
                    abstractionLevel.RemoveAt(lastLnkEventIndex);
                    abstractionLevel.Insert(lastLnkEventIndex, _event);
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
