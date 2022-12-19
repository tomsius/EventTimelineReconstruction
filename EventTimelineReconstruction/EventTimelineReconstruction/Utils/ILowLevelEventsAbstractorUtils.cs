using System.Collections.Generic;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Utils;

public interface ILowLevelEventsAbstractorUtils
{
    string AddMailUser(string extraValue, string description);
    string GetExtraTillSha256(Dictionary<string, string> extra);
    string GetFirefoxExtraFromWebhistSource(Dictionary<string, string> extra);
    string GetFilename(EventViewModel eventViewModel);
    string GetKeywordsFromExtra(Dictionary<string, string> extra, string data);
    string GetShort(string data);
    string GetSummaryFromShort(string data);
    string GetUrl(string data);
    bool IsValidFileEvent(EventViewModel eventViewModel);
    bool IsValidRegEvent(EventViewModel eventViewModel);
}
