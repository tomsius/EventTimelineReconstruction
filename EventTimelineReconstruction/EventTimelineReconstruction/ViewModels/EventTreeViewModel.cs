using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using EventTimelineReconstruction.Commands;

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

    public void Remove(EventViewModel eventViewModel)
    {
        _events.Remove(eventViewModel);
    }
}
