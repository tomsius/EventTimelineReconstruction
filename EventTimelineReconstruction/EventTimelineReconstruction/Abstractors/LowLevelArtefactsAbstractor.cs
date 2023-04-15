﻿using System;
using System.Collections.Generic;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Abstractors;

public sealed class LowLevelArtefactsAbstractor : ILowLevelArtefactsAbstractor
{
    private readonly ILowLevelArtefactsAbstractorUtils _lowLevelArtefactsAbstractorUtils;

    public int LinesSkipped { get; private set; }
    public int LinesNeglected { get; private set; }

    public LowLevelArtefactsAbstractor(ILowLevelArtefactsAbstractorUtils lowLevelArtefactsAbstractorUtils)
    {
        _lowLevelArtefactsAbstractorUtils = lowLevelArtefactsAbstractorUtils;
    }

    public List<ISerializableLevel> FormLowLevelArtefacts(List<EventViewModel> events, double periodInMinutes = 1.0)
    {
        List<ISerializableLevel> lowLevelArtefacts = new(events.Count);

        for (int i = 0; i < events.Count; i++)
        {
            LowLevelArtefactViewModel lowLevelArtefact = null;

            switch (events[i].Source)
            {
                case "WEBHIST":
                    if (_lowLevelArtefactsAbstractorUtils.IsValidWebhistLine(events[i].SourceType, events[i].Type))
                    {
                        lowLevelArtefact = this.FormEvent(events[i]);

                        if (lowLevelArtefact.SourceType.ToLower().Contains("cookies"))
                        {
                            lowLevelArtefact = this.NormalizeCookie(lowLevelArtefacts, lowLevelArtefact);
                        }
                    }

                    break;
                case "LNK":
                    lowLevelArtefact = this.FormEvent(events[i]);
                    break;
                case "FILE":
                    lowLevelArtefact = this.FormEvent(events[i]);
                    int needsSkipping = this.SkipFileEvents(events, i, periodInMinutes);

                    if (!IsFileEventValid(lowLevelArtefacts, lowLevelArtefact))
                    {
                        lowLevelArtefact = null;
                    }

                    i += needsSkipping;
                    break;
                default:
                    break;
            }

            if (lowLevelArtefact is not null)
            {
                lowLevelArtefacts.Add(lowLevelArtefact);
            }
        }

        return lowLevelArtefacts;
    }

    private LowLevelArtefactViewModel FormEvent(EventViewModel eventViewModel)
    {
        string extraValue = _lowLevelArtefactsAbstractorUtils.GetExtraValue(eventViewModel.Extra);

        LowLevelArtefactViewModel result = new(
            DateOnly.FromDateTime(eventViewModel.FullDate),
            TimeOnly.FromDateTime(eventViewModel.FullDate),
            eventViewModel.Timezone.DisplayName,
            eventViewModel.MACB,
            eventViewModel.Source,
            eventViewModel.SourceType,
            eventViewModel.Type,
            eventViewModel.User,
            eventViewModel.Host,
            eventViewModel.Short,
            eventViewModel.Description,
            eventViewModel.Version.ToString(),
            eventViewModel.Filename,
            eventViewModel.INode,
            eventViewModel.Notes,
            eventViewModel.Format,
            extraValue,
            eventViewModel.SourceLine
        );

        return result;
    }

    private LowLevelArtefactViewModel NormalizeCookie(List<ISerializableLevel> lowLevelArtefacts, LowLevelArtefactViewModel current)
    {
        string currentAddress = _lowLevelArtefactsAbstractorUtils.GetAddress(current.Description);

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
                string previousAddress = _lowLevelArtefactsAbstractorUtils.GetAddress(previous.Description);

                if (previousAddress == currentAddress)
                {
                    return null;
                }
            }
        }

        return current;
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
