using System;
using System.Collections.Generic;

namespace EventTimelineReconstruction.Stores;

public class FilteringStore : IFilteringStore
{
    public bool IsEnabled
    {
        get; set;
    }

    public bool AreAllFiltersApplied
    {
        get; set;
    }

    public Dictionary<string, bool> ChosenEventTypes
    {
        get;
    }

    public string Keyword
    {
        get; set;
    }

    public DateTime FromDate
    {
        get; set;
    }

    public DateTime ToDate
    {
        get; set;
    }

    public FilteringStore()
    {
        IsEnabled = false;
        Keyword = string.Empty;
        ChosenEventTypes = new();
    }

    public void SetEventTypes(Dictionary<string, bool> filterTypes)
    {
        foreach (KeyValuePair<string, bool> pair in filterTypes)
        {
            string key = pair.Key;
            bool value = pair.Value;

            if (ChosenEventTypes.ContainsKey(key))
            {
                ChosenEventTypes[key] = value;
            }
            else
            {
                ChosenEventTypes.Add(key, value);
            }
        }
    }
}
