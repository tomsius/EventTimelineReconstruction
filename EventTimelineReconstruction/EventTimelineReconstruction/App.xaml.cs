using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.ViewModels;
using EventTimelineReconstruction.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace EventTimelineReconstruction;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services => {
                services.AddSingleton<IEventsImporter, L2tCSVEventsImporter>();
                services.AddSingleton<IWorkSaver, FileWorkSaver>();
                services.AddSingleton<IWorkLoader, FileWorkLoader>();

                services.AddSingleton<EventsStore>();

                services.AddSingleton<EventDetailsViewModel>();
                services.AddSingleton(s => new EventTreeViewModel(s.GetRequiredService<EventDetailsViewModel>()));
                services.AddSingleton(s => new ImportViewModel(s.GetRequiredService<EventTreeViewModel>(), s.GetRequiredService<EventsStore>()));
                services.AddSingleton<SaveWorkViewModel>();
                services.AddSingleton<LoadWorkViewModel>();
                services.AddSingleton(s => new HiddenEventsViewModel(s.GetRequiredService<EventsStore>()));
                services.AddSingleton(s => new MainWindowViewModel(s.GetRequiredService<EventTreeViewModel>(), s.GetRequiredService<EventDetailsViewModel>(), s.GetRequiredService<HiddenEventsViewModel>()));

                services.AddSingleton(s => new ImportView() 
                {
                    DataContext = s.GetRequiredService<ImportViewModel>()
                });
                services.AddSingleton(s => new SaveWorkView() {
                    DataContext = s.GetRequiredService<SaveWorkViewModel>()
                });

                services.AddSingleton(s => new LoadWorkView() {
                    DataContext = s.GetRequiredService<LoadWorkViewModel>()
                });
                services.AddSingleton(s => new EventTreeView() {
                    DataContext = s.GetRequiredService<EventTreeViewModel>()
                });
                services.AddSingleton(s => new EventDetailsView() {
                    DataContext = s.GetRequiredService<EventDetailsViewModel>()
                });
                services.AddSingleton(s => new HiddenEventsView() {
                    DataContext = s.GetRequiredService<HiddenEventsViewModel>()
                });
                services.AddSingleton(s => new MainWindow(s.GetRequiredService<ImportView>(), s.GetRequiredService<SaveWorkView>(), s.GetRequiredService<LoadWorkView>(), s.GetRequiredService<HiddenEventsView>()) {
                    DataContext = s.GetRequiredService<MainWindowViewModel>()
                });
            })
            .Build();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        _host.Start();

        MainWindow = _host.Services.GetRequiredService<MainWindow>();
        MainWindow.Show();

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _host.Dispose();

        base.OnExit(e);
    }
}
