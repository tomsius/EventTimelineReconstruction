using System.Linq;
using System.Collections.Generic;
using EventTimelineReconstruction.ViewModels;
using EventTimelineReconstruction.Extensions;

namespace EventTimelineReconstruction.Commands;
public class MoveEventUpCommand : CommandBase
{
    private readonly EventTreeViewModel _treeViewModel;
    private readonly EventDetailsViewModel _eventDetailsViewModel;

    public MoveEventUpCommand(EventTreeViewModel treeViewModel, EventDetailsViewModel eventDetailsViewModel)
    {
        _eventDetailsViewModel = eventDetailsViewModel;
        _treeViewModel = treeViewModel;
    }

    public override bool CanExecute(object parameter)
    {
        bool isRootLevelItem = false;
        foreach (EventViewModel item in _treeViewModel.Events) {
            if (item == _eventDetailsViewModel.SelectedEvent) {
                isRootLevelItem = true;
                break;
            }
        }

        return isRootLevelItem == false && base.CanExecute(parameter);
    }

    public override void Execute(object parameter)
    {
        Stack<EventViewModel> stack = new();
        EventViewModel grandParent = null;
        //EventViewModel parent = null;

        foreach (EventViewModel item in _treeViewModel.Events)
        {
            if (item.ContainsChild(_eventDetailsViewModel.SelectedEvent)) {
                _treeViewModel.AddEvent(_eventDetailsViewModel.SelectedEvent);
                item.RemoveChild(_eventDetailsViewModel.SelectedEvent);

                _treeViewModel.UpdateOrdering();
                return;
            }

            stack.Push(item);
        }

        while (stack.Count > 0) {
            grandParent = stack.Pop();

            foreach (EventViewModel parent in grandParent.Children)
            {
                if (parent.ContainsChild(_eventDetailsViewModel.SelectedEvent))
                {
                    grandParent.AddChild(_eventDetailsViewModel.SelectedEvent);
                    parent.RemoveChild(_eventDetailsViewModel.SelectedEvent);

                    grandParent.Children.Sort();

                    return;
                }

                stack.Push(parent);
            }
        }
    }
}
