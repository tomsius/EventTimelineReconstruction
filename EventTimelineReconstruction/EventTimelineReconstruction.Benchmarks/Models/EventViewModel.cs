using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace EventTimelineReconstruction.Benchmarks.Models;

public class EventViewModel
{
    private readonly EventModel _eventModel;
    private readonly ObservableCollection<EventViewModel> _children;

    public ObservableCollection<EventViewModel> Children
    {
        get
        {
            return _children;
        }
    }

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
            return _eventModel.Extra;
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
        }
    }

    private Brush? _colour;

    public Brush? Colour
    {
        get
        {
            return _colour;
        }
        set
        {
            _colour = value;
        }
    }

    public EventViewModel(EventModel eventModel)
    {
        _eventModel = eventModel;
        IsVisible = true;
        Colour = Brushes.Black;

        _children = new();
    }

    public string Serialize()
    {
        StringBuilder serializedExtra = new();
        foreach (KeyValuePair<string, string> pair in _eventModel.Extra)
        {
            serializedExtra.Append(pair.Key);
            serializedExtra.Append(':');
            serializedExtra.Append(pair.Value);
            serializedExtra.Append(';');
        }

        serializedExtra.Remove(serializedExtra.Length - 1, 1);

        return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22}",
            _eventModel.Date.Year, _eventModel.Date.Month, _eventModel.Date.Day, _eventModel.Time.Hour, _eventModel.Time.Minute, _eventModel.Time.Second,
            _eventModel.Timezone.ToSerializedString(),
            _eventModel.MACB,
            _eventModel.Source,
            _eventModel.SourceType,
            _eventModel.Type,
            _eventModel.User,
            _eventModel.Host,
            _eventModel.Short,
            _eventModel.Description,
            _eventModel.Version.ToString(CultureInfo.InvariantCulture),
            _eventModel.Filename,
            _eventModel.INode,
            _eventModel.Notes,
            _eventModel.Format,
            serializedExtra.ToString(),
            _isVisible,
            _colour.ToString()
            );
    }

    public void AddChild(EventViewModel child)
    {
        _children.Add(child);
    }
}
