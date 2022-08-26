using System.Windows.Controls;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Commands;

public class FilterTypeChangedCommand : CommandBase
{
    private FilterViewModel _filterViewModel;

    public FilterTypeChangedCommand(FilterViewModel filterViewModel)
    {
        _filterViewModel = filterViewModel;
    }

    public override void Execute(object parameter)
    {
        _filterViewModel.AreAllFiltersApplied = !_filterViewModel.AreAllFiltersApplied;
    }
}
