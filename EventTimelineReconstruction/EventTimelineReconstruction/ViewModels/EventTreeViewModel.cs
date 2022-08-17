using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Extensions;

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

    public ICommand ShowDetailsCommand { get; }

    public EventTreeViewModel(EventDetailsViewModel eventDetailsViewModel)
    {
        _events = new();
        ShowDetailsCommand = new ShowEventDetailsCommand(eventDetailsViewModel);
    }

    public void LoadEvents(IEnumerable<EventViewModel> events)
    {
        _events.Clear();

        foreach (EventViewModel entity in events) {
            _events.Add(entity);
        }
    }

    public void AddEvent(EventViewModel eventViewModel)
    {
        _events.Add(eventViewModel);
    }

    public void RemoveEvent(EventViewModel eventViewModel)
    {
        _events.Remove(eventViewModel);
    }

    public void UpdateOrdering()
    {
        _events.Sort();
    }
}
