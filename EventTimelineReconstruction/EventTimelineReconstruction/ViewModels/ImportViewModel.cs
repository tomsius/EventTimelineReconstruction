using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.Validators;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Input;

namespace EventTimelineReconstruction.ViewModels;

public class ImportViewModel : ViewModelBase, INotifyDataErrorInfo, IFileSelectable
{
    private readonly ITimeValidator _validator;
    private readonly IErrorsViewModel _errorsViewModel;

    public IErrorsViewModel ErrorsViewModel
    {
        get
        {
            return _errorsViewModel;
        }
    }

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

            _errorsViewModel.ClearErrors(nameof(FileName));

            if (string.IsNullOrEmpty(_fileName))
            {
                _errorsViewModel.AddError(nameof(FileName), (string)App.Current.Resources["Error_Filename"]);
            }
        }
    }

    private DateTime _fromDate;

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

            this.ValidateDate();
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
            
            this.ValidateHours(nameof(FromHours), _fromHours, "Error_From_Hours");
            this.ValidateDate();
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

            this.ValidateMinutes(nameof(FromMinutes), _fromMinutes, "Error_From_Minutes");
            this.ValidateDate();
        }
    }

    public DateTime FullFromDate
    {
        get
        {
            return new DateTime(FromDate.Year, FromDate.Month, FromDate.Day, FromHours, FromMinutes, 0);
        }
    }

    private DateTime _toDate;

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

            this.ValidateDate();
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

            this.ValidateHours(nameof(ToHours), _toHours, "Error_To_Hours");
            this.ValidateDate();
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

            this.ValidateMinutes(nameof(ToMinutes), _toMinutes, "Error_To_Minutes");
            this.ValidateDate();
        }
    }

    public DateTime FullToDate
    {
        get
        {
            return new DateTime(ToDate.Year, ToDate.Month, ToDate.Day, ToHours, ToMinutes, 0);
        }
    }

    private bool _isImporting;

    public bool IsImporting
    {
        get
        {
            return _isImporting;
        }
        set
        {
            _isImporting = value;
            this.OnPropertyChanged(nameof(IsImporting));
        }
    }

    public bool HasErrors
    {
        get
        {
            return _errorsViewModel.HasErrors;
        }
    }

    public ICommand ChooseFileCommand { get; }
    public ICommand ImportCommand { get; }

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    public ImportViewModel(EventTreeViewModel viewModel, IEventsStore store, ITimeValidator validator, IErrorsViewModel errorsViewModel, IDateTimeProvider dateTimeProvider)
    {
        _errorsViewModel = errorsViewModel;
        _errorsViewModel.ErrorsChanged += this.ErrorsViewModel_ErrorsChanged;
        _validator = validator;

        _fromDate = dateTimeProvider.Now;
        _toDate = dateTimeProvider.Now;

        ChooseFileCommand = new ChooseLoadFileCommand(this);
        ImportCommand = new ImportEventsCommand(this, store, viewModel);
    }

    private void ErrorsViewModel_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
    {
        ErrorsChanged?.Invoke(this, e);
        this.OnPropertyChanged(nameof(HasErrors));
    }

    public IEnumerable GetErrors(string propertyName)
    {
        return _errorsViewModel.GetErrors(propertyName);
    }

    private void ValidateDate()
    {
        _errorsViewModel.ClearErrors(nameof(FromDate));
        _errorsViewModel.ClearErrors(nameof(ToDate));

        try
        {
            if (!_validator.AreDatesValid(FullFromDate, FullToDate))
            {
                _errorsViewModel.AddError(nameof(FromDate), (string)App.Current.Resources["Error_From_After_To"]);
                _errorsViewModel.AddError(nameof(ToDate), (string)App.Current.Resources["Error_To_Before_From"]);
            }
        }
        catch (ArgumentOutOfRangeException) { }
    }

    private void ValidateHours(string propertyName, int hours, string resourceKey)
    {
        _errorsViewModel.ClearErrors(propertyName);

        if (!_validator.AreHoursValid(hours))
        {
            _errorsViewModel.AddError(propertyName, (string)App.Current.Resources[resourceKey]);
        }
    }

    private void ValidateMinutes(string propertyName, int minutes, string resourceKey)
    {
        _errorsViewModel.ClearErrors(propertyName);

        if (!_validator.AreMinutesValid(minutes))
        {
            _errorsViewModel.AddError(propertyName, (string)App.Current.Resources[resourceKey]);
        }
    }
}
