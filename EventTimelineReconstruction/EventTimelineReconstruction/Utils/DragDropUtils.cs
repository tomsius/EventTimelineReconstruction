using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using EventTimelineReconstruction.ViewModels;
using System.Collections.Generic;
using System.Text;

namespace EventTimelineReconstruction.Utils;

public class DragDropUtils : IDragDropUtils
{
    public TreeViewItem GetNearestContainer(UIElement element)
    {
        // Walk up the element tree to the nearest tree view item.
        TreeViewItem container = element as TreeViewItem;

        while (container == null && element != null)
        {
            element = VisualTreeHelper.GetParent(element) as UIElement;
            container = element as TreeViewItem;
        }

        return container;
    }

    public bool CheckDropTarget(EventViewModel sourceItem, EventViewModel targetItem)
    {
        //Check whether the target item is meeting condition
        Queue<EventViewModel> queue = new();
        queue.Enqueue(sourceItem);

        while (queue.Count > 0)
        {
            EventViewModel eventViewModel = queue.Dequeue();
            foreach (EventViewModel child in eventViewModel.Children)
            {
                if (child == targetItem)
                {
                    return false;
                }

                queue.Enqueue(child);
            }
        }

        return sourceItem != targetItem;
    }

    public void CopyItem(EventViewModel sourceItem, EventViewModel targetItem, TreeViewItem draggedItemElement, EventTreeViewModel vm)
    {
        string question = (string)App.Current.Resources["DragDrop_Question"];
        string into = (string)App.Current.Resources["DragDrop_Into"];
        string targetName = targetItem != null ? targetItem.DisplayName : (string)App.Current.Resources["DragDrop_First_Level"];
        string confirmation = (string)App.Current.Resources["DragDrop_Confirmation"];

        StringBuilder message = new();
        message
            .Append(question)
            .Append(' ')
            .Append(sourceItem.DisplayName)
            .Append(' ')
            .Append(into)
            .Append(' ')
            .Append(targetName)
            .Append('?');

        //Asking user wether he want to drop the dragged TreeViewItem here or not
        if (MessageBox.Show(message.ToString(), confirmation, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            //adding dragged TreeViewItem in target TreeViewItem
            MoveChild(sourceItem, targetItem, draggedItemElement, vm);
        }
    }

    private static void MoveChild(EventViewModel sourceItem, EventViewModel targetItem, TreeViewItem draggedItemElement, EventTreeViewModel vm)
    {
        TreeViewItem parentItem = FindParent(draggedItemElement);

        if (parentItem == null)
        {
            vm.RemoveEvent(sourceItem);
        }
        else
        {
            EventViewModel parentViewModel = parentItem.Header as EventViewModel;
            parentViewModel.RemoveChild(sourceItem);
        }

        if (targetItem == null)
        {
            vm.AddEvent(sourceItem);
        }
        else
        {
            targetItem.AddChild(sourceItem);
        }
    }

    private static TreeViewItem FindParent(TreeViewItem draggedItemElement)
    {
        DependencyObject parent = VisualTreeHelper.GetParent(draggedItemElement);
        while (!(parent is TreeViewItem || parent is TreeView))
        {
            parent = VisualTreeHelper.GetParent(parent);
        }

        return (parent as ItemsControl) as TreeViewItem;
    }
}
