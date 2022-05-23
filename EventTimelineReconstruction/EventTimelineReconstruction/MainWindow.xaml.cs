using System.Windows;
using EventTimelineReconstruction.Views;

namespace EventTimelineReconstruction
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ImportView _view;

        public MainWindow(ImportView view)
        {
            this.InitializeComponent();
            _view = view;
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            _view.Show();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Save");
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Load");
        }
    }
}
