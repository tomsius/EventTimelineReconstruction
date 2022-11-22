using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Utils;

public class HighLevelEventsAbstractorUtils : IHighLevelEventsAbstractorUtils
{
    public string GenerateVisitValue(string data)
    {
        bool isGoogleBrowser = data.Contains("Type");
        string keyword = this.GetKeyword(data, isGoogleBrowser);
        string count = this.GetCount(data, isGoogleBrowser);
        string visitValue = $"{keyword} {count}";

        return visitValue.TrimEnd();
    }

    private string GetKeyword(string data, bool isGoogleBrowser)
    {
        int startIndex = this.GetKeywordStartIndex(data, isGoogleBrowser);
        int endIndex = this.GetKeywordEndIndex(data, startIndex);
        int keywordLength = endIndex - startIndex;

        string keyword = data.Substring(startIndex, keywordLength);

        if (!isGoogleBrowser && keyword == "LINK" && data.ToLower().Contains("pdf"))
        {
            return "PDF";
        }

        return keyword;
    }

    private int GetKeywordStartIndex(string data, bool isGoogleBrowser)
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

    private int GetKeywordEndIndex(string data, int startIndex)
    {
        int endIndex = data.IndexOf(' ', startIndex);

        if (endIndex == -1)
        {
            return data.Length - 1;
        }

        return endIndex;
    }

    private string GetCount(string data, bool isGoogleBrowser)
    {
        string count;

        if (isGoogleBrowser)
        {
            count = this.GetCountFromGoogleBrowser(data);
        }
        else
        {
            count = this.GetCountFromFirefoxBrowser(data);
        }

        return count == "0" ? "" : count;
    }

    private string GetCountFromGoogleBrowser(string data)
    {
        string key = "type count ";
        int startIndex = data.IndexOf(key);
        int endIndex = data.IndexOf(' ', startIndex + key.Length);
        int countLength = endIndex - startIndex;

        return data.Substring(startIndex, countLength);
    }

    private string GetCountFromFirefoxBrowser(string data)
    {
        string key = "[count: ";
        int startIndex = data.IndexOf(key);
        int endIndex = data.IndexOf(']', startIndex + key.Length);
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
        string[] splits = data.Split(" Origin: ");
        string[] combinations = splits[0].Split('-');

        if (combinations.Length <= 0)
        {
            return "-";
        }

        return combinations[^1];
    }

    public string GetMailUrl(string data)
    {
        return "https://mail.google.com";
    }

    public string GetOrigin(string data)
    {
        string[] splits = data.Split(" Origin: ");
        return splits[^1];
    }

    public string GetShort(string data)
    {
        if (data.Contains("empty description") || !data.EndsWith("..."))
        {
            int startIndex = data.IndexOf(']');
            return data.Substring(startIndex).TrimStart();
        }

        return data;
    }

    public string GetUrlHost(string data)
    {
        int urlStart = data.IndexOf("http");
        int urlEnd = data.IndexOf("/", urlStart + 8);
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
