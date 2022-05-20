using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace EventTimelineReconstruction.Commands;

public class ImportEventsCommand : AsyncCommandBase
{
    private readonly ImportViewModel _importViewModel;
    private readonly IEventsImporter _eventsImporter;

    public ImportEventsCommand(ImportViewModel importViewModel, IEventsImporter eventsImporter)
    {
        _importViewModel = importViewModel;
        _eventsImporter = eventsImporter;
        _importViewModel.PropertyChanged += this.OnViewModelPropertyChanged;
    }

    public override bool CanExecute(object parameter)
    {
        return !string.IsNullOrEmpty(_importViewModel.FileName) && base.CanExecute(parameter);
    }

    public override async Task ExecuteAsync(object parameter)
    {
        List<EventModel> e = null;
        await Task.Run(() => e = _eventsImporter.Import(_importViewModel.FileName, _importViewModel.FromDate, _importViewModel.ToDate));
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ImportViewModel.FileName))
        {
            this.OnCanExecuteChanged();
        }
    }
}
