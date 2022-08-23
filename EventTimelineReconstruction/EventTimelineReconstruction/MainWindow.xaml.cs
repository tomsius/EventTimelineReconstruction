using System.Windows;
using EventTimelineReconstruction.Views;

namespace EventTimelineReconstruction
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ImportView _importView;
        private readonly SaveWorkView _saveWorkView;
        private readonly LoadWorkView _loadWorkView;
        private readonly HiddenEventsView _hiddenEventsView;

        public MainWindow(ImportView view, SaveWorkView saveWorkView, LoadWorkView loadWorkView, HiddenEventsView hiddenEventsView)
        {
            this.InitializeComponent();
            _importView = view;
            _saveWorkView = saveWorkView;
            _loadWorkView = loadWorkView;
            _hiddenEventsView = hiddenEventsView;
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            _importView.Show();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _saveWorkView.Show();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            _loadWorkView.Show();
        }

        private void HiddenEventsButton_Click(object sender, RoutedEventArgs e)
        {
            _hiddenEventsView.Show();
        }
    }
}
