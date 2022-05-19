using EventTimelineReconstruction.Commands;
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
            OnPropertyChanged(nameof(FileName));
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
            OnPropertyChanged(nameof(FromDate));
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
            OnPropertyChanged(nameof(FromHours));
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
            OnPropertyChanged(nameof(FromMinutes));
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
            OnPropertyChanged(nameof(ToDate));
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
            OnPropertyChanged(nameof(ToHours));
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
            OnPropertyChanged(nameof(ToMinutes));
        }
    }

    public ICommand ChooseFileCommand { get; }
    public ICommand ImportCommand { get; }
    public ICommand CancelCommand { get; }

    public ImportViewModel()
    {
        ChooseFileCommand = new ChooseFileCommand(this);
        ImportCommand = new ImportEventsCommand(this);
    }
}
