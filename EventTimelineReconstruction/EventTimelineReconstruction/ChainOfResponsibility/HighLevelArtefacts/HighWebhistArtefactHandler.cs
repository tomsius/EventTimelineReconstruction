using System;
using System.Collections.Generic;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.ChainOfResponsibility.HighLevelArtefacts;

public class HighWebhistArtefactHandler : IHighWebhistArtefactHandler
{
    private readonly IHighLevelEventsAbstractorUtils _highLevelEventsAbstractorUtils;
    private readonly ILowLevelEventsAbstractorUtils _lowLevelEventsAbstractorUtils;
    private readonly IHighLevelArtefactsAbstractorUtils _highLevelArtefactsAbstractorUtils;

    public IHandler Next { get; set; }

    public HighWebhistArtefactHandler(IHighLevelEventsAbstractorUtils highLevelEventsAbstractorUtils, ILowLevelEventsAbstractorUtils lowLevelEventsAbstractorUtils, IHighLevelArtefactsAbstractorUtils highLevelArtefactsAbstractorUtils)
    {
        _highLevelEventsAbstractorUtils = highLevelEventsAbstractorUtils;
        _lowLevelEventsAbstractorUtils = lowLevelEventsAbstractorUtils;
        _highLevelArtefactsAbstractorUtils = highLevelArtefactsAbstractorUtils;
    }

    public ISerializableLevel FormAbstractEvent(List<EventViewModel> events, List<ISerializableLevel> abstractionLevel, EventViewModel currentEvent)
    {
        if (currentEvent.Source == "WEBHIST")
        {
            HighLevelArtefactViewModel artefact = null;

            if (_highLevelEventsAbstractorUtils.IsValidWebhistLine(currentEvent))
            {
                artefact = this.FormEventFromWebhistSource(currentEvent);

                if (!IsWebhistEventValid(abstractionLevel, artefact))
                {
                    artefact = null;
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

    private HighLevelArtefactViewModel FormEventFromWebhistSource(EventViewModel eventViewModel)
    {
        string extraValue;

        if (eventViewModel.SourceType.ToLower().Contains("firefox"))
        {
            extraValue = _lowLevelEventsAbstractorUtils.GetFirefoxExtraFromWebhistSource(eventViewModel.Extra);
        }
        else
        {
            extraValue = _highLevelArtefactsAbstractorUtils.GetOtherBrowserExtraFromWebhistSource(eventViewModel.Extra);
        }

        if (_highLevelEventsAbstractorUtils.IsWebhistDownloadEvent(eventViewModel))
        {
            string downloadedFileName = _highLevelEventsAbstractorUtils.GetDownloadedFileName(eventViewModel.Description);

            return new HighLevelArtefactViewModel(
                DateOnly.FromDateTime(eventViewModel.FullDate),
                TimeOnly.FromDateTime(eventViewModel.FullDate),
                eventViewModel.Source,
                downloadedFileName,
                "Download",
                extraValue,
                eventViewModel.SourceLine,
                eventViewModel.MACB,
                eventViewModel.SourceType,
                "-");
        }
        else if (_highLevelEventsAbstractorUtils.IsWebhistMailEvent(eventViewModel))
        {
            string shortValue = _highLevelEventsAbstractorUtils.GetMailUrl(eventViewModel.Description);
            string newExtraValue = _lowLevelEventsAbstractorUtils.AddMailUser(extraValue, eventViewModel.Description);

            return new HighLevelArtefactViewModel(
                DateOnly.FromDateTime(eventViewModel.FullDate),
                TimeOnly.FromDateTime(eventViewModel.FullDate),
                eventViewModel.Source,
                shortValue,
                "Mail",
                newExtraValue,
                eventViewModel.SourceLine,
                eventViewModel.MACB,
                eventViewModel.SourceType,
                "-");
        }
        else if (_highLevelEventsAbstractorUtils.IsWebhistNamingActivityEvent(eventViewModel))
        {
            string formattedUrl = _lowLevelEventsAbstractorUtils.GetUrl(eventViewModel.Short);
            string visitValue = _highLevelEventsAbstractorUtils.GenerateVisitValue(eventViewModel.Description);

            return new HighLevelArtefactViewModel(
                DateOnly.FromDateTime(eventViewModel.FullDate),
                TimeOnly.FromDateTime(eventViewModel.FullDate),
                eventViewModel.Source,
                formattedUrl,
                visitValue,
                extraValue,
                eventViewModel.SourceLine,
                eventViewModel.MACB,
                eventViewModel.SourceType,
                "-");
        }

        return null;
    }

    private bool IsWebhistEventValid(List<ISerializableLevel> highLevelArtefacts, HighLevelArtefactViewModel current)
    {
        if (current is null)
        {
            return false;
        }

        for (int i = highLevelArtefacts.Count - 1; i >= 0; i--)
        {
            HighLevelArtefactViewModel previous = (HighLevelArtefactViewModel)highLevelArtefacts[i];
            DateTime previousTime = new(previous.Date.Year, previous.Date.Month, previous.Date.Day, previous.Time.Hour, previous.Time.Minute, previous.Time.Second);
            DateTime currentTime = new(current.Date.Year, current.Date.Month, current.Date.Day, current.Time.Hour, current.Time.Minute, current.Time.Second);
            if (previousTime.CompareTo(currentTime) != 0)
            {
                break;
            }

            if (previous.Source == "WEBHIST")
            {
                return false;
            }
        }

        return true;
    }
}
