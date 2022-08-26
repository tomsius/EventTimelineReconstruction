using System;
using System.Collections.Generic;
using System.Windows.Input;
using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Stores;

namespace EventTimelineReconstruction.ViewModels;

public class FilterViewModel : ViewModelBase
{
    private bool _areAllFiltersApplied;

    public bool AreAllFiltersApplied
    {
        get
        {
            return _areAllFiltersApplied;
        }
        set
        {
            _areAllFiltersApplied = value;
            this.OnPropertyChanged(nameof(AreAllFiltersApplied));
        }
    }

    private readonly Dictionary<string, bool> _chosenEventTypes;

    public Dictionary<string, bool> ChosenEventTypes
    {
        get
        {
            return _chosenEventTypes;
        }
    }

    private string _keyword;

    public string Keyword
    {
        get
        {
            return _keyword;
        }
        set
        {
            _keyword = value;
            this.OnPropertyChanged(nameof(Keyword));
        }
    }

    private DateTime _fromDate = DateTime.Now;

    public DateTime FromDate
    {
        get
        {
            return _fromDate;
        }
        set
        {
            _fromDate = value;
            this.OnPropertyChanged(nameof(FromDate));
        }
    }

    private int? _fromHours;

    public int? FromHours
    {
        get
        {
            return _fromHours;
        }
        set
        {
            _fromHours = value;
            this.OnPropertyChanged(nameof(FromHours));
        }
    }

    private int? _fromMinutes;

    public int? FromMinutes
    {
        get
        {
            return _fromMinutes;
        }
        set
        {
            _fromMinutes = value;
            this.OnPropertyChanged(nameof(FromMinutes));
        }
    }

    public DateTime FullFromDate
    {
        get
        {
            return new DateTime(FromDate.Year, FromDate.Month, FromDate.Day, FromHours ?? 0, FromMinutes ?? 0, 0);
        }
    }

    private DateTime _toDate = DateTime.Now;

    public DateTime ToDate
    {
        get
        {
            return _toDate;
        }
        set
        {
            _toDate = value;
            this.OnPropertyChanged(nameof(ToDate));
        }
    }

    private int? _toHours;

    public int? ToHours
    {
        get
        {
            return _toHours;
        }
        set
        {
            _toHours = value;
            this.OnPropertyChanged(nameof(ToHours));
        }
    }

    private int? _toMinutes;

    public int? ToMinutes
    {
        get
        {
            return _toMinutes;
        }
        set
        {
            _toMinutes = value;
            this.OnPropertyChanged(nameof(ToMinutes));
        }
    }

    public DateTime FullToDate
    {
        get
        {
            return new DateTime(ToDate.Year, ToDate.Month, ToDate.Day, ToHours ?? 0, ToMinutes ?? 0, 0);
        }
    }

    public ICommand InitializeCommand { get; }
    public ICommand FilterChangedCommand { get; }
    public ICommand FilterCheckedCommand { get; }
    public ICommand ApplyCommand { get; }
    public ICommand FilterCommand { get; }

    public FilterViewModel(FilteringStore filteringStore, EventTreeViewModel eventTreeViewModel)
    {
        _areAllFiltersApplied = false;
        _keyword = string.Empty;
        _chosenEventTypes = new();

        InitializeCommand = new InitializeEventTypesCommand(this);
        FilterChangedCommand = new FilterTypeChangedCommand(this);
        FilterCheckedCommand = new FilterTypeCheckedCommand(this);
        ApplyCommand = new ApplyFilterOptionsCommand(this, filteringStore, eventTreeViewModel);
        FilterCommand = new ApplyFiltersCommand(filteringStore, eventTreeViewModel);
    }

    public void UpdateChosenEventType(string key, bool value)
    {
        if (_chosenEventTypes.ContainsKey(key))
        {
            _chosenEventTypes[key] = value;
        }
        else
        {
            _chosenEventTypes.Add(key, value);
        }

        this.OnPropertyChanged(nameof(ChosenEventTypes));
    }
}
