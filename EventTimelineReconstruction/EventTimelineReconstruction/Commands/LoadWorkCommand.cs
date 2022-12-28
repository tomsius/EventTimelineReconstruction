using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.ViewModels;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace EventTimelineReconstruction.Commands;

public class LoadWorkCommand : AsyncCommandBase
{
    private readonly LoadWorkViewModel _loadWorkViewModel;
    private readonly EventTreeViewModel _eventTreeViewModel;
    private readonly AbstractedEventsViewModel _abstractedEventsViewModel;
    private readonly IEventsStore _store;
    private readonly IWorkLoader _workLoader;

    public LoadWorkCommand(LoadWorkViewModel loadWorkViewModel, EventTreeViewModel eventTreeViewModel, AbstractedEventsViewModel abstractedEventsViewModel, IEventsStore store, IWorkLoader workLoader)
    {
        _loadWorkViewModel = loadWorkViewModel;
        _eventTreeViewModel = eventTreeViewModel;
        _abstractedEventsViewModel = abstractedEventsViewModel;
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
        if (!File.Exists(_loadWorkViewModel.FileName))
        {
            string message = $"{(string)App.Current.Resources["FileNotFound_Message"]} {_loadWorkViewModel.FileName}";
            string caption = (string)App.Current.Resources["FileNotFound_Caption"];

            MessageBox.Show(message, caption, MessageBoxButton.OK);
            return;
        }

        _loadWorkViewModel.IsLoading = true;

        try
        {
            LoadedWork loadedWork = await _workLoader.LoadWork(_loadWorkViewModel.FileName);
            _store.LoadEvents(loadedWork.Events);
            _eventTreeViewModel.LoadEvents(loadedWork.Events);
            _abstractedEventsViewModel.LoadHighLevelEvents(loadedWork.HighLevelEvents);
            _abstractedEventsViewModel.LoadLowLevelEvents(loadedWork.LowLevelEvents);
            _abstractedEventsViewModel.LoadHighLevelArtefacts(loadedWork.HighLevelArtefacts);
            _abstractedEventsViewModel.LoadLowLevelArtefacts(loadedWork.LowLevelArtefacts);
        }
        catch (IOException)
        {
            string message = $"{(string)App.Current.Resources["FileInUse_Message"]} {_loadWorkViewModel.FileName}";
            string caption = (string)App.Current.Resources["FileInUse_Caption"];

            MessageBox.Show(message, caption, MessageBoxButton.OK);
        }
        catch (Exception)
        {
            string message = (string)App.Current.Resources["LoadingError_Message"];
            string caption = (string)App.Current.Resources["LoadingError_Caption"];

            MessageBox.Show(message, caption, MessageBoxButton.OK);
        }

        _loadWorkViewModel.IsLoading = false;
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LoadWorkViewModel.FileName)) {
            this.OnCanExecuteChanged();
        }
    }
}
