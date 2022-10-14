using System.Collections.Generic;
using System.Windows.Media;

namespace EventTimelineReconstruction.Stores;

public interface IColouringStore
{
    Dictionary<string, Brush> ColoursByType
    {
        get;
    }

    void SetColoursByType(Dictionary<string, Brush> coloursByType);
}