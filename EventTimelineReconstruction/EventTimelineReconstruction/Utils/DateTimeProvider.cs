using System;

namespace EventTimelineReconstruction.Utils;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now
    {
        get
        {
            return DateTime.Now.Date;
        }
    }
}
