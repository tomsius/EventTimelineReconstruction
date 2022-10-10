using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;

namespace EventTimelineReconstruction.ViewModels;

public class EventTreeViewModel : ViewModelBase
{
    private readonly RangeEnabledObservableCollection<EventViewModel> _events;
    private readonly FilteringStore _filteringStore;

    public IEnumerable<EventViewModel> Events
    {
        get
        {
            return _events;
        }
    }

    private readonly ICollectionView _eventsView;

    public ICollectionView EventsView
    {
        get
        {
            return _eventsView;
        }
    }


    public PInPoint pointRef;

    public EventViewModel DraggedItem{ get; set; }
    public EventViewModel Target { get; set; }
    public TreeViewItem DraggedItemElement { get; set; }
    public DraggableAdorner MyAdornment { get; set; }
    
    public ICommand ShowDetailsCommand { get; }
    public ICommand DragOverCommand { get; }
    public ICommand DropCommand { get; }
    public ICommand MouseMoveCommand { get; }
    public ICommand GiveFeedbackCommand { get; }

    public EventTreeViewModel(EventDetailsViewModel eventDetailsViewModel, FilteringStore filteringStore, ChangeColourViewModel changeColourViewModel, IDragDropUtils dragDropUtils)
    {
        _events = new();
        _eventsView = new ListCollectionView(_events);
        (_eventsView as ListCollectionView).CustomSort = new EventSorter();

        pointRef = new PInPoint();
        _filteringStore = filteringStore;

        ShowDetailsCommand = new ShowEventDetailsCommand(eventDetailsViewModel, changeColourViewModel);
        DragOverCommand = new DragOverEventCommand(this, dragDropUtils);
        DropCommand = new DropEventCommand(this, dragDropUtils);
        MouseMoveCommand = new MouseMoveEventCommand(this, eventDetailsViewModel, dragDropUtils);
        GiveFeedbackCommand = new GiveEventFeedbackCommand(this);
    }

    public void LoadEvents(List<EventViewModel> events)
    {
        _events.Clear();
        _events.AddRange(events);

        this.ApplyFilters();
        this.OnPropertyChanged(nameof(Events));
    }

    public void AddEvent(EventViewModel eventViewModel)
    {
        _events.Add(eventViewModel);
    }

    public void RemoveEvent(EventViewModel eventViewModel)
    {
        _events.Remove(eventViewModel);
    }

    public void ApplyFilters()
    {
        _eventsView.Filter = eventModel =>
        {
            ICollectionView childrenDataSourceView;
            Queue<EventViewModel> queue = new();

            if (eventModel is null)
            {
                return false;
            }

            foreach (EventViewModel child in ((EventViewModel)eventModel).Children)
            {
                queue.Enqueue(child);
            }

            while (queue.Count > 0)
            {
                EventViewModel eventViewModel = queue.Dequeue();
                foreach (EventViewModel child in eventViewModel.Children)
                {
                    queue.Enqueue(child);
                }

                childrenDataSourceView = CollectionViewSource.GetDefaultView(eventViewModel.Children);
                childrenDataSourceView.Filter = childModel =>
                {
                    return this.ShouldBeShown(childModel as EventViewModel);
                };
            }

            childrenDataSourceView = CollectionViewSource.GetDefaultView(((EventViewModel)eventModel).Children);
            childrenDataSourceView.Filter = childModel =>
            {
                return this.ShouldBeShown(childModel as EventViewModel);
            };

            return this.ShouldBeShown(eventModel as EventViewModel);
        };
    }

    private bool ShouldBeShown(EventViewModel model)
    {
        bool isMatchingFilters = true;

        if (_filteringStore.IsEnabled)
        {
            isMatchingFilters = this.DoesMatchFilters(model);
        }

        return model.IsVisible && isMatchingFilters;
    }

    private bool DoesMatchFilters(EventViewModel model)
    {
        bool isMatchingEventType = this.DoesMatchChosenEventType(model.Type);
        bool isMatchingKeyword = this.DoesMatchKeyword(model);
        bool isMatchingDateInterval = this.DoesMatchDateInterval(model.FullDate);

        if (_filteringStore.AreAllFiltersApplied)
        {
            return isMatchingEventType && isMatchingKeyword && isMatchingDateInterval;
        }
        else
        {
            return isMatchingEventType || isMatchingKeyword || isMatchingDateInterval;
        }
    }

    private bool DoesMatchChosenEventType(string eventType)
    {
        foreach (KeyValuePair<string, bool> pair in _filteringStore.ChosenEventTypes)
        {
            string filterType = pair.Key;
            bool isApplied = pair.Value;

            if (isApplied && eventType == filterType)
            {
                return true;
            }
        }

        return false;
    }

    private bool DoesMatchKeyword(EventViewModel model)
    {
        if (model.SourceType.ToLower().Contains(_filteringStore.Keyword))
        {
            return true;
        }

        if (model.User.ToLower().Contains(_filteringStore.Keyword))
        {
            return true;
        }

        if (model.Host.ToLower().Contains(_filteringStore.Keyword))
        {
            return true;
        }

        if (model.Description.ToLower().Contains(_filteringStore.Keyword))
        {
            return true;
        }

        if (model.Filename.ToLower().Contains(_filteringStore.Keyword))
        {
            return true;
        }

        if (model.Notes.ToLower().Contains(_filteringStore.Keyword))
        {
            return true;
        }

        foreach (KeyValuePair<string, string> pair in model.Extra)
        {
            string key = pair.Key;
            string value = pair.Value;

            if (key.ToLower().Contains(_filteringStore.Keyword) || value.ToLower().Contains(_filteringStore.Keyword))
            {
                return true;
            }
        }

        return false;
    }

    private bool DoesMatchDateInterval(DateTime eventDate)
    {
        DateTime startTime = _filteringStore.FromDate;
        DateTime endTime = _filteringStore.ToDate;

        return eventDate >= startTime && eventDate <= endTime;
    }
}
