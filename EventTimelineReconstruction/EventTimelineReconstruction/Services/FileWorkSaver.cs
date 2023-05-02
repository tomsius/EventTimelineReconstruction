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
}
