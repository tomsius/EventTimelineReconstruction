using System.Collections.Generic;
using System.Collections.ObjectModel;
using EventTimelineReconstruction.Models;

namespace EventTimelineReconstruction.ViewModels;
public class EventTreeViewModel : ViewModelBase
{
    private readonly ObservableCollection<EventViewModel> _events;

    public IEnumerable<EventViewModel> Events
    {
        get
        {
            return _events;
        }
    }

    public EventTreeViewModel()
    {
        _events = new();
    }

    public void LoadEvents(IEnumerable<EventModel> events)
    {
        _events.Clear();

        foreach (EventModel e in events) {
            EventViewModel eventViewModel = new(e);
            _events.Add(eventViewModel);
        }
    }
}
