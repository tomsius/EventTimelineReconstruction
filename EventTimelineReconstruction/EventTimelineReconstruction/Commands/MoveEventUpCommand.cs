using System.Collections.Generic;
using EventTimelineReconstruction.ViewModels;
using System.Linq;
using System.ComponentModel;

namespace EventTimelineReconstruction.Commands;

public class MoveEventUpCommand : CommandBase
{
    private readonly EventTreeViewModel _treeViewModel;
    private readonly EventDetailsViewModel _eventDetailsViewModel;

    public MoveEventUpCommand(EventTreeViewModel treeViewModel, EventDetailsViewModel eventDetailsViewModel)
    {
        _eventDetailsViewModel = eventDetailsViewModel;
        _treeViewModel = treeViewModel;
        _treeViewModel.PropertyChanged += this.OnViewModelPropertyChanged;
        _eventDetailsViewModel.PropertyChanged += this.OnViewModelPropertyChanged;
    }

    public override bool CanExecute(object parameter)
    {
        return _treeViewModel.Events.Any() && _eventDetailsViewModel.SelectedEvent != null && base.CanExecute(parameter);
    }

    public override void Execute(object parameter)
    {
        Stack<EventViewModel> stack = new();
        EventViewModel grandParent;

        foreach (EventViewModel item in _treeViewModel.Events)
        {
            if (item.ContainsChild(_eventDetailsViewModel.SelectedEvent)) {
                _treeViewModel.AddEvent(_eventDetailsViewModel.SelectedEvent);
                item.RemoveChild(_eventDetailsViewModel.SelectedEvent);

                _eventDetailsViewModel.SelectedEvent = null;
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

                    _eventDetailsViewModel.SelectedEvent = null;
                    return;
                }

                stack.Push(parent);
            }
        }
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(EventTreeViewModel.Events) || e.PropertyName == nameof(EventDetailsViewModel.SelectedEvent)) {
            this.OnCanExecuteChanged();
        }
    }
}
