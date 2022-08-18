using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using EventTimelineReconstruction.ViewModels;
using System.Collections.Generic;
using EventTimelineReconstruction.Extensions;
using System.Runtime.InteropServices;

namespace EventTimelineReconstruction.Utils;

public static class DragDropUtils
{
    public static TreeViewItem GetNearestContainer(UIElement element)
    {
        // Walk up the element tree to the nearest tree view item.
        TreeViewItem container = element as TreeViewItem;

        while (container == null && element != null) {
            element = VisualTreeHelper.GetParent(element) as UIElement;
            container = element as TreeViewItem;
        }

        return container;
    }

    public static bool CheckDropTarget(EventViewModel sourceItem, EventViewModel targetItem)
    {
        //Check whether the target item is meeting your condition
        Queue<EventViewModel> queue = new();
        queue.Enqueue(sourceItem);

        while (queue.Count > 0) {
            EventViewModel eventViewModel = queue.Dequeue();
            foreach (EventViewModel child in eventViewModel.Children) {
                if (child == targetItem) {
                    return false;
                }

                queue.Enqueue(child);
            }
        }

        return sourceItem != targetItem;
    }

    public static void CopyItem(EventViewModel sourceItem, EventViewModel targetItem, TreeViewItem draggedItemElement, EventTreeViewModel vm)
    {
        string targetName = targetItem != null ? targetItem.DisplayDate : "first level";

        //Asking user wether he want to drop the dragged TreeViewItem here or not
        if (MessageBox.Show("Would you like to move " + sourceItem.DisplayName + " into " + targetName + "?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
            //adding dragged TreeViewItem in target TreeViewItem
            MoveChild(sourceItem, targetItem, draggedItemElement, vm);
        }
    }

    private static void MoveChild(EventViewModel sourceItem, EventViewModel targetItem, TreeViewItem draggedItemElement, EventTreeViewModel vm)
    {
        var parentItem = FindParent(draggedItemElement);

        if (parentItem == null) {
            vm.RemoveEvent(sourceItem);
        }
        else {
            var parentViewModel = parentItem.Header as EventViewModel;
            parentViewModel.RemoveChild(sourceItem);
        }

        if (targetItem == null) {
            vm.AddEvent(sourceItem);
            vm.UpdateOrdering();
        }
        else {
            targetItem.AddChild(sourceItem);
            targetItem.Children.Sort();
        }
    }

    private static TreeViewItem FindParent(TreeViewItem draggedItemElement)
    {
        DependencyObject parent = VisualTreeHelper.GetParent(draggedItemElement);
        while (!(parent is TreeViewItem || parent is TreeView)) {
            parent = VisualTreeHelper.GetParent(parent);
        }

        return (parent as ItemsControl) as TreeViewItem;
    }

    [DllImport("user32.dll")]
    public static extern void GetCursorPos(ref PInPoint p);
}
