using System.Threading.Tasks;
using EventTimelineReconstruction.Models;

namespace EventTimelineReconstruction.Services;

public interface IWorkLoader
{
    public Task<LoadedWork> LoadWork(string path);
}
