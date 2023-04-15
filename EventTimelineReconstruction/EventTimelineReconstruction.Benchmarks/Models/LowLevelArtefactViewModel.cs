using System.Text;

namespace EventTimelineReconstruction.Benchmarks.Models;

public sealed class LowLevelArtefactViewModel : ISerializableLevel
{
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public string Timezone { get; set; }
    public string Macb { get; set; }
    public string Source { get; set; }
    public string SourceType { get; set; }
    public string Type { get; set; }
    public string User { get; set; }
    public string Host { get; set; }
    public string Short { get; set; }
    public string Description { get; set; }
    public string Version { get; set; }
    public string Filename { get; set; }
    public string Inode { get; set; }
    public string Notes { get; set; }
    public string Format { get; set; }
    public string Extra { get; set; }
    public int Reference { get; set; }

    public LowLevelArtefactViewModel() {}

    public LowLevelArtefactViewModel(DateOnly date, TimeOnly time, string timezone, string macb, string source, string sourceType, string type, string user, string host, string shortDesc, string description, string version, string filename, string inode, string notes, string format, string extra, int reference)
    {
        Date = new(date.Year, date.Month, date.Day);
        Time = new(time.Hour, time.Minute, time.Second);
        Timezone = timezone;
        Macb = macb;
        Source = source;
        SourceType = sourceType;
        Type = type;
        User = user;
        Host = host;
        Short = shortDesc;
        Description = description;
        Version = version;
        Filename = filename;
        Inode = inode;
        Notes = notes;
        Format = format;
        Extra = extra;
        Reference = reference;
    }

    public string Serialize()
    {
        return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21}",
            Date.Year, Date.Month, Date.Day, Time.Hour, Time.Minute, Time.Second,
            Timezone,
            Macb,
            Source,
            SourceType,
            Type,
            User,
            Host,
            Short,
            Description,
            Version,
            Filename,
            Inode,
            Notes,
            Format,
            Extra,
            Reference
            );
    }

    public static LowLevelArtefactViewModel Deserialize(string row)
    {
        string[] columns = row.Split(',');
        return ConvertRowToLowLevelArtefact(columns);
    }

    private static LowLevelArtefactViewModel ConvertRowToLowLevelArtefact(string[] columns)
    {
        DateOnly date = new(int.Parse(columns[0]), int.Parse(columns[1]), int.Parse(columns[2]));
        TimeOnly time = new(int.Parse(columns[3]), int.Parse(columns[4]), int.Parse(columns[5]));
        string timezone = GetSerializedTimezoneString(columns, 22);
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
        int reference = int.Parse(columns[^1]);

        LowLevelArtefactViewModel lowLevelArtefact = new(date, time, timezone, macb, source, sourceType, type, user, host, shortDescription, description, version, filename, inode, notes, format, extra, reference);
        return lowLevelArtefact;
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
}
