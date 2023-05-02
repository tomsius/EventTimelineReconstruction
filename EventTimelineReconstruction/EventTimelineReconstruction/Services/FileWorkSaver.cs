using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Services;

public sealed class FileWorkSaver : IWorkSaver
{
    public async Task SaveWork(string path, List<EventViewModel> events, List<ISerializableLevel> highLevelEvents, List<ISerializableLevel> lowLevelEvents, List<ISerializableLevel> highLevelArtefacts, List<ISerializableLevel> lowLevelArtefacts)
    {
        using StreamWriter outputStream = new(path);

        WriteTreeToFile(events, outputStream, 0);
        await outputStream.WriteLineAsync();
        WriteAbstractionLevelToFile(highLevelEvents, outputStream);
        await outputStream.WriteLineAsync();
        WriteAbstractionLevelToFile(lowLevelEvents, outputStream);
        await outputStream.WriteLineAsync();
        WriteAbstractionLevelToFile(highLevelArtefacts, outputStream);
        await outputStream.WriteLineAsync();
        WriteAbstractionLevelToFile(lowLevelArtefacts, outputStream);
    }

    private void WriteTreeToFile(List<EventViewModel> events, StreamWriter outputStream, int currentLevel)
    {
        Span<EventViewModel> span = CollectionsMarshal.AsSpan(events);

        for (int i = 0; i < span.Length; i++)
        {
            string serializedEventViewModel = span[i].Serialize();
            string dataToWrite = string.Format("{0}{1}", new string('\t', currentLevel), serializedEventViewModel);
            outputStream.WriteLine(dataToWrite);

            this.WriteTreeToFile(new List<EventViewModel>(span[i].Children), outputStream, currentLevel + 1);
        }
    }

    private void WriteAbstractionLevelToFile(List<ISerializableLevel> abstractionLevel, StreamWriter outputStream)
    {
        Span<ISerializableLevel> span = CollectionsMarshal.AsSpan(abstractionLevel);

        for (int i = 0; i < span.Length; i++)
        {
            string serializedLine = span[i].Serialize();
            outputStream.WriteLine(serializedLine);
        }
    }
}
