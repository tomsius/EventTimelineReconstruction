using System;
using System.Collections.Generic;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.ChainOfResponsibility.HighLevelArtefacts;

public class HighMetaArtefactHandler : IHighMetaArtefactHandler
{
    private readonly ILowLevelEventsAbstractorUtils _lowLevelEventsAbstractorUtils;
    private readonly IHighLevelArtefactsAbstractorUtils _highLevelArtefactsAbstractorUtils;

    public IHandler Next { get; set; }

    public HighMetaArtefactHandler(ILowLevelEventsAbstractorUtils lowLevelEventsAbstractorUtils, IHighLevelArtefactsAbstractorUtils highLevelArtefactsAbstractorUtils)
    {
        _lowLevelEventsAbstractorUtils = lowLevelEventsAbstractorUtils;
        _highLevelArtefactsAbstractorUtils = highLevelArtefactsAbstractorUtils;
    }

    public ISerializableLevel FormAbstractEvent(List<EventViewModel> events, List<ISerializableLevel> abstractionLevel, EventViewModel currentEvent)
    {
        if (currentEvent.Source == "META")
        {
            string shortValue = currentEvent.Filename;
            string extraValue = _lowLevelEventsAbstractorUtils.GetKeywordsFromExtra(currentEvent.Extra, currentEvent.Short);
            string descriptionValue = _highLevelArtefactsAbstractorUtils.GetDescriptionFromMetaSource(currentEvent.Description);
            return new HighLevelArtefactViewModel(
                DateOnly.FromDateTime(currentEvent.FullDate),
                TimeOnly.FromDateTime(currentEvent.FullDate),
                currentEvent.Source,
                shortValue,
                "-",
                extraValue,
                currentEvent.SourceLine,
                currentEvent.MACB,
                currentEvent.SourceType,
                descriptionValue);
        }

        if (Next is not null)
        {
            return Next.FormAbstractEvent(events, abstractionLevel, currentEvent);
        }

        return null;
    }
}
