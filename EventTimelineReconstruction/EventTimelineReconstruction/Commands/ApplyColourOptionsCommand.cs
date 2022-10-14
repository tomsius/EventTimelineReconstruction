using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Commands;

public class ApplyColourOptionsCommand : CommandBase
{
    private readonly ColourViewModel _colourViewModel;
    private readonly IColouringStore _colouringStore;

    public ApplyColourOptionsCommand(ColourViewModel colourViewModel, IColouringStore colouringStore)
    {
        _colourViewModel = colourViewModel;
        _colouringStore = colouringStore;
    }

    // TODO - make sure no transparent colour in choices
    public override void Execute(object parameter)
    {
        _colouringStore.SetColoursByType(_colourViewModel.ColoursByType);
    }
}
