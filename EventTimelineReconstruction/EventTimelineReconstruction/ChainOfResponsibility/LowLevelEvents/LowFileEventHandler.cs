using System;
using System.Collections.Generic;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.ChainOfResponsibility.LowLevelEvents;

public class LowFileEventHandler : ILowFileEventHandler
{
    private readonly ILowLevelEventsAbstractorUtils _abstractorUtils;

    public IHandler Next
    {
        get; set;
    }

    public LowFileEventHandler(ILowLevelEventsAbstractorUtils abstractorUtils)
    {
        _abstractorUtils = abstractorUtils;
    }

    public ISerializableLevel FormAbstractEvent(List<EventViewModel> events, List<ISerializableLevel> abstractionLevel, EventViewModel currentEvent)
    {
        if (currentEvent.Source == "FILE")
        {
            LowLevelEventViewModel _event = null;
            int index = events.IndexOf(currentEvent);
            int validEventIndex = GetValidFileEventIndex(events, index);

            if (validEventIndex != -1 && !_abstractorUtils.DoesNeedComposing(events, index - 1, events[validEventIndex]))
            {
                string shortValue = _abstractorUtils.GetFilename(events[validEventIndex]);
                string extraValue = _abstractorUtils.GetExtraTillSha256(events[validEventIndex].Extra);

                _event = new(
                    DateOnly.FromDateTime(events[validEventIndex].FullDate),
                    TimeOnly.FromDateTime(events[validEventIndex].FullDate),
                    events[validEventIndex].Source,
                    shortValue,
                    "-",
                    extraValue,
                    events[validEventIndex].SourceLine);
            }

            return _event;
        }

        if (Next is not null)
        {
            return Next.FormAbstractEvent(events, abstractionLevel, currentEvent);
        }

        return null;
    }

    private int GetValidFileEventIndex(List<EventViewModel> events, int startIndex)
    {
        for (int i = startIndex; i < events.Count; i++)
        {
            if (events[i].Source != "FILE")
            {
                break;
            }

            if (_abstractorUtils.IsValidFileEvent(events[startIndex]))
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
            if (_abstractorUtils.IsValidFileEvent(events[startIndex]) && events[startIndex].FullDate.CompareTo(events[startIndex + 1].FullDate) != 0)
            {
                return startIndex;
            }

            if (_abstractorUtils.IsValidFileEvent(events[startIndex]))
            {
                lastValidIndex = startIndex;
            }

            startIndex++;
        }

        return lastValidIndex;
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

            string previousFilename = _abstractorUtils.GetFilename(previous);
            string currentFilename = _abstractorUtils.GetFilename(current);

            if (previousFilename == currentFilename)
            {
                return true;
            }
        }

        return false;
    }
}
