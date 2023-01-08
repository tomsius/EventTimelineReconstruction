using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Commands;

public sealed class ApplyColourOptionsCommand : CommandBase
{
    private readonly ColourViewModel _colourViewModel;
    private readonly IColouringStore _colouringStore;

    public ApplyColourOptionsCommand(ColourViewModel colourViewModel, IColouringStore colouringStore)
    {
        _colourViewModel = colourViewModel;
        _colouringStore = colouringStore;

        colourViewModel.PropertyChanged += this.OnViewModelPropertyChanged;
    }

    public override bool CanExecute(object parameter)
    {
        SolidColorBrush transparent = Brushes.Transparent;
        bool doesContaintTransparent = false;

        foreach (KeyValuePair<string, Brush> kvp in _colourViewModel.ColoursByType)
        {
            SolidColorBrush value = (SolidColorBrush)kvp.Value;

            if (value.Color == transparent.Color && value.Opacity.CompareTo(transparent.Opacity) == 0)
            {
                doesContaintTransparent = true;
                break;
            }
        }

        return !doesContaintTransparent && base.CanExecute(parameter);
    }

    public override void Execute(object parameter)
    {
        _colouringStore.SetColoursByType(_colourViewModel.ColoursByType);
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ColourViewModel.ColoursByType))
        {
            this.OnCanExecuteChanged();
        }
    }
}
