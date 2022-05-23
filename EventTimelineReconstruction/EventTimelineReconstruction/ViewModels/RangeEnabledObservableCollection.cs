using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace EventTimelineReconstruction.ViewModels;
public class RangeEnabledObservableCollection<T> : ObservableCollection<T>
{
    public void InsertRange(IEnumerable<T> items)
    {
        this.CheckReentrancy();

        foreach (T item in items) 
        { 
            Items.Add(item);
        }

        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}
