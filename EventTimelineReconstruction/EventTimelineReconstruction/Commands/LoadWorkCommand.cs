using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace EventTimelineReconstruction.Commands;

public class LoadWorkCommand : AsyncCommandBase
{
    private readonly LoadWorkViewModel _loadWorkViewModel;
    private readonly EventTreeViewModel _eventTreeViewModel;
    private readonly IEventsStore _store;
    private readonly IWorkLoader _workLoader;

    public LoadWorkCommand(LoadWorkViewModel loadWorkViewModel, EventTreeViewModel eventTreeViewModel, IEventsStore store, IWorkLoader workLoader)
    {
        _loadWorkViewModel = loadWorkViewModel;
        _eventTreeViewModel = eventTreeViewModel;
        _store = store;
        _workLoader = workLoader;
        _loadWorkViewModel.PropertyChanged += this.OnViewModelPropertyChanged;
    }

    public override bool CanExecute(object parameter)
    {
        return !string.IsNullOrEmpty(_loadWorkViewModel.FileName) && base.CanExecute(parameter);
    }

    public override async Task ExecuteAsync(object parameter)
    {
        _loadWorkViewModel.IsLoading = true;

        List<EventViewModel> loadedEvents = await _workLoader.LoadWork(_loadWorkViewModel.FileName);
        _store.LoadEvents(loadedEvents);
        _eventTreeViewModel.LoadEvents(loadedEvents);

        _loadWorkViewModel.IsLoading = false;
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LoadWorkViewModel.FileName)) {
            this.OnCanExecuteChanged();
        }
    }
}
