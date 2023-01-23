using System.Collections.Generic;
using System.ComponentModel;
using EventTimelineReconstruction.Abstractors;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Commands;

public sealed class AbstractLowLevelArtefactsCommand : CommandBase
{
    private readonly AbstractedEventsViewModel _viewModel;
    private readonly IEventsStore _store;
    private readonly ILowLevelArtefactsAbstractor _abstractor;

    public AbstractLowLevelArtefactsCommand(AbstractedEventsViewModel abstractedEventsViewModel, IEventsStore store, ILowLevelArtefactsAbstractor lowLevelArtefactsAbstractor)
    {
        _viewModel = abstractedEventsViewModel;
        _store = store;
        _abstractor = lowLevelArtefactsAbstractor;

        _viewModel.PropertyChanged += this.OnViewModelPropertyChanged;
    }

    public override bool CanExecute(object parameter)
    {
        return !_viewModel.HasErrors;
    }

    public override void Execute(object parameter)
    {
        _viewModel.IsAbstracting = true;

        List<EventViewModel> events = _store.GetStoredEventViewModelsAsOneLevel();
        List<LowLevelArtefactViewModel> lowLevelArtefacts = _abstractor.FormLowLevelArtefacts(events, double.Parse(_viewModel.Threshold));
        _viewModel.LoadLowLevelArtefacts(lowLevelArtefacts);

        _viewModel.IsAbstracting = false;
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AbstractedEventsViewModel.Threshold))
        {
            this.OnCanExecuteChanged();
        }
    }
}
