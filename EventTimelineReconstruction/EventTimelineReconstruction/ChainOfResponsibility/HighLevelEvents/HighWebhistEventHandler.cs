using System.Collections.Generic;
using System;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.ChainOfResponsibility.HighLevelEvents;

public class HighWebhistEventHandler : IHighWebhistEventHandler
{
    private readonly IHighLevelEventsAbstractorUtils _abstractorUtils;
    private const double _minutesThreshold = 50.0;

    public IHandler Next { get; set; }

    public HighWebhistEventHandler(IHighLevelEventsAbstractorUtils abstractorUtils)
    {
        _abstractorUtils = abstractorUtils;
    }

    public ISerializableLevel FormAbstractEvent(List<EventViewModel> events, List<ISerializableLevel> abstractionLevel, EventViewModel currentEvent)
    {
        if (currentEvent.Source == "WEBHIST")
        {
            HighLevelEventViewModel _event = null;

            if (_abstractorUtils.IsValidWebhistLine(currentEvent))
            {
                _event = FormEventFromWebhistSource(currentEvent);

                if (!IsWebhistEventValid(abstractionLevel, _event))
                {
                    _event = null;
                }
            }
            
            return _event;
        }

        if (Next is not null)
        {
            return Next.FormAbstractEvent(events, abstractionLevel, currentEvent);
        }

        return null;
    }

    private HighLevelEventViewModel FormEventFromWebhistSource(EventViewModel eventViewModel)
    {
        if (_abstractorUtils.IsWebhistDownloadEvent(eventViewModel))
        {
            string downloadedFileName = _abstractorUtils.GetDownloadedFileName(eventViewModel.Description);
            return new HighLevelEventViewModel(
                DateOnly.FromDateTime(eventViewModel.FullDate),
                TimeOnly.FromDateTime(eventViewModel.FullDate),
                eventViewModel.Source,
                downloadedFileName,
                "Download",
                eventViewModel.SourceLine);
        }
        else if (_abstractorUtils.IsWebhistMailEvent(eventViewModel))
        {
            string shortValue = _abstractorUtils.GetMailUrl(eventViewModel.Short);
            return new HighLevelEventViewModel(
                DateOnly.FromDateTime(eventViewModel.FullDate),
                TimeOnly.FromDateTime(eventViewModel.FullDate),
                eventViewModel.Source,
                shortValue,
                "Mail",
                eventViewModel.SourceLine);
        }
        else if (_abstractorUtils.IsWebhistNamingActivityEvent(eventViewModel))
        {
            string urlHost = _abstractorUtils.GetUrlHost(eventViewModel.Short);
            string visitValue = _abstractorUtils.GenerateVisitValue(eventViewModel.Description);
            return new HighLevelEventViewModel(
                DateOnly.FromDateTime(eventViewModel.FullDate),
                TimeOnly.FromDateTime(eventViewModel.FullDate),
                eventViewModel.Source,
                urlHost,
                visitValue,
                eventViewModel.SourceLine);
        }

        return null;
    }

    private static bool IsWebhistEventValid(List<ISerializableLevel> highLevelEvents, HighLevelEventViewModel webhistEvent)
    {
        if (webhistEvent is null)
        {
            return false;
        }

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
