using System.Windows;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Commands;

public class ShowEventDetailsCommand : CommandBase
{
    private readonly EventDetailsViewModel _eventDetailsViewModel;

    public ShowEventDetailsCommand(EventDetailsViewModel eventDetailsViewModel)
    {
        _eventDetailsViewModel = eventDetailsViewModel;
    }

    public override void Execute(object parameter)
    {
        RoutedPropertyChangedEventArgs<object> e = (RoutedPropertyChangedEventArgs<object>)parameter;
        EventViewModel eventViewModel = e.NewValue as EventViewModel;

        if (eventViewModel != null) {
            _eventDetailsViewModel.SelectedEvent = eventViewModel;
        }
    }
}
