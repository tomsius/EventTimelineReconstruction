using System.Collections.Generic;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Services
{
    public interface IWorkSaver
    {
        public void SaveWork(string path, IEnumerable<EventViewModel> events);
    }
}