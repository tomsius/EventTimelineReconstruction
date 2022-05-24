using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.ViewModels;
using System.ComponentModel;
using System.Threading.Tasks;

namespace EventTimelineReconstruction.Commands;
public class LoadWorkCommand : AsyncCommandBase
{
    private readonly LoadWorkViewModel _loadWorkViewModel;
    private readonly EventsStore _store;
    private readonly EventTreeViewModel _eventTreeViewModel;

    public LoadWorkCommand(LoadWorkViewModel loadWorkViewModel, EventsStore store, EventTreeViewModel eventTreeViewModel)
    {
        _loadWorkViewModel = loadWorkViewModel;
        _store = store;
        _eventTreeViewModel = eventTreeViewModel;
        _loadWorkViewModel.PropertyChanged += this.OnViewModelPropertyChanged;
    }

    public override bool CanExecute(object parameter)
    {
        return !string.IsNullOrEmpty(_loadWorkViewModel.FileName) && base.CanExecute(parameter);
    }

    public override async Task ExecuteAsync(object parameter)
    {
        await _store.LoadWork(_loadWorkViewModel.FileName);
        _eventTreeViewModel.LoadEvents(_store.Events);
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LoadWorkViewModel.FileName)) {
            this.OnCanExecuteChanged();
        }
    }
}
