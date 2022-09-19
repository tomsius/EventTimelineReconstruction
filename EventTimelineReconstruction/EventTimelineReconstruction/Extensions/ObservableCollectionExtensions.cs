﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Linq;

namespace EventTimelineReconstruction.Extensions;

public static class ObservableCollectionExtensions
{
    public static void Sort<T>(this ObservableCollection<T> collection) where T : IComparable
    {
        List<T> sorted = collection.OrderBy(x => x).ToList();

        for (int i = 0; i < sorted.Count; i++)
        {
            collection.Move(collection.IndexOf(sorted[i]), i);
        }
    }
}
