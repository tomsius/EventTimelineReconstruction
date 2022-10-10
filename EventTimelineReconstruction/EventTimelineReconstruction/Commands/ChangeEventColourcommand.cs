using System.Windows.Media;
using EventTimelineReconstruction.ViewModels;
using System.ComponentModel;

namespace EventTimelineReconstruction.Commands;

public class ChangeEventColourCommand : CommandBase
{
    private readonly ChangeColourViewModel _changeColourViewModel;
    private readonly EventDetailsViewModel _eventDetailsViewModel;

    public ChangeEventColourCommand(ChangeColourViewModel changeColourViewModel, EventDetailsViewModel eventDetailsViewModel)
    {
        _changeColourViewModel = changeColourViewModel;
        _eventDetailsViewModel = eventDetailsViewModel;

        _changeColourViewModel.PropertyChanged += this.OnViewModelPropertyChanged;
        _eventDetailsViewModel.PropertyChanged += this.OnViewModelPropertyChanged;
    }

    public override bool CanExecute(object parameter)
    {
        Color transparent = Colors.Transparent;
        return _changeColourViewModel.SelectedColour != transparent && _eventDetailsViewModel.SelectedEvent != null && base.CanExecute(parameter);
    }

    public override void Execute(object parameter)
    {
        Brush brush = new SolidColorBrush(_changeColourViewModel.SelectedColour);
        brush.Freeze();
        _eventDetailsViewModel.SelectedEvent.Colour = brush;
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ChangeColourViewModel.SelectedColour) || e.PropertyName == nameof(EventDetailsViewModel.SelectedEvent))
        {
            this.OnCanExecuteChanged();
        }
    }
}
