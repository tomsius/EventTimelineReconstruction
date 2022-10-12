using System;

namespace EventTimelineReconstruction.Utils;

public interface IDateTimeProvider
{
    public DateTime Now { get; }
}
