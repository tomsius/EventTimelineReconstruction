using System;
using System.Collections.Generic;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Utils;

public interface IHighLevelEventsAbstractorUtils
{
    int FindLastEventIndexOf(List<ISerializableLevel> highLevelEvents, DateTime newEventDate, string source);
    string GenerateVisitValue(string data);
    string GetDownloadedFileName(string data);
    string GetMacAddress(string data);
    string GetMailUrl(string data);
    string GetOrigin(string data);
    string GetShort(string data);
    string GetUrlHost(string data);
    bool IsValidPeEvent(EventViewModel eventViewModel);
    bool IsValidWebhistLine(EventViewModel eventViewModel);
    bool IsWebhistDownloadEvent(EventViewModel eventViewModel);
    bool IsWebhistMailEvent(EventViewModel eventViewModel);
    bool IsWebhistNamingActivityEvent(EventViewModel eventViewModel);
}