using System;
using System.Collections.Generic;
using System.Text;

namespace EventTimelineReconstruction.ViewModels;
public class EventDetailsViewModel : ViewModelBase
{
    private EventViewModel _selectedEvent;

    public EventViewModel SelectedEvent
    {
        get 
        { 
            return _selectedEvent; 
        }
        set
        {
            _selectedEvent = value;
            this.OnPropertyChanged(nameof(SelectedEvent));
        }
    }
}
