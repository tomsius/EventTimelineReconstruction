using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Commands;

public class ApplyFiltersCommand : CommandBase
{
    private FilterViewModel _filterViewModel;
    private readonly FilteringStore _filteringStore;

    public ApplyFiltersCommand(FilterViewModel filterViewModel, FilteringStore filteringStore)
    {
        _filterViewModel = filterViewModel;
        _filteringStore = filteringStore;
    }

    public override void Execute(object parameter)
    {
        _filteringStore.AreAllFiltersApplied = _filterViewModel.AreAllFiltersApplied;
        _filteringStore.Keyword = _filterViewModel.Keyword;
        _filteringStore.FromDate = _filterViewModel.FullFromDate;
        _filteringStore.ToDate = _filterViewModel.FullToDate;
        _filteringStore.SetEventTypes(_filterViewModel.ChosenEventTypes);
    }
}
