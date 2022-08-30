using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace EventTimelineReconstruction.Utils;

public static class ColouringUtils
{
    public static Dictionary<TextBlock, ColorPicker> GetChildrenElements(StackPanel parent)
    {
        int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
        Dictionary<TextBlock, ColorPicker> children = new(childrenCount);

        for (int i = 0; i < childrenCount; i++)
        {
            DockPanel panel = VisualTreeHelper.GetChild(parent, i) as DockPanel;
            TextBlock textBlock = VisualTreeHelper.GetChild(panel, 0) as TextBlock;
            ColorPicker colorPicker = VisualTreeHelper.GetChild(panel, 1) as ColorPicker;

            children.Add(textBlock, colorPicker);
        }

        return children;
    }

    public static TextBlock GetSiblingTextBlock(ColorPicker colorPicker)
    {
        DockPanel parent = VisualTreeHelper.GetParent(colorPicker) as DockPanel;
        TextBlock sibling = VisualTreeHelper.GetChild(parent, 0) as TextBlock;

        return sibling;
    }
}
