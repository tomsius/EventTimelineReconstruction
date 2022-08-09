using System.Collections.Generic;
using EventTimelineReconstruction.Models;
using System.Linq;
using System.Threading.Tasks;
using EventTimelineReconstruction.ViewModels;
using System;
using System.IO;
using System.Windows.Media;

namespace EventTimelineReconstruction.Services;
public class FileWorkLoader : IWorkLoader
{
    public List<EventViewModel> LoadWork(string path)
    {
        IEnumerable<string> rows = File.ReadLines(path);
        List<EventViewModel> events = new(rows.Count());
        object lockObj = new();

        Parallel.ForEach(rows, line => {
            string[] columns = line.Split(',');
            EventModel eventModel = ConvertRowToModel(columns);
            EventViewModel eventViewModel = new(eventModel);
            eventViewModel.IsVisible = bool.Parse(columns[21]);
            Brush brush = (Brush)new BrushConverter().ConvertFromString(columns[22]);
            brush.Freeze();
            eventViewModel.Colour = brush;

            lock (lockObj) {
                events.Add(eventViewModel);
            }
        });

        return events;
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
        double version = double.Parse(columns[15]);
        string filename = columns[16];
        string iNode = columns[17];
        string notes = columns[18];
        string format = columns[19];
        Dictionary<string, string> extra = ConvertColumnToExtra(columns[20]);

        EventModel newEvent = new(date, time, timezone, mACB, source, sourceType, type, user, host, shortDescription, description, version, filename, iNode, notes, format, extra);
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
}
