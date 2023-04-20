using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using EventTimelineReconstruction.Benchmarks.Models;
using EventTimelineReconstruction.Benchmarks.Utils;

namespace EventTimelineReconstruction.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class HighLevelEventsBenchmarks
{
    [Params(1000, 100_000, 1_000_000)]
    public int N;

    private List<EventViewModel> _events;
    private IHighLevelEventsAbstractorUtils _abstractorUtils;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _abstractorUtils = new HighLevelEventsAbstractorUtils();

        // sukurti ivykiu sarasa
        _events = new(N);
    }

    [Benchmark(Baseline = true)]
    public List<HighLevelEventViewModel> FormHighLevelEvents_Switch()
    {
        List<HighLevelEventViewModel> highLevelEvents = new(_events.Count);

        for (int i = 0; i < _events.Count; i++)
        {
            if (_events[i].MACB.Contains('B'))
            {
                switch (_events[i].Source)
                {
                    case "LOG":
                        HighLevelEventViewModel logEvent = this.FormEventFromLogSource_Switch(_events[i]);
                        int lastLogEventIndex = FindLastEventIndexOf_Switch(highLevelEvents, _events[i].FullDate, "LOG");

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
                        HighLevelEventViewModel lnkEvent = this.FormEventFromLnkSource_Switch(_events[i]);
                        int lastLnkEventIndex = FindLastEventIndexOf_Switch(highLevelEvents, _events[i].FullDate, "LNK");

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
                        int lastMetaEventIndex = FindLastEventIndexOf_Switch(highLevelEvents, _events[i].FullDate, "META");

                        if (lastMetaEventIndex == -1)
                        {
                            HighLevelEventViewModel metaEvent = FormEventFromMetaSource_Switch(_events[i]);
                            highLevelEvents.Add(metaEvent);
                        }

                        break;
                    case "OLECF":
                        HighLevelEventViewModel olecfEvent = FormEventFromOlecfSource_Switch(_events[i]);

                        if (IsOlecfEventValid_Switch(highLevelEvents[^1], olecfEvent))
                        {
                            highLevelEvents.Add(olecfEvent);
                        }

                        break;
                    case "PE":
                        int lastPeEventIndex = FindLastEventIndexOf_Switch(highLevelEvents, _events[i].FullDate, "PE");

                        if (lastPeEventIndex == -1 && _abstractorUtils.IsValidPeEvent(_events[i]))
                        {
                            HighLevelEventViewModel peEvent = FormEventFromPeSource_Switch(_events[i]);
                            highLevelEvents.Add(peEvent);
                        }

                        break;
                    default:
                        break;
                }
            }

            if (_events[i].Source == "WEBHIST" && _abstractorUtils.IsValidWebhistLine(_events[i]))
            {
                HighLevelEventViewModel webhistEvent = this.FormEventFromWebhistSource_Switch(_events[i]);

                if (webhistEvent is not null && IsWebhistEventValid_Switch(highLevelEvents, webhistEvent))
                {
                    highLevelEvents.Add(webhistEvent);
                }
            }
        }

        return highLevelEvents;
    }

    private HighLevelEventViewModel FormEventFromLogSource_Switch(EventViewModel eventViewModel)
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

    private static int FindLastEventIndexOf_Switch(List<HighLevelEventViewModel> highLevelEvents, DateTime newEventDate, string source)
    {
        for (int i = highLevelEvents.Count - 1; i >= 0; i--)
        {
            DateTime highLevelEventDate = new(highLevelEvents[i].Date.Year, highLevelEvents[i].Date.Month, highLevelEvents[i].Date.Day, highLevelEvents[i].Time.Hour, highLevelEvents[i].Time.Minute, highLevelEvents[i].Time.Second);

            if (highLevelEventDate.CompareTo(newEventDate) == -1)
            {
                break;
            }

            if (highLevelEvents[i].Source == source && highLevelEventDate.CompareTo(newEventDate) == 0)
            {
                return i;
            }
        }

        return -1;
    }

    private HighLevelEventViewModel FormEventFromLnkSource_Switch(EventViewModel eventViewModel)
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

    private static HighLevelEventViewModel FormEventFromMetaSource_Switch(EventViewModel eventViewModel)
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

    private static HighLevelEventViewModel FormEventFromOlecfSource_Switch(EventViewModel eventViewModel)
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

    private static bool IsOlecfEventValid_Switch(HighLevelEventViewModel lastHighLevelEvent, HighLevelEventViewModel newOlecfEvent)
    {
        bool isSameSource = lastHighLevelEvent.Source == newOlecfEvent.Source;
        bool isSameShortValue = lastHighLevelEvent.Short == newOlecfEvent.Short;

        return isSameSource == false || isSameShortValue == false;
    }

    private static HighLevelEventViewModel FormEventFromPeSource_Switch(EventViewModel eventViewModel)
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

    private HighLevelEventViewModel FormEventFromWebhistSource_Switch(EventViewModel eventViewModel)
    {
        if (_abstractorUtils.IsWebhistDownloadEvent(eventViewModel))
        {
            return this.FormDownloadEvent_Switch(eventViewModel);
        }
        else if (_abstractorUtils.IsWebhistMailEvent(eventViewModel))
        {
            return this.FormMailEvent_Switch(eventViewModel);
        }
        else if (_abstractorUtils.IsWebhistNamingActivityEvent(eventViewModel))
        {
            return this.FormNamingActivityEvent_Switch(eventViewModel);
        }

        return null;
    }

    private HighLevelEventViewModel FormDownloadEvent_Switch(EventViewModel eventViewModel)
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

    private HighLevelEventViewModel FormMailEvent_Switch(EventViewModel eventViewModel)
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

    private HighLevelEventViewModel FormNamingActivityEvent_Switch(EventViewModel eventViewModel)
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

    private static bool IsWebhistEventValid_Switch(List<HighLevelEventViewModel> highLevelEvents, HighLevelEventViewModel webhistEvent)
    {
        if (highLevelEvents.Count < 2)
        {
            return true;
        }

        if (highLevelEvents[^1].Source != "WEBHIST")
        {
            return true;
        }

        if (highLevelEvents[^1].Short != webhistEvent.Short)
        {
            return true;
        }

        HighLevelEventViewModel startEvent = highLevelEvents[^1];
        DateTime startTime = new(startEvent.Date.Year, startEvent.Date.Month, startEvent.Date.Day, startEvent.Time.Hour, startEvent.Time.Minute, startEvent.Time.Second);
        DateTime endTime = new(webhistEvent.Date.Year, webhistEvent.Date.Month, webhistEvent.Date.Day, webhistEvent.Time.Hour, webhistEvent.Time.Minute, webhistEvent.Time.Second);

        if (endTime.Subtract(startTime).TotalMinutes.CompareTo(50.0) >= 0)
        {
            return true;
        }

        return false;
    }
}
