using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using EventTimelineReconstruction.Benchmarks.ChainOfResponsibility;
using EventTimelineReconstruction.Benchmarks.ChainOfResponsibility.LowLevelArtefacts;
using EventTimelineReconstruction.Benchmarks.Models;
using EventTimelineReconstruction.Benchmarks.Utils;

namespace EventTimelineReconstruction.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class LowLevelArtefactsBenchmarks
{
    [Params(1000, 10_000, 100_000, 1_000_000)]
    public int N;

    private int LinesSkipped;
    private int LinesNeglected;
    private List<EventViewModel> _artefacts;
    private ILowLevelArtefactsAbstractorUtils _lowLevelArtefactsAbstractorUtils;
    private IHandler _handler;

    [GlobalSetup]
    public void GlobalSetup()
    {
        LinesSkipped = 0;
        LinesNeglected = 0;
        _lowLevelArtefactsAbstractorUtils = new LowLevelArtefactsAbstractorUtils();

        ILowWebhistArtefactHandler webhistHandler = new LowWebhistArtefactHandler(_lowLevelArtefactsAbstractorUtils);
        ILowLnkArtefactHandler lnkHandler = new LowLnkArtefactHandler(_lowLevelArtefactsAbstractorUtils);
        ILowFileArtefactHandler fileHandler = new LowFileArtefactHandler(_lowLevelArtefactsAbstractorUtils);

        _handler = webhistHandler;
        webhistHandler.Next = lnkHandler;
        lnkHandler.Next = fileHandler;

        List<EventViewModel> possibleArtefacts = new()
        {
            new EventViewModel(new EventModel(new DateOnly(2000, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "WEBHIST", "Firefox History", "File downloaded", "User", "Host", "C:\\Documents and Settings\\PC1\\My Documents\\Downloads\\Timeline2GUI A Log2Timel...", "https://mail-attachment.googleusercontent.com/attachment/... (C:\\Documents and Settings\\PC1\\My Documents\\Downloads\\Timeline2GUI A Log2Timeline CSV parser and training scenarios.pdf). Received: 1116192 bytes out of: 1116192 bytes.", 2, "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", "13452", "-", "sqlite/firefox_history", new Dictionary<string, string>() { { "extra", "['visited from: https://www.google.com/search?client=firefox-b-ab&q=ekiga&oq=ekiga&aqs=heirloom-srp..0l5 (www.google.com)'  '(URL not typed directly)'  'Transition: LINK']" }, { "schema_match", "False" }, { "sha256_hash", "a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741" }, { "visit_type", "1" } }, 2)),
            new EventViewModel(new EventModel(new DateOnly(2002, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B..M", "LNK", "Windows Shortcut", "Creation Time", "User", "Host", "[Empty description] C:\\Program Files\\Mozilla Firefox\\firefox.exe", "[Empty description] C:\\Program Files\\Mozilla Firefox\\firefox.exe", 2, "TSK:/Documents and Settings/All Users/Start Menu/Programs/Mozilla Firefox.lnk", "13451", "-", "lnk", new Dictionary<string, string>() { { "Something6", "Something6" } }, 6)),
            new EventViewModel(new EventModel(new DateOnly(2003, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "FILE", "NTFS Creation Time", "Creation Time", "User", "Host", "C:\\Users\\User\\testas.txt", "C:\\Users\\User\\testas.txt Type: file", 2, "TSK:\\Users\\User\\testas.txt", "13452", "-", "filestat", new Dictionary<string, string>() { { "Something8", "Something8" } }, 8))
        };

        _artefacts = new(N);
        for (int i = 0; i < N; i++)
        {
            _artefacts.Add(possibleArtefacts[i % possibleArtefacts.Count]);
        }
    }

    [Benchmark(Baseline = true)]
    public List<LowLevelArtefactViewModel> FormLowLevelArtefacts_Switch()
    {
        List<LowLevelArtefactViewModel> lowLevelArtefacts = new(_artefacts.Count);

        for (int i = 0; i < _artefacts.Count; i++)
        {
            LowLevelArtefactViewModel lowLevelArtefact = null;

            switch (_artefacts[i].Source)
            {
                case "WEBHIST":
                    if (_lowLevelArtefactsAbstractorUtils.IsValidWebhistLine(_artefacts[i].SourceType, _artefacts[i].Type))
                    {
                        lowLevelArtefact = this.FormEvent_Switch(_artefacts[i]);

                        if (lowLevelArtefact.SourceType.ToLower().Contains("cookies"))
                        {
                            lowLevelArtefact = this.NormalizeCookie_Switch(lowLevelArtefacts, lowLevelArtefact);
                        }
                    }

                    break;
                case "LNK":
                    lowLevelArtefact = this.FormEvent_Switch(_artefacts[i]);
                    break;
                case "FILE":
                    lowLevelArtefact = this.FormEvent_Switch(_artefacts[i]);
                    int needsSkipping = this.SkipFileEvents_Switch(_artefacts, i, 1.0);

                    if (!IsFileEventValid_Switch(lowLevelArtefacts, lowLevelArtefact))
                    {
                        lowLevelArtefact = null;
                    }

                    i += needsSkipping;
                    break;
                default:
                    break;
            }

            if (lowLevelArtefact is not null)
            {
                lowLevelArtefacts.Add(lowLevelArtefact);
            }
        }

        return lowLevelArtefacts;
    }

    private LowLevelArtefactViewModel FormEvent_Switch(EventViewModel eventViewModel)
    {
        string extraValue = _lowLevelArtefactsAbstractorUtils.GetExtraValue(eventViewModel.Extra);

        LowLevelArtefactViewModel result = new(
            DateOnly.FromDateTime(eventViewModel.FullDate),
            TimeOnly.FromDateTime(eventViewModel.FullDate),
            eventViewModel.Timezone.DisplayName,
            eventViewModel.MACB,
            eventViewModel.Source,
            eventViewModel.SourceType,
            eventViewModel.Type,
            eventViewModel.User,
            eventViewModel.Host,
            eventViewModel.Short,
            eventViewModel.Description,
            eventViewModel.Version.ToString(),
            eventViewModel.Filename,
            eventViewModel.INode,
            eventViewModel.Notes,
            eventViewModel.Format,
            extraValue,
            eventViewModel.SourceLine
        );

        return result;
    }

    private LowLevelArtefactViewModel NormalizeCookie_Switch(List<LowLevelArtefactViewModel> lowLevelArtefacts, LowLevelArtefactViewModel current)
    {
        string currentAddress = _lowLevelArtefactsAbstractorUtils.GetAddress(current.Description);

        for (int i = lowLevelArtefacts.Count - 1; i >= 0; i--)
        {
            LowLevelArtefactViewModel previous = lowLevelArtefacts[i];
            DateTime previousTime = new(previous.Date.Year, previous.Date.Month, previous.Date.Day, previous.Time.Hour, previous.Time.Minute, previous.Time.Second);
            DateTime currentTime = new(current.Date.Year, current.Date.Month, current.Date.Day, current.Time.Hour, current.Time.Minute, current.Time.Second);
            if (previousTime.CompareTo(currentTime) != 0)
            {
                break;
            }

            if (previous.SourceType.ToLower().Contains("cookies"))
            {
                string previousAddress = _lowLevelArtefactsAbstractorUtils.GetAddress(previous.Description);

                if (previousAddress == currentAddress)
                {
                    return null;
                }
            }
        }

        return current;
    }

    private int SkipFileEvents_Switch(List<EventViewModel> events, int startIndex, double periodInMinutes)
    {
        DateTime startTime = events[startIndex].FullDate;
        int count = 0;

        if (events[startIndex].SourceType != "OS Last Access Time")
        {
            return count;
        }

        for (int i = startIndex + 1; i < events.Count; i++)
        {
            EventViewModel eventViewModel = events[i];
            DateTime endTime = events[i].FullDate;
            double differenceInMinutes = endTime.Subtract(startTime).TotalMinutes;

            if (eventViewModel.Source != "FILE" || (eventViewModel.SourceType != "OS Last Access Time" && eventViewModel.SourceType != "OS Metadata Modification Time") || differenceInMinutes.CompareTo(periodInMinutes) > 0)
            {
                break;
            }

            if (eventViewModel.SourceType == "OS Last Access Time")
            {
                LinesSkipped++;
            }
            else
            {
                LinesNeglected++;
            }

            count++;
        }

        return count;
    }

    private static bool IsFileEventValid_Switch(List<LowLevelArtefactViewModel> lowLevelArtefacts, LowLevelArtefactViewModel current)
    {
        DateTime currentTime = new(current.Date.Year, current.Date.Month, current.Date.Day, current.Time.Hour, current.Time.Minute, current.Time.Second);

        if (current.SourceType != "OS Content Modification Time")
        {
            return !DoFileAndWebhistOfSameTimeExist_Switch(lowLevelArtefacts, currentTime);
        }

        for (int i = lowLevelArtefacts.Count - 1; i >= 0; i--)
        {
            LowLevelArtefactViewModel previous = lowLevelArtefacts[i];
            DateTime previousTime = new(previous.Date.Year, previous.Date.Month, previous.Date.Day, previous.Time.Hour, previous.Time.Minute, previous.Time.Second);
            if (previousTime.CompareTo(currentTime) != 0)
            {
                break;
            }

            if (previous.SourceType == current.SourceType)
            {
                return false;
            }
        }

        return true;
    }

    private static bool DoFileAndWebhistOfSameTimeExist_Switch(List<LowLevelArtefactViewModel> lowLevelArtefacts, DateTime currentTime)
    {
        bool doesFileExist = false;
        bool doesWebhistExist = false;

        for (int i = lowLevelArtefacts.Count - 1; i >= 0; i--)
        {
            LowLevelArtefactViewModel previous = lowLevelArtefacts[i];
            DateTime previousTime = new(previous.Date.Year, previous.Date.Month, previous.Date.Day, previous.Time.Hour, previous.Time.Minute, previous.Time.Second);
            if (previousTime.CompareTo(currentTime) != 0)
            {
                break;
            }

            if (previous.Source == "FILE")
            {
                doesFileExist = true;
            }

            if (previous.Source == "WEBHIST")
            {
                doesWebhistExist = true;
            }
        }

        return doesFileExist && doesWebhistExist;
    }

    [Benchmark]
    public List<ISerializableLevel> FormLowLevelArtefacts_ChainOfResponsibility()
    {
        List<ISerializableLevel> lowLevelArtefacts = new(_artefacts.Count);

        for (int i = 0; i < _artefacts.Count; i++)
        {
            ISerializableLevel lowLevelArtefact = _handler.FormAbstractEvent(_artefacts, lowLevelArtefacts, _artefacts[i]);

            if (_artefacts[i].Source == "FILE")
            {
                int needsSkipping = this.SkipFileEvents_ChainOfResponsibility(_artefacts, i, 1.0);
                i += needsSkipping;
            }

            if (lowLevelArtefact is not null)
            {
                lowLevelArtefacts.Add(lowLevelArtefact);
            }
        }

        return lowLevelArtefacts;
    }

    private int SkipFileEvents_ChainOfResponsibility(List<EventViewModel> events, int startIndex, double periodInMinutes)
    {
        DateTime startTime = events[startIndex].FullDate;
        int count = 0;

        if (events[startIndex].SourceType != "OS Last Access Time")
        {
            return count;
        }

        for (int i = startIndex + 1; i < events.Count; i++)
        {
            EventViewModel eventViewModel = events[i];
            DateTime endTime = events[i].FullDate;
            double differenceInMinutes = endTime.Subtract(startTime).TotalMinutes;

            if (eventViewModel.Source != "FILE" || (eventViewModel.SourceType != "OS Last Access Time" && eventViewModel.SourceType != "OS Metadata Modification Time") || differenceInMinutes.CompareTo(periodInMinutes) > 0)
            {
                break;
            }

            if (eventViewModel.SourceType == "OS Last Access Time")
            {
                LinesSkipped++;
            }
            else
            {
                LinesNeglected++;
            }

            count++;
        }

        return count;
    }
}
