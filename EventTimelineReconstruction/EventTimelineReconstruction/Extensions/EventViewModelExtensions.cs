using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Extensions;
public static class EventViewModelExtensions
{
    public static void Sort(this ObservableCollection<EventViewModel> collection)
    {
        List<EventViewModel> sorted = collection.OrderBy(x => x).ToList();

        for (int i = 0; i < sorted.Count; i++)
        {
            collection.Move(collection.IndexOf(sorted[i]), i);
        }

        SortChildren(collection);
    }

    private static void SortChildren(ObservableCollection<EventViewModel> collection)
    {
        if (collection.Count == 0)
        {
            return;
        }

        Queue<EventViewModel> queue = new();
        queue.Enqueue(collection[0]);

        while (queue.Count > 0)
        {
            EventViewModel eventViewModel = queue.Dequeue();
            eventViewModel.Children.Sort();
            foreach (EventViewModel child in eventViewModel.Children)
            {
                queue.Enqueue(child);
            }
        }
    }
}
