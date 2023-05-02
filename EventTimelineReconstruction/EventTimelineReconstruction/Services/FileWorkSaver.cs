using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Services;

public sealed class FileWorkSaver : IWorkSaver
{
    public async Task SaveWork(string path, IEnumerable<EventViewModel> events, IEnumerable<ISerializableLevel> highLevelEvents, IEnumerable<ISerializableLevel> lowLevelEvents, IEnumerable<ISerializableLevel> highLevelArtefacts, IEnumerable<ISerializableLevel> lowLevelArtefacts)
    {
        using StreamWriter outputStream = new(path);

        WriteTreeToFile(events.ToArray(), outputStream, 0);
        await outputStream.WriteLineAsync();
        WriteAbstractionLevelToFile(highLevelEvents.ToArray(), outputStream);
        await outputStream.WriteLineAsync();
        WriteAbstractionLevelToFile(lowLevelEvents.ToArray(), outputStream);
        await outputStream.WriteLineAsync();
        WriteAbstractionLevelToFile(highLevelArtefacts.ToArray(), outputStream);
        await outputStream.WriteLineAsync();
        WriteAbstractionLevelToFile(lowLevelArtefacts.ToArray(), outputStream);
    }

    private void WriteTreeToFile(EventViewModel[] events, StreamWriter outputStream, int currentLevel)
    {
        Span<EventViewModel> span = events.AsSpan();

        for (int i = 0; i < span.Length; i++)
        {
            string serializedEventViewModel = span[i].Serialize();
            string dataToWrite = string.Format("{0}{1}", new string('\t', currentLevel), serializedEventViewModel);
            outputStream.WriteLine(dataToWrite);

            this.WriteTreeToFile(span[i].Children.ToArray(), outputStream, currentLevel + 1);
        }
    }

    private void WriteAbstractionLevelToFile(ISerializableLevel[] abstractionLevel, StreamWriter outputStream)
    {
        Span<ISerializableLevel> span = abstractionLevel.AsSpan();

        for (int i = 0; i < span.Length; i++)
        {
            string serializedLine = span[i].Serialize();
            outputStream.WriteLine(serializedLine);
        }
    }
}
