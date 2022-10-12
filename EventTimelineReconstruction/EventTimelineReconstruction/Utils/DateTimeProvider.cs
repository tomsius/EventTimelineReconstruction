using System;

namespace EventTimelineReconstruction.Utils;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now
    {
        get
        {
            return DateTime.Now.Date;
        }
    }
}
