using System;

namespace EventTimelineReconstruction.ViewModels;

public sealed class HighLevelArtefactViewModel : ISerializableLevel
{
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public string Source { get; set; }
    public string Short { get; set; }
    public string Visit { get; set; }
    public string Extra { get; set; }
    public int Reference { get; set; }
    public string Macb { get; set; }
    public string SourceType { get; set; }
    public string Description { get; set; }

    public HighLevelArtefactViewModel()
    {
        Visit = "-";
        Extra = "-";
        Description = "-";
    }

    public HighLevelArtefactViewModel(DateOnly date, TimeOnly time, string source, string shortDesc, string visit, string extra, int reference, string macb, string sourceType, string description)
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

    public static HighLevelArtefactViewModel Deserialize(string row)
    {
        string[] columns = row.Split(',');
        return ConvertRowToHighLevelArtefact(columns);
    }

    private static HighLevelArtefactViewModel ConvertRowToHighLevelArtefact(string[] columns)
    {
        DateOnly date = new(int.Parse(columns[0]), int.Parse(columns[1]), int.Parse(columns[2]));
        TimeOnly time = new(int.Parse(columns[3]), int.Parse(columns[4]), int.Parse(columns[5]));
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
}
