using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Services;

public class FileWorkLoader : IWorkLoader
{
    private const int _expectedEventColumnCount = 24;
    private const int _expectedLowLevelArtefactColumnCount = 22;

    public async Task<LoadedWork> LoadWork(string path)
    {
        using StreamReader inputStream = new(path);

        LoadedWork loadedWork = new();
        loadedWork.Events = await this.LoadEvents(inputStream);
        loadedWork.HighLevelEvents = await this.LoadHighLevelEvents(inputStream);
        loadedWork.LowLevelEvents = await this.LoadLowLevelEvents(inputStream);
        loadedWork.HighLevelArtefacts = await this.LoadHighLevelArtefacts(inputStream);
        loadedWork.LowLevelArtefacts = await this.LoadLowLevelArtefacts(inputStream);

        return loadedWork;
    }

    private async Task<List<EventViewModel>> LoadEvents(StreamReader inputStream)
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
                while (depth <= currentDepth) {
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
        string sourceLine = columns[^3];

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
            string[] pair = part.Split(":");
            string key = pair[0];
            string value = pair[1];

            extra.Add(key, value);
        }

        return extra;
    }

    private static EventViewModel ConvertToViewModel(EventModel eventModel, string[] columns)
    {
        EventViewModel eventViewModel = new(eventModel);
        eventViewModel.IsVisible = bool.Parse(columns[^2]);
        Brush brush = (Brush)new BrushConverter().ConvertFromString(columns[^1]);
        brush.Freeze();
        eventViewModel.Colour = brush;

        return eventViewModel;
    }

    private async Task<List<HighLevelEventViewModel>> LoadHighLevelEvents(StreamReader inputStream)
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
        string reference = columns[9];

        HighLevelEventViewModel highLevelEvent = new(date, time, source, shortDescription, visit, reference);
        return highLevelEvent;
    }

    private async Task<List<LowLevelEventViewModel>> LoadLowLevelEvents(StreamReader inputStream)
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
        string reference = columns[10];

        LowLevelEventViewModel lowLevelEvent = new(date, time, source, shortDescription, visit, extra, reference);
        return lowLevelEvent;
    }

    private async Task<List<HighLevelArtefactViewModel>> LoadHighLevelArtefacts(StreamReader inputStream)
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
        string reference = columns[10];
        string macb = columns[11];
        string sourceType = columns[12];
        string description = columns[13];

        HighLevelArtefactViewModel highLevelArtefact = new(date, time, source, shortDescription, visit, extra, reference, macb, sourceType, description);
        return highLevelArtefact;
    }

    private async Task<List<LowLevelArtefactViewModel>> LoadLowLevelArtefacts(StreamReader inputStream)
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
        string timezone = GetSerializedTimezoneString(columns, _expectedLowLevelArtefactColumnCount);
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
        string reference = columns[^1];

        LowLevelArtefactViewModel lowLevelArtefact = new (date, time, timezone, macb, source, sourceType, type, user, host, shortDescription, description, version, filename, inode, notes, format, extra, reference);
        return lowLevelArtefact;
    }
}
