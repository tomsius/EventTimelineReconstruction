using System;

namespace EventTimelineReconstruction.ViewModels;

public class LowLevelArtefactViewModel
{
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public string Timezone { get; set; }
    public string Macb { get; set; }
    public string Source { get; set; }
    public string SourceType { get; set; }
    public string Type { get; set; }
    public string User { get; set; }
    public string Host { get; set; }
    public string Short { get; set; }
    public string Description { get; set; }
    public string Version { get; set; }
    public string Filename { get; set; }
    public string Inode { get; set; }
    public string Notes { get; set; }
    public string Format { get; set; }
    public string Extra { get; set; }
    public string Reference { get; set; }
}
