using System.Windows;
using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.Validators;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Tests.IntegrationTests.Commands;

[TestClass]
public class ImportEventsCommandTests
{
    private static Application _app;

    private readonly IErrorsViewModel _errorsViewModel;
    private readonly IEventsStore _eventsStore;
    private readonly ImportViewModel _importViewModel;
    private readonly EventTreeViewModel _eventTreeViewModel;
    private readonly ImportEventsCommand _command;

    public ImportEventsCommandTests()
    {
        IDragDropUtils dragDropUtils = new DragDropUtils();
        IFilteringStore filteringStore = new FilteringStore();
        IEventsImporter eventsImporter = new L2tCSVEventsImporter();
        ITimeValidator timeValidator = new TimeValidator();
        _errorsViewModel = new ErrorsViewModel();
        IDateTimeProvider dateTimeProvider = new DateTimeProvider();
        _eventsStore = new EventsStore(eventsImporter);
        EventDetailsViewModel eventDetailsViewModel = new();
        ChangeColourViewModel changeColourViewModel = new(eventDetailsViewModel);
        _eventTreeViewModel = new(eventDetailsViewModel, filteringStore, changeColourViewModel, dragDropUtils);
        _importViewModel = new(_eventTreeViewModel, _eventsStore, timeValidator, _errorsViewModel, dateTimeProvider);

        _command = new(_importViewModel, _eventsStore, _eventTreeViewModel);
    }

    [ClassInitialize]
    public static void Initialize(TestContext context)
    {
        if (Application.Current == null)
        {
            _app = new Application();
        }

        using StreamWriter writeStream = File.CreateText(@"Import.csv");
        writeStream.WriteLine(@"date,time,timezone,MACB,source,sourcetype,type,user,host,short,desc,version,filename,inode,notes,format,extra");
        writeStream.WriteLine(@"01/01/1970,00:00:00,UTC,....,WEBHIST,MSIE Cache File URL record,Expiration Time,-,PC1-5DFC89FB1E0,Location: Visited: PC1@about:Home,Location: Visited: PC1@about:Home Number of hits: 2 Cached file size: 0,2,TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/index.dat,10536,-,msiecf,cache_directory_index: -2; recovered: False; sha256_hash: 243645de118fab85ae3e5f4f820ee50717ddf478f17f7a678c88aa5d437a7e70");
        writeStream.WriteLine(@"01/01/2003,00:00:00,UTC,....,WEBHIST,MSIE Cache File URL record,Expiration Time,-,PC1-5DFC89FB1E0,Location: Visited: PC1@about:Home,Location: Visited: PC1@about:Home Number of hits: 2 Cached file size: 0,2,TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/index.dat,10536,-,msiecf,cache_directory_index: -2; recovered: False; sha256_hash: 243645de118fab85ae3e5f4f820ee50717ddf478f17f7a678c88aa5d437a7e70");
        writeStream.WriteLine(@"01/01/2020,15:25:55,UTC,....,REG,AppCompatCache Registry Entry,File Last Modification Time,-,PC1-5DFC89FB1E0,[HKEY_LOCAL_MACHINE\System\ControlSet001\Control\Session Manager\AppCompatibi...,[HKEY_LOCAL_MACHINE\System\ControlSet001\Control\Session Manager\AppCompatibility] Cached entry: 28,2,TSK:/System Volume Information/_restore{5D162324-8035-4BDB-B6BA-8D2C3C5FBFF0}/RP2/snapshot/_REGISTRY_MACHINE_SYSTEM,13932,-,winreg/appcompatcache,sha256_hash: c822dbf91f7d96c0fcea412ed5ad22d8b1b0b7047357153d631940ac89042e38");
        writeStream.WriteLine(@"00:00:00,UTC,....,REG,AppCompatCache Registry Entry,File Last Modification Time,-,PC1-5DFC89FB1E0,[HKEY_LOCAL_MACHINE\System\ControlSet002\Control\Session Manager\AppCompatibi...,[HKEY_LOCAL_MACHINE\System\ControlSet002\Control\Session Manager\AppCompatibility] Cached entry: 28,2,TSK:/System Volume Information/_restore{5D162324-8035-4BDB-B6BA-8D2C3C5FBFF0}/RP2/snapshot/_REGISTRY_MACHINE_SYSTEM,13932,-,winreg/appcompatcache,sha256_hash: c822dbf91f7d96c0fcea412ed5ad22d8b1b0b7047357153d631940ac89042e38");
        writeStream.WriteLine(@"abc,00:00:00,UTC,....,REG,AppCompatCache Registry Entry,File Last Modification Time,-,PC1-5DFC89FB1E0,[HKEY_LOCAL_MACHINE\System\ControlSet002\Control\Session Manager\AppCompatibi...,[HKEY_LOCAL_MACHINE\System\ControlSet002\Control\Session Manager\AppCompatibility] Cached entry: 28,2,TSK:/WINDOWS/system32/config/system,7608,-,winreg/appcompatcache,sha256_hash: 9e3e9f916979ebaee33b80d75d7dc2b9e58fed306a69286b75cf3e14ade38d77");
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        File.Delete(@"Import.csv");

        _app.Shutdown();
    }

    [TestMethod]
    public void CanExecute_ShouldReturnFalse_WhenFileNameIsEmptyString()
    {
        // Arrange
        bool expected = false;
        _importViewModel.FileName = string.Empty;

        // Act
        bool actual = _command.CanExecute(null);

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void CanExecute_ShouldReturnFalse_WhenFileNameIsNull()
    {
        // Arrange
        bool expected = false;
        _importViewModel.FileName = null;

        // Act
        bool actual = _command.CanExecute(null);

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void CanExecute_ShouldReturnFalse_WhenThereAreErrors()
    {
        // Arrange
        bool expected = false;
        _importViewModel.FileName = "Test";
        _errorsViewModel.AddError("Test", "Error");

        // Act
        bool actual = _command.CanExecute(null);

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void CanExecute_ShouldReturnTrue_WhenFileNameIsNonEmptyStringAndThereAreNoErrors()
    {
        // Arrange
        bool expected = true;
        _importViewModel.FileName = "Test";

        // Act
        bool actual = _command.CanExecute(null);

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(actual);
    }

    //[TestMethod]
    //public async Task Execute_ShouldImportEventsWithinDateRange_WhneCommandIsExecuted()
    //{
    //    // Arrange
    //    List<EventViewModel> expected = new()
    //    {
    //        new EventViewModel(new EventModel(
    //            new DateOnly(2003, 1, 1),
    //            new TimeOnly(0, 0, 0),
    //            TimeZoneInfo.Utc,
    //            "....",
    //            "WEBHIST",
    //            "MSIE Cache File URL record",
    //            "Expiration Time",
    //            "-",
    //            "PC1-5DFC89FB1E0",
    //            "Location: Visited: PC1@about:Home",
    //            "Location: Visited: PC1@about:Home Number of hits: 2 Cached file size: 0",
    //            2,
    //            "TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/index.dat",
    //            "10536",
    //            "-",
    //            "msiecf",
    //            new Dictionary<string, string>() { { "cache_directory_index", "-2" }, { "recovered", "False" }, { "sha256_hash", "243645de118fab85ae3e5f4f820ee50717ddf478f17f7a678c88aa5d437a7e70" } })),
    //        new EventViewModel(new EventModel(
    //            new DateOnly(2020, 1, 1),
    //            new TimeOnly(15, 25, 55),
    //            TimeZoneInfo.Utc,
    //            "....",
    //            "REG",
    //            "AppCompatCache Registry Entry",
    //            "File Last Modification Time",
    //            "-",
    //            "PC1-5DFC89FB1E0",
    //            "[HKEY_LOCAL_MACHINE\\System\\ControlSet001\\Control\\Session Manager\\AppCompatibi...",
    //            "[HKEY_LOCAL_MACHINE\\System\\ControlSet001\\Control\\Session Manager\\AppCompatibility] Cached entry: 28",
    //            2,
    //            "TSK:/System Volume Information/_restore{5D162324-8035-4BDB-B6BA-8D2C3C5FBFF0}/RP2/snapshot/_REGISTRY_MACHINE_SYSTEM",
    //            "13932",
    //            "-",
    //            "winreg/appcompatcache",
    //            new Dictionary<string, string>() { { "sha256_hash", "c822dbf91f7d96c0fcea412ed5ad22d8b1b0b7047357153d631940ac89042e38" } }))
    //    };
    //    _importViewModel.FileName = @"Import.csv";
    //    _importViewModel.FromDate = new DateTime(2000, 1, 1);
    //    _importViewModel.ToDate = DateTime.MaxValue;

    //    // Act
    //    await _command.ExecuteAsync(null);
    //    List<EventViewModel> actual = _eventTreeViewModel.Events.ToList();

    //    // Assert
    //    Assert.AreEqual(expected.Count, _eventsStore.Events.Count);
    //    Assert.AreEqual(expected.Count, actual.Count);

    //    for (int i = 0; i < expected.Count; i++)
    //    {
    //        Assert.AreEqual(expected[i].FullDate, actual[i].FullDate);
    //        Assert.AreEqual(expected[i].Timezone, actual[i].Timezone);
    //        Assert.AreEqual(expected[i].MACB, actual[i].MACB);
    //        Assert.AreEqual(expected[i].Source, actual[i].Source);
    //        Assert.AreEqual(expected[i].SourceType, actual[i].SourceType);
    //        Assert.AreEqual(expected[i].Type, actual[i].Type);
    //        Assert.AreEqual(expected[i].User, actual[i].User);
    //        Assert.AreEqual(expected[i].Host, actual[i].Host);
    //        Assert.AreEqual(expected[i].Short, actual[i].Short);
    //        Assert.AreEqual(expected[i].Description, actual[i].Description);
    //        Assert.AreEqual(expected[i].Version, actual[i].Version);
    //        Assert.AreEqual(expected[i].Filename, actual[i].Filename);
    //        Assert.AreEqual(expected[i].INode, actual[i].INode);
    //        Assert.AreEqual(expected[i].Notes, actual[i].Notes);
    //        Assert.AreEqual(expected[i].Format, actual[i].Format);
    //        Assert.AreEqual(expected[i].Extra.Count, actual[i].Extra.Count);

    //        foreach (KeyValuePair<string, string> kvp in expected[i].Extra)
    //        {
    //            string expectedKey = kvp.Key;
    //            string expectedValue = kvp.Value;

    //            Assert.IsTrue(actual[i].Extra.ContainsKey(expectedKey));
    //            Assert.AreEqual(expectedValue, actual[i].Extra[expectedKey]);
    //        }

    //        Assert.AreEqual(expected[i].Colour.ToString(), actual[i].Colour.ToString());
    //        Assert.AreEqual(expected[i].IsVisible, actual[i].IsVisible);

    //        Assert.AreEqual(expected[i].FullDate, _eventsStore.Events[i].FullDate);
    //        Assert.AreEqual(expected[i].Timezone, _eventsStore.Events[i].Timezone);
    //        Assert.AreEqual(expected[i].MACB, _eventsStore.Events[i].MACB);
    //        Assert.AreEqual(expected[i].Source, _eventsStore.Events[i].Source);
    //        Assert.AreEqual(expected[i].SourceType, _eventsStore.Events[i].SourceType);
    //        Assert.AreEqual(expected[i].Type, _eventsStore.Events[i].Type);
    //        Assert.AreEqual(expected[i].User, _eventsStore.Events[i].User);
    //        Assert.AreEqual(expected[i].Host, _eventsStore.Events[i].Host);
    //        Assert.AreEqual(expected[i].Short, _eventsStore.Events[i].Short);
    //        Assert.AreEqual(expected[i].Description, _eventsStore.Events[i].Description);
    //        Assert.AreEqual(expected[i].Version, _eventsStore.Events[i].Version);
    //        Assert.AreEqual(expected[i].Filename, _eventsStore.Events[i].Filename);
    //        Assert.AreEqual(expected[i].INode, _eventsStore.Events[i].INode);
    //        Assert.AreEqual(expected[i].Notes, _eventsStore.Events[i].Notes);
    //        Assert.AreEqual(expected[i].Format, _eventsStore.Events[i].Format);
    //        Assert.AreEqual(expected[i].Extra.Count, _eventsStore.Events[i].Extra.Count);

    //        foreach (KeyValuePair<string, string> kvp in expected[i].Extra)
    //        {
    //            string expectedKey = kvp.Key;
    //            string expectedValue = kvp.Value;

    //            Assert.IsTrue(_eventsStore.Events[i].Extra.ContainsKey(expectedKey));
    //            Assert.AreEqual(expectedValue, _eventsStore.Events[i].Extra[expectedKey]);
    //        }

    //        Assert.AreEqual(expected[i].Colour.ToString(), _eventsStore.Events[i].Colour.ToString());
    //        Assert.AreEqual(expected[i].IsVisible, _eventsStore.Events[i].IsVisible);
    //    }
    //}
}
