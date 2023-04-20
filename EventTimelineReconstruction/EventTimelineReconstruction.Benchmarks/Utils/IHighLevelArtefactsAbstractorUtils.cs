using EventTimelineReconstruction.Benchmarks.Models;

namespace EventTimelineReconstruction.Benchmarks.Utils;
public interface IHighLevelArtefactsAbstractorUtils
{
    string GetDescriptionFromLogSource(EventViewModel eventViewModel);
    string GetDescriptionFromMetaSource(string description);
    string GetDescriptionFromRegSource(string sourceType, string description);
    int GetFileCountInRowAtSameMinute(List<EventViewModel> events, int startIndex);
    string GetFilename(string description);
    string GetOtherBrowserExtraFromWebhistSource(Dictionary<string, string> extra);
    List<int> GetValidFileEventIndices(List<EventViewModel> events, int startIndex, int endIndex);
    bool IsFileDuplicateOfLnk(List<EventViewModel> events, int startIndex, EventViewModel current);
    bool IsPeLineExecutable(EventViewModel eventViewModel);
    bool IsPeLineValid(EventViewModel eventViewModel);
}