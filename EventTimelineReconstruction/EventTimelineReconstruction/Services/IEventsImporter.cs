using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventTimelineReconstruction.Models;

namespace EventTimelineReconstruction.Services;

public interface IEventsImporter
{
    public Task<List<EventModel>> Import(string path, DateTime fromDate, DateTime toDate);
}
