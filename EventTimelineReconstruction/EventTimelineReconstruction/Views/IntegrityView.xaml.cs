using System.ComponentModel;
using System.Windows;
using EventTimelineReconstruction.ViewModels;

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

        IntegrityViewModel viewModel = (IntegrityViewModel)DataContext;
        viewModel.FileOKVisibility = Visibility.Collapsed;
        viewModel.FileUnknownVisibility = Visibility.Collapsed;
        viewModel.FileCompromisedVisibility = Visibility.Collapsed;
        viewModel.EventsOKVisibility = Visibility.Collapsed;
        viewModel.EventsCompromisedVisibility = Visibility.Collapsed;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Visibility = Visibility.Hidden;

        IntegrityViewModel viewModel = (IntegrityViewModel)DataContext;
        viewModel.FileOKVisibility = Visibility.Collapsed;
        viewModel.FileUnknownVisibility = Visibility.Collapsed;
        viewModel.FileCompromisedVisibility = Visibility.Collapsed;
        viewModel.EventsOKVisibility = Visibility.Collapsed;
        viewModel.EventsCompromisedVisibility = Visibility.Collapsed;
    }
}
