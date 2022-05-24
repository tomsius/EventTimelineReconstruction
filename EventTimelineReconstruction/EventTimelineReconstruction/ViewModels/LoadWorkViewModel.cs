using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Stores;
using System.Windows.Input;

namespace EventTimelineReconstruction.ViewModels;
public class LoadWorkViewModel : ViewModelBase, IFileSelectable
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
    public ICommand LoadCommand { get; }

    public LoadWorkViewModel(EventTreeViewModel viewModel, EventsStore store)
    {
        ChooseFileCommand = new ChooseLoadFileCommand(this);
        LoadCommand = new LoadWorkCommand(this, store, viewModel);
    }
}
