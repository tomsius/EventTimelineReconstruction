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
    private readonly IWorkSaver _workSaver;
    private readonly IWorkLoader _workLoader;

    public IEnumerable<EventViewModel> Events
    {
        get
        {
            return _events;
        }
    }

    public EventsStore(IEventsImporter eventsImporter, IWorkSaver workSaver, IWorkLoader workLoader)
    {
        _events = new();
        _eventsImporter = eventsImporter;
        _workSaver = workSaver;
        _workLoader = workLoader;
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

    public async Task SaveWork(string fileName)
    {
        await Task.Run(() => _workSaver.SaveWork(fileName, _events));
    }

    public async Task LoadWork(string fileName)
    {
        await Task.Run(() => {
            List<EventViewModel> loadedEvents = _workLoader.LoadWork(fileName);

            _events.Clear();
            _events.AddRange(loadedEvents);
            _events = _events.OrderBy(e => e.FullDate).ThenBy(e => e.Filename).ToList();
        });
    }
}
