using System.Collections.Generic;
using System.Threading.Tasks;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Services;

public interface IWorkSaver
{
    public Task SaveWork(string path, IEnumerable<EventViewModel> events, IEnumerable<ISerializableLevel> highLevelEvents, IEnumerable<ISerializableLevel> lowLevelEvents, IEnumerable<ISerializableLevel> highLevelArtefacts, IEnumerable<ISerializableLevel> lowLevelArtefacts);
}