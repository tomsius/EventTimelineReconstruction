using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Commands;

public class GiveEventFeedbackCommand : CommandBase
{
    private readonly EventTreeViewModel _eventTreeViewModel;

    public GiveEventFeedbackCommand(EventTreeViewModel eventTreeViewModel)
    {
        _eventTreeViewModel = eventTreeViewModel;
    }

    public override void Execute(object parameter)
    {
        GiveFeedbackEventArgs e = parameter as GiveFeedbackEventArgs;

        GetCursorPos(ref _eventTreeViewModel.pointRef);

        Visual visual = e.Source as TreeView;
        Point relPos = visual.PointFromScreen(_eventTreeViewModel.pointRef.GetPoint(_eventTreeViewModel.MyAdornment.CenterOffset));
        _eventTreeViewModel.MyAdornment.Arrange(new Rect(relPos, _eventTreeViewModel.MyAdornment.DesiredSize));
    }

    [DllImport("user32.dll")]
    private static extern void GetCursorPos(ref PInPoint p);
}
