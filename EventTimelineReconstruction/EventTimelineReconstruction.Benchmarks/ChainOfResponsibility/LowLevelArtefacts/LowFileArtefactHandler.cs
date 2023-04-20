using EventTimelineReconstruction.Benchmarks.Models;
using EventTimelineReconstruction.Benchmarks.Utils;

namespace EventTimelineReconstruction.Benchmarks.ChainOfResponsibility.LowLevelArtefacts;

public class LowFileArtefactHandler : ILowFileArtefactHandler
{
    private readonly ILowLevelArtefactsAbstractorUtils _abstractorUtils;

    public IHandler Next
    {
        get; set;
    }

    public LowFileArtefactHandler(ILowLevelArtefactsAbstractorUtils abstractorUtils)
    {
        _abstractorUtils = abstractorUtils;
    }

    public ISerializableLevel FormAbstractEvent(List<EventViewModel> events, List<ISerializableLevel> abstractionLevel, EventViewModel currentEvent)
    {
        if (currentEvent.Source == "FILE")
        {
            string extraValue = _abstractorUtils.GetExtraValue(currentEvent.Extra);
            LowLevelArtefactViewModel artefact = new(
                DateOnly.FromDateTime(currentEvent.FullDate),
                TimeOnly.FromDateTime(currentEvent.FullDate),
                currentEvent.Timezone.DisplayName,
                currentEvent.MACB,
                currentEvent.Source,
                currentEvent.SourceType,
                currentEvent.Type,
                currentEvent.User,
                currentEvent.Host,
                currentEvent.Short,
                currentEvent.Description,
                currentEvent.Version.ToString(),
                currentEvent.Filename,
                currentEvent.INode,
                currentEvent.Notes,
                currentEvent.Format,
                extraValue,
                currentEvent.SourceLine);

            if (!IsFileEventValid(abstractionLevel, artefact))
            {
                artefact = null;
            }

            return artefact;
        }

        if (Next is not null)
        {
            return Next.FormAbstractEvent(events, abstractionLevel, currentEvent);
        }

        return null;
    }

    private static bool IsFileEventValid(List<ISerializableLevel> lowLevelArtefacts, LowLevelArtefactViewModel current)
    {
        DateTime currentTime = new(current.Date.Year, current.Date.Month, current.Date.Day, current.Time.Hour, current.Time.Minute, current.Time.Second);

        if (current.SourceType != "OS Content Modification Time")
        {
            return !DoFileAndWebhistOfSameTimeExist(lowLevelArtefacts, currentTime);
        }

        for (int i = lowLevelArtefacts.Count - 1; i >= 0; i--)
        {
            LowLevelArtefactViewModel previous = (LowLevelArtefactViewModel)lowLevelArtefacts[i];
            DateTime previousTime = new(previous.Date.Year, previous.Date.Month, previous.Date.Day, previous.Time.Hour, previous.Time.Minute, previous.Time.Second);
            if (previousTime.CompareTo(currentTime) != 0)
            {
                break;
            }

            if (previous.SourceType == current.SourceType)
            {
                return false;
            }
        }

        return true;
    }

    private static bool DoFileAndWebhistOfSameTimeExist(List<ISerializableLevel> lowLevelArtefacts, DateTime currentTime)
    {
        bool doesFileExist = false;
        bool doesWebhistExist = false;

        for (int i = lowLevelArtefacts.Count - 1; i >= 0; i--)
        {
            LowLevelArtefactViewModel previous = (LowLevelArtefactViewModel)lowLevelArtefacts[i];
            DateTime previousTime = new(previous.Date.Year, previous.Date.Month, previous.Date.Day, previous.Time.Hour, previous.Time.Minute, previous.Time.Second);
            if (previousTime.CompareTo(currentTime) != 0)
            {
                break;
            }

            if (previous.Source == "FILE")
            {
                doesFileExist = true;
            }

            if (previous.Source == "WEBHIST")
            {
                doesWebhistExist = true;
            }
        }

        return doesFileExist && doesWebhistExist;
    }
}
