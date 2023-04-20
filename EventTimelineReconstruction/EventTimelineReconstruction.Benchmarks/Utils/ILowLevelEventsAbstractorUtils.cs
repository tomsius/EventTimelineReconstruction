using EventTimelineReconstruction.Benchmarks.Models;

namespace EventTimelineReconstruction.Benchmarks.Utils;
public interface ILowLevelEventsAbstractorUtils
{
    string AddMailUser(string extraValue, string description);
    bool DoesNeedComposing(List<EventViewModel> events, int startIndex, EventViewModel current);
    string GetExtraTillSha256(Dictionary<string, string> extra);
    string GetFilename(EventViewModel eventViewModel);
    string GetFirefoxExtraFromWebhistSource(Dictionary<string, string> extra);
    string GetKeywordsFromExtra(Dictionary<string, string> extra, string data);
    string GetShort(string data);
    string GetSummaryFromShort(string data);
    string GetUrl(string data);
    bool IsValidFileEvent(EventViewModel eventViewModel);
    bool IsValidRegEvent(EventViewModel eventViewModel);
}