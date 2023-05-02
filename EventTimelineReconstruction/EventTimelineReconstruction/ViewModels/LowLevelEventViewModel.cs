using System;

namespace EventTimelineReconstruction.ViewModels;

public sealed class LowLevelEventViewModel : ISerializableLevel
{
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public string Source { get; set; }
    public string Short { get; set; }
    public string Visit { get; set; }
    public string Extra { get; set; }
    public int Reference { get; set; }

    public LowLevelEventViewModel()
    {
        Visit = "-";
        Extra = "-";
    }

    public LowLevelEventViewModel(DateOnly date, TimeOnly time, string source, string shortDesc, string visit, string extra, int reference)
    {
        Date = new(date.Year, date.Month, date.Day);
        Time = new(time.Hour, time.Minute, time.Second);
        Source = source;
        Short = shortDesc;
        Visit = visit;
        Extra = extra;
        Reference = reference;
    }

    public string Serialize()
    {
        return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
            Date.Year, Date.Month, Date.Day, Time.Hour, Time.Minute, Time.Second,
            Source,
            Short,
            Visit,
            Extra,
            Reference
            );
    }
}
