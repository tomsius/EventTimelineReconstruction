using System.ComponentModel;
using System.Threading.Tasks;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Commands;
public class SaveWorkCommand : AsyncCommandBase
{
    private readonly SaveWorkViewModel _saveWorkViewModel;
    private readonly EventsStore _store;

    public SaveWorkCommand(SaveWorkViewModel saveWorkViewModel, EventsStore store)
    {
        _saveWorkViewModel = saveWorkViewModel;
        _store = store;
        _saveWorkViewModel.PropertyChanged += this.OnViewModelPropertyChanged;
    }

    public override bool CanExecute(object parameter)
    {
        return !string.IsNullOrEmpty(_saveWorkViewModel.FileName) && base.CanExecute(parameter);
    }

    public override async Task ExecuteAsync(object parameter)
    {
        await _store.SaveWork(_saveWorkViewModel.FileName);
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SaveWorkViewModel.FileName)) {
            this.OnCanExecuteChanged();
        }
    }
}
