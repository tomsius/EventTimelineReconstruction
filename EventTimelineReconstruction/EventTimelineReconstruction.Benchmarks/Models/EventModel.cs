namespace EventTimelineReconstruction.Benchmarks.Models;

public class EventModel
{
    public DateOnly Date
    {
        get;
    }
    public TimeOnly Time
    {
        get;
    }
    public TimeZoneInfo Timezone
    {
        get;
    }
    public string MACB
    {
        get;
    }
    public string Source
    {
        get;
    }
    public string SourceType
    {
        get;
    }
    public string Type
    {
        get;
    }
    public string User
    {
        get;
    }
    public string Host
    {
        get;
    }
    public string Short
    {
        get;
    }
    public string Description
    {
        get;
    }
    public double Version
    {
        get;
    }
    public string Filename
    {
        get;
    }
    public string INode
    {
        get;
    }
    public string Notes
    {
        get;
    }
    public string Format
    {
        get;
    }
    public Dictionary<string, string> Extra
    {
        get;
    }

    public EventModel(
        DateOnly date,
        TimeOnly time,
        TimeZoneInfo timezone,
        string mACB,
        string source,
        string sourceType,
        string type,
        string user,
        string host,
        string shortDescription,
        string description,
        double version,
        string filename,
        string iNode,
        string notes,
        string format,
        Dictionary<string, string> extra)
    {
        Date = date;
        Time = time;
        Timezone = timezone;
        MACB = mACB;
        Source = source;
        SourceType = sourceType;
        Type = type;
        User = user;
        Host = host;
        Short = shortDescription;
        Description = description;
        Version = version;
        Filename = filename;
        INode = iNode;
        Notes = notes;
        Format = format;
        Extra = extra;
    }
}
