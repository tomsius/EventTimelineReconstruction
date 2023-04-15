using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.FactoryPattern;

public interface IAbstractionLevelFactory
{
    ISerializableLevel CreateAbstractionLevel(string row, AbstractionLevel level);
}