using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Services;

public sealed class FileWorkSaver : IWorkSaver
{
    public async Task SaveWork(string path, List<EventViewModel> events, List<ISerializableLevel> highLevelEvents, List<ISerializableLevel> lowLevelEvents, List<ISerializableLevel> highLevelArtefacts, List<ISerializableLevel> lowLevelArtefacts)
    {
        using StreamWriter outputStream = new(path);

        await WriteTreeToFile(events, outputStream, 0);
        await outputStream.WriteLineAsync();
        await WriteAbstractionLevelToFile(highLevelEvents, outputStream);
        await outputStream.WriteLineAsync();
        await WriteAbstractionLevelToFile(lowLevelEvents, outputStream);
        await outputStream.WriteLineAsync();
        await WriteAbstractionLevelToFile(highLevelArtefacts, outputStream);
        await outputStream.WriteLineAsync();
        await WriteAbstractionLevelToFile(lowLevelArtefacts, outputStream);
    }

    private async Task WriteTreeToFile(List<EventViewModel> events, StreamWriter outputStream, int currentLevel)
    {
        for (int i = 0; i < events.Count; i++)
        {
            string serializedEventViewModel = events[i].Serialize();
            string dataToWrite = string.Format("{0}{1}", new string('\t', currentLevel), serializedEventViewModel);
            await outputStream.WriteLineAsync(dataToWrite);

            await this.WriteTreeToFile(events[i].Children.ToList(), outputStream, currentLevel + 1);
        }
    }

    private async Task WriteAbstractionLevelToFile(List<ISerializableLevel> abstractionLevel, StreamWriter outputStream)
    {
        for (int i = 0; i < abstractionLevel.Count; i++)
        {
            string serializedLine = abstractionLevel[i].Serialize();
            await outputStream.WriteLineAsync(serializedLine);
        }
    }

    //private static async Task WriteTreeToFile(IEnumerable<EventViewModel> events, StreamWriter outputStream, int currentLevel)
    //{
    //    foreach (EventViewModel eventViewModel in events) {
    //        string serializedEventViewModel = eventViewModel.Serialize();
    //        string dataToWrite = string.Format("{0}{1}", new string('\t', currentLevel), serializedEventViewModel);
    //        await outputStream.WriteLineAsync(dataToWrite);

    //        await WriteTreeToFile(eventViewModel.Children, outputStream, currentLevel + 1);
    //    }
    //}

    //private static async Task WriteHighLevelEventsToFile(IEnumerable<HighLevelEventViewModel> highLevelEvents, StreamWriter outputStream)
    //{
    //    foreach (HighLevelEventViewModel highLevelEvent in highLevelEvents)
    //    {
    //        string serializedLine = highLevelEvent.Serialize();
    //        await outputStream.WriteLineAsync(serializedLine);
    //    }
    //}

    //private static async Task WriteLowLevelEventsToFile(IEnumerable<LowLevelEventViewModel> lowLevelEvents, StreamWriter outputStream)
    //{
    //    foreach (LowLevelEventViewModel lowLevelEvent in lowLevelEvents)
    //    {
    //        string serializedLine = lowLevelEvent.Serialize();
    //        await outputStream.WriteLineAsync(serializedLine);
    //    }
    //}

    //private static async Task WriteHighLevelArtefactsToFile(IEnumerable<HighLevelArtefactViewModel> highLevelArtefacts, StreamWriter outputStream)
    //{
    //    foreach (HighLevelArtefactViewModel highLevelArtefact in highLevelArtefacts)
    //    {
    //        string serializedLine = highLevelArtefact.Serialize();
    //        await outputStream.WriteLineAsync(serializedLine);
    //    }
    //}

    //private static async Task WriteLowLevelArtefactsToFile(IEnumerable<LowLevelArtefactViewModel> lowLevelArtefacts, StreamWriter outputStream)
    //{
    //    foreach (LowLevelArtefactViewModel lowLevelArtefact in lowLevelArtefacts)
    //    {
    //        string serializedLine = lowLevelArtefact.Serialize();
    //        await outputStream.WriteLineAsync(serializedLine);
    //    }
    //}
}
