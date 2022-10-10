using System.Windows;
using System.Windows.Controls;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Utils;

public interface IDragDropUtils
{
    public bool CheckDropTarget(EventViewModel sourceItem, EventViewModel targetItem);
    public void CopyItem(EventViewModel sourceItem, EventViewModel targetItem, TreeViewItem draggedItemElement, EventTreeViewModel vm);
    public TreeViewItem GetNearestContainer(UIElement element);
}