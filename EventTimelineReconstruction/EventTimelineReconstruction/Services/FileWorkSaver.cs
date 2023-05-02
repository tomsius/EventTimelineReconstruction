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
        ref EventViewModel start = ref MemoryMarshal.GetArrayDataReference(events);
        ref EventViewModel end = ref Unsafe.Add(ref start, events.Length);

        while (Unsafe.IsAddressLessThan(ref start, ref end))
        {
            string serializedEventViewModel = start.Serialize();
            string dataToWrite = string.Format("{0}{1}", new string('\t', currentLevel), serializedEventViewModel);
            outputStream.WriteLine(dataToWrite);

            this.WriteTreeToFile(start.Children.ToArray(), outputStream, currentLevel + 1);

            start = ref Unsafe.Add(ref start, 1);
        }
    }

    private void WriteAbstractionLevelToFile(ISerializableLevel[] abstractionLevel, StreamWriter outputStream)
    {
        ref ISerializableLevel start = ref MemoryMarshal.GetArrayDataReference(abstractionLevel);
        ref ISerializableLevel end = ref Unsafe.Add(ref start, abstractionLevel.Length);

        while (Unsafe.IsAddressLessThan(ref start, ref end))
        {
            string serializedLine = start.Serialize();
            outputStream.WriteLine(serializedLine);

            start = ref Unsafe.Add(ref start, 1);
        }
    }
}
