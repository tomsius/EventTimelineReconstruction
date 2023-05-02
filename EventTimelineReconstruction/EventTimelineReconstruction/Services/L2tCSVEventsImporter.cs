using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EventTimelineReconstruction.Models;

namespace EventTimelineReconstruction.Services;

public sealed class L2tCSVEventsImporter : IEventsImporter
{
    private const int _colCount = 17;

    public List<EventModel> Import(string path, DateTime fromDate, DateTime toDate)
    {
        IEnumerable<string> rows = File.ReadLines(path).Skip(1);
        IEnumerator<string> enumerator = rows.GetEnumerator();
        List<EventModel> events = new();
        int lineNumber = 2;

        while (enumerator.MoveNext())
        {
            string[] columns = enumerator.Current.Split(',');

            if (columns.Length != _colCount)
            {
                lineNumber++;
                continue;
            }

            try
            {
                EventModel eventModel = ConvertRowToModel(columns, lineNumber);
                DateTime eventDate = new(eventModel.Date.Year, eventModel.Date.Month, eventModel.Date.Day,
                                         eventModel.Time.Hour, eventModel.Time.Minute, eventModel.Time.Second);

                if (DateTime.Compare(eventDate, fromDate) >= 0 && DateTime.Compare(eventDate, toDate) <= 0)
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
                lineNumber++;
            }
        }

        return events;
    }

    private static EventModel ConvertRowToModel(string[] columns, long lineNumber)
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
        int sourceLine = (int)lineNumber;

        EventModel newEvent = new(date, time, timezone, mACB, source, sourceType, type, user, host, shortDescription, description, version, filename, iNode, notes, format, extra, sourceLine);
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

        foreach (string part in extraParts) {
            string[] pair = part.Split(":", 2);
            string key = pair[0].Trim();
            string value = pair[1].Trim();

            extra.Add(key, value);
        }

        return extra;
    }
}
