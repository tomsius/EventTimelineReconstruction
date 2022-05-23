using System.Windows.Controls;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Views
{
    /// <summary>
    /// Interaction logic for EventTreeView.xaml
    /// </summary>
    public partial class EventTreeView : UserControl
    {
        public EventTreeView()
        {
            InitializeComponent();

            //DataContext = new EventTreeViewModel();
        }
    }
}
