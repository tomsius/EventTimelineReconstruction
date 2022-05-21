using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.ViewModels;
using EventTimelineReconstruction.Views;
using System.Windows;

namespace EventTimelineReconstruction;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        //IEventsImporter importer = new L2tCSVEventsImporter();
        //EventsStore store = new(importer);

        //MainWindow = new ImportView()
        //{
        //    DataContext = new ImportViewModel(store)
        //};
        MainWindow = new MainWindow();
        MainWindow.Show();

        base.OnStartup(e);
    }
}
