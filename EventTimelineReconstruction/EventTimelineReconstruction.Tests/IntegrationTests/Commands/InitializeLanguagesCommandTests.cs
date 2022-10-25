using System.Windows;
using System.Windows.Controls;
using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.Validators;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Tests.IntegrationTests.Commands;

[TestClass]
public class InitializeLanguagesCommandTests
{
    private static Application _app;
    private static string _directoryPath = @$"{Directory.GetCurrentDirectory()}/../../../Resources";

    private readonly IFileUtils _fileUtils;
    private readonly IResourcesUtils _resourcesUtils;
    private readonly IErrorsViewModel _importErrors;
    private readonly IErrorsViewModel _filterErrors;
    private readonly IErrorsViewModel _integrityErrors;
    private readonly ImportViewModel _importViewModel;
    private readonly FilterViewModel _filterViewModel;
    private readonly IntegrityViewModel _integrityViewModel;
    private readonly InitializeLanguagesCommand _command;

    public InitializeLanguagesCommandTests()
    {
        IDragDropUtils dragDropUtils = new DragDropUtils();
        IFilteringStore filteringStore = new FilteringStore();
        IFilteringUtils filteringUtils = new FilteringUtils();
        IEventsImporter eventsImporter = new L2tCSVEventsImporter();
        ITimeValidator timeValidator = new TimeValidator();
        _importErrors = new ErrorsViewModel();
        _filterErrors = new ErrorsViewModel();
        _integrityErrors = new ErrorsViewModel();
        IDateTimeProvider dateTimeProvider = new DateTimeProvider();
        IHashCalculator hashCalculator = new SHA256Calculator();
        IEventsStore eventsStore = new EventsStore(eventsImporter);
        EventDetailsViewModel eventDetailsViewModel = new();
        ChangeColourViewModel changeColourViewModel = new(eventDetailsViewModel);
        EventTreeViewModel eventTreeViewModel = new(eventDetailsViewModel, filteringStore, changeColourViewModel, dragDropUtils);
        _importViewModel = new(eventTreeViewModel, eventsStore, timeValidator, _importErrors, dateTimeProvider);
        _filterViewModel = new(filteringStore, eventTreeViewModel, timeValidator, filteringUtils, _filterErrors, dateTimeProvider);
        _integrityViewModel = new(eventsStore, hashCalculator, eventsImporter, timeValidator, _integrityErrors, dateTimeProvider);
        _fileUtils = new FileUtils();
        _resourcesUtils = new ResourcesUtils();

        _command = new(_importViewModel, _filterViewModel, _integrityViewModel, _fileUtils, _resourcesUtils);
    }

    [ClassInitialize]
    public static void Initialize(TestContext context)
    {
        if (Application.Current == null)
        {
            _app = new Application();
        }

        Directory.CreateDirectory($@"{_directoryPath}/Localizations");

        using StreamWriter enStream = File.CreateText($@"{_directoryPath}/Localizations/Resource.en.xaml");
        enStream.WriteLine(@"<ResourceDictionary xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:system=""clr-namespace:System;assembly=netstandard"">");
        enStream.WriteLine(@"<system:String x:Key=""Error_From_After_To"">The ""From"" date can not be after the ""To"" date.</system:String>");
        enStream.WriteLine(@"<system:String x:Key=""Error_To_Before_From"">The ""To"" date can not be before the ""From"" date.</system:String>");
        enStream.WriteLine(@"</ResourceDictionary>");

        using StreamWriter ltStream = File.CreateText($@"{_directoryPath}/Localizations//Resource.lt.xaml");
        ltStream.WriteLine(@"<ResourceDictionary xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:system=""clr-namespace:System;assembly=netstandard"">");
        ltStream.WriteLine(@"<system:String x:Key=""Error_From_After_To"">Data „Nuo“ negali būti vėlesnė nei „Iki“.</system:String>");
        ltStream.WriteLine(@"<system:String x:Key=""Error_To_Before_From"">Data „Iki“ negali būti ankstesnė nei „Nuo“.</system:String>");
        ltStream.WriteLine(@"</ResourceDictionary>");
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        _app.Shutdown();

        Directory.Delete(_directoryPath, true);
    }

    [STATestMethod]
    public void Execute_ShouldInitializeEnglishLanguage_WhenCommandIsExecuted()
    {
        // Arrange
        MenuItem menuItem = new() { Name = "Languages" };
        RoutedEventArgs e = new(MenuItem.LoadedEvent, menuItem);
        object parameter = (object)e;
        List<MenuItem> expected = new()
        {
            new MenuItem() { Header = "English", Tag = "en", IsChecked = true },
            new MenuItem() { Header = "lietuvių", Tag = "lt", IsChecked = false }
        };

        // Act
        _command.Execute(parameter);
        List<MenuItem> actual = menuItem.Items.Cast<MenuItem>().ToList();

        // Assert
        Assert.AreEqual(expected.Count, actual.Count);

        for (int i = 0; i < expected.Count; i++)
        {
            Assert.AreEqual(expected[i].Header, actual[i].Header);
            Assert.AreEqual(expected[i].Tag, actual[i].Tag);
            Assert.AreEqual(expected[i].IsChecked, actual[i].IsChecked);
        }
    }

    [STATestMethod]
    public void Execute_ShouldChangeLanguage_WhenMenuItemIsClicked()
    {
        // Arrange
        MenuItem menuItem = new() { Name = "Languages" };
        RoutedEventArgs initializationEvent = new(MenuItem.LoadedEvent, menuItem);
        object parameter = (object)initializationEvent;
        _command.Execute(parameter);

        _importViewModel.FromDate = new DateTime(2022, 1, 1);
        _importViewModel.ToDate = new DateTime(2000, 1, 1);
        _filterViewModel.FromDate = new DateTime(2022, 1, 1);
        _filterViewModel.ToDate = new DateTime(2000, 1, 1);
        _integrityViewModel.FromDate = new DateTime(2022, 1, 1);
        _integrityViewModel.ToDate = new DateTime(2000, 1, 1);

        MenuItem clickedItem = (MenuItem)menuItem.Items[1];
        RoutedEventArgs clickEvent = new(MenuItem.ClickEvent, clickedItem);

        List<string> expected = new() { "Data „Nuo“ negali būti vėlesnė nei „Iki“.", "Data „Iki“ negali būti ankstesnė nei „Nuo“." };

        // Act
        clickedItem.RaiseEvent(clickEvent);

        // Assert
        Assert.AreEqual(expected.Count, _importErrors.Errors.Count);
        Assert.AreEqual(expected.Count, _filterErrors.Errors.Count);
        Assert.AreEqual(expected.Count, _integrityErrors.Errors.Count);

        for (int i = 0; i < expected.Count; i++)
        {
            Assert.IsTrue(_importErrors.Errors.Contains(expected[i]));
            Assert.IsTrue(_filterErrors.Errors.Contains(expected[i]));
            Assert.IsTrue(_integrityErrors.Errors.Contains(expected[i]));
        }
    }
}
