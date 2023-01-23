using System;
using System.Collections.Generic;

namespace EventTimelineReconstruction.Stores;

public interface IFilteringStore
{
    bool AreAllFiltersApplied { get; set; }
    Dictionary<string, bool> ChosenEventTypes { get; }
    DateTime FromDate { get; set; }
    bool IsEnabled { get; set; }
    string Keyword { get; set; }
    DateTime ToDate { get; set; }

    void SetEventTypes(Dictionary<string, bool> filterTypes);
}