using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
        ref EventViewModel searchSpace = ref MemoryMarshal.GetArrayDataReference(events);

        for (int i = 0; i < events.Length; i++)
        {
            EventViewModel eventViewModel = Unsafe.Add(ref searchSpace, i);
            string serializedEventViewModel = eventViewModel.Serialize();
            string dataToWrite = string.Format("{0}{1}", new string('\t', currentLevel), serializedEventViewModel);
            outputStream.WriteLine(dataToWrite);

            this.WriteTreeToFile(eventViewModel.Children.ToArray(), outputStream, currentLevel + 1);
        }
    }

    private void WriteAbstractionLevelToFile(ISerializableLevel[] abstractionLevel, StreamWriter outputStream)
    {
        ref ISerializableLevel searchSpace = ref MemoryMarshal.GetArrayDataReference(abstractionLevel);

        for (int i = 0; i < abstractionLevel.Length; i++)
        {
            ISerializableLevel highLevelEvent = Unsafe.Add(ref searchSpace, i);
            string serializedLine = highLevelEvent.Serialize();
            outputStream.WriteLine(serializedLine);
        }
    }
}
