using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Stores;
public interface IEventsStore
{
    List<EventViewModel> Events { get; }

    List<EventViewModel> GetAllHiddenEvents();
    List<EventModel> GetStoredEventModels();
    List<EventViewModel> GetStoredEventViewModelsAsOneLevel();
    Task Import(string path, DateTime fromDate, DateTime toDate);
    void LoadEvents(List<EventViewModel> events);
}