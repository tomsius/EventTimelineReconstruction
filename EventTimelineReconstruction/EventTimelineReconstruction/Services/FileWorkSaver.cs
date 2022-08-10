using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Services;
public class FileWorkSaver : IWorkSaver
{
    public void SaveWork(string path, IEnumerable<EventViewModel> events)
    {
        using StreamWriter outputStream = new(path);

        this.WriteTreeToFile(events, outputStream, 0);
    }

    private void WriteTreeToFile(IEnumerable<EventViewModel> events, StreamWriter outputStream, int currentLevel)
    {
        foreach (EventViewModel eventViewModel in events) {
            string serializedEventViewModel = eventViewModel.Serialize();
            string dataToWrite = string.Format("{0}{1}", new string('\t', currentLevel), serializedEventViewModel);
            outputStream.WriteLine(dataToWrite);

            WriteTreeToFile(eventViewModel.Children, outputStream, currentLevel + 1);
        }
    }
}
