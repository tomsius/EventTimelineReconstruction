using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace EventTimelineReconstruction.Utils;

public class RangeEnabledObservableCollection<T> : ObservableCollection<T>
{
    public void AddRange(IEnumerable<T> items)
    {
        this.CheckReentrancy();

        foreach (T item in items)
        {
            Items.Add(item);
        }

        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
    }
}
