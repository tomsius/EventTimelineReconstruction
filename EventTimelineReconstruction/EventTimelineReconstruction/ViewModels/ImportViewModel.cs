using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Stores;
using System;
using System.Windows.Input;

namespace EventTimelineReconstruction.ViewModels;

public class ImportViewModel : ViewModelBase, IFileSelectable
{
    private string _fileName;

    public string FileName
    {
        get 
        { 
            return _fileName; 
        }
        set 
        { 
            _fileName = value;
            this.OnPropertyChanged(nameof(FileName));
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

    private int _fromHours;

    public int FromHours
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

    private int _fromMinutes;

    public int FromMinutes
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

    public DateTime FromFullDate
    {
        get
        {
            return new DateTime(FromDate.Year, FromDate.Month, FromDate.Day, FromHours, FromMinutes, 0);
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

    private int _toHours;

    public int ToHours
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

    private int _toMinutes;

    public int ToMinutes
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
            return new DateTime(ToDate.Year, ToDate.Month, ToDate.Day, ToHours, ToMinutes, 0);
        }
    }

    public ICommand ChooseFileCommand { get; }
    public ICommand ImportCommand { get; }
    public ICommand CancelCommand { get; }


    private readonly EventTreeViewModel _viewModel;

    public ImportViewModel(EventTreeViewModel viewModel, EventsStore store)
    {
        _viewModel = viewModel;
        ChooseFileCommand = new ChooseFileCommand(this);
        ImportCommand = new ImportEventsCommand(this, store, _viewModel);
    }
}
