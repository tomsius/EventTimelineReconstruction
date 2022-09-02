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

        FileOKTextBlock.Visibility = Visibility.Collapsed;
        FileUnknownTextBlock.Visibility = Visibility.Collapsed;
        FileCompromisedTextBlock.Visibility = Visibility.Collapsed;
        EventsOKTextBlock.Visibility = Visibility.Collapsed;
        EventsCompromisedTextBlock.Visibility = Visibility.Collapsed;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Visibility = Visibility.Hidden;

        FileOKTextBlock.Visibility = Visibility.Collapsed;
        FileUnknownTextBlock.Visibility = Visibility.Collapsed;
        FileCompromisedTextBlock.Visibility = Visibility.Collapsed;
        EventsOKTextBlock.Visibility = Visibility.Collapsed;
        EventsCompromisedTextBlock.Visibility = Visibility.Collapsed;
    }
}
