using System.Windows.Controls;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Views;
/// <summary>
/// Interaction logic for EventDetailsView.xaml
/// </summary>
public partial class EventDetailsView : UserControl
{
    public EventDetailsView()
    {
        InitializeComponent();

        //DataContext = new EventDetailsViewModel();
    }
}
