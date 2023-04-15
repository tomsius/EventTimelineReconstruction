namespace EventTimelineReconstruction.Benchmarks.Models;

public sealed class LoadedWorkFactory
{
    public List<EventViewModel> Events { get; set; }
    public List<ISerializableLevel> HighLevelEvents { get; set; }
    public List<ISerializableLevel> LowLevelEvents { get; set; }
    public List<ISerializableLevel> HighLevelArtefacts { get; set; }
    public List<ISerializableLevel> LowLevelArtefacts { get; set; }
}
