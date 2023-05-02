using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using EventTimelineReconstruction.Abstractors;
using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;

namespace EventTimelineReconstruction.ViewModels;

public class AbstractedEventsViewModel : ViewModelBase, INotifyDataErrorInfo
{
    private readonly IErrorsViewModel _errorsViewModel;

    private readonly RangeEnabledObservableCollection<HighLevelEventViewModel> _highLevelEvents;

    public IEnumerable<ISerializableLevel> HighLevelEvents
    {
        get
        {
            return _highLevelEvents;
        }
    }

    private HighLevelEventViewModel _selectedHighLevelEvent;

    public HighLevelEventViewModel SelectedHighLevelEvent
    {
        get
        {
            return _selectedHighLevelEvent;
        }
        set
        {
            _selectedHighLevelEvent = value;
            this.OnPropertyChanged(nameof(SelectedHighLevelEvent));
        }
    }

    private readonly RangeEnabledObservableCollection<LowLevelEventViewModel> _lowLevelEvents;

    public IEnumerable<ISerializableLevel> LowLevelEvents
    {
        get
        {
            return _lowLevelEvents;
        }
    }

    private LowLevelEventViewModel _selectedLowLevelEvent;

    public LowLevelEventViewModel SelectedLowLevelEvent
    {
        get
        {
            return _selectedLowLevelEvent;
        }
        set
        {
            _selectedLowLevelEvent = value;
            this.OnPropertyChanged(nameof(SelectedLowLevelEvent));
        }
    }

    private readonly RangeEnabledObservableCollection<HighLevelArtefactViewModel> _highLevelArtefacts;

    public IEnumerable<ISerializableLevel> HighLevelArtefacts
    {
        get
        {
            return _highLevelArtefacts;
        }
    }

    private HighLevelArtefactViewModel _selectedHighLevelArtefact;

    public HighLevelArtefactViewModel SelectedHighLevelArtefact
    {
        get
        {
            return _selectedHighLevelArtefact;
        }
        set
        {
            _selectedHighLevelArtefact = value;
            this.OnPropertyChanged(nameof(SelectedHighLevelArtefact));
        }
    }

    private readonly RangeEnabledObservableCollection<LowLevelArtefactViewModel> _lowLevelArtefacts;

    public IEnumerable<ISerializableLevel> LowLevelArtefacts
    {
        get
        {
            return _lowLevelArtefacts;
        }
    }

    private LowLevelArtefactViewModel _selectedLowLevelArtefact;

    public LowLevelArtefactViewModel SelectedLowLevelArtefact
    {
        get
        {
            return _selectedLowLevelArtefact;
        }
        set
        {
            _selectedLowLevelArtefact = value;
            this.OnPropertyChanged(nameof(SelectedLowLevelArtefact));
        }
    }

    private bool _isAbstracting;

    public bool IsAbstracting
    {
        get
        {
            return _isAbstracting;
        }
        set
        {
            _isAbstracting = value;
            this.OnPropertyChanged(nameof(IsAbstracting));
        }
    }

    private string _threshold = "1";

    public string Threshold
    {
        get
        {
            return _threshold;
        }
        set
        {
            _threshold = value;

            _errorsViewModel.ClearErrors(nameof(Threshold));

            if (!double.TryParse(_threshold, out double result) || result.CompareTo(0.0) < 0)
            {
                _errorsViewModel.AddError(nameof(Threshold), "");
            }

            this.OnPropertyChanged(nameof(Threshold));
        }
    }

    public bool HasErrors
    {
        get
        {
            return _errorsViewModel.HasErrors;
        }
    }

    public ICommand AbstractCommand { get; }
    public ICommand ChangeCommand { get; }
    public ICommand ReformLowLevelArtefactsCommand { get; }

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    public AbstractedEventsViewModel(IEventsStore store, IHighLevelEventsAbstractor highLevelEventsAbstractor, ILowLevelEventsAbstractor lowLevelEventsAbstractor, IHighLevelArtefactsAbstractor highLevelArtefactsAbstractor, ILowLevelArtefactsAbstractor lowLevelArtefactsAbstractor, IErrorsViewModel errorsViewModel)
    {
        _highLevelEvents = new();
        _lowLevelEvents = new();
        _highLevelArtefacts = new();
        _lowLevelArtefacts = new();

        _errorsViewModel = errorsViewModel;
        _errorsViewModel.ErrorsChanged += this.ErrorsViewModel_ErrorsChanged;

        AbstractCommand = new AbstractEventsCommand(this, store, highLevelEventsAbstractor, lowLevelEventsAbstractor, highLevelArtefactsAbstractor, lowLevelArtefactsAbstractor);
        ChangeCommand = new ChangeAbstractionLevelCommand(this);
        ReformLowLevelArtefactsCommand = new AbstractLowLevelArtefactsCommand(this, store, lowLevelArtefactsAbstractor);
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

    public void LoadHighLevelEvents(List<HighLevelEventViewModel> highLevelEvents)
    {
        _highLevelEvents.Clear();
        _highLevelEvents.AddRange(highLevelEvents);
        this.OnPropertyChanged(nameof(HighLevelEvents));
    }

    public void LoadLowLevelEvents(List<LowLevelEventViewModel> lowLevelEvents)
    {
        _lowLevelEvents.Clear();
        _lowLevelEvents.AddRange(lowLevelEvents);
        this.OnPropertyChanged(nameof(LowLevelEvents));
    }

    public void LoadHighLevelArtefacts(List<HighLevelArtefactViewModel> highLevelArtefacts)
    {
        _highLevelArtefacts.Clear();
        _highLevelArtefacts.AddRange(highLevelArtefacts);
        this.OnPropertyChanged(nameof(HighLevelArtefacts));
    }

    public void LoadLowLevelArtefacts(List<LowLevelArtefactViewModel> lowLevelArtefacts)
    {
        _lowLevelArtefacts.Clear();
        _lowLevelArtefacts.AddRange(lowLevelArtefacts);
        this.OnPropertyChanged(nameof(LowLevelArtefacts));
    }
}
