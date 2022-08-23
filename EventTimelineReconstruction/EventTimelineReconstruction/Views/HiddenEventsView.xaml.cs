using System.Windows;

namespace EventTimelineReconstruction.Views;

public partial class HiddenEventsView : Window
{
    public HiddenEventsView()
    {
        this.InitializeComponent();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = true;
        Visibility = Visibility.Hidden;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Visibility = Visibility.Hidden;
    }
}
