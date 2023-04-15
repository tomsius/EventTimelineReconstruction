using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using EventTimelineReconstruction.Abstractors;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Commands;

public sealed class AbstractEventsCommand : AsyncCommandBase
{
    private readonly AbstractedEventsViewModel _viewModel;
    private readonly IEventsStore _store;
    private readonly IHighLevelEventsAbstractor _highLevelEventsAbstractor;
    private readonly ILowLevelEventsAbstractor _lowLevelEventsAbstractor;
    private readonly IHighLevelArtefactsAbstractor _highLevelArtefactsAbstractor;
    private readonly ILowLevelArtefactsAbstractor _lowLevelArtefactsAbstractor;

    public AbstractEventsCommand(AbstractedEventsViewModel viewModel, IEventsStore store, IHighLevelEventsAbstractor highLevelEventsAbstractor, ILowLevelEventsAbstractor lowLevelEventsAbstractor, IHighLevelArtefactsAbstractor highLevelArtefactsAbstractor, ILowLevelArtefactsAbstractor lowLevelArtefactsAbstractor)
    {
        _viewModel = viewModel;
        _store = store;
        _highLevelEventsAbstractor = highLevelEventsAbstractor;
        _lowLevelEventsAbstractor = lowLevelEventsAbstractor;
        _highLevelArtefactsAbstractor = highLevelArtefactsAbstractor;
        _lowLevelArtefactsAbstractor = lowLevelArtefactsAbstractor;
    }

    public override async Task ExecuteAsync(object parameter)
    {
        _viewModel.IsAbstracting = true;

        List<EventViewModel> events = _store.GetStoredEventViewModelsAsOneLevel();

        await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
        {
            List<ISerializableLevel> highLevelEvents = _highLevelEventsAbstractor.FormHighLevelEvents(events);
            _viewModel.LoadHighLevelEvents(highLevelEvents);
        });

        await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
        {
            List<ISerializableLevel> lowLevelEvents = _lowLevelEventsAbstractor.FormLowLevelEvents(events);
            _viewModel.LoadLowLevelEvents(lowLevelEvents);
        });

        await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
        {
            List<ISerializableLevel> highLevelArtefacts = _highLevelArtefactsAbstractor.FormHighLevelArtefacts(events);
            _viewModel.LoadHighLevelArtefacts(highLevelArtefacts);
        });

        await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
        {
            List<ISerializableLevel> lowLevelArtefacts = _lowLevelArtefactsAbstractor.FormLowLevelArtefacts(events);
            _viewModel.LoadLowLevelArtefacts(lowLevelArtefacts);
        });

        _viewModel.IsAbstracting = false;
    }
}
