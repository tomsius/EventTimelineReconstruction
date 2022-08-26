using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Commands;

public class FilterTypeCheckedCommand : CommandBase
{
    private readonly FilterViewModel _filterViewModel;

    public FilterTypeCheckedCommand(FilterViewModel filterViewModel)
    {
        _filterViewModel = filterViewModel;
    }

    public override void Execute(object parameter)
    {
        bool isRootCheckBox = false;
        RoutedEventArgs e = parameter as RoutedEventArgs;
        CheckBox rootCheckBox = FilteringUtils.GetRootCheckBox(e.OriginalSource as CheckBox);

        if (rootCheckBox == null)
        {
            rootCheckBox = e.OriginalSource as CheckBox;
            isRootCheckBox = true;
        }

        List<CheckBox> children = FilteringUtils.GetChildrenCheckBoxes(rootCheckBox);

        UpdateCheckBoxes(isRootCheckBox, rootCheckBox, children);
        this.SaveChosenEventTypes(children);
    }

    private static void UpdateCheckBoxes(bool isRootCheckBox, CheckBox rootCheckBox, List<CheckBox> children)
    {
        if (isRootCheckBox)
        {
            UpdateChildCheckBoxes(rootCheckBox, children);
        }
        else
        {
            UpdateRootCheckBox(rootCheckBox, children);
        }
    }

    private static void UpdateChildCheckBoxes(CheckBox rootCheckBox, List<CheckBox> children)
    {
        bool newValue = rootCheckBox.IsChecked == true;

        foreach (CheckBox checkBox in children)
        {
            checkBox.IsChecked = newValue;
        }
    }

    private static void UpdateRootCheckBox(CheckBox rootCheckBox, List<CheckBox> children)
    {
        rootCheckBox.IsChecked = null;

        if (FilteringUtils.AreAllChildrenChecked(children))
        {
            rootCheckBox.IsChecked = true;
        }

        if (FilteringUtils.AreAllChildrenUnchecked(children))
        {
            rootCheckBox.IsChecked = false;
        }
    }

    private void SaveChosenEventTypes(List<CheckBox> children)
    {
        foreach (CheckBox checkBox in children)
        {
            string key = checkBox.Content as string;
            bool value = checkBox.IsChecked == true;

            _filterViewModel.UpdateChosenEventType(key, value);
        }
    }
}
