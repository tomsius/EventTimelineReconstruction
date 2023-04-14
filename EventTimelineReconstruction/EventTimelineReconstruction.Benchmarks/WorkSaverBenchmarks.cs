using System.Runtime.CompilerServices;
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
    private List<ISerializableLevel> _highLevelEvents;
    private List<ISerializableLevel> _lowLevelEvents;
    private List<ISerializableLevel> _highLevelArtefacts;
    private List<ISerializableLevel> _lowLevelArtefacts;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _events = new(N);
        _highLevelEvents = new(N);
        _lowLevelEvents = new(N);
        _highLevelArtefacts = new(N);
        _lowLevelArtefacts = new(N);

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

    [Benchmark]
    public async Task SaveWork_For()
    {
        using StreamWriter outputStream = new(@"EventsBenchmark.csv");

        await this.WriteTreeToFile_For(_events, outputStream, 0);
        await outputStream.WriteLineAsync();
        await WriteAbstractionLevelToFile_For(_highLevelEvents, outputStream);
        await outputStream.WriteLineAsync();
        await WriteAbstractionLevelToFile_For(_lowLevelEvents, outputStream);
        await outputStream.WriteLineAsync();
        await WriteAbstractionLevelToFile_For(_highLevelArtefacts, outputStream);
        await outputStream.WriteLineAsync();
        await WriteAbstractionLevelToFile_For(_lowLevelArtefacts, outputStream);
    }

    private async Task WriteTreeToFile_For(List<EventViewModel> events, StreamWriter outputStream, int currentLevel)
    {
        for (int i = 0; i < events.Count; i++)
        {
            string serializedEventViewModel = events[i].Serialize();
            string dataToWrite = string.Format("{0}{1}", new string('\t', currentLevel), serializedEventViewModel);
            await outputStream.WriteLineAsync(dataToWrite);

            await this.WriteTreeToFile_For(events[i].Children.ToList(), outputStream, currentLevel + 1);
        }
    }

    private static async Task WriteAbstractionLevelToFile_For(List<ISerializableLevel> abstractionLevel, StreamWriter outputStream)
    {
        for (int i = 0; i < abstractionLevel.Count; i++)
        {
            string serializedLine = abstractionLevel[i].Serialize();
            await outputStream.WriteLineAsync(serializedLine);
        }
    }

    [Benchmark]
    public async Task SaveWork_ForSpanList()
    {
        using StreamWriter outputStream = new(@"EventsBenchmark.csv");

        this.WriteTreeToFile_ForSpanList(_events, outputStream, 0);
        await outputStream.WriteLineAsync();
        this.WriteAbstractionLevelToFile_ForSpanList(_highLevelEvents, outputStream);
        await outputStream.WriteLineAsync();
        this.WriteAbstractionLevelToFile_ForSpanList(_lowLevelEvents, outputStream);
        await outputStream.WriteLineAsync();
        this.WriteAbstractionLevelToFile_ForSpanList(_highLevelArtefacts, outputStream);
        await outputStream.WriteLineAsync();
        this.WriteAbstractionLevelToFile_ForSpanList(_lowLevelArtefacts, outputStream);
    }

    private void WriteTreeToFile_ForSpanList(List<EventViewModel> events, StreamWriter outputStream, int currentLevel)
    {
        Span<EventViewModel> span = CollectionsMarshal.AsSpan(events);

        for (int i = 0; i < span.Length; i++)
        {
            string serializedEventViewModel = span[i].Serialize();
            string dataToWrite = string.Format("{0}{1}", new string('\t', currentLevel), serializedEventViewModel);
            outputStream.WriteLine(dataToWrite);

            this.WriteTreeToFile_ForSpanList(new List<EventViewModel>(span[i].Children), outputStream, currentLevel + 1);
        }
    }

    private void WriteAbstractionLevelToFile_ForSpanList(List<ISerializableLevel> abstractionLevel, StreamWriter outputStream)
    {
        Span<ISerializableLevel> span = CollectionsMarshal.AsSpan(abstractionLevel);

        for (int i = 0; i < span.Length; i++)
        {
            string serializedLine = span[i].Serialize();
            outputStream.WriteLine(serializedLine);
        }
    }

    [Benchmark]
    public async Task SaveWork_ForSpanArray()
    {
        using StreamWriter outputStream = new(@"EventsBenchmark.csv");

        this.WriteTreeToFile_ForSpanArray(_events.ToArray(), outputStream, 0);
        await outputStream.WriteLineAsync();
        this.WriteAbstractionLevelToFile_ForSpanArray(_highLevelEvents.ToArray(), outputStream);
        await outputStream.WriteLineAsync();
        this.WriteAbstractionLevelToFile_ForSpanArray(_lowLevelEvents.ToArray(), outputStream);
        await outputStream.WriteLineAsync();
        this.WriteAbstractionLevelToFile_ForSpanArray(_highLevelArtefacts.ToArray(), outputStream);
        await outputStream.WriteLineAsync();
        this.WriteAbstractionLevelToFile_ForSpanArray(_lowLevelArtefacts.ToArray(), outputStream);
    }

    private void WriteTreeToFile_ForSpanArray(EventViewModel[] events, StreamWriter outputStream, int currentLevel)
    {
        Span<EventViewModel> span = events.AsSpan();

        for (int i = 0; i < span.Length; i++)
        {
            string serializedEventViewModel = span[i].Serialize();
            string dataToWrite = string.Format("{0}{1}", new string('\t', currentLevel), serializedEventViewModel);
            outputStream.WriteLine(dataToWrite);

            this.WriteTreeToFile_ForSpanArray(span[i].Children.ToArray(), outputStream, currentLevel + 1);
        }
    }

    private void WriteAbstractionLevelToFile_ForSpanArray(ISerializableLevel[] abstractionLevel, StreamWriter outputStream)
    {
        Span<ISerializableLevel> span = abstractionLevel.AsSpan();

        for (int i = 0; i < span.Length; i++)
        {
            string serializedLine = span[i].Serialize();
            outputStream.WriteLine(serializedLine);
        }
    }

    [Benchmark]
    public async Task SaveWork_ForMemoryMarshal()
    {
        using StreamWriter outputStream = new(@"EventsBenchmark.csv");

        this.WriteTreeToFile_ForMemoryMarshal(_events.ToArray(), outputStream, 0);
        await outputStream.WriteLineAsync();
        this.WriteAbstractionLevelToFile_ForMemoryMarshal(_highLevelEvents.ToArray(), outputStream);
        await outputStream.WriteLineAsync();
        this.WriteAbstractionLevelToFile_ForMemoryMarshal(_lowLevelEvents.ToArray(), outputStream);
        await outputStream.WriteLineAsync();
        this.WriteAbstractionLevelToFile_ForMemoryMarshal(_highLevelArtefacts.ToArray(), outputStream);
        await outputStream.WriteLineAsync();
        this.WriteAbstractionLevelToFile_ForMemoryMarshal(_lowLevelArtefacts.ToArray(), outputStream);
    }

    private void WriteTreeToFile_ForMemoryMarshal(EventViewModel[] events, StreamWriter outputStream, int currentLevel)
    {
        ref EventViewModel searchSpace = ref MemoryMarshal.GetArrayDataReference(events);

        for (int i = 0; i < events.Length; i++)
        {
            EventViewModel eventViewModel = Unsafe.Add(ref searchSpace, i);
            string serializedEventViewModel = eventViewModel.Serialize();
            string dataToWrite = string.Format("{0}{1}", new string('\t', currentLevel), serializedEventViewModel);
            outputStream.WriteLine(dataToWrite);

            this.WriteTreeToFile_ForMemoryMarshal(eventViewModel.Children.ToArray(), outputStream, currentLevel + 1);
        }
    }

    private void WriteAbstractionLevelToFile_ForMemoryMarshal(ISerializableLevel[] abstractionLevel, StreamWriter outputStream)
    {
        ref ISerializableLevel searchSpace = ref MemoryMarshal.GetArrayDataReference(abstractionLevel);

        for (int i = 0; i < abstractionLevel.Length; i++)
        {
            ISerializableLevel highLevelEvent = Unsafe.Add(ref searchSpace, i);
            string serializedLine = highLevelEvent.Serialize();
            outputStream.WriteLine(serializedLine);
        }
    }

    [Benchmark(Baseline = true)]
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
    public async Task SaveWork_ForeachSpanList()
    {
        using StreamWriter outputStream = new(@"EventsBenchmark.csv");

        this.WriteTreeToFile_ForeachSpanList(_events, outputStream, 0);
        await outputStream.WriteLineAsync();
        this.WriteAbstractionLevelToFile_ForeachSpanList(_highLevelEvents, outputStream);
        await outputStream.WriteLineAsync();
        this.WriteAbstractionLevelToFile_ForeachSpanList(_lowLevelEvents, outputStream);
        await outputStream.WriteLineAsync();
        this.WriteAbstractionLevelToFile_ForeachSpanList(_highLevelArtefacts, outputStream);
        await outputStream.WriteLineAsync();
        this.WriteAbstractionLevelToFile_ForeachSpanList(_lowLevelArtefacts, outputStream);
    }

    private void WriteTreeToFile_ForeachSpanList(List<EventViewModel> events, StreamWriter outputStream, int currentLevel)
    {
        foreach (EventViewModel eventViewModel in CollectionsMarshal.AsSpan(events))
        {
            string serializedEventViewModel = eventViewModel.Serialize();
            string dataToWrite = string.Format("{0}{1}", new string('\t', currentLevel), serializedEventViewModel);
            outputStream.WriteLine(dataToWrite);

            this.WriteTreeToFile_ForeachSpanList(eventViewModel.Children.ToList(), outputStream, currentLevel + 1);
        }
    }

    private void WriteAbstractionLevelToFile_ForeachSpanList(List<ISerializableLevel> abstractionLevel, StreamWriter outputStream)
    {
        foreach (ISerializableLevel eventInLevel in CollectionsMarshal.AsSpan(abstractionLevel))
        {
            string serializedLine = eventInLevel.Serialize();
            outputStream.WriteLine(serializedLine);
        }
    }

    [Benchmark]
    public async Task SaveWork_ForeachSpanArray()
    {
        using StreamWriter outputStream = new(@"EventsBenchmark.csv");

        this.WriteTreeToFile_ForeachSpanArray(_events.ToArray(), outputStream, 0);
        await outputStream.WriteLineAsync();
        this.WriteAbstractionLevelToFile_ForeachSpanArray(_highLevelEvents.ToArray(), outputStream);
        await outputStream.WriteLineAsync();
        this.WriteAbstractionLevelToFile_ForeachSpanArray(_lowLevelEvents.ToArray(), outputStream);
        await outputStream.WriteLineAsync();
        this.WriteAbstractionLevelToFile_ForeachSpanArray(_highLevelArtefacts.ToArray(), outputStream);
        await outputStream.WriteLineAsync();
        this.WriteAbstractionLevelToFile_ForeachSpanArray(_lowLevelArtefacts.ToArray(), outputStream);
    }

    private void WriteTreeToFile_ForeachSpanArray(EventViewModel[] events, StreamWriter outputStream, int currentLevel)
    {
        foreach (EventViewModel eventViewModel in events.AsSpan())
        {
            string serializedEventViewModel = eventViewModel.Serialize();
            string dataToWrite = string.Format("{0}{1}", new string('\t', currentLevel), serializedEventViewModel);
            outputStream.WriteLine(dataToWrite);

            this.WriteTreeToFile_ForeachSpanArray(eventViewModel.Children.ToArray(), outputStream, currentLevel + 1);
        }
    }

    private void WriteAbstractionLevelToFile_ForeachSpanArray(ISerializableLevel[] abstractionLevel, StreamWriter outputStream)
    {
        foreach (ISerializableLevel eventInLevel in abstractionLevel.AsSpan())
        {
            string serializedLine = eventInLevel.Serialize();
            outputStream.WriteLine(serializedLine);
        }
    }

    [Benchmark]
    public async Task SaveWork_While()
    {
        using StreamWriter outputStream = new(@"EventsBenchmark.csv");

        await this.WriteTreeToFile_While(_events, outputStream, 0);
        await outputStream.WriteLineAsync();
        await this.WriteAbstractionLevelToFile_While(_highLevelEvents, outputStream);
        await outputStream.WriteLineAsync();
        await this.WriteAbstractionLevelToFile_While(_lowLevelEvents, outputStream);
        await outputStream.WriteLineAsync();
        await this.WriteAbstractionLevelToFile_While(_highLevelArtefacts, outputStream);
        await outputStream.WriteLineAsync();
        await this.WriteAbstractionLevelToFile_While(_lowLevelArtefacts, outputStream);
    }

    private async Task WriteTreeToFile_While(IEnumerable<EventViewModel> events, StreamWriter outputStream, int currentLevel)
    {
        IEnumerator<EventViewModel> enumerator = events.GetEnumerator();

        while (enumerator.MoveNext())
        {
            string serializedEventViewModel = enumerator.Current.Serialize();
            string dataToWrite = string.Format("{0}{1}", new string('\t', currentLevel), serializedEventViewModel);
            await outputStream.WriteLineAsync(dataToWrite);

            await this.WriteTreeToFile_While(enumerator.Current.Children, outputStream, currentLevel + 1);
        }
    }

    private async Task WriteAbstractionLevelToFile_While(IEnumerable<ISerializableLevel> abstractionLevel, StreamWriter outputStream)
    {
        IEnumerator<ISerializableLevel> enumerator = abstractionLevel.GetEnumerator();

        while (enumerator.MoveNext())
        {
            string serializedLine = enumerator.Current.Serialize();
            await outputStream.WriteLineAsync(serializedLine);
        }
    }

    [Benchmark]
    public async Task SaveWork_WhileMemoryMarshal()
    {
        using StreamWriter outputStream = new(@"EventsBenchmark.csv");

        this.WriteTreeToFile_WhileMemoryMarshal(_events.ToArray(), outputStream, 0);
        await outputStream.WriteLineAsync();
        this.WriteAbstractionLevelToFile_WhileMemoryMarshal(_highLevelEvents.ToArray(), outputStream);
        await outputStream.WriteLineAsync();
        this.WriteAbstractionLevelToFile_WhileMemoryMarshal(_lowLevelEvents.ToArray(), outputStream);
        await outputStream.WriteLineAsync();
        this.WriteAbstractionLevelToFile_WhileMemoryMarshal(_highLevelArtefacts.ToArray(), outputStream);
        await outputStream.WriteLineAsync();
        this.WriteAbstractionLevelToFile_WhileMemoryMarshal(_lowLevelArtefacts.ToArray(), outputStream);
    }

    private void WriteTreeToFile_WhileMemoryMarshal(EventViewModel[] events, StreamWriter outputStream, int currentLevel)
    {
        ref EventViewModel start = ref MemoryMarshal.GetArrayDataReference(events);
        ref EventViewModel end = ref Unsafe.Add(ref start, events.Length);

        while (Unsafe.IsAddressLessThan(ref start, ref end))
        {
            string serializedEventViewModel = start.Serialize();
            string dataToWrite = string.Format("{0}{1}", new string('\t', currentLevel), serializedEventViewModel);
            outputStream.WriteLine(dataToWrite);

            this.WriteTreeToFile_WhileMemoryMarshal(start.Children.ToArray(), outputStream, currentLevel + 1);

            start = ref Unsafe.Add(ref start, 1);
        }
    }

    private void WriteAbstractionLevelToFile_WhileMemoryMarshal(ISerializableLevel[] abstractionLevel, StreamWriter outputStream)
    {
        ref ISerializableLevel start = ref MemoryMarshal.GetArrayDataReference(abstractionLevel);
        ref ISerializableLevel end = ref Unsafe.Add(ref start, abstractionLevel.Length);

        while (Unsafe.IsAddressLessThan(ref start, ref end))
        {
            string serializedLine = start.Serialize();
            outputStream.WriteLine(serializedLine);

            start = ref Unsafe.Add(ref start, 1);
        }
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        File.Delete(@"EventsBenchmark.csv");
    }
}
