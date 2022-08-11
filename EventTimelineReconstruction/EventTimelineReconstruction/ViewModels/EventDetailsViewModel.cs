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
        }
    }

    public string Date
    {
        get
        {
            if (_selectedEvent == null) {
                return string.Empty;
            }

            DateOnly date = new(_selectedEvent.FullDate.Year, _selectedEvent.FullDate.Month, _selectedEvent.FullDate.Day);
            return date.ToString();
        }
    }

    public string Time
    {
        get
        {
            if (_selectedEvent == null) {
                return string.Empty;
            }

            TimeOnly time = new(_selectedEvent.FullDate.Hour, _selectedEvent.FullDate.Minute, _selectedEvent.FullDate.Second);
            return time.ToString();
        }
    }

    public string Extra
    {
        get
        {
            if (_selectedEvent == null) {
                return string.Empty;
            }

            StringBuilder sb = new();

            foreach (KeyValuePair<string, string> kvp in _selectedEvent.Extra) {
                sb.Append(kvp.Key);
                sb.Append('=');
                sb.Append(kvp.Value);
                sb.Append('\n');
            }

            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }
    }
}
