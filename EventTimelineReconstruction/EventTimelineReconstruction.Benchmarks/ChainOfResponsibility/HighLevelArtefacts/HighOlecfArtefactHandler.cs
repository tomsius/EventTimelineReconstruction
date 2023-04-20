using EventTimelineReconstruction.Benchmarks.Models;

namespace EventTimelineReconstruction.Benchmarks.ChainOfResponsibility.HighLevelArtefacts;

public class HighOlecfArtefactHandler : IHighOlecfArtefactHandler
{
    public IHandler Next { get; set; }

    public ISerializableLevel FormAbstractEvent(List<EventViewModel> events, List<ISerializableLevel> abstractionLevel, EventViewModel currentEvent)
    {
        if (currentEvent.Source == "OLECF")
        {
            string shortValue = currentEvent.Filename;
            string extraValue = currentEvent.Short;
            int length = currentEvent.Description.Length <= 80 ? currentEvent.Description.Length : 80;
            string descriptionValue = currentEvent.Description[..length];

            return new HighLevelArtefactViewModel(
                DateOnly.FromDateTime(currentEvent.FullDate),
                TimeOnly.FromDateTime(currentEvent.FullDate),
                currentEvent.Source,
                shortValue,
                "-",
                extraValue,
                currentEvent.SourceLine,
                currentEvent.MACB,
                currentEvent.SourceType,
                descriptionValue);
        }

        if (Next is not null)
        {
            return Next.FormAbstractEvent(events, abstractionLevel, currentEvent);
        }

        return null;
    }
}
