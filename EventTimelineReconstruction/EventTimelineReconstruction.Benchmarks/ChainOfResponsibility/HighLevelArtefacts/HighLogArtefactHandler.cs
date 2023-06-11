using EventTimelineReconstruction.Benchmarks.Models;
using EventTimelineReconstruction.Benchmarks.Utils;

namespace EventTimelineReconstruction.Benchmarks.ChainOfResponsibility.HighLevelArtefacts;

public class HighLogArtefactHandler : IHighLogArtefactHandler
{
    private readonly ILowLevelEventsAbstractorUtils _lowLevelEventsAbstractorUtils;
    private readonly IHighLevelArtefactsAbstractorUtils _highLevelArtefactsAbstractorUtils;

    public IHandler Next { get; set; }

    public HighLogArtefactHandler(ILowLevelEventsAbstractorUtils lowLevelEventsAbstractorUtils, IHighLevelArtefactsAbstractorUtils highLevelArtefactsAbstractorUtils)
    {
        _lowLevelEventsAbstractorUtils = lowLevelEventsAbstractorUtils;
        _highLevelArtefactsAbstractorUtils = highLevelArtefactsAbstractorUtils;
    }

    public ISerializableLevel FormAbstractEvent(List<EventViewModel> events, List<ISerializableLevel> abstractionLevel, EventViewModel currentEvent)
    {
        if (currentEvent.Source == "LOG")
        {
            string shortValue = _lowLevelEventsAbstractorUtils.GetShort(currentEvent.Description);
            string descriptionValue = _highLevelArtefactsAbstractorUtils.GetDescriptionFromLogSource(currentEvent);
            HighLevelArtefactViewModel artefact = new(
                DateOnly.FromDateTime(currentEvent.FullDate),
                TimeOnly.FromDateTime(currentEvent.FullDate),
                currentEvent.Source,
                shortValue,
                "-",
                "-",
                currentEvent.SourceLine,
                currentEvent.MACB,
                currentEvent.SourceType,
                descriptionValue);

            if (!this.IsLogEventValid(events, currentEvent))
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

    private bool IsLogEventValid(List<EventViewModel> events, EventViewModel currentEvent)
    {
        if (currentEvent.Format == "lnk")
        {
            return true;
        }

        int startIndex = events.IndexOf(currentEvent) - 1;

        if (!currentEvent.Short.StartsWith("Entry"))
        {
            return IsSingleLineOfSameTimeWithoutEntry(events, startIndex, currentEvent.FullDate);
        }

        bool isSameTimeAsReg = DoesRegOfSameTimeExist(events, startIndex, currentEvent.FullDate);

        if (isSameTimeAsReg)
        {
            return false;
        }

        return !this.IsDuplicateByShort(events, startIndex, currentEvent);
    }

    private static bool IsSingleLineOfSameTimeWithoutEntry(List<EventViewModel> events, int startIndex, DateTime currentDate)
    {
        for (int i = startIndex; i >= 0; i--)
        {
            EventViewModel previous = events[i];
            if (previous.FullDate.CompareTo(currentDate) != 0)
            {
                break;
            }

            if (previous.Source == "LOG")
            {
                return false;
            }
        }

        return true;
    }

    private static bool DoesRegOfSameTimeExist(List<EventViewModel> events, int startIndex, DateTime currentDate)
    {
        for (int i = startIndex; i >= 0; i--)
        {
            EventViewModel previous = events[i];
            if (previous.FullDate.CompareTo(currentDate) != 0)
            {
                break;
            }

            if (previous.Source == "REG" && previous.FullDate.CompareTo(currentDate) == 0)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsDuplicateByShort(List<EventViewModel> events, int startIndex, EventViewModel current)
    {
        string currentFilename = _highLevelArtefactsAbstractorUtils.GetDescriptionFromLogSource(current);

        for (int i = startIndex; i >= 0; i--)
        {
            if (events[i].Source == "LOG")
            {
                string previousFilename = _highLevelArtefactsAbstractorUtils.GetDescriptionFromLogSource(events[i]);

                if (currentFilename == previousFilename)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
