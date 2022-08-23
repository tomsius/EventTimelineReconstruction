using System.Windows.Input;
using EventTimelineReconstruction.Commands;

namespace EventTimelineReconstruction.ViewModels;
public class MainWindowViewModel
{
    private readonly EventTreeViewModel _eventTreeViewModel;

    public EventTreeViewModel EventTreeViewModel
    {
        get
        {
            return _eventTreeViewModel;
        }
    }

    private readonly EventDetailsViewModel _eventDetailsViewModel;

    public EventDetailsViewModel EventDetailsViewModel
    {
        get
        {
            return _eventDetailsViewModel;
        }
    }

    public ICommand MoveEventCommand { get; }
    public ICommand HideCommand { get; }

    public MainWindowViewModel(EventTreeViewModel eventTreeViewModel, EventDetailsViewModel eventDetailsViewModel, HiddenEventsViewModel hiddenEventsViewModel)
    {
        _eventTreeViewModel = eventTreeViewModel;
        _eventDetailsViewModel = eventDetailsViewModel;

        MoveEventCommand = new MoveEventUpCommand(eventTreeViewModel, eventDetailsViewModel);
        HideCommand = new HideEventCommand(eventTreeViewModel, eventDetailsViewModel, hiddenEventsViewModel);
    }
}
