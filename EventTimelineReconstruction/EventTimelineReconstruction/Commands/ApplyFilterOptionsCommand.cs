﻿using System.ComponentModel;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Commands;

public class ApplyFilterOptionsCommand : CommandBase
{
    private FilterViewModel _filterViewModel;
    private readonly FilteringStore _filteringStore;
    private readonly EventTreeViewModel _eventTreeViewModel;

    public ApplyFilterOptionsCommand(FilterViewModel filterViewModel, FilteringStore filteringStore, EventTreeViewModel eventTreeViewModel)
    {
        _filterViewModel = filterViewModel;
        _filteringStore = filteringStore;
        _eventTreeViewModel = eventTreeViewModel;
        _filterViewModel.PropertyChanged += this.OnViewModelPropertyChanged;
    }

    public override bool CanExecute(object parameter)
    {
        return !_filterViewModel.HasErrors && base.CanExecute(parameter);
    }

    public override void Execute(object parameter)
    {
        _filteringStore.AreAllFiltersApplied = _filterViewModel.AreAllFiltersApplied;
        _filteringStore.Keyword = _filterViewModel.Keyword;
        _filteringStore.FromDate = _filterViewModel.FullFromDate;
        _filteringStore.ToDate = _filterViewModel.FullToDate;
        _filteringStore.SetEventTypes(_filterViewModel.ChosenEventTypes);

        _eventTreeViewModel.ApplyFilters();
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(FilterViewModel.HasErrors))
        {
            this.OnCanExecuteChanged();
        }
    }
}
