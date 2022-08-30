using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Commands;

public class ApplyColourOptionsCommand : CommandBase
{
    private readonly ColourViewModel _colourViewModel;
    private readonly ColouringStore _colouringStore;

    public ApplyColourOptionsCommand(ColourViewModel colourViewModel, ColouringStore colouringStore)
    {
        _colourViewModel = colourViewModel;
        _colouringStore = colouringStore;
    }

    public override void Execute(object parameter)
    {
        _colouringStore.SetColoursByType(_colourViewModel.ColoursByType);
    }
}
