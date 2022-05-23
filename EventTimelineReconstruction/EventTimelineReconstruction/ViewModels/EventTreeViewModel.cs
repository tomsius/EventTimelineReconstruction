using System.Collections.Generic;

namespace EventTimelineReconstruction.ViewModels;
public class EventTreeViewModel : ViewModelBase
{
    private readonly RangeEnabledObservableCollection<EventViewModel> _events;

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
        _events.InsertRange(events);

        this.OnPropertyChanged(nameof(Events));
    }
}
