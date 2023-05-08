using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using EventTimelineReconstruction.Factory;
using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Services;

public sealed class FileWorkLoader : IWorkLoader
{
    private const int _expectedEventColumnCount = 24;
    private readonly IAbstractionLevelFactory _factory;

    public FileWorkLoader(IAbstractionLevelFactory factory)
    {
        _factory = factory;
    }

    public async Task<LoadedWork> LoadWork(string path)
    {
        IEnumerable<string> rows = await File.ReadAllLinesAsync(path);
        IEnumerator<string> enumerator = rows.GetEnumerator();

        LoadedWork loadedWork = new()
        {
            Events = LoadEvents(enumerator),
            HighLevelEvents = LoadAbstractionLevel(enumerator, AbstractionLevel.HighLevelEvent),
            LowLevelEvents = LoadAbstractionLevel(enumerator, AbstractionLevel.LowLevelEvent),
            HighLevelArtefacts = LoadAbstractionLevel(enumerator, AbstractionLevel.HighLevelArtefact),
            LowLevelArtefacts = LoadAbstractionLevel(enumerator, AbstractionLevel.LowLevelArtefact)
        };

        return loadedWork;
    }

    private List<EventViewModel> LoadEvents(IEnumerator<string> enumerator)
    {
        int currentDepth = 0;
        List<EventViewModel> events = new();
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

    private static int GetDepth(string col)
    {
        int depth = 0;

        while (col[depth] == '\t') {
            depth++;
        }

        return depth;
    }

    private static EventModel ConvertRowToModel(string[] columns)
    {
        DateOnly date = ConvertColumnsToDate(columns[0], columns[1], columns[2]);
        TimeOnly time = ConvertColumnsToTime(columns[3], columns[4], columns[5]);
        TimeZoneInfo timezone = TimeZoneInfo.FromSerializedString(GetSerializedTimezoneString(columns, _expectedEventColumnCount));
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

        foreach (string part in extraParts) {
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
        Brush brush = (Brush)new BrushConverter().ConvertFromString(columns[^1]);
        brush.Freeze();
        eventViewModel.Colour = brush;

        return eventViewModel;
    }

    private List<ISerializableLevel> LoadAbstractionLevel(IEnumerator<string> enumerator, AbstractionLevel abstractionLevel)
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
