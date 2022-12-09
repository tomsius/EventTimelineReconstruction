using System.Collections.Generic;

namespace EventTimelineReconstruction.Utils;

public interface ILowLevelArtefactsAbstractorUtils
{
    string GetAddress(string description);
    string GetExtraValue(Dictionary<string, string> extra);
    bool IsValidWebhistLine(string sourceType, string type);
}
