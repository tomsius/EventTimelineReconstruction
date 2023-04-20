using System.Text;

namespace EventTimelineReconstruction.Benchmarks.Utils;

public sealed class LowLevelArtefactsAbstractorUtils : ILowLevelArtefactsAbstractorUtils
{
    public string GetAddress(string description)
    {
        string startKey = "://";

        if (description.Contains("www."))
        {
            startKey = "www.";
        }

        int startIndex = description.IndexOf(startKey) + startKey.Length;

        char endKey = ' ';
        int endIndex = description.IndexOf(endKey);

        if (endIndex == -1)
        {
            endIndex = description.Length;
        }

        int addressLength = endIndex - startIndex;

        return description.Substring(startIndex, addressLength);
    }

    public string GetExtraValue(Dictionary<string, string> extra)
    {
        StringBuilder sb = new();

        foreach (KeyValuePair<string, string> kvp in extra)
        {
            string pair = FormatPair(kvp.Key, kvp.Value);
            sb.Append(pair);
            sb.Append(' ');
        }

        return sb.ToString().TrimEnd();
    }

    private static string FormatPair(string key, string value)
    {
        return $"{key}: {value}";
    }

    public bool IsValidWebhistLine(string sourceType, string type)
    {
        string lowercaseSourceType = sourceType.ToLower();
        string lowercaseType = type.ToLower();

        if (lowercaseSourceType.Contains("history"))
        {
            return true;
        }

        if (lowercaseSourceType.Contains("cookies") && !(lowercaseType.Contains("cookie expires") || lowercaseType.Contains("expiration time")))
        {
            return true;
        }

        return false;
    }
}
