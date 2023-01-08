using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.Stores;
using System.Windows.Input;

namespace EventTimelineReconstruction.ViewModels;

public sealed class LoadWorkViewModel : ViewModelBase, IFileSelectable
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

    private bool _isLoading;

    public bool IsLoading
    {
        get
        {
            return _isLoading;
        }
        set
        {
            _isLoading = value;
            this.OnPropertyChanged(nameof(IsLoading));
        }
    }

    public ICommand ChooseFileCommand { get; }
    public ICommand LoadCommand { get; }

    public LoadWorkViewModel(EventTreeViewModel eventTreeViewModel, AbstractedEventsViewModel abstractedEventsViewModel, IEventsStore store, IWorkLoader fileLoader)
    {
        ChooseFileCommand = new ChooseLoadFileCommand(this);
        LoadCommand = new LoadWorkCommand(this, eventTreeViewModel, abstractedEventsViewModel, store, fileLoader);
    }
}
