namespace EventTimelineReconstruction.Benchmarks.Utils;

public interface ILowLevelArtefactsAbstractorUtils
{
    string GetAddress(string description);
    string GetExtraValue(Dictionary<string, string> extra);
    bool IsValidWebhistLine(string sourceType, string type);
}