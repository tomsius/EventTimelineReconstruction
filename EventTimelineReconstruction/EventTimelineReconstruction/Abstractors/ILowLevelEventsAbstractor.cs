﻿using System.Collections.Generic;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Abstractors;

public interface ILowLevelEventsAbstractor
{
    List<LowLevelEventViewModel> FormLowLevelEvents(List<EventViewModel> events);
}