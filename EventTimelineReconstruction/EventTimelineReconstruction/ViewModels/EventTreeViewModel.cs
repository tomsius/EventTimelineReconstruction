using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
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
