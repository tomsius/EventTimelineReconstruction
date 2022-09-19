using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Commands;

public class ApplyColoursCommand : CommandBase
{
    private readonly ColouringStore _colouringStore;
    private readonly EventTreeViewModel _eventTreeViewModel;

    public ApplyColoursCommand(ColouringStore colouringStore, EventTreeViewModel eventTreeViewModel)
    {
        _colouringStore = colouringStore;
        _eventTreeViewModel = eventTreeViewModel;

        _eventTreeViewModel.PropertyChanged += this.OnViewModelPropertyChanged;
    }

    public override bool CanExecute(object parameter)
    {
        return _eventTreeViewModel.Events.Count() > 0 && base.CanExecute(parameter);
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
            Brush colour = _colouringStore.ColoursByType[eventTypes[^1]];
            current.Colour = colour;
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
