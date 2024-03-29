﻿using System.Collections.Generic;
using System;
using System.Windows.Media;
using EventTimelineReconstruction.Models;
using System.Text;
using System.Collections.ObjectModel;
using EventTimelineReconstruction.Extensions;
using System.Globalization;

namespace EventTimelineReconstruction.ViewModels;

public sealed class EventViewModel : ViewModelBase, IComparable
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

    public int SourceLine
    {
        get
        {
            return _eventModel.SourceLine;
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

    public string DisplayDate
    {
        get
        {
            return FullDate.ToString("dd/MM/yyyy");
        }
    }

    public string DisplayTime
    {
        get
        {
            return FullDate.ToString("HH:mm:ss");
        }
    }

    public string DisplayExtra
    {
        get
        {
            StringBuilder sb = new();

            foreach (KeyValuePair<string, string> kvp in Extra)
            {
                sb.Append(kvp.Key.Trim());
                sb.Append('=');
                sb.Append(kvp.Value);
                sb.Append(Environment.NewLine);
            }

            sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
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

        return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23}", 
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
            _eventModel.SourceLine,
            _isVisible,
            _colour.ToString()
            );
    }

    public void AddChild(EventViewModel child)
    {
        _children.Add(child);
        _children.Sort();
    }

    public void RemoveChild(EventViewModel child)
    {
        _children.Remove(child);
    }

    public bool ContainsChild(EventViewModel child)
    {
        return _children.Contains(child);
    }

    public int CompareTo(object obj)
    {
        if (obj is null)
        {
            return 1;
        }

        EventViewModel other = obj as EventViewModel;

        int cmp = FullDate.CompareTo(other.FullDate);

        if (cmp == 0)
        {
            return Filename.CompareTo(other.Filename);
        }

        return cmp;
    }
}
