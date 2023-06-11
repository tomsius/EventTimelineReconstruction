using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        foreach (EventViewModel eventViewModel in CollectionsMarshal.AsSpan(events))
        {
            string serializedEventViewModel = eventViewModel.Serialize();
            string dataToWrite = string.Format("{0}{1}", new string('\t', currentLevel), serializedEventViewModel);
            outputStream.WriteLine(dataToWrite);

            WriteTreeToFile(eventViewModel.Children.ToList(), outputStream, currentLevel + 1);
        }
    }

    private void WriteAbstractionLevelToFile(List<ISerializableLevel> abstractionLevel, StreamWriter outputStream)
    {
        foreach (ISerializableLevel eventInLevel in CollectionsMarshal.AsSpan(abstractionLevel))
        {
            string serializedLine = eventInLevel.Serialize();
            outputStream.WriteLine(serializedLine);
        }
    }
}
