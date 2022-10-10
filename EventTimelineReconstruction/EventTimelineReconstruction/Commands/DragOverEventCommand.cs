using EventTimelineReconstruction.ViewModels;
using System.Windows.Controls;
using System.Windows;
using EventTimelineReconstruction.Utils;

namespace EventTimelineReconstruction.Commands;

public class DragOverEventCommand : CommandBase
{
    private readonly EventTreeViewModel _eventTreeViewModel;
    private readonly IDragDropUtils _dragDropUtils;

    public DragOverEventCommand(EventTreeViewModel eventTreeViewModel, IDragDropUtils dragDropUtils)
    {
        _eventTreeViewModel = eventTreeViewModel;
        _dragDropUtils = dragDropUtils;
    }

    public override void Execute(object parameter)
    {
        DragEventArgs e = parameter as DragEventArgs;
        TreeViewItem item = _dragDropUtils.GetNearestContainer(e.OriginalSource as UIElement);
        EventViewModel targetViewModel = item?.Header as EventViewModel;

        if (_dragDropUtils.CheckDropTarget(_eventTreeViewModel.DraggedItem, targetViewModel)) {
            e.Effects = DragDropEffects.Move;
        }
        else {
            e.Effects = DragDropEffects.None;
        }

        e.Handled = true;
    }
}
