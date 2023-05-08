using System.Collections.Generic;
using System;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Utils;

public sealed class HighLevelEventsAbstractorUtils : IHighLevelEventsAbstractorUtils
{
    public int FindLastEventIndexOf(List<ISerializableLevel> highLevelEvents, DateTime newEventDate, string source)
    {
        for (int i = highLevelEvents.Count - 1; i >= 0; i--)
        {
            HighLevelEventViewModel highLevelEvent = (HighLevelEventViewModel)highLevelEvents[i];
            DateTime highLevelEventDate = new(highLevelEvent.Date.Year, highLevelEvent.Date.Month, highLevelEvent.Date.Day, highLevelEvent.Time.Hour, highLevelEvent.Time.Minute, highLevelEvent.Time.Second);

            if (highLevelEventDate.CompareTo(newEventDate) == -1)
            {
                break;
            }

            if (highLevelEvent.Source == source && highLevelEventDate.CompareTo(newEventDate) == 0)
            {
                return i;
            }
        }

        return -1;
    }

    public string GenerateVisitValue(string data)
    {
        bool isGoogleBrowser = data.Contains("Type");
        string keyword = GetKeyword(data, isGoogleBrowser);
        string count = GetCount(data, isGoogleBrowser);
        string visitValue = $"{keyword} {count}";

        return visitValue.TrimEnd();
    }

    private static string GetKeyword(string data, bool isGoogleBrowser)
    {
        int startIndex = GetKeywordStartIndex(data, isGoogleBrowser);
        int endIndex = GetKeywordEndIndex(data, startIndex);
        int keywordLength = endIndex - startIndex;

        string keyword = data.Substring(startIndex, keywordLength);

        if (!isGoogleBrowser && keyword == "LINK" && data.ToLower().Contains("pdf"))
        {
            return "PDF";
        }

        return keyword;
    }

    private static int GetKeywordStartIndex(string data, bool isGoogleBrowser)
    {
        if (isGoogleBrowser)
        {
            string key = "Type: [";
            return data.IndexOf(key) + key.Length;
        }
        else
        {
            string key = "Transition: ";
            return data.IndexOf(key) + key.Length;
        }
    }

    private static int GetKeywordEndIndex(string data, int startIndex)
    {
        int endIndex = data.IndexOf(' ', startIndex);

        if (endIndex == -1)
        {
            return data.Length;
        }

        return endIndex;
    }

    private static string GetCount(string data, bool isGoogleBrowser)
    {
        string count;

        if (isGoogleBrowser)
        {
            count = GetCountFromGoogleBrowser(data);
        }
        else
        {
            count = GetCountFromFirefoxBrowser(data);
        }

        return count == "0" ? "" : count;
    }

    private static string GetCountFromGoogleBrowser(string data)
    {
        string key = "type count ";

        if (!data.Contains(key))
        {
            return "";
        }

        int startIndex = data.IndexOf(key) + key.Length;
        int endIndex = data.IndexOf(')', startIndex);
        int countLength = endIndex - startIndex;

        return data.Substring(startIndex, countLength);
    }

    private static string GetCountFromFirefoxBrowser(string data)
    {
        string key = "[count: ";

        if (!data.Contains(key))
        {
            return "";
        }

        int startIndex = data.IndexOf(key) + key.Length;
        int endIndex = data.IndexOf(']', startIndex);
        int countLength = endIndex - startIndex;

        return data.Substring(startIndex, countLength);
    }

    public string GetDownloadedFileName(string data)
    {
        int fileStartIndex = data.IndexOf('(') + 1;
        int fileEndIndex = data.IndexOf(')');
        int filenameLength = fileEndIndex - fileStartIndex;

        return data.Substring(fileStartIndex, filenameLength);
    }

    public string GetMacAddress(string data)
    {
        string[] splits = data.Split("Origin: ");
        string[] combinations = splits[0].Split('-');

        if (combinations.Length <= 1)
        {
            return "-";
        }

        return combinations[^1].TrimEnd();
    }

    public string GetMailUrl(string data)
    {
        return "https://mail.google.com";
    }

    public string GetOrigin(string data)
    {
        string[] splits = data.Split("Origin: ");
        return splits[^1];
    }

    public string GetShort(string data)
    {
        if (data.ToLower().Contains("empty description") || !data.EndsWith("..."))
        {
            int index = data.IndexOf(']') + 1;
            return data[index..].TrimStart();
        }

        int startIndex = data.IndexOf('[') + 1;
        int endIndex = data.IndexOf(']');

        if (endIndex == -1)
        {
            endIndex = data.Length;
        }

        int length = endIndex - startIndex;
        return data.Substring(startIndex, length);
    }

    public string GetUrlHost(string data)
    {
        int urlStart = data.IndexOf("http");
        int urlEnd = data.IndexOf("/", urlStart + 8) + 1;
        int urlHostLength = urlEnd - urlStart;

        return data.Substring(urlStart, urlHostLength);
    }

    public bool IsValidPeEvent(EventViewModel eventViewModel)
    {
        return eventViewModel.Short == "pe_type" && eventViewModel.Filename.EndsWith(".exe");
    }

    public bool IsValidWebhistLine(EventViewModel eventViewModel)
    {
        return eventViewModel.SourceType.ToLower().Contains("history");
    }

    public bool IsWebhistDownloadEvent(EventViewModel eventViewModel)
    {
        return eventViewModel.Type.ToLower().Contains("file downloaded");
    }

    public bool IsWebhistMailEvent(EventViewModel eventViewModel)
    {
        return eventViewModel.Short.ToLower().Contains("mail");
    }

    public bool IsWebhistNamingActivityEvent(EventViewModel eventViewModel)
    {
        return eventViewModel.Short.ToLower().Contains("http");
    }
}
