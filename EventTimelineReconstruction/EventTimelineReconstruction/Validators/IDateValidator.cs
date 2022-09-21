using System;

namespace EventTimelineReconstruction.Validators;

public interface ITimeValidator
{
    bool AreDatesValid(DateTime start, DateTime end);
    bool AreHoursValid(int hours);
    bool AreMinutesValid(int minutes);
}