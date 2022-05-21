using System.Collections.Generic;
using System.Threading.Tasks;
using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Services;

namespace EventTimelineReconstruction.Stores;
public class EventsStore
{
    private readonly List<EventModel> _events;
    private readonly IEventsImporter _eventsImporter;

    public IEnumerable<EventModel> Events
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

    public async Task Load(string path, System.DateTime fromDate, System.DateTime toDate)
    {
        await Task.Run(() => {
            List<EventModel> importedEvents = _eventsImporter.Import(path, fromDate, toDate);

            _events.Clear();
            _events.AddRange(importedEvents);
            });
    }
}
