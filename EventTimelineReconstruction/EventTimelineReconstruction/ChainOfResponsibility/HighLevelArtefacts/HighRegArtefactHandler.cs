using System;
using System.Collections.Generic;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.ChainOfResponsibility.HighLevelArtefacts;

public class HighRegArtefactHandler : IHighRegArtefactHandler
{
    private readonly ILowLevelEventsAbstractorUtils _lowLevelEventsAbstractorUtils;
    private readonly IHighLevelArtefactsAbstractorUtils _highLevelArtefactsAbstractorUtils;

    public IHandler Next { get; set; }

    public HighRegArtefactHandler(ILowLevelEventsAbstractorUtils lowLevelEventsAbstractorUtils, IHighLevelArtefactsAbstractorUtils highLevelArtefactsAbstractorUtils)
    {
        _lowLevelEventsAbstractorUtils = lowLevelEventsAbstractorUtils;
        _highLevelArtefactsAbstractorUtils = highLevelArtefactsAbstractorUtils;
    }

    public ISerializableLevel FormAbstractEvent(List<EventViewModel> events, List<ISerializableLevel> abstractionLevel, EventViewModel currentEvent)
    {
        if (currentEvent.Source == "REG")
        {
            string shortValue = _lowLevelEventsAbstractorUtils.GetSummaryFromShort(currentEvent.Description);
            string extraValue = _lowLevelEventsAbstractorUtils.GetExtraTillSha256(currentEvent.Extra);
            string descriptionValue = _highLevelArtefactsAbstractorUtils.GetDescriptionFromRegSource(currentEvent.SourceType, currentEvent.Description);

            HighLevelArtefactViewModel artefact = new(
                DateOnly.FromDateTime(currentEvent.FullDate),
                TimeOnly.FromDateTime(currentEvent.FullDate),
                currentEvent.Source,
                shortValue,
                "-",
                extraValue,
                currentEvent.SourceLine,
                currentEvent.MACB,
                currentEvent.SourceType,
                descriptionValue);

            if (!IsRegEventValid(abstractionLevel, artefact))
            {
                artefact = null;
            }

            return artefact;
        }

        if (Next is not null)
        {
            return Next.FormAbstractEvent(events, abstractionLevel, currentEvent);
        }

        return null;
    }

    private static bool IsRegEventValid(List<ISerializableLevel> highLevelArtefacts, HighLevelArtefactViewModel current)
    {
        if (current.SourceType == "Registry Key: UserAssist" && !current.Description.Contains(".exe"))
        {
            return false;
        }

        if (current.SourceType == "Registry Key : BagMRU" && current.Description.Length == 0)
        {
            return false;
        }

        if (current.SourceType == "Registry Key : Run Key" || (current.SourceType == "UNKNOWN" && !current.Description.Contains('.')))
        {
            for (int i = highLevelArtefacts.Count - 1; i >= 0; i--)
            {
                HighLevelArtefactViewModel previous = (HighLevelArtefactViewModel)highLevelArtefacts[i];
                DateTime previousTime = new(previous.Date.Year, previous.Date.Month, previous.Date.Day, previous.Time.Hour, previous.Time.Minute, previous.Time.Second);
                DateTime currentTime = new(current.Date.Year, current.Date.Month, current.Date.Day, current.Time.Hour, current.Time.Minute, current.Time.Second);

                if (previousTime.CompareTo(currentTime) != 0)
                {
                    break;
                }

                if (previous.SourceType == current.SourceType)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
