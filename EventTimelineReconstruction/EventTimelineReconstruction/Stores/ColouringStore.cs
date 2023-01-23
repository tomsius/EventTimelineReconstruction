using System.Collections.Generic;
using System.Windows.Media;

namespace EventTimelineReconstruction.Stores;

public sealed class ColouringStore : IColouringStore
{
    public Dictionary<string, Brush> ColoursByType { get; }

    public ColouringStore()
    {
        ColoursByType = new();
    }

    public void SetColoursByType(Dictionary<string, Brush> coloursByType)
    {
        foreach (KeyValuePair<string, Brush> pair in coloursByType)
        {
            string key = pair.Key;
            Brush value = pair.Value;

            if (ColoursByType.ContainsKey(key))
            {
                ColoursByType[key] = value;
            }
            else
            {
                ColoursByType.Add(key, value);
            }
        }
    }
}
