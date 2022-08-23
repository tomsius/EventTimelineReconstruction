using System.ComponentModel;
using System.Windows;

namespace EventTimelineReconstruction.Views;

public partial class ImportView : Window
{
    public ImportView()
    {
        this.InitializeComponent();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Visibility = Visibility.Hidden;
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        e.Cancel = true;
        Visibility = Visibility.Hidden;
    }
}
