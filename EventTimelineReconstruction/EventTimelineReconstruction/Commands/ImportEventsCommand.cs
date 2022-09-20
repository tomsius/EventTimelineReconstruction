using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.ViewModels;
using System.ComponentModel;
using System.Threading.Tasks;

namespace EventTimelineReconstruction.Commands;

public class ImportEventsCommand : AsyncCommandBase
{
    private readonly ImportViewModel _importViewModel;
    private readonly EventsStore _store;
    private readonly EventTreeViewModel _eventTreeViewModel;

    public ImportEventsCommand(ImportViewModel importViewModel, EventsStore store, EventTreeViewModel eventTreeViewModel)
    {
        _importViewModel = importViewModel;
        _store = store;
        _eventTreeViewModel = eventTreeViewModel;
        _importViewModel.PropertyChanged += this.OnViewModelPropertyChanged;
    }

    public override bool CanExecute(object parameter)
    {
        return !string.IsNullOrEmpty(_importViewModel.FileName) && base.CanExecute(parameter);
    }

    public override async Task ExecuteAsync(object parameter)
    {
        _importViewModel.IsImporting = true;

        await _store.Import(_importViewModel.FileName, _importViewModel.FromDate, _importViewModel.ToDate);
        _eventTreeViewModel.LoadEvents(_store.Events);

        _importViewModel.IsImporting = false;
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ImportViewModel.FileName))
        {
            this.OnCanExecuteChanged();
        }
    }
}
