using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Commands;

public sealed class ChangeAbstractionLevelCommand : CommandBase
{
    private readonly AbstractedEventsViewModel _viewModel;

    public ChangeAbstractionLevelCommand(AbstractedEventsViewModel viewModel)
    {
        _viewModel = viewModel;

        _viewModel.PropertyChanged += this.OnViewModelPropertyChanged;
    }

    public override bool CanExecute(object parameter)
    {
        return _viewModel.HighLevelEvents.Any() || _viewModel.LowLevelEvents.Any() || _viewModel.HighLevelArtefacts.Any() || _viewModel.LowLevelArtefacts.Any();
    }

    public override void Execute(object parameter)
    {
        object[] parameters = parameter as object[];
        Button pressedButton = parameters[0] as Button;
        UIElementCollection abstractionLevels = ((Grid)parameters[1]).Children;
        UIElementCollection buttons = ((StackPanel)parameters[2]).Children;
        StackPanel tresholdStackPanel = (StackPanel)parameters[3];

        for (int i = 0; i < buttons.Count; i++)
        {
            if (pressedButton == buttons[i])
            {
                buttons[i].IsEnabled = false;
                abstractionLevels[i].Visibility = Visibility.Visible;
            }
            else
            {
                buttons[i].IsEnabled = true;
                abstractionLevels[i].Visibility = Visibility.Collapsed;
            }
        }

        if (!buttons[3].IsEnabled)
        {
            tresholdStackPanel.Visibility = Visibility.Visible;
        }
        else
        {
            tresholdStackPanel.Visibility = Visibility.Collapsed;
        }
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AbstractedEventsViewModel.HighLevelEvents) || e.PropertyName == nameof(AbstractedEventsViewModel.LowLevelEvents) || e.PropertyName == nameof(AbstractedEventsViewModel.HighLevelArtefacts) || e.PropertyName == nameof(AbstractedEventsViewModel.LowLevelArtefacts))
        {
            this.OnCanExecuteChanged();
        }
    }
}
