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
    private readonly List<EventViewModel> _events;
    private readonly IEventsImporter _eventsImporter;
    private readonly IWorkSaver _workSaver;

    public IEnumerable<EventViewModel> Events
    {
        get
        {
            return _events;
        }
    }

    public EventsStore(IEventsImporter eventsImporter, IWorkSaver workSaver)
    {
        _events = new();
        _eventsImporter = eventsImporter;
        _workSaver = workSaver;
    }

    public async Task Load(string path, DateTime fromDate, DateTime toDate)
    {
        await Task.Run(() => {
            List<EventModel> importedEvents = _eventsImporter.Import(path, fromDate, toDate);

            _events.Clear();
            _events.AddRange(importedEvents.Select(e => new EventViewModel(e)));
        });
    }

    internal async Task SaveWork(string fileName)
    {
        await Task.Run(() => _workSaver.SaveWork(fileName, _events));
    }
}
