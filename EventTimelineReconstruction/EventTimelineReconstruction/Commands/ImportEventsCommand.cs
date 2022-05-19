using EventTimelineReconstruction.ViewModels;
using System;
using System.ComponentModel;

namespace EventTimelineReconstruction.Commands;

public class ImportEventsCommand : CommandBase
{
    private readonly ImportViewModel _importViewModel;

    public ImportEventsCommand(ImportViewModel importViewModel)
    {
        _importViewModel = importViewModel;

        _importViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    public override bool CanExecute(object parameter)
    {
        return !string.IsNullOrEmpty(_importViewModel.FileName) && base.CanExecute(parameter);
    }

    public override void Execute(object parameter)
    {
        throw new NotImplementedException();
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ImportViewModel.FileName))
        {
            OnCanExecuteChanged();
        }
    }
}
