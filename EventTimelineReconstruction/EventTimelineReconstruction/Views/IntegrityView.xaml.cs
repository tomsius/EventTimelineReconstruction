using System.ComponentModel;
using System.Windows;

namespace EventTimelineReconstruction.Views;

public partial class IntegrityView : Window
{
    public IntegrityView()
    {
        this.InitializeComponent();
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        e.Cancel = true;
        Visibility = Visibility.Hidden;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Visibility = Visibility.Hidden;
    }
}
