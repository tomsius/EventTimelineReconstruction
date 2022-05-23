using System.Collections.Generic;
using System;
using System.Windows.Media;
using EventTimelineReconstruction.Models;

namespace EventTimelineReconstruction.ViewModels;
public class EventViewModel : ViewModelBase
{
    private readonly EventModel _eventModel;

    public DateTime FullDate
    {
        get
        {
            return new(_eventModel.Date.Year, _eventModel.Date.Month, _eventModel.Date.Day, _eventModel.Time.Hour, _eventModel.Time.Minute, _eventModel.Time.Second);
        }
    }

    public TimeZoneInfo Timezone
    {
        get
        {
            return _eventModel.Timezone;
        }
    }

    public string MACB
    {
        get
        {
            return _eventModel.MACB;
        }
    }

    public string Source
    {
        get
        {
            return _eventModel.Source;
        }
    }

    public string SourceType
    {
        get
        {
            return _eventModel.SourceType;
        }
    }

    public string Type
    {
        get
        {
            return _eventModel.Type;
        }
    }

    public string User
    {
        get
        {
            return _eventModel.User;
        }
    }

    public string Host
    {
        get
        {
            return _eventModel.Host;
        }
    }

    public string Short
    {
        get
        {
            return _eventModel.Short;
        }
    }

    public string Description
    {
        get
        {
            return _eventModel.Description;
        }
    }

    public double Version
    {
        get
        {
            return _eventModel.Version;
        }
    }

    public string Filename
    {
        get
        {
            return _eventModel.Filename;
        }
    }

    public string INode
    {
        get
        {
            return _eventModel.INode;
        }
    }

    public string Notes
    {
        get
        {
            return _eventModel.Notes;
        }
    }

    public string Format
    {
        get
        {
            return _eventModel.Format;
        }
    }

    public Dictionary<string, string> Extra
    {
        get
        {
            return new(_eventModel.Extra);
        }
    }

    private bool _isVisible;
    public bool IsVisible
    {
        get
        {
            return _isVisible;
        }
        set
        {
            _isVisible = value;
            this.OnPropertyChanged(nameof(IsVisible));
        }
    }

    private Brush _colour;
    public Brush Colour
    {
        get
        {
            return _colour;
        }
        set
        {
            _colour = value;
            this.OnPropertyChanged(nameof(Colour));
        }
    }

    public string DisplayName
    {
        get
        {
            return string.Format("{0, 10} {1}", FullDate, Filename);
        }
    }

    public EventViewModel(EventModel eventModel)
    {
        _eventModel = eventModel;

        IsVisible = true;

        // TODO - remove random colour assignment
        Random rnd = new();
        if (rnd.NextDouble() < 0.3) {
            Colour = Brushes.Aquamarine;
        }
        else
        {
            Colour = Brushes.White;
        }
    }
}
