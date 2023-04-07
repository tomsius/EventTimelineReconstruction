using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Stores;

public sealed class EventsStore : IEventsStore
{
    private List<EventViewModel> _events;
    private readonly IEventsImporter _eventsImporter;

    public List<EventViewModel> Events
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
        await Task.Run(() =>
        {
            List<EventModel> importedEvents = _eventsImporter.Import(path, fromDate, toDate);
            _events.Clear();
            _events.AddRange(importedEvents.Select(e => new EventViewModel(e)));
            _events = _events.OrderBy(e => e.SourceLine).ToList();
        });
    }

    public void LoadEvents(List<EventViewModel> events)
    {
        _events.Clear();
        _events.AddRange(events);
    }

    public List<EventViewModel> GetAllHiddenEvents()
    {
        List<EventViewModel> hiddenEvents = new(_events.Count);
        Queue<EventViewModel> queue = new(_events.Count);

        foreach (EventViewModel eventViewModel in _events)
        {
            queue.Enqueue(eventViewModel);
        }

        while (queue.Count > 0)
        {
            EventViewModel current = queue.Dequeue();
            foreach (EventViewModel eventViewModel in current.Children)
            {
                queue.Enqueue(eventViewModel);
            }

            if (current.IsVisible == false)
            {
                hiddenEvents.Add(current);
            }
        }

        return hiddenEvents;
    }

    public List<EventModel> GetStoredEventModels()
    {
        HashSet<EventModel> eventModels = new(_events.Count);
        Queue<EventViewModel> queue = new(_events.Count);

        foreach (EventViewModel eventViewModel in _events)
        {
            queue.Enqueue(eventViewModel);
        }

        while (queue.Count > 0)
        {
            EventViewModel current = queue.Dequeue();
            foreach (EventViewModel eventViewModel in current.Children)
            {
                queue.Enqueue(eventViewModel);
            }

            EventModel eventModel = new(
                new DateOnly(current.FullDate.Year, current.FullDate.Month, current.FullDate.Day),
                new TimeOnly(current.FullDate.Hour, current.FullDate.Minute, current.FullDate.Second),
                current.Timezone,
                current.MACB,
                current.Source,
                current.SourceType,
                current.Type,
                current.User,
                current.Host,
                current.Short,
                current.Description,
                current.Version,
                current.Filename,
                current.INode,
                current.Notes,
                current.Format,
                current.Extra,
                current.SourceLine
                );
            eventModels.Add(eventModel);
        }

        return eventModels.ToList();
    }

    public List<EventViewModel> GetStoredEventViewModelsAsOneLevel()
    {
        List<EventViewModel> eventModels = new(_events.Count);
        Queue<EventViewModel> queue = new(_events.Count);

        foreach (EventViewModel eventViewModel in _events)
        {
            queue.Enqueue(eventViewModel);
        }

        while (queue.Count > 0)
        {
            EventViewModel current = queue.Dequeue();
            foreach (EventViewModel eventViewModel in current.Children)
            {
                queue.Enqueue(eventViewModel);
            }

            eventModels.Add(current);
        }

        return eventModels.OrderBy(e => e.SourceLine).ToList();
    }
}
