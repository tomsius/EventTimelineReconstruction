using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Commands;

public sealed class ApplyColoursCommand : CommandBase
{
    private readonly IColouringStore _colouringStore;
    private readonly EventTreeViewModel _eventTreeViewModel;

    public ApplyColoursCommand(IColouringStore colouringStore, EventTreeViewModel eventTreeViewModel)
    {
        _colouringStore = colouringStore;
        _eventTreeViewModel = eventTreeViewModel;

        _eventTreeViewModel.PropertyChanged += this.OnViewModelPropertyChanged;
    }

    public override bool CanExecute(object parameter)
    {
        return _eventTreeViewModel.Events.Any() && base.CanExecute(parameter);
    }

    public override void Execute(object parameter)
    {
        Queue<EventViewModel> queue = new();

        foreach (EventViewModel eventViewModel in _eventTreeViewModel.Events)
        {
            queue.Enqueue(eventViewModel);
        }

        while (queue.Count > 0)
        {
            EventViewModel current = queue.Dequeue();

            foreach (EventViewModel child in current.Children)
            {
                queue.Enqueue(child);
            }

            string[] eventTypes = current.Type.Split("; ");

            foreach (var eventType in eventTypes)
            {
                if (_colouringStore.ColoursByType.ContainsKey(eventType))
                {
                    Brush colour = _colouringStore.ColoursByType[eventType];
                    current.Colour = colour;

                    break;
                }
            }
        }
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(EventTreeViewModel.Events))
        {
            this.OnCanExecuteChanged();
        }
    }
}
