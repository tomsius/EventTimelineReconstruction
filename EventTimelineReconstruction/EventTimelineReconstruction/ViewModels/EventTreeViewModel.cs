using System.Collections.Generic;
using System.Collections.ObjectModel;

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

    public void LoadEvents(IEnumerable<EventViewModel> events)
    {
        _events.Clear();

        foreach (EventViewModel entity in events) {
            _events.Add(entity);
        }

        // TODO - remove test hierarchy
        _events[0].AddChild(_events[1]);
        _events[0].AddChild(_events[2]);
        _events.RemoveAt(1);
        _events.RemoveAt(2);
    }
}
