using System;

namespace EventTimelineReconstruction.ViewModels;

public sealed class HighLevelEventViewModel : ISerializableLevel
{
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public string Source { get; set; }
    public string Short { get; set; }
    public string Visit { get; set; }
    public int Reference { get; set; }

    public HighLevelEventViewModel()
    {
        Visit = "-";
    }

    public HighLevelEventViewModel(DateOnly date, TimeOnly time, string source, string shortDesc, string visit, int reference)
    {
        Date = new(date.Year, date.Month, date.Day);
        Time = new(time.Hour, time.Minute, time.Second);
        Source = source;
        Short = shortDesc;
        Visit = visit; 
        Reference = reference;
    }

    public string Serialize()
    {
        return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
            Date.Year, Date.Month, Date.Day, Time.Hour, Time.Minute, Time.Second,
            Source,
            Short,
            Visit,
            Reference
            );
    }

    public static HighLevelEventViewModel Deserialize(string row)
    {
        string[] columns = row.Split(',');
        return ConvertRowToHighLevelEvent(columns);
    }

    private static HighLevelEventViewModel ConvertRowToHighLevelEvent(string[] columns)
    {
        DateOnly date = new(int.Parse(columns[0]), int.Parse(columns[1]), int.Parse(columns[2]));
        TimeOnly time = new(int.Parse(columns[3]), int.Parse(columns[4]), int.Parse(columns[5]));
        string source = columns[6];
        string shortDescription = columns[7];
        string visit = columns[8];
        int reference = int.Parse(columns[9]);

        HighLevelEventViewModel highLevelEvent = new(date, time, source, shortDescription, visit, reference);
        return highLevelEvent;
    }
}
