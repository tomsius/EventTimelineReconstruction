﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Extensions;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;

namespace EventTimelineReconstruction.ViewModels;

public class EventTreeViewModel : ViewModelBase
{
    private readonly ObservableCollection<EventViewModel> _events;
    private readonly FilteringStore _filteringStore;

    public IEnumerable<EventViewModel> Events
    {
        get
        {
            return _events;
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

    public EventTreeViewModel(EventDetailsViewModel eventDetailsViewModel, FilteringStore filteringStore)
    {
        _events = new();
        pointRef = new PInPoint();
        _filteringStore = filteringStore;

        ShowDetailsCommand = new ShowEventDetailsCommand(eventDetailsViewModel);
        DragOverCommand = new DragOverEventCommand(this);
        DropCommand = new DropEventCommand(this);
        MouseMoveCommand = new MouseMoveEventCommand(this, eventDetailsViewModel);
        GiveFeedbackCommand = new GiveEventFeedbackCommand(this);
    }

    public void LoadEvents(IEnumerable<EventViewModel> events)
    {
        _events.Clear();

        foreach (EventViewModel entity in events) {
            _events.Add(entity);
        }

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

    public void UpdateOrdering()
    {
        _events.Sort();
    }

    public void ApplyFilters()
    {
        ICollectionView eventsDataSourceView = CollectionViewSource.GetDefaultView(Events);

        eventsDataSourceView.Filter = eventModel =>
        {
            ICollectionView childrenDataSourceView;
            Queue<EventViewModel> queue = new();

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
        bool isMatchingEventType = DoesMatchChosenEventType(model.Type);
        bool isMatchingKeyword = DoesMatchKeyword(model);
        bool isMatchingDateInterval = DoesMatchDateInterval(model.FullDate);

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
