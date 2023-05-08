using System;
using System.Collections.Generic;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.ChainOfResponsibility.LowLevelArtefacts;

public class LowWebhistArtefactHandler : ILowWebhistArtefactHandler
{
    private readonly ILowLevelArtefactsAbstractorUtils _abstractorUtils;

    public IHandler Next { get; set; }

    public LowWebhistArtefactHandler(ILowLevelArtefactsAbstractorUtils abstractorUtils)
    {
        _abstractorUtils = abstractorUtils;
    }

    public ISerializableLevel FormAbstractEvent(List<EventViewModel> events, List<ISerializableLevel> abstractionLevel, EventViewModel currentEvent)
    {
        if (currentEvent.Source == "WEBHIST")
        {
            LowLevelArtefactViewModel artefact = null;

            if (_abstractorUtils.IsValidWebhistLine(currentEvent.SourceType, currentEvent.Type))
            {
                string extraValue = _abstractorUtils.GetExtraValue(currentEvent.Extra);
                artefact = new LowLevelArtefactViewModel(
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

                if (artefact.SourceType.ToLower().Contains("cookies"))
                {
                    artefact = this.NormalizeCookie(abstractionLevel, artefact);
                }
            }

            return artefact;
        }

        if (Next is not null)
        {
            return Next.FormAbstractEvent(events, abstractionLevel, currentEvent);
        }

        return null;
    }

    private LowLevelArtefactViewModel NormalizeCookie(List<ISerializableLevel> lowLevelArtefacts, LowLevelArtefactViewModel current)
    {
        string currentAddress = _abstractorUtils.GetAddress(current.Description);

        for (int i = lowLevelArtefacts.Count - 1; i >= 0; i--)
        {
            LowLevelArtefactViewModel previous = (LowLevelArtefactViewModel)lowLevelArtefacts[i];
            DateTime previousTime = new(previous.Date.Year, previous.Date.Month, previous.Date.Day, previous.Time.Hour, previous.Time.Minute, previous.Time.Second);
            DateTime currentTime = new(current.Date.Year, current.Date.Month, current.Date.Day, current.Time.Hour, current.Time.Minute, current.Time.Second);
            if (previousTime.CompareTo(currentTime) != 0)
            {
                break;
            }

            if (previous.SourceType.ToLower().Contains("cookies"))
            {
                string previousAddress = _abstractorUtils.GetAddress(previous.Description);

                if (previousAddress == currentAddress)
                {
                    return null;
                }
            }
        }

        return current;
    }
}
