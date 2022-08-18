using System.Windows.Controls;
using System.Windows;
using EventTimelineReconstruction.ViewModels;
using EventTimelineReconstruction.Utils;

namespace EventTimelineReconstruction.Commands;

public class DropEventCommand : CommandBase
{
    private readonly EventTreeViewModel _eventTreeViewModel;

    public DropEventCommand(EventTreeViewModel eventTreeViewModel)
    {
        _eventTreeViewModel = eventTreeViewModel;
    }

    public override void Execute(object parameter)
    {
        DragEventArgs e = parameter as DragEventArgs;
        e.Effects = DragDropEffects.None;
        e.Handled = true;

        // Verify that this is a valid drop and then store the drop target
        TreeViewItem targetItem = DragDropUtils.GetNearestContainer(e.OriginalSource as UIElement);

        if (_eventTreeViewModel.DraggedItem != null) {
            _eventTreeViewModel.Target = targetItem?.Header as EventViewModel;
            e.Effects = DragDropEffects.Move;
        }
    }
}
