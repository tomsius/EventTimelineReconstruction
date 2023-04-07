using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using EventTimelineReconstruction.Benchmarks.Models;

namespace EventTimelineReconstruction.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class EventsImporterBenchmarks
{
    [Params(1000, 10_000, 100_000, 1_000_000)]
    public int N;

    private DateTime _fromDate;
    private DateTime _toDate;

    [GlobalSetup]
    public void GlobalSetup()
    {
        if (!File.Exists(@"EventsBenchmark.csv"))
        {
            using StreamWriter writeStream = new(@"EventsBenchmark.csv", false);
            writeStream.WriteLine("header");

            for (int i = 1; i < N; i++)
            {
                writeStream.WriteLine(@"01/01/2020,15:25:55,UTC,....,REG,AppCompatCache Registry Entry,File Last Modification Time,-,PC1-5DFC89FB1E0,[HKEY_LOCAL_MACHINE\System\ControlSet001\Control\Session Manager\AppCompatibi...,[HKEY_LOCAL_MACHINE\System\ControlSet001\Control\Session Manager\AppCompatibility] Cached entry: 28,2,TSK:/System Volume Information/_restore{5D162324-8035-4BDB-B6BA-8D2C3C5FBFF0}/RP2/snapshot/_REGISTRY_MACHINE_SYSTEM,13932,-,winreg/appcompatcache,sha256_hash: c822dbf91f7d96c0fcea412ed5ad22d8b1b0b7047357153d631940ac89042e38");
            }
        }

        _fromDate = DateTime.MinValue;
        _toDate = DateTime.MaxValue;
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        File.Delete(@"EventsBenchmark.csv");
    }

    public string[] ReadLinesArray()
    {
        using StreamReader reader = new(@"EventsBenchmark.csv", new FileStreamOptions() { Access = FileAccess.Read, Mode = FileMode.Open, Share = FileShare.Read });
        List<string> lines = new();

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            lines.Add(line);
        }

        return lines.ToArray();
    }

    public IEnumerable<string> ReadLinesEnumerable()
    {
        using StreamReader reader = new(@"EventsBenchmark.csv", new FileStreamOptions() { Access = FileAccess.Read, Mode = FileMode.Open, Share = FileShare.Read });
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            yield return line;
        }
    }

    [Benchmark]
    public List<EventModel> Import_ParallelForeach()
    {
        List<string> rows = this.ReadLinesEnumerable().Skip(1).ToList();
        List<EventModel> events = new(rows.Count);
        object lockObj = new();

        Parallel.ForEach(rows, (line, _, lineNumber) =>
        {
            string[] columns = line.Split(',');

            if (columns.Length != 17)
            {
                return;
            }

            try
            {
                EventModel eventModel = ConvertRowToModel(columns, lineNumber + 2);
                DateTime eventDate = new(eventModel.Date.Year, eventModel.Date.Month, eventModel.Date.Day,
                                         eventModel.Time.Hour, eventModel.Time.Minute, eventModel.Time.Second);

                if (DateTime.Compare(eventDate, _fromDate) >= 0 && DateTime.Compare(eventDate, _toDate) <= 0)
                {
                    lock (lockObj)
                    {
                        events.Add(eventModel);
                    }
                }
            }
            catch (FormatException)
            {
                return;
            }
            catch (IndexOutOfRangeException)
            {
                return;
            }
        });

        return events;
    }

    public List<EventModel> Import_ParallelFor()
    {
        List<string> rows = this.ReadLinesEnumerable().Skip(1).ToList();
        List<EventModel> events = new(rows.Count);
        object lockObj = new();

        Parallel.For(0, rows.Count, index =>
        {
            string[] columns = rows[index].Split(',');

            if (columns.Length != 17)
            {
                return;
            }

            try
            {
                EventModel eventModel = ConvertRowToModel(columns, index + 2);
                DateTime eventDate = new(eventModel.Date.Year, eventModel.Date.Month, eventModel.Date.Day,
                                         eventModel.Time.Hour, eventModel.Time.Minute, eventModel.Time.Second);

                if (DateTime.Compare(eventDate, _fromDate) >= 0 && DateTime.Compare(eventDate, _toDate) <= 0)
                {
                    lock (lockObj)
                    {
                        events.Add(eventModel);
                    }
                }
            }
            catch (FormatException)
            {
                return;
            }
            catch (IndexOutOfRangeException)
            {
                return;
            }
        });

        return events;
    }

    [Benchmark(Baseline = true)]
    public List<EventModel> Import_Foreach()
    {
        IEnumerable<string> rows = this.ReadLinesEnumerable().Skip(1);
        List<EventModel> events = new();

        int lineNumber = 2;
        foreach (string line in rows)
        {
            string[] columns = line.Split(',');

            if (columns.Length != 17)
            {
                lineNumber++;
                continue;
            }

            try
            {
                EventModel eventModel = ConvertRowToModel(columns, lineNumber);
                DateTime eventDate = new(eventModel.Date.Year, eventModel.Date.Month, eventModel.Date.Day,
                                         eventModel.Time.Hour, eventModel.Time.Minute, eventModel.Time.Second);

                if (DateTime.Compare(eventDate, _fromDate) >= 0 && DateTime.Compare(eventDate, _toDate) <= 0)
                {
                    events.Add(eventModel);
                }
            }
            catch (FormatException)
            {
                lineNumber++;
                continue;
            }
            catch (IndexOutOfRangeException)
            {
                lineNumber++;
                continue;
            }

            lineNumber++;
        }

        return events;
    }

    [Benchmark]
    public List<EventModel> Import_ForeachLinq()
    {
        List<string> rows = this.ReadLinesEnumerable().Skip(1).ToList();
        List<EventModel> events = new();

        int lineNumber = 2;
        rows.ForEach(line =>
        {
            string[] columns = line.Split(',');

            if (columns.Length != 17)
            {
                lineNumber++;
                return;
            }

            try
            {
                EventModel eventModel = ConvertRowToModel(columns, lineNumber);
                DateTime eventDate = new(eventModel.Date.Year, eventModel.Date.Month, eventModel.Date.Day,
                                         eventModel.Time.Hour, eventModel.Time.Minute, eventModel.Time.Second);

                if (DateTime.Compare(eventDate, _fromDate) >= 0 && DateTime.Compare(eventDate, _toDate) <= 0)
                {
                    events.Add(eventModel);
                }
            }
            catch (FormatException)
            {
                lineNumber++;
                return;
            }
            catch (IndexOutOfRangeException)
            {
                lineNumber++;
                return;
            }

            lineNumber++;
        });

        return events;
    }

    [Benchmark]
    public List<EventModel> Import_For()
    {
        List<string> rows = this.ReadLinesEnumerable().Skip(1).ToList();
        List<EventModel> events = new(rows.Count);

        for (int i = 0; i < rows.Count; i++)
        {
            string[] columns = rows[i].Split(',');

            if (columns.Length != 17)
            {
                continue;
            }

            try
            {
                EventModel eventModel = ConvertRowToModel(columns, i + 2);
                DateTime eventDate = new(eventModel.Date.Year, eventModel.Date.Month, eventModel.Date.Day,
                                         eventModel.Time.Hour, eventModel.Time.Minute, eventModel.Time.Second);

                if (DateTime.Compare(eventDate, _fromDate) >= 0 && DateTime.Compare(eventDate, _toDate) <= 0)
                {
                    events.Add(eventModel);
                }
            }
            catch (FormatException)
            {
                continue;
            }
            catch (IndexOutOfRangeException)
            {
                continue;
            }
        }

        return events;
    }

    [Benchmark]
    public List<EventModel> Import_PLinq()
    {
        List<string> rows = this.ReadLinesEnumerable().Skip(1).ToList();
        List<EventModel> events = new(rows.Count);
        object lockObj = new();

        rows.AsParallel().ForAll(line =>
        {
            string[] columns = line.Split(',');

            if (columns.Length != 17)
            {
                return;
            }

            try
            {
                EventModel eventModel = ConvertRowToModel(columns, 0);
                DateTime eventDate = new(eventModel.Date.Year, eventModel.Date.Month, eventModel.Date.Day,
                                         eventModel.Time.Hour, eventModel.Time.Minute, eventModel.Time.Second);

                if (DateTime.Compare(eventDate, _fromDate) >= 0 && DateTime.Compare(eventDate, _toDate) <= 0)
                {
                    lock (lockObj)
                    {
                        events.Add(eventModel);
                    }
                }
            }
            catch (FormatException)
            {
                return;
            }
            catch (IndexOutOfRangeException)
            {
                return;
            }
        });

        return events;
    }

    [Benchmark]
    public List<EventModel> Import_ForSpanList()
    {
        List<string> rows = this.ReadLinesEnumerable().Skip(1).ToList();
        List<EventModel> events = new(rows.Count);

        Span<string> rowsAsSpan = CollectionsMarshal.AsSpan(rows);
        for (int i = 0; i < rowsAsSpan.Length; i++)
        {
            string[] columns = rowsAsSpan[i].Split(',');

            if (columns.Length != 17)
            {
                continue;
            }

            try
            {
                EventModel eventModel = ConvertRowToModel(columns, i + 2);
                DateTime eventDate = new(eventModel.Date.Year, eventModel.Date.Month, eventModel.Date.Day,
                                         eventModel.Time.Hour, eventModel.Time.Minute, eventModel.Time.Second);

                if (DateTime.Compare(eventDate, _fromDate) >= 0 && DateTime.Compare(eventDate, _toDate) <= 0)
                {
                    events.Add(eventModel);
                }
            }
            catch (FormatException)
            {
                continue;
            }
            catch (IndexOutOfRangeException)
            {
                continue;
            }
        }

        return events;
    }

    [Benchmark]
    public List<EventModel> Import_ForSpanArray()
    {
        string[] rows = this.ReadLinesEnumerable().Skip(1).ToArray();
        List<EventModel> events = new(rows.Length);
        Span<string> rowsSpan = rows.AsSpan();

        for (int i = 0; i < rowsSpan.Length; i++)
        {
            string[] columns = rowsSpan[i].Split(',');

            if (columns.Length != 17)
            {
                continue;
            }

            try
            {
                EventModel eventModel = ConvertRowToModel(columns, i + 2);
                DateTime eventDate = new(eventModel.Date.Year, eventModel.Date.Month, eventModel.Date.Day,
                                         eventModel.Time.Hour, eventModel.Time.Minute, eventModel.Time.Second);

                if (DateTime.Compare(eventDate, _fromDate) >= 0 && DateTime.Compare(eventDate, _toDate) <= 0)
                {
                    events.Add(eventModel);
                }
            }
            catch (FormatException)
            {
                continue;
            }
            catch (IndexOutOfRangeException)
            {
                continue;
            }
        }

        return events;
    }

    [Benchmark]
    public List<EventModel> Import_ForeachSpanList()
    {
        List<string> rows = this.ReadLinesEnumerable().Skip(1).ToList();
        List<EventModel> events = new(rows.Count);

        foreach (string line in CollectionsMarshal.AsSpan(rows))
        {
            string[] columns = line.Split(',');

            if (columns.Length != 17)
            {
                continue;
            }

            try
            {
                EventModel eventModel = ConvertRowToModel(columns, 0);
                DateTime eventDate = new(eventModel.Date.Year, eventModel.Date.Month, eventModel.Date.Day,
                                         eventModel.Time.Hour, eventModel.Time.Minute, eventModel.Time.Second);

                if (DateTime.Compare(eventDate, _fromDate) >= 0 && DateTime.Compare(eventDate, _toDate) <= 0)
                {
                    events.Add(eventModel);
                }
            }
            catch (FormatException)
            {
                continue;
            }
            catch (IndexOutOfRangeException)
            {
                continue;
            }
        }

        return events;
    }

    [Benchmark]
    public List<EventModel> Import_ForeachSpanArray()
    {
        string[] rows = this.ReadLinesEnumerable().Skip(1).ToArray();
        List<EventModel> events = new(rows.Length);

        foreach (string line in rows.AsSpan())
        {
            string[] columns = line.Split(',');

            if (columns.Length != 17)
            {
                continue;
            }

            try
            {
                EventModel eventModel = ConvertRowToModel(columns, 0);
                DateTime eventDate = new(eventModel.Date.Year, eventModel.Date.Month, eventModel.Date.Day,
                                         eventModel.Time.Hour, eventModel.Time.Minute, eventModel.Time.Second);

                if (DateTime.Compare(eventDate, _fromDate) >= 0 && DateTime.Compare(eventDate, _toDate) <= 0)
                {
                    events.Add(eventModel);
                }
            }
            catch (FormatException)
            {
                continue;
            }
            catch (IndexOutOfRangeException)
            {
                continue;
            }
        }

        return events;
    }

    [Benchmark]
    public List<EventModel> Import_While()
    {
        List<string> rows = this.ReadLinesEnumerable().Skip(1).ToList();
        List<EventModel> events = new(rows.Count);
        int index = 0;

        while (index < rows.Count)
        {
            string[] columns = rows[index].Split(',');

            if (columns.Length != 17)
            {
                index++;
                continue;
            }

            try
            {
                EventModel eventModel = ConvertRowToModel(columns, index + 2);
                DateTime eventDate = new(eventModel.Date.Year, eventModel.Date.Month, eventModel.Date.Day,
                                         eventModel.Time.Hour, eventModel.Time.Minute, eventModel.Time.Second);

                if (DateTime.Compare(eventDate, _fromDate) >= 0 && DateTime.Compare(eventDate, _toDate) <= 0)
                {
                    events.Add(eventModel);
                }
            }
            catch (FormatException)
            {
                continue;
            }
            catch (IndexOutOfRangeException)
            {
                continue;
            }
            finally
            {
                index++;
            }
        }

        return events;
    }

    [Benchmark]
    public async Task<List<EventModel>> Import_ParallelForeachAsync()
    {
        IEnumerable<string> rows = this.ReadLinesEnumerable().Skip(1);
        List<EventModel> events = new();
        object lockObj = new();

        await Parallel.ForEachAsync(rows, async (line, token) =>
        {
            string[] columns = line.Split(',');

            if (columns.Length != 17)
            {
                return;
            }

            try
            {
                await Task.Run(() =>
                {
                    EventModel eventModel = ConvertRowToModel(columns, 0);
                    DateTime eventDate = new(eventModel.Date.Year, eventModel.Date.Month, eventModel.Date.Day,
                                             eventModel.Time.Hour, eventModel.Time.Minute, eventModel.Time.Second);

                    if (DateTime.Compare(eventDate, _fromDate) >= 0 && DateTime.Compare(eventDate, _toDate) <= 0)
                    {
                        lock (lockObj)
                        {
                            events.Add(eventModel);
                        }
                    }
                }, token);
            }
            catch (FormatException)
            {
                return;
            }
            catch (IndexOutOfRangeException)
            {
                return;
            }
        });

        return events;
    }

    [Benchmark]
    public List<EventModel> Import_ForMemoryMarshal()
    {
        string[] rows = this.ReadLinesEnumerable().Skip(1).ToArray();
        List<EventModel> events = new();
        ref var searchSpace = ref MemoryMarshal.GetArrayDataReference(rows);

        for (int i = 0; i < rows.Length; i++)
        {
            string row = Unsafe.Add(ref searchSpace, i);
            string[] columns = row.Split(',');

            if (columns.Length != 17)
            {
                continue;
            }

            try
            {
                EventModel eventModel = ConvertRowToModel(columns, i + 2);
                DateTime eventDate = new(eventModel.Date.Year, eventModel.Date.Month, eventModel.Date.Day,
                                            eventModel.Time.Hour, eventModel.Time.Minute, eventModel.Time.Second);

                if (DateTime.Compare(eventDate, _fromDate) >= 0 && DateTime.Compare(eventDate, _toDate) <= 0)
                {
                    events.Add(eventModel);
                }
            }
            catch (FormatException)
            {
                continue;
            }
            catch (IndexOutOfRangeException)
            {
                continue;
            }
        }

        return events;
    }

    [Benchmark]
    public List<EventModel> Import_WhileMemoryMarshal()
    {
        string[] rows = this.ReadLinesEnumerable().Skip(1).ToArray();
        List<EventModel> events = new();
        ref string start = ref MemoryMarshal.GetArrayDataReference(rows);
        ref string end = ref Unsafe.Add(ref start, rows.Length);

        int lineNumber = 2;
        while (Unsafe.IsAddressLessThan(ref start, ref end))
        {
            string[] columns = start.Split(',');

            if (columns.Length != 17)
            {
                start = ref Unsafe.Add(ref start, 1);
                lineNumber++;
                continue;
            }

            try
            {
                EventModel eventModel = ConvertRowToModel(columns, lineNumber);
                DateTime eventDate = new(eventModel.Date.Year, eventModel.Date.Month, eventModel.Date.Day,
                                         eventModel.Time.Hour, eventModel.Time.Minute, eventModel.Time.Second);

                if (DateTime.Compare(eventDate, _fromDate) >= 0 && DateTime.Compare(eventDate, _toDate) <= 0)
                {
                    events.Add(eventModel);
                }
            }
            catch (FormatException)
            {
                continue;
            }
            catch (IndexOutOfRangeException)
            {
                continue;
            }
            finally
            {
                start = ref Unsafe.Add(ref start, 1);
                lineNumber++;
            }
        }

        return events;
    }

    private static EventModel ConvertRowToModel(string[] columns, long lineNumber = 0)
    {
        DateOnly date = ConvertColumnToDate(columns[0]);
        TimeOnly time = ConvertColumnToTime(columns[1]);
        TimeZoneInfo timezone = TimeZoneInfo.FindSystemTimeZoneById(columns[2]);
        string mACB = columns[3];
        string source = columns[4];
        string sourceType = columns[5];
        string type = columns[6];
        string user = columns[7];
        string host = columns[8];
        string shortDescription = columns[9];
        string description = columns[10];
        double version = double.Parse(columns[11]);
        string filename = columns[12];
        string iNode = columns[13];
        string notes = columns[14];
        string format = columns[15];
        Dictionary<string, string> extra = ConvertColumnToExtra(columns[16]);

        EventModel newEvent = new(date, time, timezone, mACB, source, sourceType, type, user, host, shortDescription, description, version, filename, iNode, notes, format, extra);
        return newEvent;
    }

    private static DateOnly ConvertColumnToDate(string column)
    {
        string[] dateParts = column.Split('/');
        int year = int.Parse(dateParts[2]);
        int month = int.Parse(dateParts[0]);
        int day = int.Parse(dateParts[1]);

        DateOnly date = new(year, month, day);
        return date;
    }

    private static TimeOnly ConvertColumnToTime(string column)
    {
        string[] timeParts = column.Split(':');
        int hours = int.Parse(timeParts[0]);
        int minutes = int.Parse(timeParts[1]);
        int seconds = int.Parse(timeParts[2]);
        TimeOnly time = new(hours, minutes, seconds);
        return time;
    }

    private static Dictionary<string, string> ConvertColumnToExtra(string column)
    {
        string[] extraParts = column.Split(';');
        Dictionary<string, string> extra = new(extraParts.Length);

        foreach (string part in extraParts)
        {
            string[] pair = part.Split(":");
            string key = pair[0].Trim();
            string value = pair[1].Trim();

            extra.Add(key, value);
        }

        return extra;
    }
}
