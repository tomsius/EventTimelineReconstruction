using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Stores;
public class EventsStore
{
    private List<EventViewModel> _events;
    private readonly IEventsImporter _eventsImporter;

    public IEnumerable<EventViewModel> Events
    {
        get
        {
            return _events;
        }
    }

    public EventsStore(IEventsImporter eventsImporter)
    {
        _events = new();
        _eventsImporter = eventsImporter;
    }

    public async Task Import(string path, DateTime fromDate, DateTime toDate)
    {
        await Task.Run(() => {
            List<EventModel> importedEvents = _eventsImporter.Import(path, fromDate, toDate);

            _events.Clear();
            _events.AddRange(importedEvents.Select(e => new EventViewModel(e)));
            _events = _events.OrderBy(e => e.FullDate).ThenBy(e => e.Filename).ToList();
        });
    }

    public void LoadEvents(List<EventViewModel> events)
    {
        _events.Clear();
        _events.AddRange(events);
    }
}
