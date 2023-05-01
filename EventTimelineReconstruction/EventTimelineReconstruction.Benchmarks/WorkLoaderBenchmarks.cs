using System.Globalization;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using EventTimelineReconstruction.Benchmarks.Factory;
using EventTimelineReconstruction.Benchmarks.Models;

namespace EventTimelineReconstruction.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class WorkLoaderBenchmarks
{
    [Params(1000, 10_000, 100_000, 1_000_000)]
    public int N;

    private AbstractionLevelFactory _factory;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _factory = new();

        if (!File.Exists(@"EventsBenchmark.etr"))
        {
            using StreamWriter writeStream = new(@"EventsBenchmark.etr", false);

            for (int i = 0; i < N / 5; i += 4)
            {
                writeStream.WriteLine("2020,4,6,5,23,0,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,MACB1,Source1,Source Type1,Type1,Username1,Hostname1,Short Description1,Full Description1,2.5,Filename1,iNode number1,Notes1,Format1,Key11:Value11;Key12:Value12,1,True,#FF000000");
                writeStream.WriteLine("\t2021,12,1,17,53,0,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,MACB3,Source3,Source Type3,Type3,Username3,Hostname3,Short Description3,Full Description3,2.5,Filename3,iNode number3,Notes3,Format3,Key31:Value31;Key32:Value32,3,True,#FF000000");
                writeStream.WriteLine("\t\t2021,12,1,17,53,0,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,MACB4,Source4,Source Type4,Type4,Username4,Hostname4,Short Description4,Full Description4,2.5,Filename4,iNode number4,Notes4,Format4,Key41:Value41;Key42:Value42,4,True,#FF000000");
                writeStream.WriteLine("\t2021,12,1,17,53,0,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,MACB5,Source5,Source Type5,Type5,Username5,Hostname5,Short Description5,Full Description5,2.5,Filename5,iNode number5,Notes5,Format5,Key51:Value51;Key52:Value52,5,True,#FF000000");
            }
            writeStream.WriteLine();
            for (int i = 0; i < N / 5; i++)
            {
                writeStream.WriteLine("2020,1,1,4,20,54,Source4,Short4,Visit4,4");
            }
            writeStream.WriteLine();
            for (int i = 0; i < N / 5; i++)
            {
                writeStream.WriteLine("2020,1,3,4,22,54,Source6,Short6,Visit6,Extra6,6");
            }
            writeStream.WriteLine();
            for (int i = 0; i < N / 5; i++)
            {
                writeStream.WriteLine("2020,1,5,4,22,54,Source8,Short8,Visit8,Extra8,8,MACB8,SourceType8,Desc8");
            }
            writeStream.WriteLine();
            for (int i = 0; i < N / 5; i++)
            {
                writeStream.WriteLine("2020,1,7,4,22,54,Vilnius,MACB10,Source10,SourceType10,Type10,User10,Host10,Short10,Desc10,2,Filename10,Inode10,Notes10,Format10,Extra10,10");
            }
        }
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        File.Delete(@"EventsBenchmark.etr");
    }

    public IEnumerable<string> ReadLinesEnumerable()
    {
        using StreamReader reader = new(@"EventsBenchmark.etr", new FileStreamOptions() { Access = FileAccess.Read, Mode = FileMode.Open, Share = FileShare.Read });
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            yield return line;
        }
    }

    [Benchmark(Baseline = true)]
    public async Task<LoadedWork> LoadWork_LineByLine()
    {
        using StreamReader inputStream = new(@"EventsBenchmark.etr", new FileStreamOptions() { Access = FileAccess.Read, Mode = FileMode.Open, Share = FileShare.Read });
        //using StreamReader inputStream = new(path);

        LoadedWork loadedWork = new()
        {
            Events = await LoadEvents_LineByLine(inputStream),
            HighLevelEvents = await LoadHighLevelEvents_LineByLine(inputStream),
            LowLevelEvents = await LoadLowLevelEvents_LineByLine(inputStream),
            HighLevelArtefacts = await LoadHighLevelArtefacts_LineByLine(inputStream),
            LowLevelArtefacts = await LoadLowLevelArtefacts_LineByLine(inputStream)
        };

        return loadedWork;
    }

    private static async Task<List<EventViewModel>> LoadEvents_LineByLine(StreamReader inputStream)
    {
        List<EventViewModel> events = new();
        string row = await inputStream.ReadLineAsync();
        int currentDepth = 0;
        Stack<EventViewModel> stack = new();

        while (row != "")
        {
            string[] columns = row.Split(',');
            int depth = GetDepth(columns[0]);
            columns[0] = columns[0].Trim(new char[] { '\t' });

            EventModel eventModel = ConvertRowToModel(columns);
            EventViewModel eventViewModel = ConvertToViewModel(eventModel, columns);

            if (depth == 0)
            {
                events.Add(eventViewModel);
            }
            else if (depth == currentDepth)
            {
                while (depth <= currentDepth)
                {
                    stack.Pop();
                    currentDepth--;
                }

                currentDepth++;

                EventViewModel current = stack.Peek();
                current.AddChild(eventViewModel);
            }
            else if (depth > currentDepth)
            {
                currentDepth++;
                EventViewModel current = stack.Peek();
                current.AddChild(eventViewModel);
            }
            else if (depth < currentDepth)
            {
                while (depth <= currentDepth)
                {
                    stack.Pop();
                    currentDepth--;
                }

                currentDepth++;
                EventViewModel current = stack.Peek();
                current.AddChild(eventViewModel);
            }

            stack.Push(eventViewModel);
            row = await inputStream.ReadLineAsync();
        }

        return events;
    }

    private static int GetDepth(string col)
    {
        int depth = 0;

        while (col[depth] == '\t')
        {
            depth++;
        }

        return depth;
    }

    private static EventModel ConvertRowToModel(string[] columns)
    {
        DateOnly date = ConvertColumnsToDate(columns[0], columns[1], columns[2]);
        TimeOnly time = ConvertColumnsToTime(columns[3], columns[4], columns[5]);
        TimeZoneInfo timezone = TimeZoneInfo.FromSerializedString(GetSerializedTimezoneString(columns, 24));
        string mACB = columns[^17];
        string source = columns[^16];
        string sourceType = columns[^15];
        string type = columns[^14];
        string user = columns[^13];
        string host = columns[^12];
        string shortDescription = columns[^11];
        string description = columns[^10];
        double version = double.Parse(columns[^9], CultureInfo.InvariantCulture);
        string filename = columns[^8];
        string iNode = columns[^7];
        string notes = columns[^6];
        string format = columns[^5];
        Dictionary<string, string> extra = ConvertColumnToExtra(columns[^4]);
        int sourceLine = int.Parse(columns[^3]);

        EventModel newEvent = new(date, time, timezone, mACB, source, sourceType, type, user, host, shortDescription, description, version, filename, iNode, notes, format, extra, sourceLine);
        return newEvent;
    }

    private static string GetSerializedTimezoneString(string[] columns, int expectedColumnCount)
    {
        int offset = 6;
        int timezoneCellSpan = columns.Length - expectedColumnCount;
        StringBuilder sb = new();

        for (int i = 0; i <= timezoneCellSpan; i++)
        {
            int index = offset + i;
            sb.Append(columns[index]);
            sb.Append(", ");
        }

        sb.Remove(sb.Length - 2, 2);

        return sb.ToString();
    }

    private static DateOnly ConvertColumnsToDate(string yearColumn, string monthColumn, string dayColumn)
    {
        int year = int.Parse(yearColumn);
        int month = int.Parse(monthColumn);
        int day = int.Parse(dayColumn);

        DateOnly date = new(year, month, day);
        return date;
    }

    private static TimeOnly ConvertColumnsToTime(string hoursColumn, string minutesColumn, string secondsColumn)
    {
        int hours = int.Parse(hoursColumn);
        int minutes = int.Parse(minutesColumn);
        int seconds = int.Parse(secondsColumn);
        TimeOnly time = new(hours, minutes, seconds);
        return time;
    }

    private static Dictionary<string, string> ConvertColumnToExtra(string column)
    {
        string[] extraParts = column.Split(';');
        Dictionary<string, string> extra = new(extraParts.Length);

        foreach (string part in extraParts)
        {
            string[] pair = part.Split(":", 2);
            string key = pair[0];
            string value = pair[1];

            extra.Add(key, value);
        }

        return extra;
    }

    private static EventViewModel ConvertToViewModel(EventModel eventModel, string[] columns)
    {
        EventViewModel eventViewModel = new(eventModel)
        {
            IsVisible = bool.Parse(columns[^2])
        };

        return eventViewModel;
    }

    private static async Task<List<HighLevelEventViewModel>> LoadHighLevelEvents_LineByLine(StreamReader inputStream)
    {
        List<HighLevelEventViewModel> highLevelEvents = new();

        string row = await inputStream.ReadLineAsync();

        while (row != "")
        {
            string[] columns = row.Split(',');
            HighLevelEventViewModel highLevelEvent = ConvertRowToHighLevelEvent(columns);
            highLevelEvents.Add(highLevelEvent);

            row = await inputStream.ReadLineAsync();
        }

        return highLevelEvents;
    }

    private static HighLevelEventViewModel ConvertRowToHighLevelEvent(string[] columns)
    {
        DateOnly date = ConvertColumnsToDate(columns[0], columns[1], columns[2]);
        TimeOnly time = ConvertColumnsToTime(columns[3], columns[4], columns[5]);
        string source = columns[6];
        string shortDescription = columns[7];
        string visit = columns[8];
        int reference = int.Parse(columns[9]);

        HighLevelEventViewModel highLevelEvent = new(date, time, source, shortDescription, visit, reference);
        return highLevelEvent;
    }

    private static async Task<List<LowLevelEventViewModel>> LoadLowLevelEvents_LineByLine(StreamReader inputStream)
    {
        List<LowLevelEventViewModel> lowLevelEvents = new();

        string row = await inputStream.ReadLineAsync();

        while (row != "")
        {
            string[] columns = row.Split(',');
            LowLevelEventViewModel lowLevelEvent = ConvertRowToLowLevelEvent(columns);
            lowLevelEvents.Add(lowLevelEvent);

            row = await inputStream.ReadLineAsync();
        }

        return lowLevelEvents;
    }

    private static LowLevelEventViewModel ConvertRowToLowLevelEvent(string[] columns)
    {
        DateOnly date = ConvertColumnsToDate(columns[0], columns[1], columns[2]);
        TimeOnly time = ConvertColumnsToTime(columns[3], columns[4], columns[5]);
        string source = columns[6];
        string shortDescription = columns[7];
        string visit = columns[8];
        string extra = columns[9];
        int reference = int.Parse(columns[10]);

        LowLevelEventViewModel lowLevelEvent = new(date, time, source, shortDescription, visit, extra, reference);
        return lowLevelEvent;
    }

    private static async Task<List<HighLevelArtefactViewModel>> LoadHighLevelArtefacts_LineByLine(StreamReader inputStream)
    {
        List<HighLevelArtefactViewModel> highLevelArtefacts = new();

        string row = await inputStream.ReadLineAsync();

        while (row != "")
        {
            string[] columns = row.Split(',');
            HighLevelArtefactViewModel highLevelArtefact = ConvertRowToHighLevelArtefact(columns);
            highLevelArtefacts.Add(highLevelArtefact);

            row = await inputStream.ReadLineAsync();
        }

        return highLevelArtefacts;
    }

    private static HighLevelArtefactViewModel ConvertRowToHighLevelArtefact(string[] columns)
    {
        DateOnly date = ConvertColumnsToDate(columns[0], columns[1], columns[2]);
        TimeOnly time = ConvertColumnsToTime(columns[3], columns[4], columns[5]);
        string source = columns[6];
        string shortDescription = columns[7];
        string visit = columns[8];
        string extra = columns[9];
        int reference = int.Parse(columns[10]);
        string macb = columns[11];
        string sourceType = columns[12];
        string description = columns[13];

        HighLevelArtefactViewModel highLevelArtefact = new(date, time, source, shortDescription, visit, extra, reference, macb, sourceType, description);
        return highLevelArtefact;
    }

    private static async Task<List<LowLevelArtefactViewModel>> LoadLowLevelArtefacts_LineByLine(StreamReader inputStream)
    {
        List<LowLevelArtefactViewModel> lowLevelArtefacts = new();

        string row = await inputStream.ReadLineAsync();

        while (row != null)
        {
            string[] columns = row.Split(',');
            LowLevelArtefactViewModel lowLevelArtefact = ConvertRowToLowLevelArtefact(columns);
            lowLevelArtefacts.Add(lowLevelArtefact);

            row = await inputStream.ReadLineAsync();
        }

        return lowLevelArtefacts;
    }

    private static LowLevelArtefactViewModel ConvertRowToLowLevelArtefact(string[] columns)
    {
        DateOnly date = ConvertColumnsToDate(columns[0], columns[1], columns[2]);
        TimeOnly time = ConvertColumnsToTime(columns[3], columns[4], columns[5]);
        string timezone = GetSerializedTimezoneString(columns, 22);
        string macb = columns[^15];
        string source = columns[^14];
        string sourceType = columns[^13];
        string type = columns[^12];
        string user = columns[^11];
        string host = columns[^10];
        string shortDescription = columns[^9];
        string description = columns[^8];
        string version = columns[^7];
        string filename = columns[^6];
        string inode = columns[^5];
        string notes = columns[^4];
        string format = columns[^3];
        string extra = columns[^2];
        int reference = int.Parse(columns[^1]);

        LowLevelArtefactViewModel lowLevelArtefact = new(date, time, timezone, macb, source, sourceType, type, user, host, shortDescription, description, version, filename, inode, notes, format, extra, reference);
        return lowLevelArtefact;
    }

    [Benchmark]
    public LoadedWorkFactory LoadWork_Factory()
    {
        IEnumerable<string> rows = this.ReadLinesEnumerable();
        //IEnumerable<string> rows = File.ReadAllLinesAsync();
        IEnumerator<string> enumerator = rows.GetEnumerator();

        LoadedWorkFactory loadedWork = new()
        {
            Events = LoadEvents_Factory(enumerator),
            HighLevelEvents = LoadAbstractionLevel_Factory(enumerator, AbstractionLevel.HighLevelEvent),
            LowLevelEvents = LoadAbstractionLevel_Factory(enumerator, AbstractionLevel.LowLevelEvent),
            HighLevelArtefacts = LoadAbstractionLevel_Factory(enumerator, AbstractionLevel.HighLevelArtefact),
            LowLevelArtefacts = LoadAbstractionLevel_Factory(enumerator, AbstractionLevel.LowLevelArtefact)
        };

        return loadedWork;
    }

    private static List<EventViewModel> LoadEvents_Factory(IEnumerator<string> enumerator)
    {
        List<EventViewModel> events = new();
        int currentDepth = 0;
        Stack<EventViewModel> stack = new();

        while (enumerator.MoveNext() && string.IsNullOrEmpty(enumerator.Current) == false)
        {
            string[] columns = enumerator.Current.Split(',');
            int depth = GetDepth(columns[0]);
            columns[0] = columns[0].Trim(new char[] { '\t' });

            EventModel eventModel = ConvertRowToModel(columns);
            EventViewModel eventViewModel = ConvertToViewModel(eventModel, columns);

            if (depth == 0)
            {
                events.Add(eventViewModel);
            }
            else if (depth == currentDepth)
            {
                while (depth <= currentDepth)
                {
                    stack.Pop();
                    currentDepth--;
                }

                currentDepth++;

                EventViewModel current = stack.Peek();
                current.AddChild(eventViewModel);
            }
            else if (depth > currentDepth)
            {
                currentDepth++;
                EventViewModel current = stack.Peek();
                current.AddChild(eventViewModel);
            }
            else if (depth < currentDepth)
            {
                while (depth <= currentDepth)
                {
                    stack.Pop();
                    currentDepth--;
                }

                currentDepth++;
                EventViewModel current = stack.Peek();
                current.AddChild(eventViewModel);
            }

            stack.Push(eventViewModel);
        }

        return events;
    }

    private List<ISerializableLevel> LoadAbstractionLevel_Factory(IEnumerator<string> enumerator, AbstractionLevel abstractionLevel)
    {
        List<ISerializableLevel> abstractionEvents = new();

        while (enumerator.MoveNext() && string.IsNullOrEmpty(enumerator.Current) == false)
        {
            ISerializableLevel abstractionEvent = _factory.CreateAbstractionLevel(enumerator.Current, abstractionLevel);
            abstractionEvents.Add(abstractionEvent);
        }

        return abstractionEvents;
    }
}
