using System;

namespace EventTimelineReconstruction.ViewModels;

public class HighLevelEventViewModel
{
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public string Source { get; set; }
    public string Short { get; set; }
    public string Visit { get; set; }
    public string Reference { get; set; }

    public HighLevelEventViewModel()
    {
        Visit = "-";
    }
}
