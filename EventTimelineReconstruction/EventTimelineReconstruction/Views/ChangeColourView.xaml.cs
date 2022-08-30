using System.ComponentModel;
using System.Windows;

namespace EventTimelineReconstruction.Views;

public partial class ChangeColourView : Window
{
    public ChangeColourView()
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
