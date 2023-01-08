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
public class CheckIntegrityCommandTests
{
    private readonly IErrorsViewModel _errorsViewModel;
    private readonly IHashCalculator _hashCalculator;
    private readonly IEventsImporter _eventsImporter;
    private readonly IEventsStore _eventsStore;
    private readonly IntegrityViewModel _integrityViewModel;
    private readonly CheckIntegrityCommand _command;

    public CheckIntegrityCommandTests()
    {
        ITimeValidator timeValidator = new TimeValidator();
        IDateTimeProvider dateTimeProvider = new DateTimeProvider();

        _errorsViewModel = new ErrorsViewModel();
        _hashCalculator = new SHA256Calculator();
        _eventsImporter = new L2tCSVEventsImporter();
        _eventsStore = new EventsStore(_eventsImporter);
        _integrityViewModel = new(_eventsStore, _hashCalculator, _eventsImporter, timeValidator, _errorsViewModel, dateTimeProvider);

        _command = new(_integrityViewModel, _eventsStore, _hashCalculator, _eventsImporter);
    }

    [ClassInitialize]
    public static void Initialize(TestContext context)
    {
        using StreamWriter integrityStream = File.CreateText(@"Integrity.csv");
        integrityStream.WriteLine(@"date,time,timezone,MACB,source,sourcetype,type,user,host,short,desc,version,filename,inode,notes,format,extra");
        integrityStream.WriteLine(@"01/01/1970,00:00:00,UTC,....,WEBHIST,MSIE Cache File URL record,Expiration Time,-,PC1-5DFC89FB1E0,Location: Visited: PC1@about:Home,Location: Visited: PC1@about:Home Number of hits: 2 Cached file size: 0,2,TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/index.dat,10536,-,msiecf,cache_directory_index: -2; recovered: False; sha256_hash: 243645de118fab85ae3e5f4f820ee50717ddf478f17f7a678c88aa5d437a7e70");
        integrityStream.WriteLine(@"01/01/2003,00:00:00,UTC,....,WEBHIST,MSIE Cache File URL record,Expiration Time,-,PC1-5DFC89FB1E0,Location: Visited: PC1@about:Home,Location: Visited: PC1@about:Home Number of hits: 2 Cached file size: 0,2,TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/index.dat,10536,-,msiecf,cache_directory_index: -2; recovered: False; sha256_hash: 243645de118fab85ae3e5f4f820ee50717ddf478f17f7a678c88aa5d437a7e70");
        integrityStream.WriteLine(@"01/01/2020,15:25:55,UTC,....,REG,AppCompatCache Registry Entry,File Last Modification Time,-,PC1-5DFC89FB1E0,[HKEY_LOCAL_MACHINE\System\ControlSet001\Control\Session Manager\AppCompatibi...,[HKEY_LOCAL_MACHINE\System\ControlSet001\Control\Session Manager\AppCompatibility] Cached entry: 28,2,TSK:/System Volume Information/_restore{5D162324-8035-4BDB-B6BA-8D2C3C5FBFF0}/RP2/snapshot/_REGISTRY_MACHINE_SYSTEM,13932,-,winreg/appcompatcache,sha256_hash: c822dbf91f7d96c0fcea412ed5ad22d8b1b0b7047357153d631940ac89042e38");
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        File.Delete(@"Integrity.csv");
    }

    [TestMethod]
    public void CanExecute_ShouldReturnFalse_WhenFileNameIsEmptyString()
    {
        // Arrange
        bool expected = false;
        _integrityViewModel.FileName = string.Empty;

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
        _integrityViewModel.FileName = null;

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
        _integrityViewModel.FileName = "Test";
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
        _integrityViewModel.FileName = "Test";

        // Act
        bool actual = _command.CanExecute(null);

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(actual);
    }

    [STATestMethod]
    public async Task Execute_ShouldShowOnlyFileOKTextBlock_WhenHashValuesAreTheSameAndThereAreNoEventsInStore()
    {
        // Arrange
        _integrityViewModel.FileName = @"Integrity.csv";
        _integrityViewModel.HashValue = "3868294A8EB4A3BB5584B9628960DD362013F1F8F6C6F837691E4EABF4558AFF";

        // Act
        await _command.ExecuteAsync(null);

        // Assert
        Assert.AreEqual(Visibility.Visible, _integrityViewModel.FileOKVisibility);
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.FileUnknownVisibility);
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.FileCompromisedVisibility);
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.EventsOKVisibility);
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.EventsCompromisedVisibility);
    }

    [STATestMethod]
    public async Task Execute_ShouldShowOnlyFileUnknownTextBlock_WhenHashValueIsNotGivenAndThereAreNoEventsInStore()
    {
        // Arrange
        _integrityViewModel.FileName = @"Integrity.csv";
        _integrityViewModel.HashValue = string.Empty;

        // Act
        await _command.ExecuteAsync(null);

        // Assert
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.FileOKVisibility);
        Assert.AreEqual(Visibility.Visible, _integrityViewModel.FileUnknownVisibility);
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.FileCompromisedVisibility);
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.EventsOKVisibility);
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.EventsCompromisedVisibility);
    }

    [STATestMethod]
    public async Task Execute_ShouldShowOnlyFileCompromisedTextBlock_WhenHashValuesAreDifferentAndThereAreNoEventsInStore()
    {
        // Arrange
        _integrityViewModel.FileName = @"Integrity.csv";
        _integrityViewModel.HashValue = "ABC";

        // Act
        await _command.ExecuteAsync(null);

        // Assert
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.FileOKVisibility);
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.FileUnknownVisibility);
        Assert.AreEqual(Visibility.Visible, _integrityViewModel.FileCompromisedVisibility);
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.EventsOKVisibility);
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.EventsCompromisedVisibility);
    }

    [STATestMethod]
    public async Task Execute_ShouldShowFileOKAndEventsOKTextBlocks_WhenHashValuesAndEventsInStoreAreTheSame()
    {
        // Arrange
        _integrityViewModel.FileName = @"Integrity.csv";
        _integrityViewModel.HashValue = "3868294A8EB4A3BB5584B9628960DD362013F1F8F6C6F837691E4EABF4558AFF";
        _integrityViewModel.FromDate = new DateTime(1970, 1, 1);
        _integrityViewModel.FromHours = 0;
        _integrityViewModel.FromMinutes = 0;
        _integrityViewModel.ToDate = new DateTime(2022, 1, 1);
        _integrityViewModel.ToHours = 0;
        _integrityViewModel.ToMinutes = 0;

        List<EventViewModel> events = new()
        { 
            new(new EventModel(new DateOnly(1970, 1, 1), new TimeOnly(0, 0, 0), TimeZoneInfo.Utc, "....", "WEBHIST", "MSIE Cache File URL record", "Expiration Time", "-" , "PC1-5DFC89FB1E0", "Location: Visited: PC1@about:Home", "Location: Visited: PC1@about:Home Number of hits: 2 Cached file size: 0", 2, "TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/index.dat", "10536", "-", "msiecf", new Dictionary<string, string>() { { "cache_directory_index", "-2" }, { "recovered", "False" }, { "sha256_hash", "243645de118fab85ae3e5f4f820ee50717ddf478f17f7a678c88aa5d437a7e70" } }, 2)),
            new(new EventModel(new DateOnly(2003, 1, 1), new TimeOnly(0, 0, 0), TimeZoneInfo.Utc, "....", "WEBHIST", "MSIE Cache File URL record", "Expiration Time", "-" , "PC1-5DFC89FB1E0", "Location: Visited: PC1@about:Home", "Location: Visited: PC1@about:Home Number of hits: 2 Cached file size: 0", 2, "TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/index.dat", "10536", "-", "msiecf", new Dictionary<string, string>() { { "cache_directory_index", "-2" }, { "recovered", "False" }, { "sha256_hash", "243645de118fab85ae3e5f4f820ee50717ddf478f17f7a678c88aa5d437a7e70" } }, 3)),
            new(new EventModel(new DateOnly(2020, 1, 1), new TimeOnly(15, 25, 55), TimeZoneInfo.Utc, "....", "REG", "AppCompatCache Registry Entry", "File Last Modification Time", "-" , "PC1-5DFC89FB1E0", "[HKEY_LOCAL_MACHINE\\System\\ControlSet001\\Control\\Session Manager\\AppCompatibi...", "[HKEY_LOCAL_MACHINE\\System\\ControlSet001\\Control\\Session Manager\\AppCompatibility] Cached entry: 28", 2, "TSK:/System Volume Information/_restore{5D162324-8035-4BDB-B6BA-8D2C3C5FBFF0}/RP2/snapshot/_REGISTRY_MACHINE_SYSTEM", "13932", "-", "winreg/appcompatcache", new Dictionary<string, string>() { { "sha256_hash", "c822dbf91f7d96c0fcea412ed5ad22d8b1b0b7047357153d631940ac89042e38" } }, 4))
        };
        _eventsStore.LoadEvents(events);

        // Act
        await _command.ExecuteAsync(null);

        // Assert
        Assert.AreEqual(Visibility.Visible, _integrityViewModel.FileOKVisibility);
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.FileUnknownVisibility);
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.FileCompromisedVisibility);
        Assert.AreEqual(Visibility.Visible, _integrityViewModel.EventsOKVisibility);
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.EventsCompromisedVisibility);
    }

    [STATestMethod]
    public async Task Execute_ShouldShowFileUnknownAndEventsOKTextBlocks_WhenHashValueIsNotGivenAndEventsInStoreAreTheSame()
    {
        // Arrange
        _integrityViewModel.FileName = @"Integrity.csv";
        _integrityViewModel.HashValue = string.Empty;
        _integrityViewModel.FromDate = new DateTime(1970, 1, 1);
        _integrityViewModel.FromHours = 0;
        _integrityViewModel.FromMinutes = 0;
        _integrityViewModel.ToDate = new DateTime(2022, 1, 1);
        _integrityViewModel.ToHours = 0;
        _integrityViewModel.ToMinutes = 0;

        List<EventViewModel> events = new()
        {
            new(new EventModel(new DateOnly(1970, 1, 1), new TimeOnly(0, 0, 0), TimeZoneInfo.Utc, "....", "WEBHIST", "MSIE Cache File URL record", "Expiration Time", "-" , "PC1-5DFC89FB1E0", "Location: Visited: PC1@about:Home", "Location: Visited: PC1@about:Home Number of hits: 2 Cached file size: 0", 2, "TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/index.dat", "10536", "-", "msiecf", new Dictionary<string, string>() { { "cache_directory_index", "-2" }, { "recovered", "False" }, { "sha256_hash", "243645de118fab85ae3e5f4f820ee50717ddf478f17f7a678c88aa5d437a7e70" } }, 2)),
            new(new EventModel(new DateOnly(2003, 1, 1), new TimeOnly(0, 0, 0), TimeZoneInfo.Utc, "....", "WEBHIST", "MSIE Cache File URL record", "Expiration Time", "-" , "PC1-5DFC89FB1E0", "Location: Visited: PC1@about:Home", "Location: Visited: PC1@about:Home Number of hits: 2 Cached file size: 0", 2, "TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/index.dat", "10536", "-", "msiecf", new Dictionary<string, string>() { { "cache_directory_index", "-2" }, { "recovered", "False" }, { "sha256_hash", "243645de118fab85ae3e5f4f820ee50717ddf478f17f7a678c88aa5d437a7e70" } }, 3)),
            new(new EventModel(new DateOnly(2020, 1, 1), new TimeOnly(15, 25, 55), TimeZoneInfo.Utc, "....", "REG", "AppCompatCache Registry Entry", "File Last Modification Time", "-" , "PC1-5DFC89FB1E0", "[HKEY_LOCAL_MACHINE\\System\\ControlSet001\\Control\\Session Manager\\AppCompatibi...", "[HKEY_LOCAL_MACHINE\\System\\ControlSet001\\Control\\Session Manager\\AppCompatibility] Cached entry: 28", 2, "TSK:/System Volume Information/_restore{5D162324-8035-4BDB-B6BA-8D2C3C5FBFF0}/RP2/snapshot/_REGISTRY_MACHINE_SYSTEM", "13932", "-", "winreg/appcompatcache", new Dictionary<string, string>() { { "sha256_hash", "c822dbf91f7d96c0fcea412ed5ad22d8b1b0b7047357153d631940ac89042e38" } }, 4))
        };
        _eventsStore.LoadEvents(events);

        // Act
        await _command.ExecuteAsync(null);

        // Assert
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.FileOKVisibility);
        Assert.AreEqual(Visibility.Visible, _integrityViewModel.FileUnknownVisibility);
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.FileCompromisedVisibility);
        Assert.AreEqual(Visibility.Visible, _integrityViewModel.EventsOKVisibility);
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.EventsCompromisedVisibility);
    }

    [STATestMethod]
    public async Task Execute_ShouldShowFileCompromisedAndEventsOKTextBlocks_WhenHashValuesAreDifferentAndEventsInStoreAreTheSame()
    {
        // Arrange
        _integrityViewModel.FileName = @"Integrity.csv";
        _integrityViewModel.HashValue = "ABC";
        _integrityViewModel.FromDate = new DateTime(1970, 1, 1);
        _integrityViewModel.FromHours = 0;
        _integrityViewModel.FromMinutes = 0;
        _integrityViewModel.ToDate = new DateTime(2022, 1, 1);
        _integrityViewModel.ToHours = 0;
        _integrityViewModel.ToMinutes = 0;

        List<EventViewModel> events = new()
        {
            new(new EventModel(new DateOnly(1970, 1, 1), new TimeOnly(0, 0, 0), TimeZoneInfo.Utc, "....", "WEBHIST", "MSIE Cache File URL record", "Expiration Time", "-" , "PC1-5DFC89FB1E0", "Location: Visited: PC1@about:Home", "Location: Visited: PC1@about:Home Number of hits: 2 Cached file size: 0", 2, "TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/index.dat", "10536", "-", "msiecf", new Dictionary<string, string>() { { "cache_directory_index", "-2" }, { "recovered", "False" }, { "sha256_hash", "243645de118fab85ae3e5f4f820ee50717ddf478f17f7a678c88aa5d437a7e70" } }, 2)),
            new(new EventModel(new DateOnly(2003, 1, 1), new TimeOnly(0, 0, 0), TimeZoneInfo.Utc, "....", "WEBHIST", "MSIE Cache File URL record", "Expiration Time", "-" , "PC1-5DFC89FB1E0", "Location: Visited: PC1@about:Home", "Location: Visited: PC1@about:Home Number of hits: 2 Cached file size: 0", 2, "TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/index.dat", "10536", "-", "msiecf", new Dictionary<string, string>() { { "cache_directory_index", "-2" }, { "recovered", "False" }, { "sha256_hash", "243645de118fab85ae3e5f4f820ee50717ddf478f17f7a678c88aa5d437a7e70" } }, 3)),
            new(new EventModel(new DateOnly(2020, 1, 1), new TimeOnly(15, 25, 55), TimeZoneInfo.Utc, "....", "REG", "AppCompatCache Registry Entry", "File Last Modification Time", "-" , "PC1-5DFC89FB1E0", "[HKEY_LOCAL_MACHINE\\System\\ControlSet001\\Control\\Session Manager\\AppCompatibi...", "[HKEY_LOCAL_MACHINE\\System\\ControlSet001\\Control\\Session Manager\\AppCompatibility] Cached entry: 28", 2, "TSK:/System Volume Information/_restore{5D162324-8035-4BDB-B6BA-8D2C3C5FBFF0}/RP2/snapshot/_REGISTRY_MACHINE_SYSTEM", "13932", "-", "winreg/appcompatcache", new Dictionary<string, string>() { { "sha256_hash", "c822dbf91f7d96c0fcea412ed5ad22d8b1b0b7047357153d631940ac89042e38" } }, 4))
        };
        _eventsStore.LoadEvents(events);

        // Act
        await _command.ExecuteAsync(null);

        // Assert
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.FileOKVisibility);
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.FileUnknownVisibility);
        Assert.AreEqual(Visibility.Visible, _integrityViewModel.FileCompromisedVisibility);
        Assert.AreEqual(Visibility.Visible, _integrityViewModel.EventsOKVisibility);
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.EventsCompromisedVisibility);
    }

    [STATestMethod]
    public async Task Execute_ShouldShowFileOKAndEventsCompromisedTextBlocks_WhenHashValuesAreTheSameAndEventsInStoreAreDifferent()
    {
        // Arrange
        _integrityViewModel.FileName = @"Integrity.csv";
        _integrityViewModel.HashValue = "3868294A8EB4A3BB5584B9628960DD362013F1F8F6C6F837691E4EABF4558AFF";
        _integrityViewModel.FromDate = new DateTime(1970, 1, 1);
        _integrityViewModel.FromHours = 0;
        _integrityViewModel.FromMinutes = 0;
        _integrityViewModel.ToDate = new DateTime(2022, 1, 1);
        _integrityViewModel.ToHours = 0;
        _integrityViewModel.ToMinutes = 0;

        List<EventViewModel> events = new()
        {
            new(new EventModel(new DateOnly(1970, 1, 1), new TimeOnly(0, 0, 0), TimeZoneInfo.Utc, "....", "WEBHIST", "MSIE Cache File URL record", "Expiration Time", "-" , "PC1-5DFC89FB1E0", "Location: Visited: PC1@about:Home", "Location: Visited: PC1@about:Home Number of hits: 2 Cached file size: 0", 2, "TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/index.dat", "10536", "-", "msiecf", new Dictionary<string, string>() { { "cache_directory_index", "-2" }, { "recovered", "False" }, { "sha256_hash", "243645de118fab85ae3e5f4f820ee50717ddf478f17f7a678c88aa5d437a7e70" } }, 1)),
            new(new EventModel(new DateOnly(2003, 1, 1), new TimeOnly(0, 0, 0), TimeZoneInfo.Utc, "....", "WEBHIST", "MSIE Cache File URL record", "Expiration Time", "-" , "PC1-5DFC89FB1E0", "Location: Visited: PC1@about:Home", "Location: Visited: PC1@about:Home Number of hits: 2 Cached file size: 0", 2, "TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/index.dat", "10536", "-", "msiecf", new Dictionary<string, string>() { { "cache_directory_index", "-2" }, { "recovered", "False" }, { "sha256_hash", "243645de118fab85ae3e5f4f820ee50717ddf478f17f7a678c88aa5d437a7e70" } }, 2))
        };
        _eventsStore.LoadEvents(events);

        // Act
        await _command.ExecuteAsync(null);

        // Assert
        Assert.AreEqual(Visibility.Visible, _integrityViewModel.FileOKVisibility);
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.FileUnknownVisibility);
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.FileCompromisedVisibility);
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.EventsOKVisibility);
        Assert.AreEqual(Visibility.Visible, _integrityViewModel.EventsCompromisedVisibility);
    }

    [STATestMethod]
    public async Task Execute_ShouldShowFileUnknownAndEventsCompromisedTextBlocks_WhenHashValueIsNotGivenAndEventsInStoreAreDifferent()
    {
        // Arrange
        _integrityViewModel.FileName = @"Integrity.csv";
        _integrityViewModel.HashValue = string.Empty;
        _integrityViewModel.FromDate = new DateTime(1970, 1, 1);
        _integrityViewModel.FromHours = 0;
        _integrityViewModel.FromMinutes = 0;
        _integrityViewModel.ToDate = new DateTime(2022, 1, 1);
        _integrityViewModel.ToHours = 0;
        _integrityViewModel.ToMinutes = 0;

        List<EventViewModel> events = new()
        {
            new(new EventModel(new DateOnly(1970, 1, 1), new TimeOnly(0, 0, 0), TimeZoneInfo.Utc, "....", "WEBHIST", "MSIE Cache File URL record", "Expiration Time", "-" , "PC1-5DFC89FB1E0", "Location: Visited: PC1@about:Home", "Location: Visited: PC1@about:Home Number of hits: 2 Cached file size: 0", 2, "TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/index.dat", "10536", "-", "msiecf", new Dictionary<string, string>() { { "cache_directory_index", "-2" }, { "recovered", "False" }, { "sha256_hash", "243645de118fab85ae3e5f4f820ee50717ddf478f17f7a678c88aa5d437a7e70" } }, 1)),
            new(new EventModel(new DateOnly(2003, 1, 1), new TimeOnly(0, 0, 0), TimeZoneInfo.Utc, "....", "WEBHIST", "MSIE Cache File URL record", "Expiration Time", "-" , "PC1-5DFC89FB1E0", "Location: Visited: PC1@about:Home", "Location: Visited: PC1@about:Home Number of hits: 2 Cached file size: 0", 2, "TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/index.dat", "10536", "-", "msiecf", new Dictionary<string, string>() { { "cache_directory_index", "-2" }, { "recovered", "False" }, { "sha256_hash", "243645de118fab85ae3e5f4f820ee50717ddf478f17f7a678c88aa5d437a7e70" } }, 2))
        };
        _eventsStore.LoadEvents(events);

        // Act
        await _command.ExecuteAsync(null);

        // Assert
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.FileOKVisibility);
        Assert.AreEqual(Visibility.Visible, _integrityViewModel.FileUnknownVisibility);
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.FileCompromisedVisibility);
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.EventsOKVisibility);
        Assert.AreEqual(Visibility.Visible, _integrityViewModel.EventsCompromisedVisibility);
    }

    [STATestMethod]
    public async Task Execute_ShouldShowFileCompromisedAndEventsCompromisedTextBlocks_WhenHashValuesAndEventsInStoreAreDifferent()
    {
        // Arrange
        _integrityViewModel.FileName = @"Integrity.csv";
        _integrityViewModel.HashValue = "ABC";
        _integrityViewModel.FromDate = new DateTime(1970, 1, 1);
        _integrityViewModel.FromHours = 0;
        _integrityViewModel.FromMinutes = 0;
        _integrityViewModel.ToDate = new DateTime(2022, 1, 1);
        _integrityViewModel.ToHours = 0;
        _integrityViewModel.ToMinutes = 0;

        List<EventViewModel> events = new()
        {
            new(new EventModel(new DateOnly(1970, 1, 1), new TimeOnly(0, 0, 0), TimeZoneInfo.Utc, "....", "WEBHIST", "MSIE Cache File URL record", "Expiration Time", "-" , "PC1-5DFC89FB1E0", "Location: Visited: PC1@about:Home", "Location: Visited: PC1@about:Home Number of hits: 2 Cached file size: 0", 2, "TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/index.dat", "10536", "-", "msiecf", new Dictionary<string, string>() { { "cache_directory_index", "-2" }, { "recovered", "False" }, { "sha256_hash", "243645de118fab85ae3e5f4f820ee50717ddf478f17f7a678c88aa5d437a7e70" } }, 1)),
            new(new EventModel(new DateOnly(2003, 1, 1), new TimeOnly(0, 0, 0), TimeZoneInfo.Utc, "....", "WEBHIST", "MSIE Cache File URL record", "Expiration Time", "-" , "PC1-5DFC89FB1E0", "Location: Visited: PC1@about:Home", "Location: Visited: PC1@about:Home Number of hits: 2 Cached file size: 0", 2, "TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/index.dat", "10536", "-", "msiecf", new Dictionary<string, string>() { { "cache_directory_index", "-2" }, { "recovered", "False" }, { "sha256_hash", "243645de118fab85ae3e5f4f820ee50717ddf478f17f7a678c88aa5d437a7e70" } }, 2)),
            new(new EventModel(new DateOnly(2021, 1, 1), new TimeOnly(15, 25, 55), TimeZoneInfo.Utc, "....", "REG", "AppCompatCache Registry Entry", "File Last Modification Time", "-" , "PC1-5DFC89FB1E0", "[HKEY_LOCAL_MACHINE\\System\\ControlSet001\\Control\\Session Manager\\AppCompatibi...", "[HKEY_LOCAL_MACHINE\\System\\ControlSet001\\Control\\Session Manager\\AppCompatibility] Cached entry: 28", 2, "TSK:/System Volume Information/_restore{5D162324-8035-4BDB-B6BA-8D2C3C5FBFF0}/RP2/snapshot/_REGISTRY_MACHINE_SYSTEM", "13932", "-", "winreg/appcompatcache", new Dictionary<string, string>() { { "sha256_hash", "c822dbf91f7d96c0fcea412ed5ad22d8b1b0b7047357153d631940ac89042e38" } }, 3))
        };
        _eventsStore.LoadEvents(events);

        // Act
        await _command.ExecuteAsync(null);

        // Assert
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.FileOKVisibility);
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.FileUnknownVisibility);
        Assert.AreEqual(Visibility.Visible, _integrityViewModel.FileCompromisedVisibility);
        Assert.AreEqual(Visibility.Collapsed, _integrityViewModel.EventsOKVisibility);
        Assert.AreEqual(Visibility.Visible, _integrityViewModel.EventsCompromisedVisibility);
    }
}
