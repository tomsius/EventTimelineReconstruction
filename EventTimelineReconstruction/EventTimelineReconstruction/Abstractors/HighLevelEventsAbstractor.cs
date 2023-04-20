using System.Collections.Generic;
using EventTimelineReconstruction.ChainOfResponsibility;
using EventTimelineReconstruction.ChainOfResponsibility.HighLevelEvents;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Abstractors;

public sealed class HighLevelEventsAbstractor : IHighLevelEventsAbstractor
{
    private readonly IHandler _handler;

    public HighLevelEventsAbstractor(
        IHighLogEventHandler logHandler,
        IHighLnkEventHandler lnkHandler,
        IHighMetaEventHandler metaHandler,
        IHighOlecfEventHandler olecfHandler,
        IHighPeEventHandler peHandler,
        IHighWebhistEventHandler webhistHandler)
    {
        _handler = logHandler;
        logHandler.Next = lnkHandler;
        lnkHandler.Next = metaHandler;
        metaHandler.Next = olecfHandler;
        olecfHandler.Next = peHandler;
        peHandler.Next = webhistHandler;
    }

    public List<HighLevelEventViewModel> FormHighLevelEvents(List<EventViewModel> events)
    {
        List<HighLevelEventViewModel> highLevelEvents = new(events.Count);

        for (int i = 0; i < events.Count; i++)
        {
            ISerializableLevel highLevelEvent = _handler.FormAbstractEvent(events, highLevelEvents, events[i]);

            if (highLevelEvent is not null)
            {
                highLevelEvents.Add(highLevelEvent);
            }
        }

        return highLevelEvents;
    }

    private HighLevelEventViewModel FormEventFromLogSource(EventViewModel eventViewModel)
    {
        string macAddress = _abstractorUtils.GetMacAddress(eventViewModel.Short);
        string origin = _abstractorUtils.GetOrigin(eventViewModel.Short);
        string shortValue = $"MAC Address: {macAddress}. Origin: {origin}.";

        HighLevelEventViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = shortValue,
            Reference = eventViewModel.SourceLine
        };

        return result;
    }

    private static int FindLastEventIndexOf(List<ISerializableLevel> highLevelEvents, DateTime newEventDate, string source)
    {
        for (int i = highLevelEvents.Count - 1; i >= 0; i--)
        {
            HighLevelEventViewModel highLevelEvent = (HighLevelEventViewModel)highLevelEvents[i];
            DateTime highLevelEventDate = new(highLevelEvent.Date.Year, highLevelEvent.Date.Month, highLevelEvent.Date.Day, highLevelEvent.Time.Hour, highLevelEvent.Time.Minute, highLevelEvent.Time.Second);

            if (highLevelEventDate.CompareTo(newEventDate) == -1)
            {
                break;
            }

            if (highLevelEvent.Source == source && highLevelEventDate.CompareTo(newEventDate) == 0)
            {
                return i;
            }
        }

        return -1;
    }

    private HighLevelEventViewModel FormEventFromLnkSource(EventViewModel eventViewModel)
    {
        string shortValue = _abstractorUtils.GetShort(eventViewModel.Short);

        HighLevelEventViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = shortValue,
            Reference = eventViewModel.SourceLine
        };

        return result;
    }

    private static HighLevelEventViewModel FormEventFromMetaSource(EventViewModel eventViewModel)
    {
        string shortValue = eventViewModel.Filename;

        HighLevelEventViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = shortValue,
            Reference = eventViewModel.SourceLine
        };

        return result;
    }

    private static HighLevelEventViewModel FormEventFromOlecfSource(EventViewModel eventViewModel)
    {
        string shortValue = eventViewModel.Filename;

        HighLevelEventViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = shortValue,
            Reference = eventViewModel.SourceLine
        };

        return result;
    }

    private static bool IsOlecfEventValid(HighLevelEventViewModel lastHighLevelEvent, HighLevelEventViewModel newOlecfEvent)
    {
        bool isSameSource = lastHighLevelEvent.Source == newOlecfEvent.Source;
        bool isSameShortValue = lastHighLevelEvent.Short == newOlecfEvent.Short;

        return isSameSource == false || isSameShortValue == false;
    }

    private static HighLevelEventViewModel FormEventFromPeSource(EventViewModel eventViewModel)
    {
        string shortValue = eventViewModel.Filename;

        HighLevelEventViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = shortValue,
            Reference = eventViewModel.SourceLine
        };

        return result;
    }

    private HighLevelEventViewModel FormEventFromWebhistSource(EventViewModel eventViewModel)
    {
        if (_abstractorUtils.IsWebhistDownloadEvent(eventViewModel))
        {
            return this.FormDownloadEvent(eventViewModel);
        }
        else if (_abstractorUtils.IsWebhistMailEvent(eventViewModel))
        {
            return this.FormMailEvent(eventViewModel);
        }
        else if (_abstractorUtils.IsWebhistNamingActivityEvent(eventViewModel))
        {
            return this.FormNamingActivityEvent(eventViewModel);
        }

        return null;
    }

    private HighLevelEventViewModel FormDownloadEvent(EventViewModel eventViewModel)
    {
        string downloadedFileName = _abstractorUtils.GetDownloadedFileName(eventViewModel.Description);

        HighLevelEventViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = downloadedFileName,
            Reference = eventViewModel.SourceLine,
            Visit = "Download"
        };

        return result;
    }

    private HighLevelEventViewModel FormMailEvent(EventViewModel eventViewModel)
    {
        string shortValue = _abstractorUtils.GetMailUrl(eventViewModel.Short);

        HighLevelEventViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = shortValue,
            Reference = eventViewModel.SourceLine,
            Visit = "Mail"
        };

        return result;
    }

    private HighLevelEventViewModel FormNamingActivityEvent(EventViewModel eventViewModel)
    {
        string urlHost = _abstractorUtils.GetUrlHost(eventViewModel.Short);
        string visitValue = _abstractorUtils.GenerateVisitValue(eventViewModel.Description);

        HighLevelEventViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = urlHost,
            Reference = eventViewModel.SourceLine,
            Visit = visitValue
        };

        return result;
    }

    private static bool IsWebhistEventValid(List<ISerializableLevel> highLevelEvents, HighLevelEventViewModel webhistEvent)
    {
        if (highLevelEvents.Count < 2)
        {
            return true;
        }

        if (((HighLevelEventViewModel)highLevelEvents[^1]).Source != "WEBHIST")
        {
            return true;
        }

        if (((HighLevelEventViewModel)highLevelEvents[^1]).Short != webhistEvent.Short)
        {
            return true;
        }

        HighLevelEventViewModel startEvent = (HighLevelEventViewModel)highLevelEvents[^1];
        DateTime startTime = new(startEvent.Date.Year, startEvent.Date.Month, startEvent.Date.Day, startEvent.Time.Hour, startEvent.Time.Minute, startEvent.Time.Second);
        DateTime endTime = new(webhistEvent.Date.Year, webhistEvent.Date.Month, webhistEvent.Date.Day, webhistEvent.Time.Hour, webhistEvent.Time.Minute, webhistEvent.Time.Second);

        if (endTime.Subtract(startTime).TotalMinutes.CompareTo(_minutesThreshold) >= 0)
        {
            return true;
        }

        return false;
    }
}
