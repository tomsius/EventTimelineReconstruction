using System.ComponentModel;
using System.Windows;

namespace EventTimelineReconstruction.Views;
/// <summary>
/// Interaction logic for SaveWorkView.xaml
/// </summary>
public partial class SaveWorkView : Window
{
    public SaveWorkView()
    {
        InitializeComponent();
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
