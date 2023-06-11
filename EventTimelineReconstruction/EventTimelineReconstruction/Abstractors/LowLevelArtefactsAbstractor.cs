using System;
using System.Collections.Generic;
using EventTimelineReconstruction.ChainOfResponsibility;
using EventTimelineReconstruction.ChainOfResponsibility.LowLevelArtefacts;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Abstractors;

public sealed class LowLevelArtefactsAbstractor : ILowLevelArtefactsAbstractor
{
    private readonly IHandler _handler;

    public int LinesSkipped { get; private set; }
    public int LinesNeglected { get; private set; }

    public LowLevelArtefactsAbstractor(
        ILowWebhistArtefactHandler webhistHandler,
        ILowLnkArtefactHandler lnkHandler,
        ILowFileArtefactHandler fileHandler)
    {
        _handler = webhistHandler;
        webhistHandler.Next = lnkHandler;
        lnkHandler.Next = fileHandler;
    }

    public List<ISerializableLevel> FormLowLevelArtefacts(List<EventViewModel> events, double periodInMinutes = 1.0)
    {
        List<ISerializableLevel> lowLevelArtefacts = new(events.Count);

        for (int i = 0; i < events.Count; i++)
        {
            ISerializableLevel lowLevelArtefact = _handler.FormAbstractEvent(events, lowLevelArtefacts, events[i]);

            if (events[i].Source == "FILE")
            {
                int needsSkipping = this.SkipFileEvents(events, i, periodInMinutes);
                i += needsSkipping;
            }

            if (lowLevelArtefact is not null)
            {
                lowLevelArtefacts.Add(lowLevelArtefact);
            }
        }

        return lowLevelArtefacts;
    }

    private int SkipFileEvents(List<EventViewModel> events, int startIndex, double periodInMinutes)
    {
        DateTime startTime = events[startIndex].FullDate;
        int count = 0;

        if (events[startIndex].SourceType != "OS Last Access Time")
        {
            return count;
        }

        for (int i = startIndex + 1; i < events.Count; i++)
        {
            EventViewModel eventViewModel = events[i];
            DateTime endTime = events[i].FullDate;
            double differenceInMinutes = endTime.Subtract(startTime).TotalMinutes;

            if (eventViewModel.Source != "FILE" || (eventViewModel.SourceType != "OS Last Access Time" && eventViewModel.SourceType != "OS Metadata Modification Time") || differenceInMinutes.CompareTo(periodInMinutes) > 0)
            {
                break;
            }

            if (eventViewModel.SourceType == "OS Last Access Time")
            {
                LinesSkipped++;
            }
            else
            {
                LinesNeglected++;
            }

            count++;
        }

        return count;
    }
}
