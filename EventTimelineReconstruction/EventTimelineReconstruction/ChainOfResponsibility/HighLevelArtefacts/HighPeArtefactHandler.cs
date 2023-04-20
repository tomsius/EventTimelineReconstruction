using System;
using System.Collections.Generic;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.ChainOfResponsibility.HighLevelArtefacts;

public class HighPeArtefactHandler : IHighPeArtefactHandler
{
    private readonly IHighLevelArtefactsAbstractorUtils _abstractorUtils;

    public IHandler Next { get; set; }

    public HighPeArtefactHandler(IHighLevelArtefactsAbstractorUtils abstractorUtils)
    {
        _abstractorUtils = abstractorUtils;
    }

    public ISerializableLevel FormAbstractEvent(List<EventViewModel> events, List<ISerializableLevel> abstractionLevel, EventViewModel currentEvent)
    {
        if (currentEvent.Source == "PE")
        {
            if (this.IsPeEventValid(abstractionLevel, currentEvent))
            {
                string shortValue = currentEvent.Filename;
                return new HighLevelArtefactViewModel(
                    DateOnly.FromDateTime(currentEvent.FullDate),
                    TimeOnly.FromDateTime(currentEvent.FullDate),
                    currentEvent.Source,
                    shortValue,
                    "-",
                    "-",
                    currentEvent.SourceLine,
                    currentEvent.MACB,
                    currentEvent.SourceType,
                    currentEvent.Description);
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

    private bool IsPeEventValid(List<ISerializableLevel> highLevelArtefacts, EventViewModel current)
    {
        if (_abstractorUtils.IsPeLineExecutable(current))
        {
            return true;
        }

        if (!_abstractorUtils.IsPeLineValid(current))
        {
            return false;
        }

        for (int i = highLevelArtefacts.Count - 1; i >= 0; i--)
        {
            HighLevelArtefactViewModel previous = (HighLevelArtefactViewModel)highLevelArtefacts[i];
            DateTime previousTime = new(previous.Date.Year, previous.Date.Month, previous.Date.Day, previous.Time.Hour, previous.Time.Minute, previous.Time.Second);

            if (previousTime.CompareTo(current.FullDate) != 0)
            {
                break;
            }

            if (previous.Source == "PE")
            {
                return false;
            }
        }

        return true;
    }
}
