using System.Collections.Generic;
using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.ViewModels;
using System;
using System.IO;
using System.Windows.Media;
using System.Globalization;
using System.Threading.Tasks;

namespace EventTimelineReconstruction.Services;

public class FileWorkLoader : IWorkLoader
{
    public async Task<List<EventViewModel>> LoadWork(string path)
    {
        List<EventViewModel> events = new();
        using StreamReader inputStream = new(path);
        string row = await inputStream.ReadLineAsync();
        int currentDepth = 0;
        Stack<EventViewModel> stack = new();

        while (row != null)
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
        TimeZoneInfo timezone = TimeZoneInfo.FromSerializedString(columns[6]);
        string mACB = columns[7];
        string source = columns[8];
        string sourceType = columns[9];
        string type = columns[10];
        string user = columns[11];
        string host = columns[12];
        string shortDescription = columns[13];
        string description = columns[14];
        double version = double.Parse(columns[15], CultureInfo.InvariantCulture);
        string filename = columns[16];
        string iNode = columns[17];
        string notes = columns[18];
        string format = columns[19];
        Dictionary<string, string> extra = ConvertColumnToExtra(columns[20]);
        string sourceLine = columns[21];

        EventModel newEvent = new(date, time, timezone, mACB, source, sourceType, type, user, host, shortDescription, description, version, filename, iNode, notes, format, extra, sourceLine);
        return newEvent;
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
        eventViewModel.IsVisible = bool.Parse(columns[22]);
        Brush brush = (Brush)new BrushConverter().ConvertFromString(columns[23]);
        brush.Freeze();
        eventViewModel.Colour = brush;

        return eventViewModel;
    }
}
