using System.ComponentModel;
using System.Threading.Tasks;
using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Commands;

public class SaveWorkCommand : AsyncCommandBase
{
    private readonly SaveWorkViewModel _saveWorkViewModel;
    private readonly EventTreeViewModel _eventTreeViewModel;
    private readonly IWorkSaver _workSaver;

    public SaveWorkCommand(SaveWorkViewModel saveWorkViewModel, EventTreeViewModel eventTreeViewModel, IWorkSaver workSaver)
    {
        _saveWorkViewModel = saveWorkViewModel;
        _eventTreeViewModel = eventTreeViewModel;
        _workSaver = workSaver;
        _saveWorkViewModel.PropertyChanged += this.OnViewModelPropertyChanged;
    }

    public override bool CanExecute(object parameter)
    {
        return !string.IsNullOrEmpty(_saveWorkViewModel.FileName) && base.CanExecute(parameter);
    }

    public override async Task ExecuteAsync(object parameter)
    {
        await Task.Run(() => _workSaver.SaveWork(_saveWorkViewModel.FileName, _eventTreeViewModel.Events));
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SaveWorkViewModel.FileName)) {
            this.OnCanExecuteChanged();
        }
    }
}
