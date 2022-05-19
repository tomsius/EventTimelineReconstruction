﻿using EventTimelineReconstruction.ViewModels;
using Microsoft.Win32;

namespace EventTimelineReconstruction.Commands;

public class ChooseFileCommand : CommandBase
{
    private readonly IFileSelectable _viewModel;

    public ChooseFileCommand(IFileSelectable viewModel)
    {
        _viewModel = viewModel;
    }

    public override void Execute(object parameter)
    {
        OpenFileDialog openFileDialog = new();
        openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";

        if (openFileDialog.ShowDialog() == true)
        {
            _viewModel.FileName = openFileDialog.FileName;
        }
    }
}
