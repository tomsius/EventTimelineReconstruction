using System;
using System.Collections.Generic;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.ChainOfResponsibility.LowLevelEvents;

public class LowWebhistEventHandler : ILowWebhistEventHandler
{
    private readonly IHighLevelEventsAbstractorUtils _highLevelEventsAbstractorUtils;
    private readonly ILowLevelEventsAbstractorUtils _lowLevelEventsAbstractorUtils;

    private const double _minutesThreshold = 30.0;

    public IHandler Next
    {
        get; set;
    }

    public LowWebhistEventHandler(IHighLevelEventsAbstractorUtils highLevelEventsAbstractorUtils, ILowLevelEventsAbstractorUtils lowLevelEventsAbstractorUtils)
    {
        _highLevelEventsAbstractorUtils = highLevelEventsAbstractorUtils;
        _lowLevelEventsAbstractorUtils = lowLevelEventsAbstractorUtils;
    }

    public ISerializableLevel FormAbstractEvent(List<EventViewModel> events, List<ISerializableLevel> abstractionLevel, EventViewModel currentEvent)
    {
        if (currentEvent.Source == "WEBHIST")
        {
            LowLevelEventViewModel _event = null;

            if (_highLevelEventsAbstractorUtils.IsValidWebhistLine(currentEvent))
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

    private LowLevelEventViewModel FormEventFromWebhistSource(EventViewModel eventViewModel)
    {
        string extraValue = "-";

        if (eventViewModel.SourceType.ToLower().Contains("firefox"))
        {
            extraValue = _lowLevelEventsAbstractorUtils.GetFirefoxExtraFromWebhistSource(eventViewModel.Extra);
        }

        if (_highLevelEventsAbstractorUtils.IsWebhistDownloadEvent(eventViewModel))
        {
            string downloadedFileName = _highLevelEventsAbstractorUtils.GetDownloadedFileName(eventViewModel.Description);
            return new LowLevelEventViewModel(
                DateOnly.FromDateTime(eventViewModel.FullDate),
                TimeOnly.FromDateTime(eventViewModel.FullDate),
                eventViewModel.Source,
                downloadedFileName,
                "Download",
                extraValue,
                eventViewModel.SourceLine);
        }
        else if (_highLevelEventsAbstractorUtils.IsWebhistMailEvent(eventViewModel))
        {
            string shortValue = _highLevelEventsAbstractorUtils.GetMailUrl(eventViewModel.Description);
            string newExtraValue = _lowLevelEventsAbstractorUtils.AddMailUser(extraValue, eventViewModel.Description);
            return new LowLevelEventViewModel(
                DateOnly.FromDateTime(eventViewModel.FullDate),
                TimeOnly.FromDateTime(eventViewModel.FullDate),
                eventViewModel.Source,
                shortValue,
                "Mail",
                newExtraValue,
                eventViewModel.SourceLine);
        }
        else if (_highLevelEventsAbstractorUtils.IsWebhistNamingActivityEvent(eventViewModel))
        {
            string formattedUrl = _lowLevelEventsAbstractorUtils.GetUrl(eventViewModel.Short);
            string visitValue = _highLevelEventsAbstractorUtils.GenerateVisitValue(eventViewModel.Description);
            return new LowLevelEventViewModel(
                DateOnly.FromDateTime(eventViewModel.FullDate),
                TimeOnly.FromDateTime(eventViewModel.FullDate),
                eventViewModel.Source,
                formattedUrl,
                visitValue,
                extraValue,
                eventViewModel.SourceLine);
        }

        return null;
    }

    private static bool IsWebhistEventValid(List<ISerializableLevel> lowLevelEvents, LowLevelEventViewModel lowLevelEvent)
    {
        if (lowLevelEvent is null)
        {
            return false;
        }

        if (lowLevelEvents.Count == 0)
        {
            return true;
        }

        if (((LowLevelEventViewModel)lowLevelEvents[^1]).Short != lowLevelEvent.Short)
        {
            return true;
        }

        LowLevelEventViewModel startEvent = (LowLevelEventViewModel)lowLevelEvents[^1];
        DateTime startTime = new(startEvent.Date.Year, startEvent.Date.Month, startEvent.Date.Day, startEvent.Time.Hour, startEvent.Time.Minute, startEvent.Time.Second);
        DateTime endTime = new(lowLevelEvent.Date.Year, lowLevelEvent.Date.Month, lowLevelEvent.Date.Day, lowLevelEvent.Time.Hour, lowLevelEvent.Time.Minute, lowLevelEvent.Time.Second);

        if (endTime.Subtract(startTime).TotalMinutes.CompareTo(_minutesThreshold) >= 0)
        {
            return true;
        }

        return false;
    }
}
