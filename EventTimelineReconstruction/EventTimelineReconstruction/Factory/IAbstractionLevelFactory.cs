using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Factory;
public interface IAbstractionLevelFactory
{
    ISerializableLevel CreateAbstractionLevel(string row, AbstractionLevel level);
}