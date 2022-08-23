using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using EventTimelineReconstruction.Utils;

namespace EventTimelineReconstruction.Views;

public partial class EventTreeView : UserControl
{
    public EventTreeView()
    {
        this.InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        AdornerLayerHelper.AdLayer = AdornerLayer.GetAdornerLayer(this);
    }
}
