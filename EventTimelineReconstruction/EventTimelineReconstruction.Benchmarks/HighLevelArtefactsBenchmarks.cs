using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using EventTimelineReconstruction.Benchmarks.Models;
using EventTimelineReconstruction.Benchmarks.Utils;

namespace EventTimelineReconstruction.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class HighLevelArtefactsBenchmarks
{
    [Params(1000, 100_000, 1_000_000)]
    public int N;

    private List<EventViewModel> _events;
    private HighLevelEventsAbstractorUtils _highLevelEventsAbstractorUtils;
    private LowLevelEventsAbstractorUtils _lowLevelEventsAbstractorUtils;
    private HighLevelArtefactsAbstractorUtils _highLevelArtefactsAbstractorUtils;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _highLevelEventsAbstractorUtils = new();
        _lowLevelEventsAbstractorUtils = new();
        _highLevelArtefactsAbstractorUtils = new();

        // sukurti ivykiu sarasa
        _events = new(N);
    }

    [Benchmark(Baseline = true)]
    public List<HighLevelArtefactViewModel> FormHighLevelArtefacts_Switch()
    {
        List<HighLevelArtefactViewModel> highLevelArtefacts = new(_events.Count);

        for (int i = 0; i < _events.Count; i++)
        {
            HighLevelArtefactViewModel highLevelArtefact = null;

            switch (_events[i].Source)
            {
                case "WEBHIST":
                    if (_highLevelEventsAbstractorUtils.IsValidWebhistLine(_events[i]))
                    {
                        highLevelArtefact = this.FormEventFromWebhistSource_Switch(_events[i]);

                        if (!IsWebhistEventValid_Switch(highLevelArtefacts, highLevelArtefact))
                        {
                            highLevelArtefact = null;
                        }
                    }

                    break;
                case "LNK":
                    highLevelArtefact = this.FormEventFromLnkSource_Switch(_events[i]);

                    if (!IsLnkEventValid_Switch(highLevelArtefacts, highLevelArtefact))
                    {
                        highLevelArtefact = null;
                    }

                    break;
                case "FILE":
                    if (i == 0)
                    {
                        highLevelArtefact = this.FormEventFromFileSource_Switch(_events[i]);
                        break;
                    }

                    if (!_highLevelArtefactsAbstractorUtils.IsFileDuplicateOfLnk(_events, i - 1, _events[i]))
                    {
                        int fileCountInRowAtSameMinute = _highLevelArtefactsAbstractorUtils.GetFileCountInRowAtSameMinute(_events, i);
                        int endIndex = i + fileCountInRowAtSameMinute;

                        if (fileCountInRowAtSameMinute <= 36)
                        {
                            for (int index = i; index < endIndex; index++)
                            {
                                HighLevelArtefactViewModel artefact = this.FormEventFromFileSource_Switch(_events[i]);
                                // single artefact per second
                                if (IsFileEventValid_Switch(highLevelArtefacts, artefact))
                                {
                                    highLevelArtefacts.Add(artefact);
                                }
                            }
                        }
                        else
                        {
                            List<int> validIndices = _highLevelArtefactsAbstractorUtils.GetValidFileEventIndices(_events, i, endIndex);
                            for (int index = 0; index < validIndices.Count; index++)
                            {
                                HighLevelArtefactViewModel artefact = this.FormEventFromFileSource_Switch(_events[validIndices[index]]);
                                highLevelArtefacts.Add(artefact);
                            }
                        }

                        i += fileCountInRowAtSameMinute - 1;
                    }

                    break;
                case "LOG":
                    highLevelArtefact = this.FormEventFromLogSource_Switch(_events[i]);

                    if (!this.IsLogEventValid_Switch(_events, i))
                    {
                        highLevelArtefact = null;
                    }

                    break;
                case "REG":
                    highLevelArtefact = this.FormEventFromRegSource_Switch(_events[i]);

                    if (!IsRegEventValid_Switch(highLevelArtefacts, highLevelArtefact))
                    {
                        highLevelArtefact = null;
                    }

                    break;
                case "META":
                    highLevelArtefact = this.FormEventFromMetaSource_Switch(_events[i]);
                    break;
                case "OLECF":
                    // Choose the last line of the same OLECF time - skip same time OLECF type events
                    while (i < _events.Count - 1 && AreOlecfEventsOfSameTime_Switch(_events[i], _events[i + 1]))
                    {
                        i++;
                    }

                    highLevelArtefact = FormEventFromOlecfSource_Switch(_events[i]);
                    break;
                case "PE":
                    if (this.IsPeEventValid_Switch(highLevelArtefacts, _events[i]))
                    {
                        highLevelArtefact = FormEventFromPeSource_Switch(_events[i]);
                    }

                    break;
                default:
                    break;
            }

            if (IsEventValid_Switch(highLevelArtefacts, highLevelArtefact))
            {
                highLevelArtefacts.Add(highLevelArtefact);
            }
        }

        return highLevelArtefacts;
    }

    private HighLevelArtefactViewModel FormEventFromWebhistSource_Switch(EventViewModel eventViewModel)
    {
        string extraValue;

        if (eventViewModel.SourceType.ToLower().Contains("firefox"))
        {
            extraValue = _lowLevelEventsAbstractorUtils.GetFirefoxExtraFromWebhistSource(eventViewModel.Extra);
        }
        else
        {
            extraValue = _highLevelArtefactsAbstractorUtils.GetOtherBrowserExtraFromWebhistSource(eventViewModel.Extra);
        }

        if (_highLevelEventsAbstractorUtils.IsWebhistDownloadEvent(eventViewModel))
        {
            return this.FormDownloadEvent_Switch(eventViewModel, extraValue);
        }
        else if (_highLevelEventsAbstractorUtils.IsWebhistMailEvent(eventViewModel))
        {
            return this.FormMailEvent_Switch(eventViewModel, extraValue);
        }
        else if (_highLevelEventsAbstractorUtils.IsWebhistNamingActivityEvent(eventViewModel))
        {
            return this.FormNamingActivityEvent_Switch(eventViewModel, extraValue);
        }

        return null;
    }

    private HighLevelArtefactViewModel FormDownloadEvent_Switch(EventViewModel eventViewModel, string extraValue)
    {
        string downloadedFileName = _highLevelEventsAbstractorUtils.GetDownloadedFileName(eventViewModel.Description);

        HighLevelArtefactViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = downloadedFileName,
            Extra = extraValue,
            Reference = eventViewModel.SourceLine,
            Visit = "Download",
            Macb = eventViewModel.MACB,
            SourceType = eventViewModel.SourceType,
            Description = eventViewModel.Description
        };

        return result;
    }

    private HighLevelArtefactViewModel FormMailEvent_Switch(EventViewModel eventViewModel, string extraValue)
    {
        string shortValue = _highLevelEventsAbstractorUtils.GetMailUrl(eventViewModel.Description);
        string newExtraValue = _lowLevelEventsAbstractorUtils.AddMailUser(extraValue, eventViewModel.Description);

        HighLevelArtefactViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = shortValue,
            Extra = newExtraValue,
            Reference = eventViewModel.SourceLine,
            Visit = "Mail",
            Macb = eventViewModel.MACB,
            SourceType = eventViewModel.SourceType,
            Description = eventViewModel.Description
        };

        return result;
    }

    private HighLevelArtefactViewModel FormNamingActivityEvent_Switch(EventViewModel eventViewModel, string extraValue)
    {
        string formattedUrl = _lowLevelEventsAbstractorUtils.GetUrl(eventViewModel.Short);
        string visitValue = _highLevelEventsAbstractorUtils.GenerateVisitValue(eventViewModel.Description);

        HighLevelArtefactViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = formattedUrl,
            Extra = extraValue,
            Reference = eventViewModel.SourceLine,
            Visit = visitValue,
            Macb = eventViewModel.MACB,
            SourceType = eventViewModel.SourceType,
            Description = eventViewModel.Description
        };

        return result;
    }

    private static bool IsWebhistEventValid_Switch(List<HighLevelArtefactViewModel> highLevelArtefacts, HighLevelArtefactViewModel current)
    {
        if (current is null)
        {
            return false;
        }

        for (int i = highLevelArtefacts.Count - 1; i >= 0; i--)
        {
            HighLevelArtefactViewModel previous = highLevelArtefacts[i];
            DateTime previousTime = new(previous.Date.Year, previous.Date.Month, previous.Date.Day, previous.Time.Hour, previous.Time.Minute, previous.Time.Second);
            DateTime currentTime = new(current.Date.Year, current.Date.Month, current.Date.Day, current.Time.Hour, current.Time.Minute, current.Time.Second);
            if (previousTime.CompareTo(currentTime) != 0)
            {
                break;
            }

            if (previous.Source == "WEBHIST")
            {
                return false;
            }
        }

        return true;
    }

    private HighLevelArtefactViewModel FormEventFromLnkSource_Switch(EventViewModel eventViewModel)
    {
        string shortValue = _highLevelEventsAbstractorUtils.GetShort(eventViewModel.Description);

        HighLevelArtefactViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = shortValue,
            Reference = eventViewModel.SourceLine,
            Macb = eventViewModel.MACB,
            SourceType = eventViewModel.SourceType
        };

        return result;
    }

    private static bool IsLnkEventValid_Switch(List<HighLevelArtefactViewModel> highLevelArtefacts, HighLevelArtefactViewModel current)
    {
        for (int i = highLevelArtefacts.Count - 1; i >= 0; i--)
        {
            HighLevelArtefactViewModel previous = highLevelArtefacts[i];

            if (previous.Short == current.Short && previous.Source == "LNK")
            {
                return false;
            }
        }

        return true;
    }

    private HighLevelArtefactViewModel FormEventFromFileSource_Switch(EventViewModel eventViewModel)
    {
        string shortValue = _highLevelArtefactsAbstractorUtils.GetFilename(eventViewModel.Description);
        string extraValue = _lowLevelEventsAbstractorUtils.GetExtraTillSha256(eventViewModel.Extra);

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
            Description = eventViewModel.Description
        };

        return result;
    }

    private static bool IsFileEventValid_Switch(List<HighLevelArtefactViewModel> highLevelArtefacts, HighLevelArtefactViewModel current)
    {
        if (highLevelArtefacts.Count == 0)
        {
            return true;
        }

        HighLevelArtefactViewModel previous = highLevelArtefacts[^1];
        DateTime previousTime = new(previous.Date.Year, previous.Date.Month, previous.Date.Day, previous.Time.Hour, previous.Time.Minute, previous.Time.Second);
        DateTime currentTime = new(current.Date.Year, current.Date.Month, current.Date.Day, current.Time.Hour, current.Time.Minute, current.Time.Second);

        return previousTime.CompareTo(currentTime) != 0;
    }

    private HighLevelArtefactViewModel FormEventFromLogSource_Switch(EventViewModel eventViewModel)
    {
        string shortValue = _lowLevelEventsAbstractorUtils.GetShort(eventViewModel.Description);
        string descriptionValue = _highLevelArtefactsAbstractorUtils.GetDescriptionFromLogSource(eventViewModel);

        HighLevelArtefactViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = shortValue,
            Reference = eventViewModel.SourceLine,
            Macb = eventViewModel.MACB,
            SourceType = eventViewModel.SourceType,
            Description = descriptionValue
        };

        return result;
    }

    private bool IsLogEventValid_Switch(List<EventViewModel> events, int startIndex)
    {
        EventViewModel current = events[startIndex];

        if (current.Format == "lnk")
        {
            return true;
        }

        if (!current.Short.StartsWith("Entry"))
        {
            return IsSingleLineOfSameTimeWithoutEntry_Switch(events, startIndex - 1, current.FullDate);
        }

        bool isSameTimeAsReg = DoesRegOfSameTimeExist_Switch(events, startIndex - 1, current.FullDate);

        if (isSameTimeAsReg)
        {
            return false;
        }

        return !this.IsDuplicateByShort_Switch(events, startIndex - 1, current);
    }

    private static bool IsSingleLineOfSameTimeWithoutEntry_Switch(List<EventViewModel> events, int startIndex, DateTime currentDate)
    {
        for (int i = startIndex; i >= 0; i--)
        {
            EventViewModel previous = events[i];
            if (previous.FullDate.CompareTo(currentDate) != 0)
            {
                break;
            }

            if (previous.Source == "LOG")
            {
                return false;
            }
        }

        return true;
    }

    private static bool DoesRegOfSameTimeExist_Switch(List<EventViewModel> events, int startIndex, DateTime currentDate)
    {
        for (int i = startIndex; i >= 0; i--)
        {
            EventViewModel previous = events[i];
            if (previous.FullDate.CompareTo(currentDate) != 0)
            {
                break;
            }

            if (previous.Source == "REG" && previous.FullDate.CompareTo(currentDate) == 0)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsDuplicateByShort_Switch(List<EventViewModel> events, int startIndex, EventViewModel current)
    {
        string currentFilename = _highLevelArtefactsAbstractorUtils.GetDescriptionFromLogSource(current);

        for (int i = startIndex; i >= 0; i--)
        {
            if (events[i].Source == "LOG")
            {
                string previousFilename = _highLevelArtefactsAbstractorUtils.GetDescriptionFromLogSource(events[i]);

                if (currentFilename == previousFilename)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private HighLevelArtefactViewModel FormEventFromRegSource_Switch(EventViewModel eventViewModel)
    {
        string shortValue = _lowLevelEventsAbstractorUtils.GetSummaryFromShort(eventViewModel.Description);
        string extraValue = _lowLevelEventsAbstractorUtils.GetExtraTillSha256(eventViewModel.Extra);
        string descriptionValue = _highLevelArtefactsAbstractorUtils.GetDescriptionFromRegSource(eventViewModel.SourceType, eventViewModel.Description);

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

    private static bool IsRegEventValid_Switch(List<HighLevelArtefactViewModel> highLevelArtefacts, HighLevelArtefactViewModel current)
    {
        if (current.SourceType == "Registry Key: UserAssist" && !current.Description.Contains(".exe"))
        {
            return false;
        }

        if (current.SourceType == "Registry Key : BagMRU" && current.Description.Length == 0)
        {
            return false;
        }

        if (current.SourceType == "Registry Key : Run Key" || (current.SourceType == "UNKNOWN" && !current.Description.Contains('.')))
        {
            for (int i = highLevelArtefacts.Count - 1; i >= 0; i--)
            {
                HighLevelArtefactViewModel previous = highLevelArtefacts[i];
                DateTime previousTime = new(previous.Date.Year, previous.Date.Month, previous.Date.Day, previous.Time.Hour, previous.Time.Minute, previous.Time.Second);
                DateTime currentTime = new(current.Date.Year, current.Date.Month, current.Date.Day, current.Time.Hour, current.Time.Minute, current.Time.Second);

                if (previousTime.CompareTo(currentTime) != 0)
                {
                    break;
                }

                if (previous.SourceType == current.SourceType)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private HighLevelArtefactViewModel FormEventFromMetaSource_Switch(EventViewModel eventViewModel)
    {
        string shortValue = eventViewModel.Filename;
        string extraValue = _lowLevelEventsAbstractorUtils.GetKeywordsFromExtra(eventViewModel.Extra, eventViewModel.Short);
        string descriptionValue = _highLevelArtefactsAbstractorUtils.GetDescriptionFromMetaSource(eventViewModel.Description);

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

    private static bool AreOlecfEventsOfSameTime_Switch(EventViewModel firstEvent, EventViewModel secondEvent)
    {
        bool isSameSource = firstEvent.Source == secondEvent.Source;
        bool isSameTime = firstEvent.FullDate.CompareTo(secondEvent.FullDate) == 0;

        return isSameSource && isSameTime;
    }

    private static HighLevelArtefactViewModel FormEventFromOlecfSource_Switch(EventViewModel eventViewModel)
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

    private bool IsPeEventValid_Switch(List<HighLevelArtefactViewModel> highLevelArtefacts, EventViewModel current)
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
            HighLevelArtefactViewModel previous = highLevelArtefacts[i];
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

    private static HighLevelArtefactViewModel FormEventFromPeSource_Switch(EventViewModel eventViewModel)
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

    private static bool IsEventValid_Switch(List<HighLevelArtefactViewModel> highLevelArtefacts, HighLevelArtefactViewModel current)
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

        return timeDifference.CompareTo(15.0) >= 0;
    }
}
