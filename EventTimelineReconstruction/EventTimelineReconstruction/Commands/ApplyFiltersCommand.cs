using System.Windows.Controls;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Commands;

public sealed class ApplyFiltersCommand : CommandBase
{
    private readonly IFilteringStore _filteringStore;
    private readonly EventTreeViewModel _eventTreeViewModel;

    public ApplyFiltersCommand(IFilteringStore filteringStore, EventTreeViewModel eventTreeViewModel)
    {
        _filteringStore = filteringStore;
        _eventTreeViewModel = eventTreeViewModel;
    }

    public override void Execute(object parameter)
    {
        object[] values = parameter as object[];
        Button enableButton = values[0] as Button;
        Button disableButton = values[1] as Button;

        enableButton.IsEnabled = !enableButton.IsEnabled;
        disableButton.IsEnabled = !disableButton.IsEnabled;

        _filteringStore.IsEnabled = disableButton.IsEnabled;
        _eventTreeViewModel.ApplyFilters();
    }
}
