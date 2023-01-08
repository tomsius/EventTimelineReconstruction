using EventTimelineReconstruction.ViewModels;
using Microsoft.Win32;

namespace EventTimelineReconstruction.Commands;

public sealed class ChooseSaveFileCommand : CommandBase
{
    private readonly IFileSelectable _viewModel;

    public ChooseSaveFileCommand(IFileSelectable viewModel)
    {
        _viewModel = viewModel;
    }

    public override void Execute(object parameter)
    {
        SaveFileDialog saveFileDialog = new()
        {
            Filter = "Event Timeline Reconstruction files (*.etr)|*.etr"
        };

        if (saveFileDialog.ShowDialog() == true) {
            _viewModel.FileName = saveFileDialog.FileName;
        }
    }
}
