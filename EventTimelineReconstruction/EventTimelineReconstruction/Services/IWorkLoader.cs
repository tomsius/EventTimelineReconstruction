using System.Collections.Generic;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Services;
public interface IWorkLoader
{
    public List<EventViewModel> LoadWork(string path);
}
