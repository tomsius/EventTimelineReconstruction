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
    public void SaveWork_Foreach()
    {
        using StreamWriter outputStream = new(@"EventsBenchmark.csv");

        this.WriteTreeToFile_Foreach(_events, outputStream, 0);
    }

    private void WriteTreeToFile_Foreach(IEnumerable<EventViewModel> events, StreamWriter outputStream, int currentLevel)
    {
        foreach (EventViewModel eventViewModel in events)
        {
            string serializedEventViewModel = eventViewModel.Serialize();
            string dataToWrite = string.Format("{0}{1}", new string('\t', currentLevel), serializedEventViewModel);
            outputStream.WriteLine(dataToWrite);

            this.WriteTreeToFile_Foreach(eventViewModel.Children, outputStream, currentLevel + 1);
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
