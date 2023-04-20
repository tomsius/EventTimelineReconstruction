using System;
using System.Collections.Generic;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.ChainOfResponsibility.HighLevelArtefacts;

public class HighFileArtefactHandler : IHighFileArtefactHandler
{
    private readonly ILowLevelEventsAbstractorUtils _lowLevelEventsAbstractorUtils;
    private readonly IHighLevelArtefactsAbstractorUtils _highLevelArtefactsAbstractorUtils;

    private const int _fileArtefactsInRowCount = 36;

    public IHandler Next { get; set; }

    public HighFileArtefactHandler(ILowLevelEventsAbstractorUtils lowLevelEventsAbstractorUtils, IHighLevelArtefactsAbstractorUtils highLevelArtefactsAbstractorUtils)
    {
        _lowLevelEventsAbstractorUtils = lowLevelEventsAbstractorUtils;
        _highLevelArtefactsAbstractorUtils = highLevelArtefactsAbstractorUtils;
    }

    public ISerializableLevel FormAbstractEvent(List<EventViewModel> events, List<ISerializableLevel> abstractionLevel, EventViewModel currentEvent)
    {
        if (currentEvent.Source == "FILE")
        {
            int index = events.IndexOf(currentEvent);
            string shortValue = _highLevelArtefactsAbstractorUtils.GetFilename(currentEvent.Description);
            string extraValue = _lowLevelEventsAbstractorUtils.GetExtraTillSha256(currentEvent.Extra);

            if (index == 0)
            {
                return new HighLevelArtefactViewModel(
                    DateOnly.FromDateTime(currentEvent.FullDate),
                    TimeOnly.FromDateTime(currentEvent.FullDate),
                    currentEvent.Source,
                    shortValue,
                    "-",
                    extraValue,
                    currentEvent.SourceLine,
                    currentEvent.MACB,
                    currentEvent.SourceType,
                    currentEvent.Description);
            }

            if (!_highLevelArtefactsAbstractorUtils.IsFileDuplicateOfLnk(events, index - 1, currentEvent))
            {
                int fileCountInRowAtSameMinute = _highLevelArtefactsAbstractorUtils.GetFileCountInRowAtSameMinute(events, index);
                int endIndex = index + fileCountInRowAtSameMinute;

                if (fileCountInRowAtSameMinute <= _fileArtefactsInRowCount)
                {
                    for (int i = index; i < endIndex; i++)
                    {
                        shortValue = _highLevelArtefactsAbstractorUtils.GetFilename(events[index + i].Description);
                        extraValue = _lowLevelEventsAbstractorUtils.GetExtraTillSha256(events[index + i].Extra);

                        HighLevelArtefactViewModel artefact = new(
                            DateOnly.FromDateTime(currentEvent.FullDate),
                            TimeOnly.FromDateTime(currentEvent.FullDate),
                            events[index + i].Source,
                            shortValue,
                            "-",
                            extraValue,
                            events[index + i].SourceLine,
                            events[index + i].MACB,
                            events[index + i].SourceType,
                            events[index + i].Description);
                        // single artefact per second
                        if (IsFileEventValid(abstractionLevel, artefact))
                        {
                            abstractionLevel.Add(artefact);
                        }
                    }
                }
                else
                {
                    List<int> validIndices = _highLevelArtefactsAbstractorUtils.GetValidFileEventIndices(events, index, endIndex);
                    for (int i = 0; i < validIndices.Count; i++)
                    {
                        shortValue = _highLevelArtefactsAbstractorUtils.GetFilename(events[validIndices[i]].Description);
                        extraValue = _lowLevelEventsAbstractorUtils.GetExtraTillSha256(events[validIndices[i]].Extra);

                        HighLevelArtefactViewModel artefact = new(
                            DateOnly.FromDateTime(events[validIndices[i]].FullDate),
                            TimeOnly.FromDateTime(events[validIndices[i]].FullDate),
                            events[validIndices[i]].Source,
                            shortValue,
                            "-",
                            extraValue,
                            events[validIndices[i]].SourceLine,
                            events[validIndices[i]].MACB,
                            events[validIndices[i]].SourceType,
                            events[validIndices[i]].Description);
                        abstractionLevel.Add(artefact);
                    }
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

    private bool IsFileEventValid(List<ISerializableLevel> highLevelArtefacts, HighLevelArtefactViewModel current)
    {
        if (highLevelArtefacts.Count == 0)
        {
            return true;
        }

        HighLevelArtefactViewModel previous = (HighLevelArtefactViewModel)highLevelArtefacts[^1];
        DateTime previousTime = new(previous.Date.Year, previous.Date.Month, previous.Date.Day, previous.Time.Hour, previous.Time.Minute, previous.Time.Second);
        DateTime currentTime = new(current.Date.Year, current.Date.Month, current.Date.Day, current.Time.Hour, current.Time.Minute, current.Time.Second);

        return previousTime.CompareTo(currentTime) != 0;
    }
}
