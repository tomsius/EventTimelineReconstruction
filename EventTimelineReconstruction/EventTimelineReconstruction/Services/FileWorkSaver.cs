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
        await WriteAbstractionLevelToFile(highLevelEvents, outputStream);
        await outputStream.WriteLineAsync();
        await WriteAbstractionLevelToFile(lowLevelEvents, outputStream);
        await outputStream.WriteLineAsync();
        await WriteAbstractionLevelToFile(highLevelArtefacts, outputStream);
        await outputStream.WriteLineAsync();
        await WriteAbstractionLevelToFile(lowLevelArtefacts, outputStream);
    }

    private async Task WriteTreeToFile(IEnumerable<EventViewModel> events, StreamWriter outputStream, int currentLevel)
    {
        IEnumerator<EventViewModel> enumerator = events.GetEnumerator();

        while (enumerator.MoveNext())
        {
            string serializedEventViewModel = enumerator.Current.Serialize();
            string dataToWrite = string.Format("{0}{1}", new string('\t', currentLevel), serializedEventViewModel);
            await outputStream.WriteLineAsync(dataToWrite);

            await this.WriteTreeToFile(enumerator.Current.Children, outputStream, currentLevel + 1);
        }
    }

    private async Task WriteAbstractionLevelToFile(IEnumerable<ISerializableLevel> abstractionLevel, StreamWriter outputStream)
    {
        IEnumerator<ISerializableLevel> enumerator = abstractionLevel.GetEnumerator();

        while (enumerator.MoveNext())
        {
            string serializedLine = enumerator.Current.Serialize();
            await outputStream.WriteLineAsync(serializedLine);
        }
    }
}
