using System.Collections.Generic;
using System.Threading.Tasks;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Services;

public interface IWorkLoader
{
    public Task<List<EventViewModel>> LoadWork(string path);
}
