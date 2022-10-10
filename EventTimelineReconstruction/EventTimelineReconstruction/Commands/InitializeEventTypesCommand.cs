using EventTimelineReconstruction.Utils;
using System.Collections.Generic;
using System.Windows.Controls;
using EventTimelineReconstruction.ViewModels;
using System.Windows;

namespace EventTimelineReconstruction.Commands;

public class InitializeEventTypesCommand : CommandBase
{
    private readonly FilterViewModel _filterViewModel;
    private readonly IFilteringUtils _filteringUtils;

    public InitializeEventTypesCommand(FilterViewModel filterViewModel, IFilteringUtils filteringUtils)
    {
        _filterViewModel = filterViewModel;
        _filteringUtils = filteringUtils;
    }

    public override void Execute(object parameter)
    {
        RoutedEventArgs e = parameter as RoutedEventArgs;
        List<CheckBox> children = _filteringUtils.GetChildrenCheckBoxes(e.OriginalSource as CheckBox);

        foreach (CheckBox checkBox in children)
        {
            string key = checkBox.Content as string;
            bool value = checkBox.IsChecked == true;

            _filterViewModel.UpdateChosenEventType(key, value);
        }
    }
}
