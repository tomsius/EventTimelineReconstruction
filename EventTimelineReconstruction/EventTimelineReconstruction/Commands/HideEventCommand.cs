using System.ComponentModel;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Commands;

public class HideEventCommand : CommandBase
{
    private readonly EventDetailsViewModel _eventDetailsViewModel;

    public HideEventCommand(EventDetailsViewModel eventDetailsViewModel)
    {
        _eventDetailsViewModel = eventDetailsViewModel;
        _eventDetailsViewModel.PropertyChanged += this.OnViewModelPropertyChanged;
    }

    public override bool CanExecute(object parameter)
    {
        return _eventDetailsViewModel.SelectedEvent != null && base.CanExecute(parameter);
    }

    public override void Execute(object parameter)
    {
        _eventDetailsViewModel.SelectedEvent.IsVisible = false;
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(EventDetailsViewModel.SelectedEvent)) {
            this.OnCanExecuteChanged();
        }
    }
}
