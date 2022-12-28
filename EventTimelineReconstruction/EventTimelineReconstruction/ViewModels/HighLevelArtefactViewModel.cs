using System;

namespace EventTimelineReconstruction.ViewModels;

public class HighLevelArtefactViewModel
{
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public string Source { get; set; }
    public string Short { get; set; }
    public string Visit { get; set; }
    public string Extra { get; set; }
    public string Reference { get; set; }
    public string Macb { get; set; }
    public string SourceType { get; set; }
    public string Description { get; set; }

    public HighLevelArtefactViewModel()
    {
        Visit = "-";
        Extra = "-";
        Description = "-";
    }

    public HighLevelArtefactViewModel(DateOnly date, TimeOnly time, string source, string shortDesc, string visit, string extra, string reference, string macb, string sourceType, string description)
    {
        Date = new(date.Year, date.Month, date.Day);
        Time = new(time.Hour, time.Minute, time.Second);
        Source = source;
        Short = shortDesc;
        Visit = visit;
        Extra = extra;
        Reference = reference;
        Macb = macb;
        SourceType = sourceType;
        Description = description;
    }

    public string Serialize()
    {
        return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}",
            Date.Year, Date.Month, Date.Day, Time.Hour, Time.Minute, Time.Second,
            Source,
            Short,
            Visit,
            Extra,
            Reference,
            Macb,
            SourceType,
            Description
            );
    }
}
