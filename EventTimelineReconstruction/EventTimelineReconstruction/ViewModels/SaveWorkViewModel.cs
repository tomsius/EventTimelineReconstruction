using System.Windows.Input;
using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Services;

namespace EventTimelineReconstruction.ViewModels;

public sealed class SaveWorkViewModel : ViewModelBase, IFileSelectable
{
    private string _fileName;

    public string FileName
    {
        get
        {
            return _fileName;
        }
        set
        {
            _fileName = value;
            this.OnPropertyChanged(nameof(FileName));
        }
    }

    private bool _isSaving;

    public bool IsSaving
    {
        get
        {
            return _isSaving;
        }
        set
        {
            _isSaving = value;
            this.OnPropertyChanged(nameof(IsSaving));
        }
    }

    public ICommand ChooseFileCommand { get; }
    public ICommand SaveCommand { get; }

    public SaveWorkViewModel(EventTreeViewModel eventTreeViewModel, AbstractedEventsViewModel abstractedEventsViewModel, IWorkSaver fileSaver)
    {
        ChooseFileCommand = new ChooseSaveFileCommand(this);
        SaveCommand = new SaveWorkCommand(this, eventTreeViewModel, abstractedEventsViewModel, fileSaver);
    }
}
