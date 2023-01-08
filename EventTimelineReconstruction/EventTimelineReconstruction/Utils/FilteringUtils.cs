using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Collections.Generic;

namespace EventTimelineReconstruction.Utils;

public sealed class FilteringUtils : IFilteringUtils
{
    public CheckBox GetRootCheckBox(CheckBox item)
    {
        DependencyObject parent = VisualTreeHelper.GetParent(item);
        parent = VisualTreeHelper.GetParent(parent);

        DependencyObject child = VisualTreeHelper.GetChild(parent, 0);

        return child as CheckBox;
    }

    public List<CheckBox> GetChildrenCheckBoxes(CheckBox rootCheckBox)
    {
        DependencyObject parent = VisualTreeHelper.GetParent(rootCheckBox);
        DependencyObject rootObject = VisualTreeHelper.GetChild(parent, 1);
        int childrenCount = VisualTreeHelper.GetChildrenCount(rootObject);

        List<CheckBox> children = new(childrenCount);

        for (int i = 0; i < childrenCount; i++)
        {
            children.Add(VisualTreeHelper.GetChild(rootObject, i) as CheckBox);
        }

        return children;
    }

    public bool AreAllChildrenChecked(List<CheckBox> children)
    {
        bool areAllChecked = true;

        foreach (CheckBox checkBox in children)
        {
            if (checkBox.IsChecked != true)
            {
                areAllChecked = false;
                break;
            }
        }

        return areAllChecked;
    }

    public bool AreAllChildrenUnchecked(List<CheckBox> children)
    {
        bool areAllUnchecked = true;

        foreach (CheckBox checkBox in children)
        {
            if (checkBox.IsChecked != false)
            {
                areAllUnchecked = false;
                break;
            }
        }

        return areAllUnchecked;
    }
}
