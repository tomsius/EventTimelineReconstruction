﻿namespace EventTimelineReconstruction.ViewModels;
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

    public MainWindowViewModel(EventTreeViewModel eventTreeViewModel, EventDetailsViewModel eventDetailsViewModel)
    {
        _eventTreeViewModel = eventTreeViewModel;
        _eventDetailsViewModel = eventDetailsViewModel;
    }
}
