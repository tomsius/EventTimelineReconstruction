using System;
using System.Collections.Generic;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Abstractors;

public class LowLevelArtefactsAbstractor
{
    private readonly ILowLevelArtefactsAbstractorUtils _lowLevelArtefactsAbstractorUtils;

    public int LinesSkipped { get; private set; }
    public int LinesNeglected { get; private set; }

    public LowLevelArtefactsAbstractor(ILowLevelArtefactsAbstractorUtils lowLevelArtefactsAbstractorUtils)
    {
        _lowLevelArtefactsAbstractorUtils = lowLevelArtefactsAbstractorUtils;
    }

    public List<LowLevelArtefactViewModel> FormLowLevelArtefacts(List<EventViewModel> events, double periodInMinutes = 1.0)
    {
        List<LowLevelArtefactViewModel> lowLevelArtefacts = new(events.Count);

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

                    if (!this.IsFileEventValid(lowLevelArtefacts, lowLevelArtefact)) 
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

        LowLevelArtefactViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Timezone = eventViewModel.Timezone.DisplayName,
            Macb = eventViewModel.MACB,
            Source = eventViewModel.Source,
            SourceType = eventViewModel.SourceType,
            Type = eventViewModel.Type,
            User = eventViewModel.User,
            Host = eventViewModel.Host,
            Short = eventViewModel.Short,
            Description = eventViewModel.Description,
            Version = eventViewModel.Version.ToString(),
            Filename = eventViewModel.Filename,
            Inode = eventViewModel.INode,
            Notes = eventViewModel.Notes,
            Format = eventViewModel.Format,
            Extra = extraValue,
            Reference = eventViewModel.SourceLine
        };

        return result;
    }

    private LowLevelArtefactViewModel NormalizeCookie(List<LowLevelArtefactViewModel> lowLevelArtefacts, LowLevelArtefactViewModel current)
    {
        string currentAddress = _lowLevelArtefactsAbstractorUtils.GetAddress(current.Description);

        for (int i = lowLevelArtefacts.Count - 1; i >= 0; i--)
        {
            LowLevelArtefactViewModel previous = lowLevelArtefacts[i];
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

    private bool IsFileEventValid(List<LowLevelArtefactViewModel> lowLevelArtefacts, LowLevelArtefactViewModel current)
    {
        DateTime currentTime = new(current.Date.Year, current.Date.Month, current.Date.Day, current.Time.Hour, current.Time.Minute, current.Time.Second);

        if (current.SourceType != "OS Content Modification Time")
        {
            return !this.DoFileAndWebhistOfSameTimeExist(lowLevelArtefacts, currentTime);
        }

        for (int i = lowLevelArtefacts.Count - 1; i >= 0; i--)
        {
            LowLevelArtefactViewModel previous = lowLevelArtefacts[i];
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

    private bool DoFileAndWebhistOfSameTimeExist(List<LowLevelArtefactViewModel> lowLevelArtefacts, DateTime currentTime)
    {
        bool doesFileExist = false;
        bool doesWebhistExist = false;

        for (int i = lowLevelArtefacts.Count - 1; i >= 0; i--)
        {
            LowLevelArtefactViewModel previous = lowLevelArtefacts[i];
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
