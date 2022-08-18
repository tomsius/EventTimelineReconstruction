using EventTimelineReconstruction.ViewModels;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;

namespace EventTimelineReconstruction.Utils;

public class DraggableAdorner : Adorner
{
    public Point CenterOffset;
    FormattedText txt;
    Rect renderRect;
    SolidColorBrush rectBrush;

    public DraggableAdorner(UIElement adornedElement, UIElement rootVisual) : base(adornedElement)
    {
        IsHitTestVisible = false;
        Point relativePoint = adornedElement.TransformToAncestor(rootVisual)
                      .Transform(new Point(0, 0));
        CenterOffset = new Point(-relativePoint.X + 20, -relativePoint.Y);

        EventViewModel eventModel = (adornedElement as TreeViewItem).Header as EventViewModel;
        txt = new(eventModel.DisplayName, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Segoe UI"), 12, eventModel.Colour, VisualTreeHelper.GetDpi(this).PixelsPerDip);
        renderRect = new Rect(new Size(txt.Width, txt.Height));
        rectBrush = new SolidColorBrush(Colors.White);
    }
    protected override void OnRender(DrawingContext drawingContext)
    {
        drawingContext.DrawRectangle(rectBrush, null, renderRect);
        drawingContext.DrawText(txt, new Point());
    }
}
