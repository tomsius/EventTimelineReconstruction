using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.ViewModels;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace EventTimelineReconstruction.Commands;

public class ImportEventsCommand : AsyncCommandBase
{
    private readonly ImportViewModel _importViewModel;
    private readonly IEventsStore _store;
    private readonly EventTreeViewModel _eventTreeViewModel;

    public ImportEventsCommand(ImportViewModel importViewModel, IEventsStore store, EventTreeViewModel eventTreeViewModel)
    {
        _importViewModel = importViewModel;
        _store = store;
        _eventTreeViewModel = eventTreeViewModel;
        _importViewModel.PropertyChanged += this.OnViewModelPropertyChanged;
    }

    public override bool CanExecute(object parameter)
    {
        return !string.IsNullOrEmpty(_importViewModel.FileName) && !_importViewModel.HasErrors && base.CanExecute(parameter);
    }

    public override async Task ExecuteAsync(object parameter)
    {
        if (!File.Exists(_importViewModel.FileName))
        {
            string message = $"{(string)App.Current.Resources["FileNotFound_Message"]} {_importViewModel.FileName}";
            string caption = (string)App.Current.Resources["FileNotFound_Caption"];

            MessageBox.Show(message, caption, MessageBoxButton.OK);
            return;
        }

        _importViewModel.IsImporting = true;

        try
        {
            await _store.Import(_importViewModel.FileName, _importViewModel.FromDate, _importViewModel.ToDate);
            _eventTreeViewModel.LoadEvents(_store.Events);
        }
        catch (IOException)
        {
            string message = $"{(string)App.Current.Resources["FileInUse_Message"]} {_importViewModel.FileName}";
            string caption = (string)App.Current.Resources["FileInUse_Caption"];

            MessageBox.Show(message, caption, MessageBoxButton.OK);
        }

        _importViewModel.IsImporting = false;
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ImportViewModel.FileName) || e.PropertyName == nameof(ImportViewModel.HasErrors))
        {
            this.OnCanExecuteChanged();
        }
    }
}
