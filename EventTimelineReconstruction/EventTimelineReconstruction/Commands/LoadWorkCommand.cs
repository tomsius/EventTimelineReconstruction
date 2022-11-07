using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

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
            List<EventViewModel> loadedEvents = await _workLoader.LoadWork(_loadWorkViewModel.FileName);
            _store.LoadEvents(loadedEvents);
            _eventTreeViewModel.LoadEvents(loadedEvents);
        }
        catch (Exception)
        {
            string message = (string)App.Current.Resources["LoadingError_Message"];
            string caption = (string)App.Current.Resources["LoadingError_Caption"];

            MessageBox.Show(message, caption, MessageBoxButton.OK);
        }
        finally
        {
            _loadWorkViewModel.IsLoading = false;
        }
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LoadWorkViewModel.FileName)) {
            this.OnCanExecuteChanged();
        }
    }
}
