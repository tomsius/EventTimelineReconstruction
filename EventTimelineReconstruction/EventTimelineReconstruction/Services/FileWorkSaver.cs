using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Services;

public sealed class FileWorkSaver : IWorkSaver
{
    public async Task SaveWork(string path, IEnumerable<EventViewModel> events, IEnumerable<ISerializableLevel> highLevelEvents, IEnumerable<ISerializableLevel> lowLevelEvents, IEnumerable<ISerializableLevel> highLevelArtefacts, IEnumerable<ISerializableLevel> lowLevelArtefacts)
    {
        using StreamWriter outputStream = new(path);

        await WriteTreeToFile(events, outputStream, 0);
        await outputStream.WriteLineAsync();
        await WriteHighLevelEventsToFile(highLevelEvents, outputStream);
        await outputStream.WriteLineAsync();
        await WriteLowLevelEventsToFile(lowLevelEvents, outputStream);
        await outputStream.WriteLineAsync();
        await WriteHighLevelArtefactsToFile(highLevelArtefacts, outputStream);
        await outputStream.WriteLineAsync();
        await WriteLowLevelArtefactsToFile(lowLevelArtefacts, outputStream);
    }

    private static async Task WriteTreeToFile(IEnumerable<EventViewModel> events, StreamWriter outputStream, int currentLevel)
    {
        foreach (EventViewModel eventViewModel in events) {
            string serializedEventViewModel = eventViewModel.Serialize();
            string dataToWrite = string.Format("{0}{1}", new string('\t', currentLevel), serializedEventViewModel);
            await outputStream.WriteLineAsync(dataToWrite);

            await WriteTreeToFile(eventViewModel.Children, outputStream, currentLevel + 1);
        }
    }

    private static async Task WriteHighLevelEventsToFile(IEnumerable<ISerializableLevel> highLevelEvents, StreamWriter outputStream)
    {
        foreach (HighLevelEventViewModel highLevelEvent in highLevelEvents)
        {
            string serializedLine = highLevelEvent.Serialize();
            await outputStream.WriteLineAsync(serializedLine);
        }
    }

    private static async Task WriteLowLevelEventsToFile(IEnumerable<ISerializableLevel> lowLevelEvents, StreamWriter outputStream)
    {
        foreach (LowLevelEventViewModel lowLevelEvent in lowLevelEvents)
        {
            string serializedLine = lowLevelEvent.Serialize();
            await outputStream.WriteLineAsync(serializedLine);
        }
    }

    private static async Task WriteHighLevelArtefactsToFile(IEnumerable<ISerializableLevel> highLevelArtefacts, StreamWriter outputStream)
    {
        foreach (HighLevelArtefactViewModel highLevelArtefact in highLevelArtefacts)
        {
            string serializedLine = highLevelArtefact.Serialize();
            await outputStream.WriteLineAsync(serializedLine);
        }
    }

    private static async Task WriteLowLevelArtefactsToFile(IEnumerable<ISerializableLevel> lowLevelArtefacts, StreamWriter outputStream)
    {
        foreach (LowLevelArtefactViewModel lowLevelArtefact in lowLevelArtefacts)
        {
            string serializedLine = lowLevelArtefact.Serialize();
            await outputStream.WriteLineAsync(serializedLine);
        }
    }
}
