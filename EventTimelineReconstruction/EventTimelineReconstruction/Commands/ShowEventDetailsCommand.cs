using System.Windows;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Commands;

public class ShowEventDetailsCommand : CommandBase
{
    private readonly EventDetailsViewModel _eventDetailsViewModel;
    private readonly ChangeColourViewModel _changeColourViewModel;

    public ShowEventDetailsCommand(EventDetailsViewModel eventDetailsViewModel, ChangeColourViewModel changeColourViewModel)
    {
        _eventDetailsViewModel = eventDetailsViewModel;
        _changeColourViewModel = changeColourViewModel;
    }

    public override void Execute(object parameter)
    {
        RoutedPropertyChangedEventArgs<object> e = (RoutedPropertyChangedEventArgs<object>)parameter;

        if (e.NewValue is EventViewModel eventViewModel)
        {
            _eventDetailsViewModel.SelectedEvent = eventViewModel;
            _changeColourViewModel.SetBrushColour(eventViewModel.Colour);
        }
    }
}
