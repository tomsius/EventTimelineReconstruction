using System.Collections.Generic;
using EventTimelineReconstruction.ChainOfResponsibility;
using EventTimelineReconstruction.ChainOfResponsibility.HighLevelEvents;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Abstractors;

public sealed class HighLevelEventsAbstractor : IHighLevelEventsAbstractor
{
    private readonly IHandler _handler;

    public HighLevelEventsAbstractor(
        IHighLogEventHandler logHandler, 
        IHighLnkEventHandler lnkHandler, 
        IHighMetaEventHandler metaHandler,
        IHighOlecfEventHandler olecfHandler,
        IHighPeEventHandler peHandler,
        IHighWebhistEventHandler webhistHandler)
    {
        _handler = logHandler;
        logHandler.Next = lnkHandler;
        lnkHandler.Next = metaHandler;
        metaHandler.Next = olecfHandler;
        olecfHandler.Next = peHandler;
        peHandler.Next = webhistHandler;
    }

    public List<ISerializableLevel> FormHighLevelEvents(List<EventViewModel> events)
    {
        List<ISerializableLevel> highLevelEvents = new(events.Count);

        for (int i = 0; i < events.Count; i++)
        {
            ISerializableLevel highLevelEvent = _handler.FormAbstractEvent(events, highLevelEvents, events[i]);

            if (highLevelEvent is not null)
            {
                highLevelEvents.Add(highLevelEvent);
            }
        }

        return highLevelEvents;
    }
}
