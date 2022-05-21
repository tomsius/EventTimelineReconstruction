using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.ViewModels;
using EventTimelineReconstruction.Views;

namespace EventTimelineReconstruction
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO - dependency injection
            IEventsImporter importer = new L2tCSVEventsImporter();
            EventsStore store = new(importer);

            ImportView view = new ImportView() {
                DataContext = new ImportViewModel(store)
            };

            view.Show();
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
