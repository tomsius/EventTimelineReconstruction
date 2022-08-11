using System.Windows.Input;
using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.Stores;

namespace EventTimelineReconstruction.ViewModels;
public class SaveWorkViewModel : ViewModelBase, IFileSelectable
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

    public ICommand ChooseFileCommand { get; }
    public ICommand SaveCommand { get; }

    public SaveWorkViewModel(EventTreeViewModel eventTreeViewModel, IWorkSaver fileSaver)
    {
        ChooseFileCommand = new ChooseSaveFileCommand(this);
        SaveCommand = new SaveWorkCommand(this, eventTreeViewModel, fileSaver);
    }
}
