using System.Collections.Generic;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace EventTimelineReconstruction.Utils;

public interface IColouringUtils
{
    public Dictionary<TextBlock, ColorPicker> GetChildrenElements(StackPanel parent);
    public TextBlock GetSiblingTextBlock(ColorPicker colorPicker);
}