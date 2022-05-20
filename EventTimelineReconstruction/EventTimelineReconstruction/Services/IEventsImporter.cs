using System;
using System.Collections.Generic;
using EventTimelineReconstruction.Models;

namespace EventTimelineReconstruction.Services;

public interface IEventsImporter
{
    public List<EventModel> Import(string path, DateTime fromDate, DateTime toDate);
}
