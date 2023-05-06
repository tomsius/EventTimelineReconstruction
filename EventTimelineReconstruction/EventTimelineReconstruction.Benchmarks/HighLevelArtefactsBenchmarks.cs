using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using EventTimelineReconstruction.Benchmarks.ChainOfResponsibility;
using EventTimelineReconstruction.Benchmarks.ChainOfResponsibility.HighLevelArtefacts;
using EventTimelineReconstruction.Benchmarks.Models;
using EventTimelineReconstruction.Benchmarks.Utils;

namespace EventTimelineReconstruction.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class HighLevelArtefactsBenchmarks
{
    [Params(1000, 10_000, 100_000, 1_000_000)]
    public int N;

    private List<EventViewModel> _artefacts;
    private IHighLevelEventsAbstractorUtils _highLevelEventsAbstractorUtils;
    private ILowLevelEventsAbstractorUtils _lowLevelEventsAbstractorUtils;
    private IHighLevelArtefactsAbstractorUtils _highLevelArtefactsAbstractorUtils;
    private IHandler _handler;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _highLevelEventsAbstractorUtils = new HighLevelEventsAbstractorUtils();
        _lowLevelEventsAbstractorUtils = new LowLevelEventsAbstractorUtils();
        _highLevelArtefactsAbstractorUtils = new HighLevelArtefactsAbstractorUtils();

        IHighWebhistArtefactHandler webhistHandler = new HighWebhistArtefactHandler(_highLevelEventsAbstractorUtils, _lowLevelEventsAbstractorUtils, _highLevelArtefactsAbstractorUtils);
        IHighLnkArtefactHandler lnkHandler = new HighLnkArtefactHandler(_highLevelEventsAbstractorUtils);
        IHighFileArtefactHandler fileHandler = new HighFileArtefactHandler(_lowLevelEventsAbstractorUtils, _highLevelArtefactsAbstractorUtils);
        IHighLogArtefactHandler logHandler = new HighLogArtefactHandler(_lowLevelEventsAbstractorUtils, _highLevelArtefactsAbstractorUtils);
        IHighRegArtefactHandler regHandler = new HighRegArtefactHandler(_lowLevelEventsAbstractorUtils, _highLevelArtefactsAbstractorUtils);
        IHighMetaArtefactHandler metaHandler = new HighMetaArtefactHandler(_lowLevelEventsAbstractorUtils, _highLevelArtefactsAbstractorUtils);
        IHighOlecfArtefactHandler olecfHandler = new HighOlecfArtefactHandler();
        IHighPeArtefactHandler peHandler = new HighPeArtefactHandler(_highLevelArtefactsAbstractorUtils);

        _handler = webhistHandler;
        webhistHandler.Next = lnkHandler;
        lnkHandler.Next = fileHandler;
        fileHandler.Next = logHandler;
        logHandler.Next = regHandler;
        regHandler.Next = metaHandler;
        metaHandler.Next = olecfHandler;
        olecfHandler.Next = peHandler;

        List<EventViewModel> possibleArtefacts = new()
        {
            new EventViewModel(new EventModel(new DateOnly(2000, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "FILE", "NTFS Creation Time", "Creation Time", "User", "Host", "C:\\Users\\User\\testas0.txt", "C:\\Users\\User\\testas0.txt Type: file", 2, "TSK:\\Users\\User\\testas0.txt", "13452", "-", "filestat", new Dictionary<string, string>() { { "something0", "something0" } }, 0)),
            new EventViewModel(new EventModel(new DateOnly(2000, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "WEBHIST", "Firefox History", "File downloaded", "User", "Host", "C:\\Documents and Settings\\PC1\\My Documents\\Downloads\\Timeline2GUI A Log2Timel...", "https://mail-attachment.googleusercontent.com/attachment/... (C:\\Documents and Settings\\PC1\\My Documents\\Downloads\\Timeline2GUI A Log2Timeline CSV parser and training scenarios.pdf). Received: 1116192 bytes out of: 1116192 bytes.", 2, "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", "13452", "-", "sqlite/firefox_history", new Dictionary<string, string>() { { "extra", "['visited from: https://www.google.com/search?client=firefox-b-ab&q=ekiga&oq=ekiga&aqs=heirloom-srp..0l5 (www.google.com)'  '(URL not typed directly)'  'Transition: LINK']" }, { "schema_match", "False" }, { "sha256_hash", "a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741" }, { "visit_type", "1" } }, 3)),
            new EventViewModel(new EventModel(new DateOnly(2003, 2, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "LOG", "System", "Creation Time", "User", "Host", "SPOOLSV.EXE was run 1 time(s)", "Prefetch [SPOOLSV0.EXE] was executed - run count 1 path: \\WINDOWS\\SYSTEM32\\SPOOLSV0.EXE hash: 0x282F76A7 volume: 1 [serial number: 0x6C91EF9F  device path: \\DEVICE\\HARDDISKVOLUME0]", 2, "TSK:/WINDOWS/Prefetch/SPOOLSV0.EXE-282F76A7.pf", "13452", "-", "prefetch", new Dictionary<string, string>(), 7)),
            new EventViewModel(new EventModel(new DateOnly(2008, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, ".A..", "REG", "Registry Key: UserAssist", "Last Time Executed", "User", "Host", "UEME_RUNPATH:::{645FF040-5081-101B-9F08-00AA002F954E} Count: 1", "[HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\UserAssist\\{75048700-EF1F-11D0-9888-006097DEACF9}\\Count] Value name: UEME_RUNPATH:::{645FF040-5081-101B-9F08-00AA002F954E}\\test14.exe Count: 1", 2, "TSK:/Documents and Settings/PC1/NTUSER.DAT", "13452", "-", "winreg/userassist", new Dictionary<string, string>() { { "something14", "something14" } }, 14)),
            new EventViewModel(new EventModel(new DateOnly(2011, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "META", "System", "Creation Time", "User", "Host", "Short Description", "Full Description21", 2, "testas.docx", "13452", "-", "Format", new Dictionary<string, string>() { { "number_of_paragraphs", "3" }, { "total_time", "1" } }, 21)),
            new EventViewModel(new EventModel(new DateOnly(2012, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "PE", "PE Compilation time", "Creation Time", "User", "Host", "pe_type", "Something22", 2, "test.txt", "13452", "-", "pe", new Dictionary<string, string>(), 22)),
            new EventViewModel(new EventModel(new DateOnly(2015, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "OLECF", "OLECF Item", "Creation Time", "User", "Host", "Name: data", "Something something something something something something something something 30", 2, "TSK:/WINDOWS/system32/wmimgmt.msc", "13452", "-", "olecf/olecf_default", new Dictionary<string, string>(), 30)),
            new EventViewModel(new EventModel(new DateOnly(2016, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B..M", "LNK", "Windows Shortcut", "Creation Time", "User", "Host", "[Empty description] C:\\Program Files\\Mozilla Firefox\\firefox.exe", "[Empty description] C:\\Program Files\\Mozilla Firefox\\firefox.exe", 2, "TSK:/Documents and Settings/All Users/Start Menu/Programs/Mozilla Firefox.lnk", "13451", "-", "lnk", new Dictionary<string, string>(), 31))
        };

        _artefacts = new(N);
        for (int i = 0; i < N; i++)
        {
            _artefacts.Add(possibleArtefacts[i % possibleArtefacts.Count]);
        }
    }

    [Benchmark(Baseline = true)]
    public List<HighLevelArtefactViewModel> FormHighLevelArtefacts_Switch()
    {
        List<HighLevelArtefactViewModel> highLevelArtefacts = new(_artefacts.Count);

        for (int i = 0; i < _artefacts.Count; i++)
        {
            HighLevelArtefactViewModel highLevelArtefact = null;

            switch (_artefacts[i].Source)
            {
                case "WEBHIST":
                    if (_highLevelEventsAbstractorUtils.IsValidWebhistLine(_artefacts[i]))
                    {
                        highLevelArtefact = this.FormEventFromWebhistSource_Switch(_artefacts[i]);

                        if (!IsWebhistEventValid_Switch(highLevelArtefacts, highLevelArtefact))
                        {
                            highLevelArtefact = null;
                        }
                    }

                    break;
                case "LNK":
                    highLevelArtefact = this.FormEventFromLnkSource_Switch(_artefacts[i]);

                    if (!IsLnkEventValid_Switch(highLevelArtefacts, highLevelArtefact))
                    {
                        highLevelArtefact = null;
                    }

                    break;
                case "FILE":
                    if (i == 0)
                    {
                        highLevelArtefact = this.FormEventFromFileSource_Switch(_artefacts[i]);
                        break;
                    }

                    if (!_highLevelArtefactsAbstractorUtils.IsFileDuplicateOfLnk(_artefacts, i - 1, _artefacts[i]))
                    {
                        int fileCountInRowAtSameMinute = _highLevelArtefactsAbstractorUtils.GetFileCountInRowAtSameMinute(_artefacts, i);
                        int endIndex = i + fileCountInRowAtSameMinute;

                        if (fileCountInRowAtSameMinute <= 36)
                        {
                            for (int index = i; index < endIndex; index++)
                            {
                                HighLevelArtefactViewModel artefact = this.FormEventFromFileSource_Switch(_artefacts[i]);
                                // single artefact per second
                                if (IsFileEventValid_Switch(highLevelArtefacts, artefact))
                                {
                                    highLevelArtefacts.Add(artefact);
                                }
                            }
                        }
                        else
                        {
                            List<int> validIndices = _highLevelArtefactsAbstractorUtils.GetValidFileEventIndices(_artefacts, i, endIndex);
                            for (int index = 0; index < validIndices.Count; index++)
                            {
                                HighLevelArtefactViewModel artefact = this.FormEventFromFileSource_Switch(_artefacts[validIndices[index]]);
                                highLevelArtefacts.Add(artefact);
                            }
                        }

                        i += fileCountInRowAtSameMinute - 1;
                    }

                    break;
                case "LOG":
                    highLevelArtefact = this.FormEventFromLogSource_Switch(_artefacts[i]);

                    if (!this.IsLogEventValid_Switch(_artefacts, i))
                    {
                        highLevelArtefact = null;
                    }

                    break;
                case "REG":
                    highLevelArtefact = this.FormEventFromRegSource_Switch(_artefacts[i]);

                    if (!IsRegEventValid_Switch(highLevelArtefacts, highLevelArtefact))
                    {
                        highLevelArtefact = null;
                    }

                    break;
                case "META":
                    highLevelArtefact = this.FormEventFromMetaSource_Switch(_artefacts[i]);
                    break;
                case "OLECF":
                    // Choose the last line of the same OLECF time - skip same time OLECF type events
                    while (i < _artefacts.Count - 1 && AreOlecfEventsOfSameTime_Switch(_artefacts[i], _artefacts[i + 1]))
                    {
                        i++;
                    }

                    highLevelArtefact = FormEventFromOlecfSource_Switch(_artefacts[i]);
                    break;
                case "PE":
                    if (this.IsPeEventValid_Switch(highLevelArtefacts, _artefacts[i]))
                    {
                        highLevelArtefact = FormEventFromPeSource_Switch(_artefacts[i]);
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

    [Benchmark]
    public List<ISerializableLevel> FormHighLevelArtefacts_ChainOfResponsibility()
    {
        List<ISerializableLevel> highLevelArtefacts = new(_artefacts.Count);

        for (int i = 0; i < _artefacts.Count; i++)
        {
            if (_artefacts[i].Source == "OLECF")
            {
                while (i < _artefacts.Count - 1 && AreOlecfEventsOfSameTime_ChainOfResponsibility(_artefacts[i], _artefacts[i + 1]))
                {
                    i++;
                }
            }

            ISerializableLevel highLevelArtefact = _handler.FormAbstractEvent(_artefacts, highLevelArtefacts, _artefacts[i]);

            if (_artefacts[i].Source == "FILE")
            {
                if (!_highLevelArtefactsAbstractorUtils.IsFileDuplicateOfLnk(_artefacts, i - 1, _artefacts[i]))
                {
                    int fileCountInRowAtSameMinute = _highLevelArtefactsAbstractorUtils.GetFileCountInRowAtSameMinute(_artefacts, i);
                    i += fileCountInRowAtSameMinute - 1;
                }
            }

            if (IsEventValid_ChainOfResponsibility(highLevelArtefacts, (HighLevelArtefactViewModel)highLevelArtefact))
            {
                highLevelArtefacts.Add(highLevelArtefact);
            }
        }

        return highLevelArtefacts;
    }

    private static bool AreOlecfEventsOfSameTime_ChainOfResponsibility(EventViewModel firstEvent, EventViewModel secondEvent)
    {
        bool isSameSource = firstEvent.Source == secondEvent.Source;
        bool isSameTime = firstEvent.FullDate.CompareTo(secondEvent.FullDate) == 0;

        return isSameSource && isSameTime;
    }

    private static bool IsEventValid_ChainOfResponsibility(List<ISerializableLevel> highLevelArtefacts, HighLevelArtefactViewModel current)
    {
        if (current is null)
        {
            return false;
        }

        if (highLevelArtefacts.Count == 0)
        {
            return true;
        }

        HighLevelArtefactViewModel previous = (HighLevelArtefactViewModel)highLevelArtefacts[^1];

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
