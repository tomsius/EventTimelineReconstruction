using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using EventTimelineReconstruction.Benchmarks.Models;
using Microsoft.Extensions.Logging;

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

    [Benchmark]
    public async Task SaveWork_For()
    {
        using StreamWriter outputStream = new(@"EventsBenchmark.csv");

        await this.WriteTreeToFile_For(_events, outputStream, 0);
        await outputStream.WriteLineAsync();
        await WriteHighLevelEventsToFile_For(_highLevelEvents, outputStream);
        await outputStream.WriteLineAsync();
        await WriteLowLevelEventsToFile_For(_lowLevelEvents, outputStream);
        await outputStream.WriteLineAsync();
        await WriteHighLevelArtefactsToFile_For(_highLevelArtefacts, outputStream);
        await outputStream.WriteLineAsync();
        await WriteLowLevelArtefactsToFile_For(_lowLevelArtefacts, outputStream);
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

    private static async Task WriteHighLevelEventsToFile_For(List<HighLevelEventViewModel> highLevelEvents, StreamWriter outputStream)
    {
        for (int i = 0; i < highLevelEvents.Count; i++)
        {
            string serializedLine = highLevelEvents[i].Serialize();
            await outputStream.WriteLineAsync(serializedLine);
        }
    }

    private static async Task WriteLowLevelEventsToFile_For(List<LowLevelEventViewModel> lowLevelEvents, StreamWriter outputStream)
    {
        for (int i = 0; i < lowLevelEvents.Count; i++)
        {
            string serializedLine = lowLevelEvents[i].Serialize();
            await outputStream.WriteLineAsync(serializedLine);
        }
    }

    private static async Task WriteHighLevelArtefactsToFile_For(List<HighLevelArtefactViewModel> highLevelArtefacts, StreamWriter outputStream)
    {
        for (int i = 0; i < highLevelArtefacts.Count; i++)
        {
            string serializedLine = highLevelArtefacts[i].Serialize();
            await outputStream.WriteLineAsync(serializedLine);
        }
    }

    private static async Task WriteLowLevelArtefactsToFile_For(List<LowLevelArtefactViewModel> lowLevelArtefacts, StreamWriter outputStream)
    {
        for (int i = 0; i < lowLevelArtefacts.Count; i++)
        {
            string serializedLine = lowLevelArtefacts[i].Serialize();
            await outputStream.WriteLineAsync(serializedLine);
        }
    }

    [Benchmark]
    public async Task SaveWork_ForSpanList()
    {
        using StreamWriter outputStream = new(@"EventsBenchmark.csv");

        this.WriteTreeToFile_ForSpanList(_events, outputStream, 0);
        await outputStream.WriteLineAsync();
        this.WriteHighLevelEventsToFile_ForSpanList(_highLevelEvents, outputStream);
        await outputStream.WriteLineAsync();
        this.WriteLowLevelEventsToFile_ForSpanList(_lowLevelEvents, outputStream);
        await outputStream.WriteLineAsync();
        this.WriteHighLevelArtefactsToFile_ForSpanList(_highLevelArtefacts, outputStream);
        await outputStream.WriteLineAsync();
        this.WriteLowLevelArtefactsToFile_ForSpanList(_lowLevelArtefacts, outputStream);
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

    private void WriteHighLevelEventsToFile_ForSpanList(List<HighLevelEventViewModel> highLevelEvents, StreamWriter outputStream)
    {
        Span<HighLevelEventViewModel> span = CollectionsMarshal.AsSpan(highLevelEvents);

        for (int i = 0; i < span.Length; i++)
        {
            string serializedLine = span[i].Serialize();
            outputStream.WriteLine(serializedLine);
        }
    }

    private void WriteLowLevelEventsToFile_ForSpanList(List<LowLevelEventViewModel> lowLevelEvents, StreamWriter outputStream)
    {
        Span<LowLevelEventViewModel> span = CollectionsMarshal.AsSpan(lowLevelEvents);

        for (int i = 0; i < span.Length; i++)
        {
            string serializedLine = span[i].Serialize();
            outputStream.WriteLine(serializedLine);
        }
    }

    private void WriteHighLevelArtefactsToFile_ForSpanList(List<HighLevelArtefactViewModel> highLevelArtefacts, StreamWriter outputStream)
    {
        Span<HighLevelArtefactViewModel> span = CollectionsMarshal.AsSpan(highLevelArtefacts);

        for (int i = 0; i < span.Length; i++)
        {
            string serializedLine = span[i].Serialize();
            outputStream.WriteLine(serializedLine);
        }
    }

    private void WriteLowLevelArtefactsToFile_ForSpanList(List<LowLevelArtefactViewModel> lowLevelArtefacts, StreamWriter outputStream)
    {
        Span<LowLevelArtefactViewModel> span = CollectionsMarshal.AsSpan(lowLevelArtefacts);

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
        this.WriteHighLevelEventsToFile_ForSpanArray(_highLevelEvents.ToArray(), outputStream);
        await outputStream.WriteLineAsync();
        this.WriteLowLevelEventsToFile_ForSpanArray(_lowLevelEvents.ToArray(), outputStream);
        await outputStream.WriteLineAsync();
        this.WriteHighLevelArtefactsToFile_ForSpanArray(_highLevelArtefacts.ToArray(), outputStream);
        await outputStream.WriteLineAsync();
        this.WriteLowLevelArtefactsToFile_ForSpanArray(_lowLevelArtefacts.ToArray(), outputStream);
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

    private void WriteHighLevelEventsToFile_ForSpanArray(HighLevelEventViewModel[] highLevelEvents, StreamWriter outputStream)
    {
        Span<HighLevelEventViewModel> span = highLevelEvents.AsSpan();

        for (int i = 0; i < span.Length; i++)
        {
            string serializedLine = span[i].Serialize();
            outputStream.WriteLine(serializedLine);
        }
    }

    private void WriteLowLevelEventsToFile_ForSpanArray(LowLevelEventViewModel[] lowLevelEvents, StreamWriter outputStream)
    {
        Span<LowLevelEventViewModel> span = lowLevelEvents.AsSpan();

        for (int i = 0; i < span.Length; i++)
        {
            string serializedLine = span[i].Serialize();
            outputStream.WriteLine(serializedLine);
        }
    }

    private void WriteHighLevelArtefactsToFile_ForSpanArray(HighLevelArtefactViewModel[] highLevelArtefacts, StreamWriter outputStream)
    {
        Span<HighLevelArtefactViewModel> span = highLevelArtefacts.AsSpan();

        for (int i = 0; i < span.Length; i++)
        {
            string serializedLine = span[i].Serialize();
            outputStream.WriteLine(serializedLine);
        }
    }

    private void WriteLowLevelArtefactsToFile_ForSpanArray(LowLevelArtefactViewModel[] lowLevelArtefacts, StreamWriter outputStream)
    {
        Span<LowLevelArtefactViewModel> span = lowLevelArtefacts.AsSpan();

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
        this.WriteHighLevelEventsToFile_ForMemoryMarshal(_highLevelEvents.ToArray(), outputStream);
        await outputStream.WriteLineAsync();
        this.WriteLowLevelEventsToFile_ForMemoryMarshal(_lowLevelEvents.ToArray(), outputStream);
        await outputStream.WriteLineAsync();
        this.WriteHighLevelArtefactsToFile_ForMemoryMarshal(_highLevelArtefacts.ToArray(), outputStream);
        await outputStream.WriteLineAsync();
        this.WriteLowLevelArtefactsToFile_ForMemoryMarshal(_lowLevelArtefacts.ToArray(), outputStream);
    }

    private void WriteTreeToFile_ForMemoryMarshal(EventViewModel[] events, StreamWriter outputStream, int currentLevel)
    {
        ref var searchSpace = ref MemoryMarshal.GetArrayDataReference(events);

        for (int i = 0; i < events.Length; i++)
        {
            EventViewModel eventViewModel = Unsafe.Add(ref searchSpace, i);
            string serializedEventViewModel = eventViewModel.Serialize();
            string dataToWrite = string.Format("{0}{1}", new string('\t', currentLevel), serializedEventViewModel);
            outputStream.WriteLine(dataToWrite);

            this.WriteTreeToFile_ForMemoryMarshal(eventViewModel.Children.ToArray(), outputStream, currentLevel + 1);
        }
    }

    private void WriteHighLevelEventsToFile_ForMemoryMarshal(HighLevelEventViewModel[] highLevelEvents, StreamWriter outputStream)
    {
        ref var searchSpace = ref MemoryMarshal.GetArrayDataReference(highLevelEvents);

        for (int i = 0; i < highLevelEvents.Length; i++)
        {
            HighLevelEventViewModel highLevelEvent = Unsafe.Add(ref searchSpace, i);
            string serializedLine = highLevelEvent.Serialize();
            outputStream.WriteLine(serializedLine);
        }
    }

    private void WriteLowLevelEventsToFile_ForMemoryMarshal(LowLevelEventViewModel[] lowLevelEvents, StreamWriter outputStream)
    {
        ref var searchSpace = ref MemoryMarshal.GetArrayDataReference(lowLevelEvents);

        for (int i = 0; i < lowLevelEvents.Length; i++)
        {
            LowLevelEventViewModel lowLevelEvent = Unsafe.Add(ref searchSpace, i);
            string serializedLine = lowLevelEvent.Serialize();
            outputStream.WriteLine(serializedLine);
        }
    }

    private void WriteHighLevelArtefactsToFile_ForMemoryMarshal(HighLevelArtefactViewModel[] highLevelArtefacts, StreamWriter outputStream)
    {
        ref var searchSpace = ref MemoryMarshal.GetArrayDataReference(highLevelArtefacts);

        for (int i = 0; i < highLevelArtefacts.Length; i++)
        {
            HighLevelArtefactViewModel highLevelArtefact = Unsafe.Add(ref searchSpace, i);
            string serializedLine = highLevelArtefact.Serialize();
            outputStream.WriteLine(serializedLine);
        }
    }

    private void WriteLowLevelArtefactsToFile_ForMemoryMarshal(LowLevelArtefactViewModel[] lowLevelArtefacts, StreamWriter outputStream)
    {
        ref var searchSpace = ref MemoryMarshal.GetArrayDataReference(lowLevelArtefacts);

        for (int i = 0; i < lowLevelArtefacts.Length; i++)
        {
            LowLevelArtefactViewModel lowLevelArtefact = Unsafe.Add(ref searchSpace, i);
            string serializedLine = lowLevelArtefact.Serialize();
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
        this.WriteHighLevelEventsToFile_ForeachSpanList(_highLevelEvents, outputStream);
        await outputStream.WriteLineAsync();
        this.WriteLowLevelEventsToFile_ForeachSpanList(_lowLevelEvents, outputStream);
        await outputStream.WriteLineAsync();
        this.WriteHighLevelArtefactsToFile_ForeachSpanList(_highLevelArtefacts, outputStream);
        await outputStream.WriteLineAsync();
        this.WriteLowLevelArtefactsToFile_ForeachSpanList(_lowLevelArtefacts, outputStream);
    }

    private void WriteTreeToFile_ForeachSpanList(List<EventViewModel> events, StreamWriter outputStream, int currentLevel)
    {
        foreach (EventViewModel eventViewModel in CollectionsMarshal.AsSpan(events))
        {
            string serializedEventViewModel = eventViewModel.Serialize();
            string dataToWrite = string.Format("{0}{1}", new string('\t', currentLevel), serializedEventViewModel);
            outputStream.WriteLine(dataToWrite);

            this.WriteTreeToFile_ForeachSpanList(new List<EventViewModel>(eventViewModel.Children), outputStream, currentLevel + 1);
        }
    }

    private void WriteHighLevelEventsToFile_ForeachSpanList(List<HighLevelEventViewModel> highLevelEvents, StreamWriter outputStream)
    {
        foreach (HighLevelEventViewModel highLevelEvent in CollectionsMarshal.AsSpan(highLevelEvents))
        {
            string serializedLine = highLevelEvent.Serialize();
            outputStream.WriteLine(serializedLine);
        }
    }

    private void WriteLowLevelEventsToFile_ForeachSpanList(List<LowLevelEventViewModel> lowLevelEvents, StreamWriter outputStream)
    {
        foreach (LowLevelEventViewModel lowLevelEvent in CollectionsMarshal.AsSpan(lowLevelEvents))
        {
            string serializedLine = lowLevelEvent.Serialize();
            outputStream.WriteLine(serializedLine);
        }
    }

    private void WriteHighLevelArtefactsToFile_ForeachSpanList(List<HighLevelArtefactViewModel> highLevelArtefacts, StreamWriter outputStream)
    {
        foreach (HighLevelArtefactViewModel highLevelArtefact in CollectionsMarshal.AsSpan(highLevelArtefacts))
        {
            string serializedLine = highLevelArtefact.Serialize();
            outputStream.WriteLine(serializedLine);
        }
    }

    private void WriteLowLevelArtefactsToFile_ForeachSpanList(List<LowLevelArtefactViewModel> lowLevelArtefacts, StreamWriter outputStream)
    {
        foreach (LowLevelArtefactViewModel lowLevelArtefact in CollectionsMarshal.AsSpan(lowLevelArtefacts))
        {
            string serializedLine = lowLevelArtefact.Serialize();
            outputStream.WriteLine(serializedLine);
        }
    }

    [Benchmark]
    public async Task SaveWork_ForeachSpanArray()
    {
        using StreamWriter outputStream = new(@"EventsBenchmark.csv");

        this.WriteTreeToFile_ForeachSpanArray(_events.ToArray(), outputStream, 0);
        await outputStream.WriteLineAsync();
        this.WriteHighLevelEventsToFile_ForeachSpanArray(_highLevelEvents.ToArray(), outputStream);
        await outputStream.WriteLineAsync();
        this.WriteLowLevelEventsToFile_ForeachSpanArray(_lowLevelEvents.ToArray(), outputStream);
        await outputStream.WriteLineAsync();
        this.WriteHighLevelArtefactsToFile_ForeachSpanArray(_highLevelArtefacts.ToArray(), outputStream);
        await outputStream.WriteLineAsync();
        this.WriteLowLevelArtefactsToFile_ForeachSpanArray(_lowLevelArtefacts.ToArray(), outputStream);
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

    private void WriteHighLevelEventsToFile_ForeachSpanArray(HighLevelEventViewModel[] highLevelEvents, StreamWriter outputStream)
    {
        foreach (HighLevelEventViewModel highLevelEvent in highLevelEvents.AsSpan())
        {
            string serializedLine = highLevelEvent.Serialize();
            outputStream.WriteLine(serializedLine);
        }
    }

    private void WriteLowLevelEventsToFile_ForeachSpanArray(LowLevelEventViewModel[] lowLevelEvents, StreamWriter outputStream)
    {
        foreach (LowLevelEventViewModel lowLevelEvent in lowLevelEvents.AsSpan())
        {
            string serializedLine = lowLevelEvent.Serialize();
            outputStream.WriteLine(serializedLine);
        }
    }

    private void WriteHighLevelArtefactsToFile_ForeachSpanArray(HighLevelArtefactViewModel[] highLevelArtefacts, StreamWriter outputStream)
    {
        foreach (HighLevelArtefactViewModel highLevelArtefact in highLevelArtefacts.AsSpan())
        {
            string serializedLine = highLevelArtefact.Serialize();
            outputStream.WriteLine(serializedLine);
        }
    }

    private void WriteLowLevelArtefactsToFile_ForeachSpanArray(LowLevelArtefactViewModel[] lowLevelArtefacts, StreamWriter outputStream)
    {
        foreach (LowLevelArtefactViewModel lowLevelArtefact in lowLevelArtefacts.AsSpan())
        {
            string serializedLine = lowLevelArtefact.Serialize();
            outputStream.WriteLine(serializedLine);
        }
    }

    [Benchmark]
    public async Task SaveWork_While()
    {
        using StreamWriter outputStream = new(@"EventsBenchmark.csv");

        await this.WriteTreeToFile_While(_events, outputStream, 0);
        await outputStream.WriteLineAsync();
        await this.WriteHighLevelEventsToFile_While(_highLevelEvents, outputStream);
        await outputStream.WriteLineAsync();
        await this.WriteLowLevelEventsToFile_While(_lowLevelEvents, outputStream);
        await outputStream.WriteLineAsync();
        await this.WriteHighLevelArtefactsToFile_While(_highLevelArtefacts, outputStream);
        await outputStream.WriteLineAsync();
        await this.WriteLowLevelArtefactsToFile_While(_lowLevelArtefacts, outputStream);
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

    private async Task WriteHighLevelEventsToFile_While(IEnumerable<HighLevelEventViewModel> highLevelEvents, StreamWriter outputStream)
    {
        IEnumerator<HighLevelEventViewModel> enumerator = highLevelEvents.GetEnumerator();

        while (enumerator.MoveNext())
        {
            string serializedLine = enumerator.Current.Serialize();
            await outputStream.WriteLineAsync(serializedLine);
        }
    }

    private async Task WriteLowLevelEventsToFile_While(IEnumerable<LowLevelEventViewModel> lowLevelEvents, StreamWriter outputStream)
    {
        IEnumerator<LowLevelEventViewModel> enumerator = lowLevelEvents.GetEnumerator();

        while (enumerator.MoveNext())
        {
            string serializedLine = enumerator.Current.Serialize();
            await outputStream.WriteLineAsync(serializedLine);
        }
    }

    private async Task WriteHighLevelArtefactsToFile_While(IEnumerable<HighLevelArtefactViewModel> highLevelArtefacts, StreamWriter outputStream)
    {
        IEnumerator<HighLevelArtefactViewModel> enumerator = highLevelArtefacts.GetEnumerator();

        while (enumerator.MoveNext())
        {
            string serializedLine = enumerator.Current.Serialize();
            await outputStream.WriteLineAsync(serializedLine);
        }
    }

    private async Task WriteLowLevelArtefactsToFile_While(IEnumerable<LowLevelArtefactViewModel> lowLevelArtefacts, StreamWriter outputStream)
    {
        IEnumerator<LowLevelArtefactViewModel> enumerator = lowLevelArtefacts.GetEnumerator();

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
        this.WriteHighLevelEventsToFile_WhileMemoryMarshal(_highLevelEvents.ToArray(), outputStream);
        await outputStream.WriteLineAsync();
        this.WriteLowLevelEventsToFile_WhileMemoryMarshal(_lowLevelEvents.ToArray(), outputStream);
        await outputStream.WriteLineAsync();
        this.WriteHighLevelArtefactsToFile_WhileMemoryMarshal(_highLevelArtefacts.ToArray(), outputStream);
        await outputStream.WriteLineAsync();
        this.WriteLowLevelArtefactsToFile_WhileMemoryMarshal(_lowLevelArtefacts.ToArray(), outputStream);
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

    private void WriteHighLevelEventsToFile_WhileMemoryMarshal(HighLevelEventViewModel[] highLevelEvents, StreamWriter outputStream)
    {
        ref HighLevelEventViewModel start = ref MemoryMarshal.GetArrayDataReference(highLevelEvents);
        ref HighLevelEventViewModel end = ref Unsafe.Add(ref start, highLevelEvents.Length);

        while (Unsafe.IsAddressLessThan(ref start, ref end))
        {
            string serializedLine = start.Serialize();
            outputStream.WriteLine(serializedLine);

            start = ref Unsafe.Add(ref start, 1);
        }
    }

    private void WriteLowLevelEventsToFile_WhileMemoryMarshal(LowLevelEventViewModel[] lowLevelEvents, StreamWriter outputStream)
    {
        ref LowLevelEventViewModel start = ref MemoryMarshal.GetArrayDataReference(lowLevelEvents);
        ref LowLevelEventViewModel end = ref Unsafe.Add(ref start, lowLevelEvents.Length);

        while (Unsafe.IsAddressLessThan(ref start, ref end))
        {
            string serializedLine = start.Serialize();
            outputStream.WriteLine(serializedLine);

            start = ref Unsafe.Add(ref start, 1);
        }
    }

    private void WriteHighLevelArtefactsToFile_WhileMemoryMarshal(HighLevelArtefactViewModel[] highLevelArtefacts, StreamWriter outputStream)
    {
        ref HighLevelArtefactViewModel start = ref MemoryMarshal.GetArrayDataReference(highLevelArtefacts);
        ref HighLevelArtefactViewModel end = ref Unsafe.Add(ref start, highLevelArtefacts.Length);

        while (Unsafe.IsAddressLessThan(ref start, ref end))
        {
            string serializedLine = start.Serialize();
            outputStream.WriteLine(serializedLine);

            start = ref Unsafe.Add(ref start, 1);
        }
    }

    private  void WriteLowLevelArtefactsToFile_WhileMemoryMarshal(LowLevelArtefactViewModel[] lowLevelArtefacts, StreamWriter outputStream)
    {
        ref LowLevelArtefactViewModel start = ref MemoryMarshal.GetArrayDataReference(lowLevelArtefacts);
        ref LowLevelArtefactViewModel end = ref Unsafe.Add(ref start, lowLevelArtefacts.Length);

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
