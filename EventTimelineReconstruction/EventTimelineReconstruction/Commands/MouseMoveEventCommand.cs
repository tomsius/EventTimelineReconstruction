using System.Windows.Input;
using System.Windows;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;
using System.Windows.Controls;

namespace EventTimelineReconstruction.Commands;

public sealed class MouseMoveEventCommand : CommandBase
{
    private readonly EventTreeViewModel _eventTreeViewModel;
    private readonly EventDetailsViewModel _eventDetailsViewModel;
    private readonly IDragDropUtils _dragDropUtils;

    public MouseMoveEventCommand(EventTreeViewModel eventTreeViewModel, EventDetailsViewModel eventDetailsViewModel, IDragDropUtils dragDropUtils)
    {
        _eventTreeViewModel = eventTreeViewModel;
        _eventDetailsViewModel = eventDetailsViewModel;
        _dragDropUtils = dragDropUtils;
    }

    public override void Execute(object parameter)
    {
        MouseEventArgs e = parameter as MouseEventArgs;

        if (e.LeftButton == MouseButtonState.Pressed) {
            _eventTreeViewModel.DraggedItem = _eventDetailsViewModel.SelectedEvent;
            _eventTreeViewModel.DraggedItemElement = _dragDropUtils.GetNearestContainer(e.OriginalSource as UIElement);

            if (_eventTreeViewModel.DraggedItem != null && _eventTreeViewModel.DraggedItemElement != null) {
                _eventTreeViewModel.MyAdornment = new DraggableAdorner(_eventTreeViewModel.DraggedItemElement, e.Source as TreeView);
                AdornerLayerHelper.AdLayer.Add(_eventTreeViewModel.MyAdornment);

                DragDropEffects finalDropEffect = DragDrop.DoDragDrop(e.Source as TreeView, _eventDetailsViewModel.SelectedEvent, DragDropEffects.Move);

                AdornerLayerHelper.AdLayer.Remove(_eventTreeViewModel.MyAdornment);

                //Checking target is not null and item is dragging(moving)
                if (finalDropEffect == DragDropEffects.Move) {
                    // A Move drop was accepted
                    if (_eventTreeViewModel.DraggedItem != _eventTreeViewModel.Target) {
                        _dragDropUtils.CopyItem(_eventTreeViewModel.DraggedItem, _eventTreeViewModel.Target, _eventTreeViewModel.DraggedItemElement, _eventTreeViewModel);
                        _eventTreeViewModel.Target = null;
                        _eventTreeViewModel.DraggedItem = null;
                    }
                }
            }
        }
    }
}
