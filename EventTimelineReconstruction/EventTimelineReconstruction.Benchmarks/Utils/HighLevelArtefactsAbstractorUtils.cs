using System.Text;
using EventTimelineReconstruction.Benchmarks.Models;

namespace EventTimelineReconstruction.Benchmarks.Utils;

public sealed class HighLevelArtefactsAbstractorUtils : IHighLevelArtefactsAbstractorUtils
{
    private const double _secondsThreshold = 2.0;

    public string GetDescriptionFromLogSource(EventViewModel eventViewModel)
    {
        if (eventViewModel.Format == "lnk")
        {
            return eventViewModel.Filename;
        }

        return GetFilenameFromDescription(eventViewModel.Description);
    }

    private static string GetFilenameFromDescription(string description)
    {
        string startKey = " Path: ";
        int startIndex = description.IndexOf(startKey);

        if (startIndex == -1)
        {
            return description;
        }

        return description[(startIndex + startKey.Length)..];
    }

    public string GetDescriptionFromMetaSource(string description)
    {
        StringBuilder sb = new();
        string[] keys = new string[] { "Author: ", "Last saved by: ", "Number of pages: ", "Number of words: ", "Number of characters: ", "Number of characters with spaces: ", "Number of lines: " };

        for (int i = 0; i < keys.Length; i++)
        {
            string value = GetValueFromDescription(description, keys[i]);

            if (value != "")
            {
                AppendFormattedValue(sb, value);
            }
        }

        return sb.ToString().TrimEnd();
    }

    private static string GetValueFromDescription(string description, string key)
    {
        int startIndex = description.IndexOf(key);

        if (startIndex == -1)
        {
            return "";
        }

        int endIndex = description.IndexOf(' ', startIndex + key.Length);

        if (endIndex == -1)
        {
            endIndex = description.Length;
        }

        int valueLength = endIndex - startIndex;
        return description.Substring(startIndex, valueLength);
    }

    public string GetDescriptionFromRegSource(string sourceType, string description)
    {
        return sourceType switch
        {
            "Registry Key: UserAssist" => GetUserAssistDescription(description),
            "Registry Key : MRU List" => GetMRUListDescription(description),
            "Registry Key : MRUListEx" => GetMRUListExDescription(description),
            "Registry Key : Typed URLs" => GetTypedUrlsDescription(description),
            "Registry Key : BagMRU" => GetBagMRUDescription(description),
            "Registry Key : Run Key" => description,
            "UNKNOWN" => GetUnknownDescription(description),
            _ => "",
        };
    }

    private static string GetUserAssistDescription(string description)
    {
        string startKey = "Value name: ";
        string endKey = " Count";
        int startIndex = description.IndexOf(startKey) + startKey.Length;
        int endIndex = description.IndexOf(endKey, startIndex);
        int valueLength = endIndex - startIndex;
        string valueName = description.Substring(startIndex, valueLength);

        startIndex = valueName.LastIndexOf('\\');

        if (startIndex == -1)
        {
            return valueName;
        }

        return valueName[(startIndex + 1)..];
    }

    private static string GetMRUListDescription(string description)
    {
        StringBuilder sb = new();
        string startKey = "]: ";
        string endKey = " Index:";

        int startIndex = description.IndexOf(startKey);
        while (startIndex != -1)
        {
            startIndex += startKey.Length;
            int endIndex = description.IndexOf(endKey, startIndex);

            if (endIndex == -1)
            {
                endIndex = description.Length;
            }

            string value = GetValueBetween(description, startIndex, endIndex);
            AppendFormattedValue(sb, value, "; ");

            startIndex = description.IndexOf(startKey, endIndex);
        }

        return sb.Remove(sb.Length - 2, 2).ToString();
    }

    private static string GetValueBetween(string description, int startIndex, int endIndex)
    {
        int valueLength = endIndex - startIndex;
        return description.Substring(startIndex, valueLength);
    }

    private static void AppendFormattedValue(StringBuilder sb, string value, string separator = " ")
    {
        sb.Append(value);
        sb.Append(separator);
    }

    private static string GetMRUListExDescription(string description)
    {
        if (description.Contains("Path:"))
        {
            return GetMRUListDescriptionExtended(description);
        }
        else
        {
            return GetMRUListDescription(description);
        }
    }

    private static string GetMRUListDescriptionExtended(string description)
    {
        StringBuilder sb = new();
        string startKey = "Path: ";
        string endKey = "  Shell item:";

        int startIndex = description.IndexOf(startKey);
        while (startIndex != -1)
        {
            startIndex += startKey.Length;
            int endIndex = description.IndexOf(endKey, startIndex);

            if (endIndex == -1)
            {
                endIndex = description.Length;
            }

            string value = GetValueBetween(description, startIndex, endIndex);
            AppendFormattedValue(sb, value, "; ");

            startIndex = description.IndexOf(startKey, endIndex);
        }

        return sb.Remove(sb.Length - 2, 2).ToString();
    }

    private static string GetTypedUrlsDescription(string description)
    {
        StringBuilder sb = new();
        string startKey = ": ";
        string endKey = " url";

        int startIndex = description.IndexOf(startKey);
        while (startIndex != -1)
        {
            startIndex += startKey.Length;
            int endIndex = description.IndexOf(endKey, startIndex);

            if (endIndex == -1)
            {
                endIndex = description.Length;
            }

            string value = GetValueBetween(description, startIndex, endIndex);
            AppendFormattedValue(sb, value);

            startIndex = description.IndexOf(startKey, endIndex);
        }

        return sb.ToString().TrimEnd();
    }

    private static string GetBagMRUDescription(string description)
    {
        StringBuilder sb = new();
        string startKey = "Shell item path: ";
        string endKey = " Index:";

        int startIndex = description.IndexOf(startKey);
        while (startIndex != -1)
        {
            startIndex += startKey.Length;
            int endIndex = description.IndexOf(endKey, startIndex);

            if (endIndex == -1)
            {
                endIndex = description.Length;
            }

            string value = GetValueBetween(description, startIndex, endIndex);
            AppendFormattedValue(sb, value, "; ");

            startIndex = description.IndexOf(startKey, endIndex);
        }

        if (sb.Length == 0)
        {
            return sb.ToString();
        }

        return sb.Remove(sb.Length - 2, 2).ToString();
    }

    private static string GetUnknownDescription(string description)
    {
        string startKey = "File Path: [REG_SZ] ";
        string endKey = " Position:";
        int startIndex = description.IndexOf(startKey);

        if (startIndex == -1)
        {
            return description;
        }

        startIndex += startKey.Length;
        int endIndex = description.IndexOf(endKey, startIndex);
        int fullFilePathLength = endIndex - startIndex;
        string fullFilePath = description.Substring(startIndex, fullFilePathLength);

        return GetFileNameFromPath(fullFilePath);
    }

    private static string GetFileNameFromPath(string fullFilePath)
    {
        int startIndex = fullFilePath.LastIndexOf('\\');

        if (startIndex != -1)
        {
            return fullFilePath[(startIndex + 1)..];
        }

        return fullFilePath;
    }

    public string GetFilename(string description)
    {
        string startKey = "/";
        string endKey = " Type:";
        int startIndex = description.LastIndexOf(startKey);
        int endIndex = description.LastIndexOf(endKey);

        if (startIndex == -1)
        {
            startKey = "Origin: ";
            startIndex = description.LastIndexOf(startKey);

            if (startIndex != -1)
            {
                startIndex += startKey.Length - 1;
            }
        }

        if (endIndex == -1)
        {
            endIndex = description.Length;
        }

        startIndex += 1;
        int filenameLength = endIndex - startIndex;
        return description.Substring(startIndex, filenameLength);
    }

    public string GetOtherBrowserExtraFromWebhistSource(Dictionary<string, string> extra)
    {
        StringBuilder sb = new();

        foreach (KeyValuePair<string, string> kvp in extra)
        {
            if (kvp.Key == "schema_match" || kvp.Key == "sha256_hash")
            {
                break;
            }

            string extraValue = FormatExtraFieldValue(kvp.Key, kvp.Value);
            AppendFormattedValue(sb, extraValue);
        }

        return sb.ToString().TrimEnd();
    }

    private static string FormatExtraFieldValue(string key, string value)
    {
        return $"{key}: {value}";
    }

    public int GetFileCountInRowAtSameMinute(List<EventViewModel> events, int startIndex)
    {
        int count = 0;

        for (int i = startIndex; i < events.Count; i++)
        {
            EventViewModel current = events[i];
            if (current.Source != "FILE")
            {
                break;
            }

            count++;
        }

        return count;
    }

    public List<int> GetValidFileEventIndices(List<EventViewModel> events, int startIndex, int endIndex)
    {
        List<int> indices = new(endIndex - startIndex)
        {
            startIndex
        };

        for (int i = startIndex + 1; i <= endIndex && i < events.Count; i++)
        {
            if (events[i].Description.Contains(".exe"))
            {
                indices.Add(i);
            }
        }

        return indices;
    }

    public bool IsFileDuplicateOfLnk(List<EventViewModel> events, int startIndex, EventViewModel current)
    {
        if (!current.Filename.EndsWith(".lnk"))
        {
            return false;
        }

        for (int i = startIndex; i >= 0; i--)
        {
            EventViewModel previous = events[i];
            if (previous.Source != "LNK")
            {
                continue;
            }

            double timeDifference = current.FullDate.Subtract(previous.FullDate).TotalSeconds;
            if (timeDifference.CompareTo(_secondsThreshold) > 0)
            {
                break;
            }

            string lnkArtefact = GetArtefactName(previous.Filename);
            string fileArtefact = GetArtefactName(current.Filename);

            if (lnkArtefact == fileArtefact)
            {
                return true;
            }
        }

        return false;
    }

    private static string GetArtefactName(string filename)
    {
        char startKey = '/';
        int startIndex = filename.LastIndexOf(startKey);

        if (startIndex == -1)
        {
            return filename;
        }

        return filename[startIndex..];
    }

    public bool IsPeLineExecutable(EventViewModel eventViewModel)
    {
        return eventViewModel.Filename.EndsWith(".exe");
    }

    public bool IsPeLineValid(EventViewModel eventViewModel)
    {
        return eventViewModel.Short == "pe_type";
    }
}
