using System;
using System.Collections.Generic;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Abstractors;

public sealed class HighLevelEventsAbstractor : IHighLevelEventsAbstractor
{
    private const double _minutesThreshold = 50.0;

    private readonly IHighLevelEventsAbstractorUtils _abstractorUtils;

    public HighLevelEventsAbstractor(IHighLevelEventsAbstractorUtils abstractorUtils)
    {
        _abstractorUtils = abstractorUtils;
    }

    public List<ISerializableLevel> FormHighLevelEvents(List<EventViewModel> events)
    {
        List<ISerializableLevel> highLevelEvents = new(events.Count);

        for (int i = 0; i < events.Count; i++)
        {
            if (events[i].MACB.Contains('B'))
            {
                switch (events[i].Source)
                {
                    case "LOG":
                        HighLevelEventViewModel logEvent = this.FormEventFromLogSource(events[i]);
                        int lastLogEventIndex = FindLastEventIndexOf(highLevelEvents, events[i].FullDate, "LOG");

                        if (lastLogEventIndex != -1)
                        {
                            highLevelEvents.RemoveAt(lastLogEventIndex);
                            highLevelEvents.Insert(lastLogEventIndex, logEvent);
                        }
                        else
                        {
                            highLevelEvents.Add(logEvent);
                        }

                        break;
                    case "LNK":
                        HighLevelEventViewModel lnkEvent = this.FormEventFromLnkSource(events[i]);
                        int lastLnkEventIndex = FindLastEventIndexOf(highLevelEvents, events[i].FullDate, "LNK");

                        if (lastLnkEventIndex != -1)
                        {
                            highLevelEvents.RemoveAt(lastLnkEventIndex);
                            highLevelEvents.Insert(lastLnkEventIndex, lnkEvent);
                        }
                        else
                        {
                            highLevelEvents.Add(lnkEvent);
                        }

                        break;
                    case "META":
                        int lastMetaEventIndex = FindLastEventIndexOf(highLevelEvents, events[i].FullDate, "META");

                        if (lastMetaEventIndex == -1)
                        {
                            HighLevelEventViewModel metaEvent = FormEventFromMetaSource(events[i]);
                            highLevelEvents.Add(metaEvent);
                        }

                        break;
                    case "OLECF":
                        HighLevelEventViewModel olecfEvent = FormEventFromOlecfSource(events[i]);

                        if (IsOlecfEventValid((HighLevelEventViewModel)highLevelEvents[^1], olecfEvent))
                        {
                            highLevelEvents.Add(olecfEvent);
                        }

                        break;
                    case "PE":
                        int lastPeEventIndex = FindLastEventIndexOf(highLevelEvents, events[i].FullDate, "PE");

                        if (lastPeEventIndex == -1 && _abstractorUtils.IsValidPeEvent(events[i]))
                        {
                            HighLevelEventViewModel peEvent = FormEventFromPeSource(events[i]);
                            highLevelEvents.Add(peEvent);
                        }

                        break;
                    default:
                        break;
                }
            }

            if (events[i].Source == "WEBHIST" && _abstractorUtils.IsValidWebhistLine(events[i]))
            {
                HighLevelEventViewModel webhistEvent = this.FormEventFromWebhistSource(events[i]);

                if (webhistEvent is not null && IsWebhistEventValid(highLevelEvents, webhistEvent))
                {
                    highLevelEvents.Add(webhistEvent);
                }
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
