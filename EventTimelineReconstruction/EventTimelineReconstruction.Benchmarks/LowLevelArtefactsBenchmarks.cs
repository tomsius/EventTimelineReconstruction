using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using EventTimelineReconstruction.Benchmarks.Models;
using EventTimelineReconstruction.Benchmarks.Utils;

namespace EventTimelineReconstruction.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class LowLevelArtefactsBenchmarks
{
    [Params(1000, 100_000, 1_000_000)]
    public int N;

    private int LinesSkipped;
    private int LinesNeglected;
    private List<EventViewModel> _events;
    private ILowLevelArtefactsAbstractorUtils _lowLevelArtefactsAbstractorUtils;

    [GlobalSetup]
    public void GlobalSetup()
    {
        LinesSkipped = 0;
        LinesNeglected = 0;
        _lowLevelArtefactsAbstractorUtils = new LowLevelArtefactsAbstractorUtils();

        // sukurti ivykiu sarasa
        _events = new(N);
    }

    [Benchmark(Baseline = true)]
    public List<LowLevelArtefactViewModel> FormLowLevelArtefacts_Switch()
    {
        List<LowLevelArtefactViewModel> lowLevelArtefacts = new(_events.Count);

        for (int i = 0; i < _events.Count; i++)
        {
            LowLevelArtefactViewModel lowLevelArtefact = null;

            switch (_events[i].Source)
            {
                case "WEBHIST":
                    if (_lowLevelArtefactsAbstractorUtils.IsValidWebhistLine(_events[i].SourceType, _events[i].Type))
                    {
                        lowLevelArtefact = this.FormEvent_Switch(_events[i]);

                        if (lowLevelArtefact.SourceType.ToLower().Contains("cookies"))
                        {
                            lowLevelArtefact = this.NormalizeCookie_Switch(lowLevelArtefacts, lowLevelArtefact);
                        }
                    }

                    break;
                case "LNK":
                    lowLevelArtefact = this.FormEvent_Switch(_events[i]);
                    break;
                case "FILE":
                    lowLevelArtefact = this.FormEvent_Switch(_events[i]);
                    int needsSkipping = this.SkipFileEvents_Switch(_events, i, 1.0);

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
}
