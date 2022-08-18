using EventTimelineReconstruction.ViewModels;
using System.Windows.Controls;
using System.Windows;
using EventTimelineReconstruction.Utils;

namespace EventTimelineReconstruction.Commands;

public class DragOverEventCommand : CommandBase
{
    private readonly EventTreeViewModel _eventTreeViewModel;

    public DragOverEventCommand(EventTreeViewModel eventTreeViewModel)
    {
        _eventTreeViewModel = eventTreeViewModel;
    }

    public override void Execute(object parameter)
    {
        DragEventArgs e = parameter as DragEventArgs;
        TreeViewItem item = DragDropUtils.GetNearestContainer(e.OriginalSource as UIElement);
        EventViewModel targetViewModel = item?.Header as EventViewModel;

        if (DragDropUtils.CheckDropTarget(_eventTreeViewModel.DraggedItem, targetViewModel)) {
            e.Effects = DragDropEffects.Move;
        }
        else {
            e.Effects = DragDropEffects.None;
        }

        e.Handled = true;
    }
}
