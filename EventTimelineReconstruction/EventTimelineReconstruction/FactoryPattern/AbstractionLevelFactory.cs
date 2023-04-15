using System;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.FactoryPattern;

public class AbstractionLevelFactory : IAbstractionLevelFactory
{
    public ISerializableLevel CreateAbstractionLevel(string row, AbstractionLevel level)
    {
        return level switch
        {
            AbstractionLevel.HighLevelEvent => HighLevelEventViewModel.Deserialize(row),
            AbstractionLevel.LowLevelEvent => LowLevelEventViewModel.Deserialize(row),
            AbstractionLevel.HighLevelArtefact => HighLevelArtefactViewModel.Deserialize(row),
            AbstractionLevel.LowLevelArtefact => LowLevelArtefactViewModel.Deserialize(row),
            _ => throw new NotSupportedException(),
        };
    }
}
