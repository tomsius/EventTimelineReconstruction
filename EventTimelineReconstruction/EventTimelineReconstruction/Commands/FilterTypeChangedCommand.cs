using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Commands;

public sealed class FilterTypeChangedCommand : CommandBase
{
    private readonly FilterViewModel _filterViewModel;

    public FilterTypeChangedCommand(FilterViewModel filterViewModel)
    {
        _filterViewModel = filterViewModel;
    }

    public override void Execute(object parameter)
    {
        _filterViewModel.AreAllFiltersApplied = !_filterViewModel.AreAllFiltersApplied;
    }
}
