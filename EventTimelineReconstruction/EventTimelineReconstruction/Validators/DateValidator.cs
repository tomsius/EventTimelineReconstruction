using System;

namespace EventTimelineReconstruction.Validators;

public sealed class TimeValidator : ITimeValidator
{
    public bool AreDatesValid(DateTime start, DateTime end)
    {
        return start <= end;
    }

    public bool AreHoursValid(int hours)
    {
        return hours >= TimeOnly.MinValue.Hour && hours <= TimeOnly.MaxValue.Hour;
    }

    public bool AreMinutesValid(int minutes)
    {
        return minutes >= TimeOnly.MinValue.Minute && minutes <= TimeOnly.MaxValue.Minute;
    }
}
