using EventTimelineReconstruction.Utils;
using System.Collections.Generic;
using System.Windows.Controls;
using EventTimelineReconstruction.ViewModels;
using System.Windows;

namespace EventTimelineReconstruction.Commands;

public class InitializeEventTypesCommand : CommandBase
{
    private FilterViewModel _filterViewModel;

    public InitializeEventTypesCommand(FilterViewModel filterViewModel)
    {
        _filterViewModel = filterViewModel;
    }

    public override void Execute(object parameter)
    {
        RoutedEventArgs e = parameter as RoutedEventArgs;
        List<CheckBox> children = FilteringUtils.GetChildrenCheckBoxes(e.OriginalSource as CheckBox);

        foreach (CheckBox checkBox in children)
        {
            string key = checkBox.Content as string;
            bool value = checkBox.IsChecked == true;

            _filterViewModel.UpdateChosenEventType(key, value);
        }
    }
}
