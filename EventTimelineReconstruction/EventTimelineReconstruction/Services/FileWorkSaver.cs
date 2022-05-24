using System.Collections.Generic;
using System.IO;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Services;
public class FileWorkSaver : IWorkSaver
{
    public void SaveWork(string path, IEnumerable<EventViewModel> events)
    {
        using StreamWriter outputStream = new(path);

        foreach (EventViewModel eventViewModel in events) {
            string serializedEventViewModel = eventViewModel.Serialize();
            outputStream.WriteLine(serializedEventViewModel);
        }
    }
}
