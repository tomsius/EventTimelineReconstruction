using System;
using System.Collections.Generic;

namespace EventTimelineReconstruction.Models;

public class EventModel
{
    public DateOnly Date { get; }
    public TimeOnly Time { get; }
    public TimeZoneInfo Timezone { get; }
    public string MACB { get; }
    public string Source { get; }
    public string SourceType { get; }
    public string Type { get; }
    public string User { get; }
    public string Host { get; }
    public string Short { get; }
    public string Description { get; }
    public double Version { get; }
    public string Filename { get; }
    public string INode { get; }
    public string Notes { get; }
    public string Format { get; }
    public Dictionary<string, string> Extra { get; }

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

    public override bool Equals(object obj)
    {
        if (obj is EventModel other)
        {
            if (Date.CompareTo(other.Date) != 0 ||
                Time.CompareTo(other.Time) != 0 || 
                !Timezone.Equals(other.Timezone) || 
                MACB != other.MACB || 
                Source != other.Source || 
                SourceType != other.SourceType || 
                Type != other.Type ||
                User != other.User ||
                Host != other.Host ||
                Short != other.Short ||
                Description != other.Description ||
                Version.CompareTo(other.Version) != 0 ||
                Filename != other.Filename ||
                INode != other.INode ||
                Notes != other.Notes ||
                Format != other.Format ||
                Extra.Count != other.Extra.Count)
            {
                return false;
            }

            foreach (KeyValuePair<string, string> pair in Extra)
            {
                string key = pair.Key;
                string value = pair.Value;

                if (!other.Extra.ContainsKey(key) || value != other.Extra[key])
                {
                    return false;
                }
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public override int GetHashCode()
    {
        HashCode hashCode = new();
        hashCode.Add(Date);
        hashCode.Add(Time);
        hashCode.Add(Timezone);
        hashCode.Add(MACB);
        hashCode.Add(Source);
        hashCode.Add(SourceType);
        hashCode.Add(Type);
        hashCode.Add(User);
        hashCode.Add(Host);
        hashCode.Add(Short);
        hashCode.Add(Description);
        hashCode.Add(Version);
        hashCode.Add(Filename);
        hashCode.Add(INode);
        hashCode.Add(Notes);
        hashCode.Add(Format);
        hashCode.Add(Extra);

        return hashCode.GetHashCode();
    }
}
