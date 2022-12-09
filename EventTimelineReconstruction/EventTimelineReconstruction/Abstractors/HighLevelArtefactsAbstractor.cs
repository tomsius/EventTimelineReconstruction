using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;
using System;
using System.Collections.Generic;

namespace EventTimelineReconstruction.Abstractors;

public class HighLevelArtefactsAbstractor : IHighLevelArtefactsAbstractor
{
    private const double _minutesThreshold = 15.0;
    private const int _fileArtefactsInRowCount = 36;
    private readonly IHighLevelEventsAbstractorUtils _highLevelEventsAbstractorUtils;
    private readonly ILowLevelEventsAbstractorUtils _lowLevelEventsAbstractorUtils;
    private readonly IHighLevelArtefactsAbstractorUtils _highLevelArtefactsAbstractorUtils;

    public HighLevelArtefactsAbstractor(IHighLevelEventsAbstractorUtils highLevelEventsAbstractorUtils, ILowLevelEventsAbstractorUtils lowLevelEventsAbstractorUtils, IHighLevelArtefactsAbstractorUtils highLevelArtefactsAbstractorUtils)
    {
        _highLevelEventsAbstractorUtils = highLevelEventsAbstractorUtils;
        _lowLevelEventsAbstractorUtils = lowLevelEventsAbstractorUtils;
        _highLevelArtefactsAbstractorUtils = highLevelArtefactsAbstractorUtils;
    }

    public List<HighLevelArtefactViewModel> FormHighLevelArtefacts(List<EventViewModel> events)
    {
        List<HighLevelArtefactViewModel> highLevelArtefacts = new(events.Count);

        for (int i = 0; i < events.Count; i++)
        {
            HighLevelArtefactViewModel highLevelArtefact = null;

            switch (events[i].Source)
            {
                case "WEBHIST":
                    if (_highLevelEventsAbstractorUtils.IsValidWebhistLine(events[i]))
                    {
                        highLevelArtefact = this.FormEventFromWebhistSource(events[i]);

                        if (!this.IsWebhistEventValid(highLevelArtefacts, highLevelArtefact))
                        {
                            highLevelArtefact = null;
                        }
                    }

                    break;
                case "LNK":
                    highLevelArtefact = this.FormEventFromLnkSource(events[i]);

                    if (!this.IsLnkEventValid(highLevelArtefacts, highLevelArtefact))
                    {
                        highLevelArtefact = null;
                    }

                    break;
                case "FILE":
                    if (i == 0)
                    {
                        highLevelArtefact = this.FormEventFromFileSource(events[i]);
                        break;
                    }

                    if (!_highLevelArtefactsAbstractorUtils.IsFileDuplicateOfLnk(events, i - 1, events[i]))
                    {
                        int fileCountInRowAtSameMinute = _highLevelArtefactsAbstractorUtils.GetFileCountInRowAtSameMinute(events, i);
                        int endIndex = i + fileCountInRowAtSameMinute;

                        if (fileCountInRowAtSameMinute <= _fileArtefactsInRowCount)
                        {
                            for (int index = i; index < endIndex; index++)
                            {
                                HighLevelArtefactViewModel artefact = this.FormEventFromFileSource(events[i]);
                                // single artefact per second
                                if (this.IsFileEventValid(highLevelArtefacts, artefact))
                                {
                                    highLevelArtefacts.Add(artefact);
                                }
                            }
                        }
                        else
                        {
                            List<int> validIndices = _highLevelArtefactsAbstractorUtils.GetValidFileEventIndices(events, i, endIndex);
                            for (int index = 0; index < validIndices.Count; index++)
                            {
                                HighLevelArtefactViewModel artefact = this.FormEventFromFileSource(events[validIndices[index]]);
                                highLevelArtefacts.Add(artefact);
                            }
                        }

                        i += fileCountInRowAtSameMinute - 1;
                    }

                    break;
                case "LOG":
                    highLevelArtefact = this.FormEventFromLogSource(events[i]);

                    if (!this.IsLogEventValid(events, i))
                    {
                        highLevelArtefact = null;
                    }

                    break;
                case "REG":
                    highLevelArtefact = this.FormEventFromRegSource(events[i]);

                    if (!this.IsRegEventValid(highLevelArtefacts, highLevelArtefact))
                    {
                        highLevelArtefact = null;
                    }

                    break;
                case "META":
                    highLevelArtefact = this.FormEventFromMetaSource(events[i]);
                    break;
                case "OLECF":
                    // Choose the last line of the same OLECF time - skip same time OLECF type events
                    while (i < events.Count - 1 && this.AreOlecfEventsOfSameTime(events[i], events[i + 1]))
                    {
                        i++;
                    }

                    highLevelArtefact = this.FormEventFromOlecfSource(events[i]);
                    break;
                case "PE":
                    if (this.IsPeEventValid(highLevelArtefacts, events[i]))
                    {
                        highLevelArtefact = this.FormEventFromPeSource(events[i]);
                    }

                    break;
                default:
                    break;
            }

            if (this.IsEventValid(highLevelArtefacts, highLevelArtefact))
            {
                highLevelArtefacts.Add(highLevelArtefact);
            }
        }

        return highLevelArtefacts;
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
            return this.FormDownloadEvent(eventViewModel, extraValue);
        }
        else if (_highLevelEventsAbstractorUtils.IsWebhistMailEvent(eventViewModel))
        {
            return this.FormMailEvent(eventViewModel, extraValue);
        }
        else if (_highLevelEventsAbstractorUtils.IsWebhistNamingActivityEvent(eventViewModel))
        {
            return this.FormNamingActivityEvent(eventViewModel, extraValue);
        }

        return null;
    }

    private HighLevelArtefactViewModel FormDownloadEvent(EventViewModel eventViewModel, string extraValue)
    {
        string downloadedFileName = _highLevelEventsAbstractorUtils.GetDownloadedFileName(eventViewModel.Description);

        HighLevelArtefactViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = downloadedFileName,
            Extra = extraValue,
            Reference = eventViewModel.SourceLine,
            Visit = "Download",
            Macb = eventViewModel.MACB,
            SourceType = eventViewModel.SourceType,
            Description = eventViewModel.Description
        };

        return result;
    }

    private HighLevelArtefactViewModel FormMailEvent(EventViewModel eventViewModel, string extraValue)
    {
        string shortValue = _highLevelEventsAbstractorUtils.GetMailUrl(eventViewModel.Description);
        string newExtraValue = _lowLevelEventsAbstractorUtils.AddMailUser(extraValue, eventViewModel.Description);

        HighLevelArtefactViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = shortValue,
            Extra = newExtraValue,
            Reference = eventViewModel.SourceLine,
            Visit = "Mail",
            Macb = eventViewModel.MACB,
            SourceType = eventViewModel.SourceType,
            Description = eventViewModel.Description
        };

        return result;
    }

    private HighLevelArtefactViewModel FormNamingActivityEvent(EventViewModel eventViewModel, string extraValue)
    {
        string formattedUrl = _lowLevelEventsAbstractorUtils.GetUrl(eventViewModel.Short);
        string visitValue = _highLevelEventsAbstractorUtils.GenerateVisitValue(eventViewModel.Description);

        HighLevelArtefactViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = formattedUrl,
            Extra = extraValue,
            Reference = eventViewModel.SourceLine,
            Visit = visitValue,
            Macb = eventViewModel.MACB,
            SourceType = eventViewModel.SourceType,
            Description = eventViewModel.Description
        };

        return result;
    }

    private bool IsWebhistEventValid(List<HighLevelArtefactViewModel> highLevelArtefacts, HighLevelArtefactViewModel current)
    {
        if (current is null)
        {
            return false;
        }

        for (int i = highLevelArtefacts.Count - 1; i >= 0; i--)
        {
            HighLevelArtefactViewModel previous = highLevelArtefacts[i];
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

    private HighLevelArtefactViewModel FormEventFromLnkSource(EventViewModel eventViewModel)
    {
        string shortValue = _highLevelEventsAbstractorUtils.GetShort(eventViewModel.Description);

        HighLevelArtefactViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = shortValue,
            Reference = eventViewModel.SourceLine,
            Macb = eventViewModel.MACB,
            SourceType = eventViewModel.SourceType
        };

        return result;
    }

    private bool IsLnkEventValid(List<HighLevelArtefactViewModel> highLevelArtefacts, HighLevelArtefactViewModel current)
    {
        for (int i = highLevelArtefacts.Count - 1; i >= 0; i--)
        {
            HighLevelArtefactViewModel previous = highLevelArtefacts[i];

            if (previous.Short == current.Short && previous.Source == "LNK")
            {
                return false;
            }
        }

        return true;
    }

    private HighLevelArtefactViewModel FormEventFromFileSource(EventViewModel eventViewModel)
    {
        string shortValue = _highLevelArtefactsAbstractorUtils.GetFilename(eventViewModel.Description);
        string extraValue = _lowLevelEventsAbstractorUtils.GetExtraTillSha256(eventViewModel.Extra);

        HighLevelArtefactViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = shortValue,
            Extra = extraValue,
            Reference = eventViewModel.SourceLine,
            Macb = eventViewModel.MACB,
            SourceType = eventViewModel.SourceType,
            Description = eventViewModel.Description
        };

        return result;
    }

    private bool IsFileEventValid(List<HighLevelArtefactViewModel> highLevelArtefacts, HighLevelArtefactViewModel current)
    {
        HighLevelArtefactViewModel previous = highLevelArtefacts[^1];
        DateTime previousTime = new(previous.Date.Year, previous.Date.Month, previous.Date.Day, previous.Time.Hour, previous.Time.Minute, previous.Time.Second);
        DateTime currentTime = new(current.Date.Year, current.Date.Month, current.Date.Day, current.Time.Hour, current.Time.Minute, current.Time.Second);

        return previousTime.CompareTo(currentTime) != 0;
    }

    private HighLevelArtefactViewModel FormEventFromLogSource(EventViewModel eventViewModel)
    {
        string shortValue = _lowLevelEventsAbstractorUtils.GetShort(eventViewModel.Description);
        string descriptionValue = _highLevelArtefactsAbstractorUtils.GetDescriptionFromLogSource(eventViewModel);

        HighLevelArtefactViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = shortValue,
            Reference = eventViewModel.SourceLine,
            Macb = eventViewModel.MACB,
            SourceType = eventViewModel.SourceType,
            Description = descriptionValue
        };

        return result;
    }

    private bool IsLogEventValid(List<EventViewModel> events, int startIndex)
    {
        EventViewModel current = events[startIndex];

        if (current.Format == "lnk")
        {
            return true;
        }

        if (!current.Short.StartsWith("Entry"))
        {
            return this.IsSingleLineOfSameTimeWithoutEntry(events, startIndex - 1, current.FullDate);
        }

        bool isSameTimeAsReg = this.DoesRegOfSameTimeExist(events, startIndex - 1, current.FullDate);

        if (isSameTimeAsReg)
        {
            return false;
        }

        return !this.IsDuplicateByShort(events, startIndex - 1, current);
    }

    private bool IsSingleLineOfSameTimeWithoutEntry(List<EventViewModel> events, int startIndex, DateTime currentDate)
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

    private bool DoesRegOfSameTimeExist(List<EventViewModel> events, int startIndex, DateTime currentDate)
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

    private HighLevelArtefactViewModel FormEventFromRegSource(EventViewModel eventViewModel)
    {
        string shortValue = _lowLevelEventsAbstractorUtils.GetSummaryFromShort(eventViewModel.Description);
        string extraValue = _lowLevelEventsAbstractorUtils.GetExtraTillSha256(eventViewModel.Extra);
        string descriptionValue = _highLevelArtefactsAbstractorUtils.GetDescriptionFromRegSource(eventViewModel.SourceType, eventViewModel.Description);

        HighLevelArtefactViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = shortValue,
            Extra = extraValue,
            Reference = eventViewModel.SourceLine,
            Macb = eventViewModel.MACB,
            SourceType = eventViewModel.SourceType,
            Description = descriptionValue
        };

        return result;
    }

    private bool IsRegEventValid(List<HighLevelArtefactViewModel> highLevelArtefacts, HighLevelArtefactViewModel current)
    {
        if (current.SourceType == "Registry Key: UserAssist" && !current.Description.Contains(".exe"))
        {
            return false;
        }

        if (current.SourceType == "Registry Key : BagMRU" && current.Description.Length == 0)
        {
            return false;
        }

        if (current.SourceType == "Registry Key : Run Key" || (current.SourceType == "UNKNOWN" && !current.Description.Contains('.')))
        {
            for (int i = highLevelArtefacts.Count - 1; i >= 0; i--)
            {
                HighLevelArtefactViewModel previous = highLevelArtefacts[i];
                DateTime previousTime = new(previous.Date.Year, previous.Date.Month, previous.Date.Day, previous.Time.Hour, previous.Time.Minute, previous.Time.Second);
                DateTime currentTime = new(current.Date.Year, current.Date.Month, current.Date.Day, current.Time.Hour, current.Time.Minute, current.Time.Second);

                if (previousTime.CompareTo(currentTime) != 0)
                {
                    break;
                }

                if (previous.SourceType == current.SourceType)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private HighLevelArtefactViewModel FormEventFromMetaSource(EventViewModel eventViewModel)
    {
        string shortValue = eventViewModel.Filename;
        string extraValue = _lowLevelEventsAbstractorUtils.GetKeywordsFromExtra(eventViewModel.Extra, eventViewModel.Short);
        string descriptionValue = _highLevelArtefactsAbstractorUtils.GetDescriptionFromMetaSource(eventViewModel.Description);

        HighLevelArtefactViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = shortValue,
            Extra = extraValue,
            Reference = eventViewModel.SourceLine,
            Macb = eventViewModel.MACB,
            SourceType = eventViewModel.SourceType,
            Description = descriptionValue
        };

        return result;
    }

    private bool AreOlecfEventsOfSameTime(EventViewModel firstEvent, EventViewModel secondEvent)
    {
        bool isSameSource = firstEvent.Source == secondEvent.Source;
        bool isSameTime = firstEvent.FullDate.CompareTo(secondEvent.FullDate) == 0;

        return isSameSource && isSameTime;
    }

    private HighLevelArtefactViewModel FormEventFromOlecfSource(EventViewModel eventViewModel)
    {
        string shortValue = eventViewModel.Filename;
        string extraValue = eventViewModel.Short;
        int length = eventViewModel.Description.Length <= 80 ? eventViewModel.Description.Length : 80;
        string descriptionValue = eventViewModel.Description.Substring(0, length);

        HighLevelArtefactViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = shortValue,
            Extra = extraValue,
            Reference = eventViewModel.SourceLine,
            Macb = eventViewModel.MACB,
            SourceType = eventViewModel.SourceType,
            Description = descriptionValue
        };

        return result;
    }

    private bool IsPeEventValid(List<HighLevelArtefactViewModel> highLevelArtefacts, EventViewModel current)
    {
        if (_highLevelArtefactsAbstractorUtils.IsPeLineExecutable(current))
        {
            return true;
        }

        if (!_highLevelArtefactsAbstractorUtils.IsPeLineValid(current))
        {
            return false;
        }

        for (int i = highLevelArtefacts.Count - 1; i >= 0; i--)
        {
            HighLevelArtefactViewModel previous = highLevelArtefacts[i];
            DateTime previousTime = new(previous.Date.Year, previous.Date.Month, previous.Date.Day, previous.Time.Hour, previous.Time.Minute, previous.Time.Second);

            if (previousTime.CompareTo(current.FullDate) != 0)
            {
                break;
            }

            if (previous.Source == "PE")
            {
                return false;
            }
        }

        return true;
    }

    private HighLevelArtefactViewModel FormEventFromPeSource(EventViewModel eventViewModel)
    {
        string shortValue = eventViewModel.Filename;

        HighLevelArtefactViewModel result = new()
        {
            Date = DateOnly.FromDateTime(eventViewModel.FullDate),
            Time = TimeOnly.FromDateTime(eventViewModel.FullDate),
            Source = eventViewModel.Source,
            Short = shortValue,
            Reference = eventViewModel.SourceLine,
            Macb = eventViewModel.MACB,
            SourceType = eventViewModel.SourceType,
            Description = eventViewModel.Description
        };

        return result;
    }

    private bool IsEventValid(List<HighLevelArtefactViewModel> highLevelArtefacts, HighLevelArtefactViewModel current)
    {
        if (current is null)
        {
            return false;
        }

        if (highLevelArtefacts.Count == 0)
        {
            return true;
        }

        HighLevelArtefactViewModel previous = highLevelArtefacts[^1];

        if (previous.Short != current.Short)
        {
            return true;
        }

        DateTime previousTime = new(previous.Date.Year, previous.Date.Month, previous.Date.Day, previous.Time.Hour, previous.Time.Minute, previous.Time.Second);
        DateTime currentTime = new(current.Date.Year, current.Date.Month, current.Date.Day, current.Time.Hour, current.Time.Minute, current.Time.Second);
        double timeDifference = currentTime.Subtract(previousTime).TotalMinutes;

        return timeDifference.CompareTo(_minutesThreshold) >= 0;
    }
}
