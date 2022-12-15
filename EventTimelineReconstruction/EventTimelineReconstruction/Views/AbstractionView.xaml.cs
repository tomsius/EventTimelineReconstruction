using System.ComponentModel;
using System.Windows;

namespace EventTimelineReconstruction.Views;
/// <summary>
/// Interaction logic for AbstractionView.xaml
/// </summary>
public partial class AbstractionView : Window
{
    public AbstractionView()
    {
        InitializeComponent();
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        e.Cancel = true;
        Visibility = Visibility.Hidden;
    }
}
