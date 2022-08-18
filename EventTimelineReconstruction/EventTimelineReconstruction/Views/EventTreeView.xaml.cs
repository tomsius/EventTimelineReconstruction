using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using EventTimelineReconstruction.Utils;

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
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AdornerLayerHelper.AdLayer = AdornerLayer.GetAdornerLayer(this);
        }
    }
}
