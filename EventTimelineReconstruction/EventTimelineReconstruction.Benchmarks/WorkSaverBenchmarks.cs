using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using EventTimelineReconstruction.Benchmarks.Models;

namespace EventTimelineReconstruction.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class WorkSaverBenchmarks
{
    [Params(1000, 100_000, 1_000_000)]
    public int N;

    private List<EventViewModel> _events;
    private List<HighLevelEventViewModel> _highLevelEvents;
    private List<LowLevelEventViewModel> _lowLevelEvents;
    private List<HighLevelArtefactViewModel> _highLevelArtefacts;
    private List<LowLevelArtefactViewModel> _lowLevelArtefacts;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _events = new(N);

        for (int i = 0; i < N; i++)
        {
            _events.Add(new EventViewModel(
                new EventModel(
                    new DateOnly(2020, 4, 6),
                    new TimeOnly(5, 23),
                    TimeZoneInfo.Utc,
                    "MACB1",
                    "Source1",
                    "Source Type1",
                    "Type1",
                    "Username1",
                    "Hostname1",
                    "Short Description1",
                    "Full Description1",
                    2.5,
                    "Filename1",
                    "iNode number1",
                    "Notes1",
                    "Format1",
                    new Dictionary<string, string>() { { "Key11", "Value11" }, { "Key12", "Value12" } }
                    )));

            _highLevelEvents.Add(new HighLevelEventViewModel(
                new DateOnly(2020, 4, 6),
                new TimeOnly(5, 23),
                "Source",
                "Short Description",
                "Visit",
                1
            ));

            _lowLevelEvents.Add(new LowLevelEventViewModel(
                new DateOnly(2020, 4, 6),
                new TimeOnly(5, 23),
                "Source",
                "Short Description",
                "Visit",
                "Extra",
                1
            ));

            _highLevelArtefacts.Add(new HighLevelArtefactViewModel(
                new DateOnly(2020, 4, 6),
                new TimeOnly(5, 23),
                "Source",
                "Short Description",
                "Visit",
                "Extra",
                1,
                "MACB",
                "SourceType",
                "Description"
            ));

            _lowLevelArtefacts.Add(new LowLevelArtefactViewModel(
                new DateOnly(2020, 4, 6),
                new TimeOnly(5, 23),
                "Timezone",
                "MACB",
                "Soruce",
                "SourceType",
                "Type",
                "User",
                "Host",
                "Short Description",
                "Description",
                "Version",
                "Filename",
                "INode",
                "Notes",
                "Format",
                "Extra",
                1
            ));
        }
    }

    [Benchmark(Baseline = true)]
    public void SaveWork_For()
    {
        using StreamWriter outputStream = new(@"EventsBenchmark.csv");

        this.WriteTreeToFile_For(_events, outputStream, 0);
    }

    private void WriteTreeToFile_For(List<EventViewModel> events, StreamWriter outputStream, int currentLevel)
    {
        for (int i = 0; i < events.Count; i++)
        {
            string serializedEventViewModel = events[i].Serialize();
            string dataToWrite = string.Format("{0}{1}", new string('\t', currentLevel), serializedEventViewModel);
            outputStream.WriteLine(dataToWrite);

            this.WriteTreeToFile_For(new List<EventViewModel>(events[i].Children), outputStream, currentLevel + 1);
        }
    }

    [Benchmark]
    public async Task SaveWork_Foreach()
    {
        using StreamWriter outputStream = new(@"EventsBenchmark.csv");

        await this.WriteTreeToFile_Foreach(_events, outputStream, 0);
        await outputStream.WriteLineAsync();
        await this.WriteHighLevelEventsToFile_Foreach(_highLevelEvents, outputStream);
        await outputStream.WriteLineAsync();
        await this.WriteLowLevelEventsToFile_Foreach(_lowLevelEvents, outputStream);
        await outputStream.WriteLineAsync();
        await this.WriteHighLevelArtefactsToFile_Foreach(_highLevelArtefacts, outputStream);
        await outputStream.WriteLineAsync();
        await this.WriteLowLevelArtefactsToFile_Foreach(_lowLevelArtefacts, outputStream);
    }

    private async Task WriteTreeToFile_Foreach(IEnumerable<EventViewModel> events, StreamWriter outputStream, int currentLevel)
    {
        foreach (EventViewModel eventViewModel in events)
        {
            string serializedEventViewModel = eventViewModel.Serialize();
            string dataToWrite = string.Format("{0}{1}", new string('\t', currentLevel), serializedEventViewModel);
            await outputStream.WriteLineAsync(dataToWrite);

            await this.WriteTreeToFile_Foreach(eventViewModel.Children, outputStream, currentLevel + 1);
        }
    }

    private async Task WriteHighLevelEventsToFile_Foreach(IEnumerable<HighLevelEventViewModel> highLevelEvents, StreamWriter outputStream)
    {
        foreach (HighLevelEventViewModel highLevelEvent in highLevelEvents)
        {
            string serializedLine = highLevelEvent.Serialize();
            await outputStream.WriteLineAsync(serializedLine);
        }
    }

    private async Task WriteLowLevelEventsToFile_Foreach(IEnumerable<LowLevelEventViewModel> lowLevelEvents, StreamWriter outputStream)
    {
        foreach (LowLevelEventViewModel lowLevelEvent in lowLevelEvents)
        {
            string serializedLine = lowLevelEvent.Serialize();
            await outputStream.WriteLineAsync(serializedLine);
        }
    }

    private async Task WriteHighLevelArtefactsToFile_Foreach(IEnumerable<HighLevelArtefactViewModel> highLevelArtefacts, StreamWriter outputStream)
    {
        foreach (HighLevelArtefactViewModel highLevelArtefact in highLevelArtefacts)
        {
            string serializedLine = highLevelArtefact.Serialize();
            await outputStream.WriteLineAsync(serializedLine);
        }
    }

    private async Task WriteLowLevelArtefactsToFile_Foreach(IEnumerable<LowLevelArtefactViewModel> lowLevelArtefacts, StreamWriter outputStream)
    {
        foreach (LowLevelArtefactViewModel lowLevelArtefact in lowLevelArtefacts)
        {
            string serializedLine = lowLevelArtefact.Serialize();
            await outputStream.WriteLineAsync(serializedLine);
        }
    }

    [Benchmark]
    public void SaveWork_ForSpan()
    {
        using StreamWriter outputStream = new(@"EventsBenchmark.csv");

        this.WriteTreeToFile_ForSpan(_events, outputStream, 0);
    }

    private void WriteTreeToFile_ForSpan(List<EventViewModel> events, StreamWriter outputStream, int currentLevel)
    {
        Span<EventViewModel> span = CollectionsMarshal.AsSpan(events);

        for (int i = 0; i < span.Length; i++)
        {
            string serializedEventViewModel = span[i].Serialize();
            string dataToWrite = string.Format("{0}{1}", new string('\t', currentLevel), serializedEventViewModel);
            outputStream.WriteLine(dataToWrite);

            this.WriteTreeToFile_ForSpan(new List<EventViewModel>(span[i].Children), outputStream, currentLevel + 1);
        }
    }

    [Benchmark]
    public void SaveWork_ForeachSpan()
    {
        using StreamWriter outputStream = new(@"EventsBenchmark.csv");

        this.WriteTreeToFile_ForeachSpan(_events, outputStream, 0);
    }

    private void WriteTreeToFile_ForeachSpan(List<EventViewModel> events, StreamWriter outputStream, int currentLevel)
    {
        foreach (EventViewModel eventViewModel in CollectionsMarshal.AsSpan(events))
        {
            string serializedEventViewModel = eventViewModel.Serialize();
            string dataToWrite = string.Format("{0}{1}", new string('\t', currentLevel), serializedEventViewModel);
            outputStream.WriteLine(dataToWrite);

            this.WriteTreeToFile_ForeachSpan(new List<EventViewModel>(eventViewModel.Children), outputStream, currentLevel + 1);
        }
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        File.Delete(@"EventsBenchmark.csv");
    }
}
