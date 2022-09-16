using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;

namespace EventTimelineReconstruction.ViewModels;

public class HiddenEventsViewModel : ViewModelBase
{
    private readonly ObservableCollection<EventViewModel> _hiddenEvents;
    private readonly EventsStore _eventsStore;

    public IEnumerable<EventViewModel> HiddenEvents
    {
        get
        {
            return _hiddenEvents;
        }
    }

    private readonly CollectionView _hiddenEventsView;

    public CollectionView HiddenEventsView
    {
        get
        {
            return _hiddenEventsView;
        }
    }

    private EventViewModel _selectedHiddenEvent;

    public EventViewModel SelectedHiddenEvent
    {
        get
        {
            return _selectedHiddenEvent;
        }

        set
        {
            _selectedHiddenEvent = value;
            this.OnPropertyChanged(nameof(SelectedHiddenEvent));
        }
    }

    public ICommand UnhideCommand { get; }

    public HiddenEventsViewModel(EventsStore eventsStore, EventTreeViewModel eventTreeViewModel)
    {
        _hiddenEvents = new();
        _hiddenEventsView = new ListCollectionView(_hiddenEvents);
        (_hiddenEventsView as ListCollectionView).CustomSort = new EventSorter();

        _eventsStore = eventsStore;

        UnhideCommand = new UnhideEventCommand(this, eventTreeViewModel);

        eventTreeViewModel.PropertyChanged += this.Initialize;
    }

    public void Initialize(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(EventTreeViewModel.Events))
        {
            List<EventViewModel> hiddenEvents = _eventsStore.GetAllHiddenEvents();
            _hiddenEvents.Clear();

            foreach (EventViewModel entity in hiddenEvents) {
                _hiddenEvents.Add(entity);
            }
        }
    }

    public void AddHiddenEvent(EventViewModel hiddenEvent)
    {
        _hiddenEvents.Add(hiddenEvent);
    }

    public void RemoveHiddenEvent(EventViewModel hiddenEvent)
    {
        _hiddenEvents.Remove(hiddenEvent);
    }
}
