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
        MainWindow = new ImportView()
        {
            DataContext = new ImportViewModel()
        };
        MainWindow.Show();

        base.OnStartup(e);
    }
}
