using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using EventTimelineReconstruction.Benchmarks.ChainOfResponsibility;
using EventTimelineReconstruction.Benchmarks.ChainOfResponsibility.LowLevelEvents;
using EventTimelineReconstruction.Benchmarks.Models;
using EventTimelineReconstruction.Benchmarks.Utils;

namespace EventTimelineReconstruction.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class LowLevelEventsBenchmarks
{
    [Params(1000, 100_000, 1_000_000)]
    public int N;

    private List<EventViewModel> _events;
    private IHighLevelEventsAbstractorUtils _highLevelAbstractorUtils;
    private ILowLevelEventsAbstractorUtils _lowLevelAbstractorUtils;
    private IHandler _handler;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _highLevelAbstractorUtils = new HighLevelEventsAbstractorUtils();
        _lowLevelAbstractorUtils = new LowLevelEventsAbstractorUtils();

        ILowWebhistEventHandler webhistHandler = new LowWebhistEventHandler(_highLevelAbstractorUtils, _lowLevelAbstractorUtils);
        ILowFileEventHandler fileHandler = new LowFileEventHandler(_lowLevelAbstractorUtils);
        ILowLnkEventHandler lnkHandler = new LowLnkEventHandler(_highLevelAbstractorUtils, _lowLevelAbstractorUtils);
        ILowLogEventHandler logHandler = new LowLogEventHandler(_lowLevelAbstractorUtils);
        ILowMetaEventHandler metaHandler = new LowMetaEventHandler(_lowLevelAbstractorUtils);
        ILowRegEventHandler regHandler = new LowRegEventHandler(_lowLevelAbstractorUtils);
        ILowOlecfEventHandler olecfHandler = new LowOlecfEventHandler();
        ILowPeEventHandler peHandler = new LowPeEventHandler(_highLevelAbstractorUtils);
        ILowRecbinEventHandler recbinHandler = new LowRecbinEventHandler(_lowLevelAbstractorUtils);

        _handler = webhistHandler;
        webhistHandler.Next = fileHandler;
        fileHandler.Next = lnkHandler;
        lnkHandler.Next = logHandler;
        logHandler.Next = metaHandler;
        metaHandler.Next = regHandler;
        regHandler.Next = olecfHandler;
        olecfHandler.Next = peHandler;
        peHandler.Next = recbinHandler;

        // sukurti ivykiu sarasa
        _events = new(N);
    }

    [Benchmark(Baseline = true)]
    public List<LowLevelEventViewModel> FormLowLevelEvents_Switch()
    {
        List<LowLevelEventViewModel> lowLevelEvents = new(_events.Count);

        for (int i = 0; i < _events.Count; i++)
        {
            LowLevelEventViewModel lowLevelEvent = null;

            switch (_events[i].Source)
            {
                case "WEBHIST":
                    if (_highLevelAbstractorUtils.IsValidWebhistLine(_events[i]))
                    {
                        lowLevelEvent = this.FormEventFromWebhistSource_Switch(_events[i]);

                        if (!IsWebhistEventValid_Switch(lowLevelEvents, lowLevelEvent))
                        {
                            lowLevelEvent = null;
                        }
                    }

                    break;
                case "FILE":
                    int validEventIndex = this.GetValidFileEventIndex_Switch(_events, i);

                    if (validEventIndex != -1 && !this.DoesNeedComposing_Switch(_events, i - 1, _events[validEventIndex]))
                    {
                        lowLevelEvent = this.FormEventFromFileSource_Switch(_events[validEventIndex]);

                        int offset = validEventIndex - i;
                        i += offset;

                        bool isSearchedFormat = _events[i].Format == "lnk/shell_items" || _events[i].Format == "filestat";
                        if (isSearchedFormat)
                        {
                            DateTime date = _events[i].FullDate;

                            while (i < _events.Count - 1 && _events[i].Source == "FILE" && _events[i + 1].FullDate.CompareTo(date) == 0)
                            {
                                i++;
                            }
                        }
                    }

                    break;
                case "LNK":
                    if (!this.DoesNeedComposing_Switch(_events, i - 1, _events[i]))
                    {
                        lowLevelEvent = this.FormEventFromLnkSource_Switch(_events[i]);
                    }

                    break;
                case "LOG":
                    if (!this.DoesNeedComposing_Switch(_events, i - 1, _events[i]))
                    {
                        lowLevelEvent = this.FormEventFromLogSource_Switch(_events[i]);
                    }

                    break;
                case "META":
                    lowLevelEvent = this.FormEventFromMetaSource_Switch(_events[i]);
                    break;
                case "REG":
                    if (_lowLevelAbstractorUtils.IsValidRegEvent(_events[i]))
                    {
                        lowLevelEvent = this.FormEventFromRegSource_Switch(_events[i]);
                    }

                    break;
                case "OLECF":
                    while (i < _events.Count - 1 && AreOlecfEventsOfSameTime_Switch(_events[i], _events[i + 1]))
                    {
                        i++;
                    }

                    lowLevelEvent = FormEventFromOlecfSource_Switch(_events[i]);
                    break;
                case "PE":
                    if (_highLevelAbstractorUtils.IsValidPeEvent(_events[i]))
                    {
                        lowLevelEvent = FormEventFromPeSource_Switch(_events[i]);
                    }

                    break;
                case "RECBIN":
                    lowLevelEvent = this.FormEventFromRecbinSource_Switch(_events[i]);
                    break;
                default:
                    break;
            }

            if (lowLevelEvent is not null)
            {
                lowLevelEvents.Add(lowLevelEvent);
                NormalizeEvents_Switch(lowLevelEvents, _events, _events[i]);
            }
        }

        return lowLevelEvents;
    }

    private static void NormalizeEvents_Switch(List<LowLevelEventViewModel> lowLevelEvents, List<EventViewModel> events, EventViewModel currentEvent)
    {
        if (lowLevelEvents.Count < 2)
        {
            return;
        }

        int eventIndex = GetEventIndex_Switch(events, lowLevelEvents[^2].Reference);
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

    private static int GetEventIndex_Switch(List<EventViewModel> events, int reference)
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

    private bool DoesNeedComposing_Switch(List<EventViewModel> events, int startIndex, EventViewModel current)
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

    private static bool IsWebhistEventValid_Switch(List<LowLevelEventViewModel> lowLevelEvents, LowLevelEventViewModel lowLevelEvent)
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

        if (endTime.Subtract(startTime).TotalMinutes.CompareTo(30.0) >= 0)
        {
            return true;
        }

        return false;
    }

    private LowLevelEventViewModel FormEventFromWebhistSource_Switch(EventViewModel eventViewModel)
    {
        string extraValue = "-";

        if (eventViewModel.SourceType.ToLower().Contains("firefox"))
        {
            extraValue = _lowLevelAbstractorUtils.GetFirefoxExtraFromWebhistSource(eventViewModel.Extra);
        }

        if (_highLevelAbstractorUtils.IsWebhistDownloadEvent(eventViewModel))
        {
            return this.FormDownloadEvent_Switch(eventViewModel, extraValue);
        }
        else if (_highLevelAbstractorUtils.IsWebhistMailEvent(eventViewModel))
        {
            return this.FormMailEvent_Switch(eventViewModel, extraValue);
        }
        else if (_highLevelAbstractorUtils.IsWebhistNamingActivityEvent(eventViewModel))
        {
            return this.FormNamingActivityEvent_Switch(eventViewModel, extraValue);
        }

        return null;
    }

    private LowLevelEventViewModel FormDownloadEvent_Switch(EventViewModel eventViewModel, string extraValue)
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

    private LowLevelEventViewModel FormMailEvent_Switch(EventViewModel eventViewModel, string extraValue)
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

    private LowLevelEventViewModel FormNamingActivityEvent_Switch(EventViewModel eventViewModel, string extraValue)
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

    private int GetValidFileEventIndex_Switch(List<EventViewModel> events, int startIndex)
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
                    return GetIndexOfLastFileEntryShellItemEventOfSameTime_Switch(events, startIndex);
                }
                else
                {
                    return this.GetIndexOfLastFileEventOfSameTime_Switch(events, startIndex);
                }
            }
        }

        return -1;
    }

    private static int GetIndexOfLastFileEntryShellItemEventOfSameTime_Switch(List<EventViewModel> events, int startIndex)
    {
        int lastIndex = FindLastIndexOfSameTimeAndSourceType_Switch(events, startIndex);
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

    private static int FindLastIndexOfSameTimeAndSourceType_Switch(List<EventViewModel> events, int startIndex)
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

    private int GetIndexOfLastFileEventOfSameTime_Switch(List<EventViewModel> events, int startIndex)
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

    private LowLevelEventViewModel FormEventFromFileSource_Switch(EventViewModel eventViewModel)
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

    private LowLevelEventViewModel FormEventFromLnkSource_Switch(EventViewModel eventViewModel)
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

    private LowLevelEventViewModel FormEventFromLogSource_Switch(EventViewModel eventViewModel)
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

    private LowLevelEventViewModel FormEventFromMetaSource_Switch(EventViewModel eventViewModel)
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

    private LowLevelEventViewModel FormEventFromRegSource_Switch(EventViewModel eventViewModel)
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

    private static bool AreOlecfEventsOfSameTime_Switch(EventViewModel firstEvent, EventViewModel secondEvent)
    {
        bool isSameSource = firstEvent.Source == secondEvent.Source;
        bool isSameTime = firstEvent.FullDate.CompareTo(secondEvent.FullDate) == 0;

        return isSameSource && isSameTime;
    }

    private static LowLevelEventViewModel FormEventFromOlecfSource_Switch(EventViewModel eventViewModel)
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

    private static LowLevelEventViewModel FormEventFromPeSource_Switch(EventViewModel eventViewModel)
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

    private LowLevelEventViewModel FormEventFromRecbinSource_Switch(EventViewModel eventViewModel)
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

    [Benchmark]
    public List<ISerializableLevel> FormLowLevelEvents_ChainOfResponsibility()
    {
        List<ISerializableLevel> lowLevelEvents = new(_events.Count);

        for (int i = 0; i < _events.Count; i++)
        {
            if (_events[i].Source == "OLECF")
            {
                while (i < _events.Count - 1 && AreOlecfEventsOfSameTime_ChainOfResponsibility(_events[i], _events[i + 1]))
                {
                    i++;
                }
            }

            ISerializableLevel lowLevelEvent = _handler.FormAbstractEvent(_events, lowLevelEvents, _events[i]);

            if (_events[i].Source == "FILE")
            {
                int validEventIndex = this.GetValidFileEventIndex_ChainOfResponsibility(_events, i);

                if (validEventIndex != -1 && !_lowLevelAbstractorUtils.DoesNeedComposing(_events, i - 1, _events[validEventIndex]))
                {
                    int offset = validEventIndex - i;
                    i += offset;

                    bool isSearchedFormat = _events[i].Format == "lnk/shell_items" || _events[i].Format == "filestat";
                    if (isSearchedFormat)
                    {
                        DateTime date = _events[i].FullDate;

                        while (i < _events.Count - 1 && _events[i].Source == "FILE" && _events[i + 1].FullDate.CompareTo(date) == 0)
                        {
                            i++;
                        }
                    }
                }
            }

            if (lowLevelEvent is not null)
            {
                lowLevelEvents.Add(lowLevelEvent);
                NormalizeEvents_ChainOfResponsibility(lowLevelEvents, _events, _events[i]);
            }
        }

        return lowLevelEvents;
    }

    private static void NormalizeEvents_ChainOfResponsibility(List<ISerializableLevel> lowLevelEvents, List<EventViewModel> events, EventViewModel currentEvent)
    {
        if (lowLevelEvents.Count < 2)
        {
            return;
        }

        int eventIndex = GetEventIndex_ChainOfResponsibility(events, ((LowLevelEventViewModel)lowLevelEvents[^2]).Reference);
        EventViewModel lastEvent = events[eventIndex];

        if (lastEvent.FullDate.CompareTo(currentEvent.FullDate) != 0 || ((LowLevelEventViewModel)lowLevelEvents[^2]).Short != ((LowLevelEventViewModel)lowLevelEvents[^1]).Short)
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

    private static int GetEventIndex_ChainOfResponsibility(List<EventViewModel> events, int reference)
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

    private int GetValidFileEventIndex_ChainOfResponsibility(List<EventViewModel> events, int startIndex)
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
                    return GetIndexOfLastFileEntryShellItemEventOfSameTime_ChainOfResponsibility(events, startIndex);
                }
                else
                {
                    return this.GetIndexOfLastFileEventOfSameTime_ChainOfResponsibility(events, startIndex);
                }
            }
        }

        return -1;
    }

    private static int GetIndexOfLastFileEntryShellItemEventOfSameTime_ChainOfResponsibility(List<EventViewModel> events, int startIndex)
    {
        int lastIndex = FindLastIndexOfSameTimeAndSourceType_ChainOfResponsibility(events, startIndex);
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

    private static int FindLastIndexOfSameTimeAndSourceType_ChainOfResponsibility(List<EventViewModel> events, int startIndex)
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

    private int GetIndexOfLastFileEventOfSameTime_ChainOfResponsibility(List<EventViewModel> events, int startIndex)
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

    private static bool AreOlecfEventsOfSameTime_ChainOfResponsibility(EventViewModel firstEvent, EventViewModel secondEvent)
    {
        bool isSameSource = firstEvent.Source == secondEvent.Source;
        bool isSameTime = firstEvent.FullDate.CompareTo(secondEvent.FullDate) == 0;

        return isSameSource && isSameTime;
    }
}
