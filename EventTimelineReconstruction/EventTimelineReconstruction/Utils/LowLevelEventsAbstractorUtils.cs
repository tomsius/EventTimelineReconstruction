using System.Collections.Generic;
using System.Text;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Utils;

public class LowLevelEventsAbstractorUtils : ILowLevelEventsAbstractorUtils
{
    public string AddMailUser(string extraValue, string description)
    {
        string keyword = "@gmail.com";
        if (!description.Contains(keyword))
        {
            return extraValue;
        }

        StringBuilder sb = new(extraValue);
        string mailUser = this.ExtractMailUser(description);
        sb.Append(" Mail User: ");
        sb.Append(mailUser);

        return sb.ToString().TrimStart(new char[] { '-', ' ' });
    }

    private string ExtractMailUser(string description)
    {
        string keyword = " - ";
        int startIndex = description.IndexOf(keyword) + keyword.Length;
        int endIndex = description.IndexOf(keyword, startIndex);
        int mailUserLength = endIndex - startIndex;

        return description.Substring(startIndex, mailUserLength);
    }

    public string GetExtraTillSha256(Dictionary<string, string> extra)
    {
        StringBuilder sb = new();

        foreach (KeyValuePair<string, string> kvp in extra)
        {
            if (kvp.Key == "sha256_hash")
            {
                break;
            }

            string extraValue = this.FormatExtraFieldValue(kvp.Key, kvp.Value);
            sb.Append(extraValue);
            sb.Append(' ');
        }

        return sb.ToString().TrimEnd();
    }

    private string FormatExtraFieldValue(string key, string value)
    {
        return $"{key}: {value}";
    }

    public string GetFirefoxExtraFromWebhistSource(Dictionary<string, string> extra)
    {
        string key = "extra";
        bool doesExtraExist = extra.TryGetValue(key, out string value);

        if (doesExtraExist)
        {
            return this.ExtractVisit(value);
        }

        return this.ExtractTillSchemaMatch(extra);
    }

    private string ExtractTillSchemaMatch(Dictionary<string, string> extra)
    {
        StringBuilder sb = new();
        string key = "schema_match";

        foreach (KeyValuePair<string, string> kvp in extra)
        {
            if (kvp.Key == key)
            {
                break;
            }

            sb.Append(this.FormatExtraFieldValue(kvp.Key, kvp.Value));
            sb.Append(' ');
        }

        return sb.ToString().TrimEnd();
    }

    private string ExtractVisit(string extra)
    {
        char key = '\'';
        int startIndex = extra.IndexOf(key);
        int endIndex = extra.IndexOf(key, startIndex + 1);
        int visitLength = endIndex - startIndex;
        string visitValue = extra.Substring(startIndex, visitLength);

        string skipValue = "visited from: ";
        startIndex = skipValue.Length + 1;
        endIndex = visitValue.IndexOf(' ', startIndex);
        visitLength = endIndex - startIndex;
        string visit = visitValue.Substring(startIndex, visitLength);

        StringBuilder sb = new(visit);

        if (visit.Contains("username="))
        {
            string uername = this.ExtractUsername(visit);
            sb.Append(' ');
            sb.Append(uername);
        }

        return sb.ToString();
    }

    private string ExtractUsername(string visit)
    {
        int startIndex = visit.IndexOf("username=");
        int endIndex = visit.IndexOf('&', startIndex);

        if (endIndex == -1)
        {
            endIndex = visit.IndexOf(' ', startIndex);
        }

        int usernameLength = endIndex - startIndex;

        return visit.Substring(startIndex, usernameLength);
    }

    public string GetKeywordsFromExtra(Dictionary<string, string> extra, string data)
    {
        string numberOfParagraphsKey = "number_of_paragraphs";
        string totalTimeKey = "total_time";
        bool doesNumberOfParagraphsKeyExist = extra.TryGetValue(numberOfParagraphsKey, out string numberOfParagraphsValue);
        bool doesTotalTimeKeyExist = extra.TryGetValue(totalTimeKey, out string totalTimeValue);

        if (!doesNumberOfParagraphsKeyExist && !doesTotalTimeKeyExist)
        {
            return data;
        }

        string beginValue = this.GetBeginValue(data);
        string extraValues = this.FormExtraValues(numberOfParagraphsKey, numberOfParagraphsValue, totalTimeKey, totalTimeValue);
        
        return this.FormExtraResult(beginValue, extraValues);
    }

    private string GetBeginValue(string data)
    {
        string keyword = "Creating App";

        if (data.StartsWith(keyword))
        {
            return keyword;
        }

        return "";
    }

    private string FormExtraValues(string numberOfParagraphsKey, string numberOfParagraphsValue, string totalTimeKey, string totalTimeValue)
    {
        StringBuilder sb = new();

        if (numberOfParagraphsValue is not null)
        {
            string numberOfParagraphs = this.FormatExtraFieldValue(numberOfParagraphsKey, numberOfParagraphsValue);
            sb.Append(numberOfParagraphs);
        }

        sb.Append(' ');

        if (totalTimeValue is not null)
        {
            string totalTime = this.FormatExtraFieldValue(totalTimeKey, totalTimeValue);
            sb.Append(totalTime);
        }

        return sb.ToString().TrimStart();
    }

    private string FormExtraResult(string beginValue, string extraValues)
    {
        StringBuilder sb = new();

        sb.Append(beginValue);
        sb.Append(' ');
        sb.Append(extraValues);

        return sb.ToString().Trim();
    }

    public string GetFilename(EventViewModel eventViewModel)
    {
        return eventViewModel.Source switch
        {
            "FILE" => this.GetFilenameFromFileSource(eventViewModel.Short),
            "LNK" => this.GetFilenameFromLnkSource(eventViewModel.Description),
            "LOG" => this.GetFilenameFromLogSource(eventViewModel.Description),
            _ => "",
        };
    }

    private string GetFilenameFromFileSource(string data)
    {
        string startKey = "Name: ";
        char endKey = ' ';
        char extensionKey = '.';
        int startIndex = data.IndexOf(startKey);

        if (startIndex == -1)
        {
            startIndex = data.LastIndexOf('\\');

            if (startIndex == -1)
            {
                return data;
            }
            else
            {
                return data.Substring(startIndex + 1);
            }
        }

        startIndex += startKey.Length;
        int extensionIndex = data.IndexOf(extensionKey, startIndex);
        int endIndex = data.IndexOf(endKey, extensionIndex);

        if (endIndex == -1)
        {
            return data.Substring(startIndex);
        }

        int filenameLength = endIndex - startIndex;

        return data.Substring(startIndex, filenameLength);
    }

    private string GetFilenameFromLnkSource(string data)
    {
        string startKey = "Link target: ";
        int startIndex = data.IndexOf(startKey) + startKey.Length;

        return data.Substring(startIndex);
    }

    // TODO - paziureti kaip is tikruju atrodo reikiamas LOG source Description
    private string GetFilenameFromLogSource(string data)
    {
        string startKey = " Path: ";
        int startIndex = data.IndexOf(startKey) + startKey.Length;

        string fullFilename = data.Substring(startIndex);
        startIndex = fullFilename.LastIndexOf('\\') + 1;

        return fullFilename.Substring(startIndex);
    }

    public string GetShort(string data)
    {
        string key = "Unpinned Path: ";
        int startIndex = data.IndexOf(key) + key.Length;

        return data.Substring(startIndex);
    }

    public string GetSummaryFromShort(string data)
    {
        char startKey = '{';
        char endKey = '}';
        char extensionKey = '.';
        int startIndex = data.IndexOf(startKey);
        int endIndex = data.IndexOf(endKey, startIndex);
        int extensionIndex = data.IndexOf(extensionKey, endIndex);

        if (extensionIndex == -1 || data[extensionIndex + 1] == '.')
        {
            return data.Substring(startIndex, endIndex - startIndex + 1);
        }

        endIndex = data.LastIndexOf(' ');

        if (endIndex == -1 || endIndex < extensionIndex)
        {
            return data.Substring(startIndex);
        }

        return data.Substring(startIndex, endIndex - startIndex);
    }

    public string GetUrl(string data)
    {
        int urlStart = data.IndexOf("http");
        int hostEnd = data.IndexOf("/", urlStart + 8) + 1;

        if (hostEnd == data.Length)
        {
            return data.Substring(urlStart);
        }

        int urlEnd = data.IndexOf("/", hostEnd + 1);
        int urlHostLength = urlEnd != -1 ? urlEnd - urlStart + 1 : data.Length - urlStart;

        return data.Substring(urlStart, urlHostLength);
    }

    public bool IsValidFileEvent(EventViewModel eventViewModel)
    {
        string value = "C:\\Users\\User\\AppData\\";
        bool doesShortStartWithValue = eventViewModel.Short.StartsWith(value);

        if (doesShortStartWithValue)
        {
            string sourceType = eventViewModel.SourceType;

            return sourceType != "OS Last Access Time" && sourceType != "OS Metadata Modification Time" && sourceType != "OS Content Modification Time";
        }

        return true;
    }

    public bool IsValidRegEvent(EventViewModel eventViewModel)
    {
        string sourceType = "Registry Key: UserAssist";
        char key = '{';
        bool isValidSourceType = eventViewModel.SourceType == sourceType;
        bool isValidShortValue = eventViewModel.Short.Contains(key);

        return isValidSourceType && isValidShortValue;
    }
}
