using System;
using System.Collections.Generic;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Abstractors;

public sealed class LowLevelEventsAbstractor : ILowLevelEventsAbstractor
{
    private const double _minutesThreshold = 30.0;

    private readonly IHighLevelEventsAbstractorUtils _highLevelAbstractorUtils;
    private readonly ILowLevelEventsAbstractorUtils _lowLevelAbstractorUtils;

    public LowLevelEventsAbstractor(IHighLevelEventsAbstractorUtils highLevelAbstractorUtils, ILowLevelEventsAbstractorUtils lowLevelAbstractorUtils)
    {
        _highLevelAbstractorUtils = highLevelAbstractorUtils;
        _lowLevelAbstractorUtils = lowLevelAbstractorUtils;
    }

    public List<LowLevelEventViewModel> FormLowLevelEvents(List<EventViewModel> events)
    {
        List<LowLevelEventViewModel> lowLevelEvents = new(events.Count);

        for (int i = 0; i < events.Count; i++)
        {
            LowLevelEventViewModel lowLevelEvent = null;

            switch (events[i].Source)
            {
                case "WEBHIST":
                    if (_highLevelAbstractorUtils.IsValidWebhistLine(events[i]))
                    {
                        lowLevelEvent = this.FormEventFromWebhistSource(events[i]);

                        if (!IsWebhistEventValid(lowLevelEvents, lowLevelEvent))
                        {
                            lowLevelEvent = null;
                        }
                    }

                    break;
                case "FILE":
                    int validEventIndex = this.GetValidFileEventIndex(events, i);

                    if (validEventIndex != -1 && !this.DoesNeedComposing(events, i - 1, events[validEventIndex]))
                    {
                        lowLevelEvent = this.FormEventFromFileSource(events[validEventIndex]);

                        int offset = validEventIndex - i;
                        i += offset;

                        bool isSearchedFormat = events[i].Format == "lnk/shell_items" || events[i].Format == "filestat";
                        if (isSearchedFormat)
                        {
                            DateTime date = events[i].FullDate;

                            while (i < events.Count - 1 && events[i].Source == "FILE" && events[i + 1].FullDate.CompareTo(date) == 0)
                            {
                                i++;
                            }
                        }
                    }

                    break;
                case "LNK":
                    if (!this.DoesNeedComposing(events, i - 1, events[i]))
                    {
                        lowLevelEvent = this.FormEventFromLnkSource(events[i]);
                    }

                    break;
                case "LOG":
                    if (!this.DoesNeedComposing(events, i - 1, events[i]))
                    {
                        lowLevelEvent = this.FormEventFromLogSource(events[i]);
                    }

                    break;
                case "META":
                    lowLevelEvent = this.FormEventFromMetaSource(events[i]);
                    break;
                case "REG":
                    if (_lowLevelAbstractorUtils.IsValidRegEvent(events[i]))
                    {
                        lowLevelEvent = this.FormEventFromRegSource(events[i]);
                    }

                    break;
                case "OLECF":
                    // Choose the last line of the same OLECF time - skip same time OLECF type events
                    while (i < events.Count - 1 && AreOlecfEventsOfSameTime(events[i], events[i + 1]))
                    {
                        i++;
                    }

                    lowLevelEvent = FormEventFromOlecfSource(events[i]);
                    break;
                case "PE":
                    if (_highLevelAbstractorUtils.IsValidPeEvent(events[i]))
                    {
                        lowLevelEvent = FormEventFromPeSource(events[i]);
                    }

                    break;
                case "RECBIN":
                    lowLevelEvent = this.FormEventFromRecbinSource(events[i]);
                    break;
                default:
                    break;
            }

            if (lowLevelEvent is not null)
            {
                lowLevelEvents.Add(lowLevelEvent);
                NormalizeEvents(lowLevelEvents, events, events[i]);
            }
        }

        return lowLevelEvents;
    }

    private static void NormalizeEvents(List<LowLevelEventViewModel> lowLevelEvents, List<EventViewModel> events, EventViewModel currentEvent)
    {
        if (lowLevelEvents.Count < 2)
        {
            return;
        }

        int eventIndex = GetEventIndex(events, lowLevelEvents[^2].Reference);
        EventViewModel lastEvent = events[eventIndex];

        if (lastEvent.FullDate.CompareTo(currentEvent.FullDate) != 0 || lowLevelEvents[^2].Short != lowLevelEvents[^1].Short)
        {
            return;
        }

        if (lastEvent.MACB.Contains('B') && !currentEvent.MACB.Contains('B'))
        {
            lowLevelEvents.RemoveAt(lowLevelEvents.Count - 1);
            return;
        }

        if (!lastEvent.MACB.Contains('B') && currentEvent.MACB.Contains('B'))
        {
            lowLevelEvents.RemoveAt(lowLevelEvents.Count - 2);
            return;
        }

        if (lastEvent.Filename.Contains(".lnk") && !currentEvent.Filename.Contains(".lnk"))
        {
            lowLevelEvents.RemoveAt(lowLevelEvents.Count - 1);
            return;
        }

        if (!lastEvent.Filename.Contains(".lnk") && currentEvent.Filename.Contains(".lnk"))
        {
            lowLevelEvents.RemoveAt(lowLevelEvents.Count - 2);
            return;
        }

        if (lastEvent.Source == "REG" && currentEvent.Source == "REG")
        {
            lowLevelEvents.RemoveAt(lowLevelEvents.Count - 1);
            return;
        }

        if ((lastEvent.Source == "REG" || lastEvent.Source == "OLECF" || lastEvent.Source == "LOG") && currentEvent.Source == "LOG")
        {
            lowLevelEvents.RemoveAt(lowLevelEvents.Count - 2);
            return;
        }

        if (lastEvent.Source == "LOG" && (currentEvent.Source == "REG" || currentEvent.Source == "OLECF" || currentEvent.Source == "LOG"))
        {
            lowLevelEvents.RemoveAt(lowLevelEvents.Count - 1);
            return;
        }

        lowLevelEvents.RemoveAt(lowLevelEvents.Count - 1);
    }

    private static int GetEventIndex(List<EventViewModel> events, int reference)
    {
        int index = -1;

        for (int i = events.Count - 1; i >= 0; i--)
        {
            if (events[i].SourceLine == reference)
            {
                index = i;
                break;
            }
        }

        return index;
    }

    private bool DoesNeedComposing(List<EventViewModel> events, int startIndex, EventViewModel current)
    {
        for (int i = startIndex; i >= 0; i--)
        {
            EventViewModel previous = events[i];
            if (previous.FullDate.CompareTo(current.FullDate) != 0)
            {
                break;
            }

            string previousFilename = _lowLevelAbstractorUtils.GetFilename(previous);
            string currentFilename = _lowLevelAbstractorUtils.GetFilename(current);

            if (previousFilename == currentFilename)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsWebhistEventValid(List<LowLevelEventViewModel> lowLevelEvents, LowLevelEventViewModel lowLevelEvent)
    {
        if (lowLevelEvent is null)
        {
            return false;
        }

        if (lowLevelEvents.Count == 0)
        {
            return true;
        }

        if (lowLevelEvents[^1].Short != lowLevelEvent.Short)
        {
            return true;
        }

        LowLevelEventViewModel startEvent = lowLevelEvents[^1];
        DateTime startTime = new(startEvent.Date.Year, startEvent.Date.Month, startEvent.Date.Day, startEvent.Time.Hour, startEvent.Time.Minute, startEvent.Time.Second);
        DateTime endTime = new(lowLevelEvent.Date.Year, lowLevelEvent.Date.Month, lowLevelEvent.Date.Day, lowLevelEvent.Time.Hour, lowLevelEvent.Time.Minute, lowLevelEvent.Time.Second);

        if (endTime.Subtract(startTime).TotalMinutes.CompareTo(_minutesThreshold) >= 0)
        {
            return true;
        }

        return false;
    }

    private LowLevelEventViewModel FormEventFromWebhistSource(EventViewModel eventViewModel)
    {
        string extraValue = "-";

        if (eventViewModel.SourceType.ToLower().Contains("firefox"))
        {
            extraValue = _lowLevelAbstractorUtils.GetFirefoxExtraFromWebhistSource(eventViewModel.Extra);
        }

        if (_highLevelAbstractorUtils.IsWebhistDownloadEvent(eventViewModel))
        {
            return this.FormDownloadEvent(eventViewModel, extraValue);
        }
        else if (_highLevelAbstractorUtils.IsWebhistMailEvent(eventViewModel))
        {
            return this.FormMailEvent(eventViewModel, extraValue);
        }
        else if (_highLevelAbstractorUtils.IsWebhistNamingActivityEvent(eventViewModel))
        {
            return this.FormNamingActivityEvent(eventViewModel, extraValue);
        }

        return null;
    }

    private LowLevelEventViewModel FormDownloadEvent(EventViewModel eventViewModel, string extraValue)
    {
        string downloadedFileName = _highLevelAbstractorUtils.GetDownloadedFileName(eventViewModel.Description);

        LowLevelEventViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = downloadedFileName,
            Extra = extraValue,
            Reference = eventViewModel.SourceLine,
            Visit = "Download"
        };

        return result;
    }

    private LowLevelEventViewModel FormMailEvent(EventViewModel eventViewModel, string extraValue)
    {
        string shortValue = _highLevelAbstractorUtils.GetMailUrl(eventViewModel.Description);
        string newExtraValue = _lowLevelAbstractorUtils.AddMailUser(extraValue, eventViewModel.Description);

        LowLevelEventViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = shortValue,
            Extra = newExtraValue,
            Reference = eventViewModel.SourceLine,
            Visit = "Mail"
        };

        return result;
    }

    private LowLevelEventViewModel FormNamingActivityEvent(EventViewModel eventViewModel, string extraValue)
    {
        string formattedUrl = _lowLevelAbstractorUtils.GetUrl(eventViewModel.Short);
        string visitValue = _highLevelAbstractorUtils.GenerateVisitValue(eventViewModel.Description);

        LowLevelEventViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = formattedUrl,
            Extra = extraValue,
            Reference = eventViewModel.SourceLine,
            Visit = visitValue
        };

        return result;
    }

    private int GetValidFileEventIndex(List<EventViewModel> events, int startIndex)
    {
        for (int i = startIndex; i < events.Count; i++)
        {
            if (events[i].Source != "FILE")
            {
                break;
            }

            if (_lowLevelAbstractorUtils.IsValidFileEvent(events[startIndex]))
            {
                if (events[startIndex].SourceType == "File entry shell item")
                {
                    return GetIndexOfLastFileEntryShellItemEventOfSameTime(events, startIndex);
                }
                else
                {
                    return this.GetIndexOfLastFileEventOfSameTime(events, startIndex);
                }
            }
        }

        return -1;
    }

    private static int GetIndexOfLastFileEntryShellItemEventOfSameTime(List<EventViewModel> events, int startIndex)
    {
        int lastIndex = FindLastIndexOfSameTimeAndSourceType(events, startIndex);
        int output = -1;

        while (startIndex <= lastIndex)
        {
            bool isSearchedFormat = events[startIndex].Format == "lnk/shell_items" || events[startIndex].Format == "filestat";
            if (isSearchedFormat)
            {
                output = startIndex;
            }

            startIndex++;
        }

        return output == -1 ? lastIndex : output;
    }

    private static int FindLastIndexOfSameTimeAndSourceType(List<EventViewModel> events, int startIndex)
    {
        while (startIndex < events.Count - 1 && events[startIndex].Source == "FILE" && events[startIndex + 1].Source == "FILE")
        {
            bool isSearchedSourceType = events[startIndex].SourceType == "File entry shell item";
            bool isSameDate = events[startIndex].FullDate.CompareTo(events[startIndex + 1].FullDate) == 0;

            if (isSearchedSourceType && isSameDate)
            {
                startIndex++;
            }
            else
            {
                break;
            }
        }

        return startIndex;
    }

    private int GetIndexOfLastFileEventOfSameTime(List<EventViewModel> events, int startIndex)
    {
        int lastValidIndex = startIndex;

        while (startIndex < events.Count - 1 && events[startIndex].Source == "FILE" && events[startIndex + 1].Source == "FILE")
        {
            if (_lowLevelAbstractorUtils.IsValidFileEvent(events[startIndex]) && events[startIndex].FullDate.CompareTo(events[startIndex + 1].FullDate) != 0)
            {
                return startIndex;
            }

            if (_lowLevelAbstractorUtils.IsValidFileEvent(events[startIndex]))
            {
                lastValidIndex = startIndex;
            }

            startIndex++;
        }

        return lastValidIndex;
    }

    private LowLevelEventViewModel FormEventFromFileSource(EventViewModel eventViewModel)
    {
        string shortValue = _lowLevelAbstractorUtils.GetFilename(eventViewModel);
        string extraValue = _lowLevelAbstractorUtils.GetExtraTillSha256(eventViewModel.Extra);

        LowLevelEventViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = shortValue,
            Extra = extraValue,
            Reference = eventViewModel.SourceLine
        };

        return result;
    }

    private LowLevelEventViewModel FormEventFromLnkSource(EventViewModel eventViewModel)
    {
        string shortValue = _highLevelAbstractorUtils.GetShort(eventViewModel.Description);

        LowLevelEventViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = shortValue,
            Reference = eventViewModel.SourceLine
        };

        return result;
    }

    private LowLevelEventViewModel FormEventFromLogSource(EventViewModel eventViewModel)
    {
        string shortValue = _lowLevelAbstractorUtils.GetShort(eventViewModel.Description);

        LowLevelEventViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = shortValue,
            Reference = eventViewModel.SourceLine
        };

        return result;
    }

    private LowLevelEventViewModel FormEventFromMetaSource(EventViewModel eventViewModel)
    {
        string shortValue = eventViewModel.Filename;
        string extraValue = _lowLevelAbstractorUtils.GetKeywordsFromExtra(eventViewModel.Extra, eventViewModel.Short);

        LowLevelEventViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = shortValue,
            Extra = extraValue,
            Reference = eventViewModel.SourceLine
        };

        return result;
    }

    private LowLevelEventViewModel FormEventFromRegSource(EventViewModel eventViewModel)
    {
        string shortValue = _lowLevelAbstractorUtils.GetSummaryFromShort(eventViewModel.Description);
        string extraValue = _lowLevelAbstractorUtils.GetExtraTillSha256(eventViewModel.Extra);

        LowLevelEventViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = shortValue,
            Extra = extraValue,
            Reference = eventViewModel.SourceLine
        };

        return result;
    }

    private static bool AreOlecfEventsOfSameTime(EventViewModel firstEvent, EventViewModel secondEvent)
    {
        bool isSameSource = firstEvent.Source == secondEvent.Source;
        bool isSameTime = firstEvent.FullDate.CompareTo(secondEvent.FullDate) == 0;

        return isSameSource && isSameTime;
    }

    private static LowLevelEventViewModel FormEventFromOlecfSource(EventViewModel eventViewModel)
    {
        string shortValue = eventViewModel.Filename;
        string extraValue = eventViewModel.Short;

        LowLevelEventViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = shortValue,
            Extra = extraValue,
            Reference = eventViewModel.SourceLine
        };

        return result;
    }

    private static LowLevelEventViewModel FormEventFromPeSource(EventViewModel eventViewModel)
    {
        string shortValue = eventViewModel.Filename;

        LowLevelEventViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = shortValue,
            Reference = eventViewModel.SourceLine
        };

        return result;
    }

    private LowLevelEventViewModel FormEventFromRecbinSource(EventViewModel eventViewModel)
    {
        string shortValue = eventViewModel.Short;
        string extraValue = _lowLevelAbstractorUtils.GetExtraTillSha256(eventViewModel.Extra);

        LowLevelEventViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = shortValue,
            Extra = extraValue,
            Reference = eventViewModel.SourceLine
        };

        return result;
    }
}
