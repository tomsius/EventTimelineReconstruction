using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Commands;

public sealed class SaveWorkCommand : AsyncCommandBase
{
    private readonly SaveWorkViewModel _saveWorkViewModel;
    private readonly EventTreeViewModel _eventTreeViewModel;
    private readonly AbstractedEventsViewModel _abstractedEventsViewModel;
    private readonly IWorkSaver _workSaver;

    public SaveWorkCommand(SaveWorkViewModel saveWorkViewModel, EventTreeViewModel eventTreeViewModel, AbstractedEventsViewModel abstractedEventsViewModel, IWorkSaver workSaver)
    {
        _saveWorkViewModel = saveWorkViewModel;
        _eventTreeViewModel = eventTreeViewModel;
        _abstractedEventsViewModel = abstractedEventsViewModel;
        _workSaver = workSaver;
        _saveWorkViewModel.PropertyChanged += this.OnViewModelPropertyChanged;
    }

    public override bool CanExecute(object parameter)
    {
        return !string.IsNullOrEmpty(_saveWorkViewModel.FileName) && base.CanExecute(parameter);
    }

    public override async Task ExecuteAsync(object parameter)
    {
        _saveWorkViewModel.IsSaving = true;

        await _workSaver.SaveWork(
            _saveWorkViewModel.FileName, 
            _eventTreeViewModel.Events.ToList(), 
            _abstractedEventsViewModel.HighLevelEvents.ToList(), 
            _abstractedEventsViewModel.LowLevelEvents.ToList(), 
            _abstractedEventsViewModel.HighLevelArtefacts.ToList(), 
            _abstractedEventsViewModel.LowLevelArtefacts.ToList());

        _saveWorkViewModel.IsSaving = false;
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SaveWorkViewModel.FileName)) {
            this.OnCanExecuteChanged();
        }
    }
}
