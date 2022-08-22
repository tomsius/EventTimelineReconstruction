using System.ComponentModel;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Commands;

public class UnhideEventCommand : CommandBase
{
    private readonly HiddenEventsViewModel _hiddenEventsViewModel;

    public UnhideEventCommand(HiddenEventsViewModel hiddenEventsViewModel)
    {
        _hiddenEventsViewModel = hiddenEventsViewModel;
        _hiddenEventsViewModel.PropertyChanged += this.OnViewModelPropertyChanged;
    }

    public override bool CanExecute(object parameter)
    {
        return _hiddenEventsViewModel.SelectedHiddenEvent != null && base.CanExecute(parameter);
    }

    public override void Execute(object parameter)
    {
        _hiddenEventsViewModel.SelectedHiddenEvent.IsVisible = true;
        _hiddenEventsViewModel.RemoveHiddenEvent(_hiddenEventsViewModel.SelectedHiddenEvent);
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(HiddenEventsViewModel.SelectedHiddenEvent))
        {
            this.OnCanExecuteChanged();
        }
    }
}
