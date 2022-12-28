using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Services;

public class FileWorkSaver : IWorkSaver
{
    public async Task SaveWork(string path, IEnumerable<EventViewModel> events, IEnumerable<HighLevelEventViewModel> highLevelEvents, IEnumerable<LowLevelEventViewModel> lowLevelEvents, IEnumerable<HighLevelArtefactViewModel> highLevelArtefacts, IEnumerable<LowLevelArtefactViewModel> lowLevelArtefacts)
    {
        using StreamWriter outputStream = new(path);

        await this.WriteTreeToFile(events, outputStream, 0);
        await outputStream.WriteLineAsync();
        await this.WriteHighLevelEventsToFile(highLevelEvents, outputStream);
        await outputStream.WriteLineAsync();
        await this.WriteLowLevelEventsToFile(lowLevelEvents, outputStream);
        await outputStream.WriteLineAsync();
        await this.WriteHighLevelArtefactsToFile(highLevelArtefacts, outputStream);
        await outputStream.WriteLineAsync();
        await this.WriteLowLevelArtefactsToFile(lowLevelArtefacts, outputStream);
    }

    private async Task WriteTreeToFile(IEnumerable<EventViewModel> events, StreamWriter outputStream, int currentLevel)
    {
        foreach (EventViewModel eventViewModel in events) {
            string serializedEventViewModel = eventViewModel.Serialize();
            string dataToWrite = string.Format("{0}{1}", new string('\t', currentLevel), serializedEventViewModel);
            await outputStream.WriteLineAsync(dataToWrite);

            await this.WriteTreeToFile(eventViewModel.Children, outputStream, currentLevel + 1);
        }
    }

    private async Task WriteHighLevelEventsToFile(IEnumerable<HighLevelEventViewModel> highLevelEvents, StreamWriter outputStream)
    {
        foreach (HighLevelEventViewModel highLevelEvent in highLevelEvents)
        {
            string serializedLine = highLevelEvent.Serialize();
            await outputStream.WriteLineAsync(serializedLine);
        }
    }

    private async Task WriteLowLevelEventsToFile(IEnumerable<LowLevelEventViewModel> lowLevelEvents, StreamWriter outputStream)
    {
        foreach (LowLevelEventViewModel lowLevelEvent in lowLevelEvents)
        {
            string serializedLine = lowLevelEvent.Serialize();
            await outputStream.WriteLineAsync(serializedLine);
        }
    }

    private async Task WriteHighLevelArtefactsToFile(IEnumerable<HighLevelArtefactViewModel> highLevelArtefacts, StreamWriter outputStream)
    {
        foreach (HighLevelArtefactViewModel highLevelArtefact in highLevelArtefacts)
        {
            string serializedLine = highLevelArtefact.Serialize();
            await outputStream.WriteLineAsync(serializedLine);
        }
    }

    private async Task WriteLowLevelArtefactsToFile(IEnumerable<LowLevelArtefactViewModel> lowLevelArtefacts, StreamWriter outputStream)
    {
        foreach (LowLevelArtefactViewModel lowLevelArtefact in lowLevelArtefacts)
        {
            string serializedLine = lowLevelArtefact.Serialize();
            await outputStream.WriteLineAsync(serializedLine);
        }
    }
}
