using EventTimelineReconstruction.ChainOfResponsibility;
using EventTimelineReconstruction.ChainOfResponsibility.HighLevelArtefacts;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;
using System;
using System.Collections.Generic;

namespace EventTimelineReconstruction.Abstractors;

public sealed class HighLevelArtefactsAbstractor : IHighLevelArtefactsAbstractor
{
    private readonly IHighLevelArtefactsAbstractorUtils _highLevelArtefactsAbstractorUtils;
    private readonly IHandler _handler;
    private const double _minutesThreshold = 15.0;

    public HighLevelArtefactsAbstractor(
        IHighLevelArtefactsAbstractorUtils highLevelArtefactsAbstractorUtils,
        IHighWebhistArtefactHandler webhistHandler, 
        IHighLnkArtefactHandler lnkHandler, 
        IHighFileArtefactHandler fileHandler, 
        IHighLogArtefactHandler logHandler, 
        IHighRegArtefactHandler regHandler,
        IHighMetaArtefactHandler metaHandler, 
        IHighOlecfArtefactHandler olecfHandler, 
        IHighPeArtefactHandler peHandler)
    {
        _highLevelArtefactsAbstractorUtils = highLevelArtefactsAbstractorUtils;
        _handler = webhistHandler;
        webhistHandler.Next = lnkHandler;
        lnkHandler.Next = fileHandler;
        fileHandler.Next = logHandler;
        logHandler.Next = regHandler;
        regHandler.Next = metaHandler;
        metaHandler.Next = olecfHandler;
        olecfHandler.Next = peHandler;
    }

    public List<HighLevelArtefactViewModel> FormHighLevelArtefacts(List<EventViewModel> events)
    {
        List<HighLevelArtefactViewModel> highLevelArtefacts = new(events.Count);

        for (int i = 0; i < events.Count; i++)
        {
            if (events[i].Source == "OLECF")
            {
                while (i < events.Count - 1 && AreOlecfEventsOfSameTime(events[i], events[i + 1]))
                {
                    i++;
                }
            }

            ISerializableLevel highLevelArtefact = _handler.FormAbstractEvent(events, highLevelArtefacts, events[i]);

            if (events[i].Source == "FILE")
            {
                if (!_highLevelArtefactsAbstractorUtils.IsFileDuplicateOfLnk(events, i - 1, events[i]))
                {
                    int fileCountInRowAtSameMinute = _highLevelArtefactsAbstractorUtils.GetFileCountInRowAtSameMinute(events, i);
                    i += fileCountInRowAtSameMinute - 1;
                }
            }

            if (IsEventValid(highLevelArtefacts, (HighLevelArtefactViewModel)highLevelArtefact))
            {
                highLevelArtefacts.Add(highLevelArtefact);
            }
        }

        return highLevelArtefacts;
    }

    private static bool AreOlecfEventsOfSameTime(EventViewModel firstEvent, EventViewModel secondEvent)
    {
        bool isSameSource = firstEvent.Source == secondEvent.Source;
        bool isSameTime = firstEvent.FullDate.CompareTo(secondEvent.FullDate) == 0;

        return isSameSource && isSameTime;
    }

    private static HighLevelArtefactViewModel FormEventFromOlecfSource(EventViewModel eventViewModel)
    {
        string shortValue = eventViewModel.Filename;
        string extraValue = eventViewModel.Short;
        int length = eventViewModel.Description.Length <= 80 ? eventViewModel.Description.Length : 80;
        string descriptionValue = eventViewModel.Description[..length];

        HighLevelArtefactViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = shortValue,
            Extra = extraValue,
            Reference = eventViewModel.SourceLine,
            Macb = eventViewModel.MACB,
            SourceType = eventViewModel.SourceType,
            Description = descriptionValue
        };

        return result;
    }

    private bool IsPeEventValid(List<ISerializableLevel> highLevelArtefacts, EventViewModel current)
    {
        if (_highLevelArtefactsAbstractorUtils.IsPeLineExecutable(current))
        {
            return true;
        }

        if (!_highLevelArtefactsAbstractorUtils.IsPeLineValid(current))
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

    private static HighLevelArtefactViewModel FormEventFromPeSource(EventViewModel eventViewModel)
    {
        string shortValue = eventViewModel.Filename;

        HighLevelArtefactViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = shortValue,
            Reference = eventViewModel.SourceLine,
            Macb = eventViewModel.MACB,
            SourceType = eventViewModel.SourceType,
            Description = eventViewModel.Description
        };

        return result;
    }

    private static bool IsEventValid(List<ISerializableLevel> highLevelArtefacts, HighLevelArtefactViewModel current)
    {
        if (current is null)
        {
            return false;
        }

        if (highLevelArtefacts.Count == 0)
        {
            return true;
        }

        HighLevelArtefactViewModel previous = highLevelArtefacts[^1];

        if (previous.Short != current.Short)
        {
            return true;
        }

        DateTime previousTime = new(previous.Date.Year, previous.Date.Month, previous.Date.Day, previous.Time.Hour, previous.Time.Minute, previous.Time.Second);
        DateTime currentTime = new(current.Date.Year, current.Date.Month, current.Date.Day, current.Time.Hour, current.Time.Minute, current.Time.Second);
        double timeDifference = currentTime.Subtract(previousTime).TotalMinutes;

        return timeDifference.CompareTo(_minutesThreshold) >= 0;
    }
}
