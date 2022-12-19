﻿using System;

namespace EventTimelineReconstruction.ViewModels;

public class HighLevelArtefactViewModel
{
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public string Source { get; set; }
    public string Short { get; set; }
    public string Visit { get; set; }
    public string Extra { get; set; }
    public string Reference { get; set; }
    public string Macb { get; set; }
    public string SourceType { get; set; }
    public string Description { get; set; }

    public HighLevelArtefactViewModel()
    {
        Visit = "-";
        Extra = "-";
        Description = "-";
    }
}
