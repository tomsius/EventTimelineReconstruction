using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Extensions;
using EventTimelineReconstruction.Utils;

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

    public EventViewModel DraggedItem{ get; set; }
    public EventViewModel Target { get; set; }
    public TreeViewItem DraggedItemElement { get; set; }
    public DraggableAdorner MyAdornment { get; set; }
    public PInPoint pointRef;

    public ICommand ShowDetailsCommand { get; }
    public ICommand DragOverCommand { get; }
    public ICommand DropCommand { get; }
    public ICommand MouseMoveCommand { get; }
    public ICommand GiveFeedbackCommand { get; }

    public EventTreeViewModel(EventDetailsViewModel eventDetailsViewModel)
    {
        _events = new();
        pointRef = new PInPoint();
        ShowDetailsCommand = new ShowEventDetailsCommand(eventDetailsViewModel);
        DragOverCommand = new DragOverEventCommand(this);
        DropCommand = new DropEventCommand(this);
        MouseMoveCommand = new MouseMoveEventCommand(this, eventDetailsViewModel);
        GiveFeedbackCommand = new GiveEventFeedbackCommand(this);
    }

    public void LoadEvents(IEnumerable<EventViewModel> events)
    {
        _events.Clear();

        foreach (EventViewModel entity in events) {
            _events.Add(entity);
        }

        ApplyFilters();

        this.OnPropertyChanged(nameof(Events));
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

    public void ApplyFilters()
    {
        ICollectionView eventsDataSourceView = CollectionViewSource.GetDefaultView(Events);

        eventsDataSourceView.Filter = eventModel =>
        {
            ICollectionView childrenDataSourceView;
            Queue<EventViewModel> queue = new();

            foreach (EventViewModel child in ((EventViewModel)eventModel).Children)
            {
                queue.Enqueue(child);
            }

            while (queue.Count > 0)
            {
                EventViewModel eventViewModel = queue.Dequeue();

                foreach (EventViewModel child in eventViewModel.Children)
                {
                    queue.Enqueue(child);
                }

                childrenDataSourceView = CollectionViewSource.GetDefaultView(eventViewModel.Children);
                childrenDataSourceView.Filter = childModel =>
                {
                    return ((EventViewModel)childModel).IsVisible;
                };
            }

            childrenDataSourceView = CollectionViewSource.GetDefaultView(((EventViewModel)eventModel).Children);
            childrenDataSourceView.Filter = childModel =>
            {
                return ((EventViewModel)childModel).IsVisible;
            };

            return ((EventViewModel)eventModel).IsVisible;
        };
    }
}
