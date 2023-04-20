using System;
using System.Collections.Generic;
using EventTimelineReconstruction.ChainOfResponsibility;
using EventTimelineReconstruction.ChainOfResponsibility.LowLevelEvents;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Abstractors;

public sealed class LowLevelEventsAbstractor : ILowLevelEventsAbstractor
{
    private readonly ILowLevelEventsAbstractorUtils _lowLevelAbstractorUtils;
    private readonly IHandler _handler;

    public LowLevelEventsAbstractor(
        ILowLevelEventsAbstractorUtils lowLevelAbstractorUtils,
        ILowWebhistEventHandler webhistHandler,
        ILowFileEventHandler fileHandler,
        ILowLnkEventHandler lnkHandler,
        ILowLogEventHandler logHandler,
        ILowMetaEventHandler metaHandler,
        ILowRegEventHandler regHandler,
        ILowOlecfEventHandler olecfHandler,
        ILowPeEventHandler peHandler,
        ILowRecbinEventHandler recbinHandler)
    {
        _lowLevelAbstractorUtils = lowLevelAbstractorUtils;
        _handler = webhistHandler;
        webhistHandler.Next = fileHandler;
        fileHandler.Next = lnkHandler;
        lnkHandler.Next = logHandler;
        logHandler.Next = metaHandler;
        metaHandler.Next = regHandler;
        regHandler.Next = olecfHandler;
        olecfHandler.Next = peHandler;
        peHandler.Next = recbinHandler;
    }

    public List<LowLevelEventViewModel> FormLowLevelEvents(List<EventViewModel> events)
    {
        List<LowLevelEventViewModel> lowLevelEvents = new(events.Count);

        for (int i = 0; i < events.Count; i++)
        {
            if (events[i].Source == "OLECF")
            {
                while (i < events.Count - 1 && AreOlecfEventsOfSameTime(events[i], events[i + 1]))
                {
                    i++;
                }
            }

            ISerializableLevel lowLevelEvent = _handler.FormAbstractEvent(events, lowLevelEvents, events[i]);

            if (events[i].Source == "FILE")
            {
                int validEventIndex = this.GetValidFileEventIndex(events, i);

                if (validEventIndex != -1 && !_lowLevelAbstractorUtils.DoesNeedComposing(events, i - 1, events[validEventIndex]))
                {
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

    private static bool AreOlecfEventsOfSameTime(EventViewModel firstEvent, EventViewModel secondEvent)
    {
        bool isSameSource = firstEvent.Source == secondEvent.Source;
        bool isSameTime = firstEvent.FullDate.CompareTo(secondEvent.FullDate) == 0;

        return isSameSource && isSameTime;
    }
}
