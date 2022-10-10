using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;

namespace EventTimelineReconstruction.ViewModels;

public class ColourViewModel : ViewModelBase
{
    private readonly Dictionary<string, Brush> _coloursByType;

    public Dictionary<string, Brush> ColoursByType
    {
        get
        {
            return _coloursByType;
        }
    }

    public ICommand InitializeCommand { get; }
    public ICommand ApplyCommand { get; }
    public ICommand ColourChangedCommand { get; }
    public ICommand ColourCommand { get; }

    public ColourViewModel(ColouringStore colouringStore, EventTreeViewModel eventTreeViewModel, IColouringUtils colouringUtils)
    {
        _coloursByType = new();

        InitializeCommand = new InitializeColoursByTypeCommand(this, colouringUtils);
        ApplyCommand = new ApplyColourOptionsCommand(this, colouringStore);
        ColourChangedCommand = new SelectedColourChangedCommand(this, colouringUtils);
        ColourCommand = new ApplyColoursCommand(colouringStore, eventTreeViewModel);
    }

    public void UpdateColourByType(string key, Brush value)
    {
        if (_coloursByType.ContainsKey(key))
        {
            _coloursByType[key] = value;
        }
        else
        {
            _coloursByType.Add(key, value);
        }

        this.OnPropertyChanged(nameof(ColoursByType));
    }
}
