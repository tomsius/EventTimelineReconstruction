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
    private readonly FormattedText _txt;
    private Rect _renderRect;
    private readonly SolidColorBrush _rectBrush;

    public DraggableAdorner(UIElement adornedElement, UIElement rootVisual) : base(adornedElement)
    {
        IsHitTestVisible = false;
        Point relativePoint = adornedElement.TransformToAncestor(rootVisual)
                      .Transform(new Point(0, 0));
        CenterOffset = new Point(-relativePoint.X + 20, -relativePoint.Y);

        EventViewModel eventModel = (adornedElement as TreeViewItem).Header as EventViewModel;
        _txt = new(eventModel.DisplayName, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Segoe UI"), 12, eventModel.Colour, VisualTreeHelper.GetDpi(this).PixelsPerDip);
        _renderRect = new Rect(new Size(_txt.Width, _txt.Height));
        _rectBrush = new SolidColorBrush(Colors.White);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        drawingContext.DrawRectangle(_rectBrush, null, _renderRect);
        drawingContext.DrawText(_txt, new Point());
    }
}
