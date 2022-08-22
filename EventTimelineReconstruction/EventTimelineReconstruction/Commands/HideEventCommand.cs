﻿using System.ComponentModel;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Commands;

public class HideEventCommand : CommandBase
{
    private readonly EventDetailsViewModel _eventDetailsViewModel;
    private readonly HiddenEventsViewModel _hiddenEventsViewModel;

    public HideEventCommand(EventDetailsViewModel eventDetailsViewModel, HiddenEventsViewModel hiddenEventsViewModel)
    {
        _eventDetailsViewModel = eventDetailsViewModel;
        _hiddenEventsViewModel = hiddenEventsViewModel;
        _eventDetailsViewModel.PropertyChanged += this.OnViewModelPropertyChanged;
    }

    public override bool CanExecute(object parameter)
    {
        return _eventDetailsViewModel.SelectedEvent != null && base.CanExecute(parameter);
    }

    public override void Execute(object parameter)
    {
        _eventDetailsViewModel.SelectedEvent.IsVisible = false;
        _hiddenEventsViewModel.AddHiddenEvent(_eventDetailsViewModel.SelectedEvent);
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(EventDetailsViewModel.SelectedEvent)) {
            this.OnCanExecuteChanged();
        }
    }
}
