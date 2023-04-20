using EventTimelineReconstruction.Abstractors;
using EventTimelineReconstruction.ChainOfResponsibility.HighLevelArtefacts;
using EventTimelineReconstruction.ChainOfResponsibility.HighLevelEvents;
using EventTimelineReconstruction.ChainOfResponsibility.LowLevelArtefacts;
using EventTimelineReconstruction.ChainOfResponsibility.LowLevelEvents;
using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.Validators;
using EventTimelineReconstruction.ViewModels;
using EventTimelineReconstruction.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace EventTimelineReconstruction;

public partial class App : Application
{
    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services => {
                services.AddTransient<IColouringUtils, ColouringUtils>();
                services.AddTransient<IDragDropUtils, DragDropUtils>();
                services.AddTransient<IFileUtils, FileUtils>();
                services.AddTransient<IFilteringUtils, FilteringUtils>();
                services.AddTransient<IResourcesUtils, ResourcesUtils>();
                services.AddTransient<IDateTimeProvider, DateTimeProvider>();

                services.AddTransient<IErrorsViewModel, ErrorsViewModel>();

                services.AddSingleton<IEventsImporter, L2tCSVEventsImporter>();
                services.AddSingleton<IWorkSaver, FileWorkSaver>();
                services.AddSingleton<IWorkLoader, FileWorkLoader>();
                services.AddSingleton<IHashCalculator, SHA256Calculator>();

                services.AddSingleton<ITimeValidator, TimeValidator>();

                services.AddSingleton<IEventsStore, EventsStore>();
                services.AddSingleton<IFilteringStore, FilteringStore>();
                services.AddSingleton<IColouringStore, ColouringStore>();

                services.AddTransient<IHighLevelEventsAbstractorUtils, HighLevelEventsAbstractorUtils>();
                services.AddTransient<ILowLevelEventsAbstractorUtils, LowLevelEventsAbstractorUtils>();
                services.AddTransient<IHighLevelArtefactsAbstractorUtils, HighLevelArtefactsAbstractorUtils>();
                services.AddTransient<ILowLevelArtefactsAbstractorUtils, LowLevelArtefactsAbstractorUtils>();

                services.AddTransient<IHighLogEventHandler, HighLogEventHandler>();
                services.AddTransient<IHighLnkEventHandler, HighLnkEventHandler>();
                services.AddTransient<IHighMetaEventHandler, HighMetaEventHandler>();
                services.AddTransient<IHighOlecfEventHandler, HighOlecfEventHandler>();
                services.AddTransient<IHighPeEventHandler, HighPeEventHandler>();
                services.AddTransient<IHighWebhistEventHandler, HighWebhistEventHandler>();

                services.AddTransient<ILowWebhistEventHandler, LowWebhistEventHandler>();
                services.AddTransient<ILowFileEventHandler, LowFileEventHandler>();
                services.AddTransient<ILowLnkEventHandler, LowLnkEventHandler>();
                services.AddTransient<ILowLogEventHandler, LowLogEventHandler>();
                services.AddTransient<ILowMetaEventHandler, LowMetaEventHandler>();
                services.AddTransient<ILowRegEventHandler, LowRegEventHandler>();
                services.AddTransient<ILowOlecfEventHandler, LowOlecfEventHandler>();
                services.AddTransient<ILowPeEventHandler, LowPeEventHandler>();
                services.AddTransient<ILowRecbinEventHandler, LowRecbinEventHandler>();

                services.AddTransient<IHighWebhistArtefactHandler, HighWebhistArtefactHandler>();
                services.AddTransient<IHighLnkArtefactHandler, HighLnkArtefactHandler>();
                services.AddTransient<IHighFileArtefactHandler, HighFileArtefactHandler>();
                services.AddTransient<IHighLogArtefactHandler, HighLogArtefactHandler>();
                services.AddTransient<IHighRegArtefactHandler, HighRegArtefactHandler>();
                services.AddTransient<IHighMetaArtefactHandler, HighMetaArtefactHandler>();
                services.AddTransient<IHighOlecfArtefactHandler, HighOlecfArtefactHandler>();
                services.AddTransient<IHighPeArtefactHandler, HighPeArtefactHandler>();

                services.AddTransient<ILowWebhistArtefactHandler, LowWebhistArtefactHandler>();
                services.AddTransient<ILowLnkArtefactHandler, LowLnkArtefactHandler>();
                services.AddTransient<ILowFileArtefactHandler, LowFileArtefactHandler>();

                services.AddTransient<IHighLevelEventsAbstractor, HighLevelEventsAbstractor>();
                services.AddTransient<ILowLevelEventsAbstractor, LowLevelEventsAbstractor>();
                services.AddTransient<IHighLevelArtefactsAbstractor, HighLevelArtefactsAbstractor>();
                services.AddTransient<ILowLevelArtefactsAbstractor, LowLevelArtefactsAbstractor>();

                services.AddSingleton<IntegrityViewModel>();
                services.AddSingleton<EventDetailsViewModel>();
                services.AddSingleton<EventTreeViewModel>();
                services.AddSingleton<ImportViewModel>();
                services.AddSingleton<SaveWorkViewModel>();
                services.AddSingleton<LoadWorkViewModel>();
                services.AddSingleton<HiddenEventsViewModel>();
                services.AddSingleton<MainWindowViewModel>();
                services.AddSingleton<FilterViewModel>();
                services.AddSingleton<ChangeColourViewModel>();
                services.AddSingleton<ColourViewModel>();
                services.AddSingleton<AbstractedEventsViewModel>();

                services.AddSingleton(s => new IntegrityView()
                {
                    DataContext = s.GetRequiredService<IntegrityViewModel>()
                });
                services.AddSingleton(s => new ColourView()
                {
                    DataContext = s.GetRequiredService<ColourViewModel>()
                });
                services.AddSingleton(s => new ChangeColourView()
                {
                    DataContext = s.GetRequiredService<ChangeColourViewModel>()
                });
                services.AddSingleton(s => new ImportView() 
                {
                    DataContext = s.GetRequiredService<ImportViewModel>()
                });
                services.AddSingleton(s => new SaveWorkView() 
                {
                    DataContext = s.GetRequiredService<SaveWorkViewModel>()
                });
                services.AddSingleton(s => new LoadWorkView() 
                {
                    DataContext = s.GetRequiredService<LoadWorkViewModel>()
                });
                services.AddSingleton(s => new EventTreeView() 
                {
                    DataContext = s.GetRequiredService<EventTreeViewModel>()
                });
                services.AddSingleton(s => new EventDetailsView() 
                {
                    DataContext = s.GetRequiredService<EventDetailsViewModel>()
                });
                services.AddSingleton(s => new HiddenEventsView() 
                {
                    DataContext = s.GetRequiredService<HiddenEventsViewModel>()
                });
                services.AddSingleton(s => new FilterView()
                {
                    DataContext = s.GetRequiredService<FilterViewModel>()
                });
                services.AddSingleton(s => new AbstractionView()
                {
                    DataContext = s.GetRequiredService<AbstractedEventsViewModel>()
                });
                services.AddSingleton(s => new MainWindow(
                    s.GetRequiredService<ImportView>(), 
                    s.GetRequiredService<SaveWorkView>(), 
                    s.GetRequiredService<LoadWorkView>(), 
                    s.GetRequiredService<HiddenEventsView>(),
                    s.GetRequiredService<FilterView>(), 
                    s.GetRequiredService<ChangeColourView>(),
                    s.GetRequiredService<ColourView>(), 
                    s.GetRequiredService<IntegrityView>(),
                    s.GetRequiredService<AbstractionView>()) {
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
