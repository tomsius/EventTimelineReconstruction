using System.Collections.Generic;
using System.Threading.Tasks;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Services;

public interface IWorkSaver
{
    public Task SaveWork(string path, List<EventViewModel> events, List<ISerializableLevel> highLevelEvents, List<ISerializableLevel> lowLevelEvents, List<ISerializableLevel> highLevelArtefacts, List<ISerializableLevel> lowLevelArtefacts);
}