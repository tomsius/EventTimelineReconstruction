using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Tests.IntegrationTests.Commands;

[TestClass]
public class LoadWorkCommandTests
{
    private readonly LoadWorkViewModel _loadWorkViewModel;
    private readonly EventTreeViewModel _eventTreeViewModel;
    private readonly IEventsStore _eventsStore;
    private readonly IWorkLoader _workLoader;
    private readonly LoadWorkCommand _command;

    public LoadWorkCommandTests()
    {
        IDragDropUtils dragDropUtils = new DragDropUtils();
        IFilteringStore filteringStore = new FilteringStore();
        IEventsImporter eventsImporter = new L2tCSVEventsImporter();
        EventDetailsViewModel eventDetailsViewModel = new();
        ChangeColourViewModel changeColourViewModel = new(eventDetailsViewModel);
        _eventTreeViewModel = new(eventDetailsViewModel, filteringStore, changeColourViewModel, dragDropUtils);
        _eventsStore = new EventsStore(eventsImporter);
        _workLoader = new FileWorkLoader();
        _loadWorkViewModel = new(_eventTreeViewModel, _eventsStore, _workLoader);

        _command = new(_loadWorkViewModel, _eventTreeViewModel, _eventsStore, _workLoader);
    }

    [ClassInitialize]
    public static void Initialize(TestContext context)
    {
        using StreamWriter writeStream = File.CreateText(@"Load.csv");
        writeStream.WriteLine(@"2022,5,28,8,33,0,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,....,WEBHIST,MSIE Cache File URL record,Expiration Time,-,PC1-5DFC89FB1E0,Location: Visited: PC1@http://www.google.com,Location: Visited: PC1@http://www.google.com Number of hits: 1 Cached file size: 0,2,TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/index.dat,10536,-,msiecf,cache_directory_index:-2; recovered:False; sha256_hash:243645de118fab85ae3e5f4f820ee50717ddf478f17f7a678c88aa5d437a7e70,True,#FFFF0000");
        writeStream.WriteLine(@"2022,5,28,8,33,0,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,....,WEBHIST,MSIE Cache File URL record,Expiration Time,-,PC1-5DFC89FB1E0,Location: :2022050220220503: PC1@http://www.google.com,Location: :2022050220220503: PC1@http://www.google.com Number of hits: 1 Cached file size: 0,2,TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/MSHist012022050220220503/index.dat,10762,-,msiecf,cache_directory_index:-2; recovered:False; sha256_hash:228b35c5c30b9314ab588e2d8a147c24c93c53fd2ae392808456e850d7e83ae3,True,#FFFF0000");
        writeStream.WriteLine(@"2022,5,28,8,33,6,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,....,WEBHIST,MSIE Cache File URL record,Expiration Time,-,PC1-5DFC89FB1E0,Location: Visited: PC1@http://www.google.com/search?hl=lt&source=hp&q=reddit&...,Location: Visited: PC1@http://www.google.com/search?hl=lt&source=hp&q=reddit&iflsig=AJiK0e8AAAAAYm-lUNEs2vr06qDmWQdRc7vnmJPbTNpu&gbv=1 Number of hits: 2 Cached file size: 0,2,TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/index.dat,10536,-,msiecf,cache_directory_index:-2; recovered:False; sha256_hash:243645de118fab85ae3e5f4f820ee50717ddf478f17f7a678c88aa5d437a7e70,True,#FF000000");
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        File.Delete(@"Load.csv");
    }

    [TestMethod]
    public void CanExecute_ShouldReturnFalse_WhenFileNameIsEmptyString()
    {
        // Arrange
        bool expected = false;
        _loadWorkViewModel.FileName = string.Empty;

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
        _loadWorkViewModel.FileName = null;

        // Act
        bool actual = _command.CanExecute(null);

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void CanExecute_ShouldReturnTrue_WhenFileNameIsNonEmptyString()
    {
        // Arrange
        bool expected = true;
        _loadWorkViewModel.FileName = "Test";

        // Act
        bool actual = _command.CanExecute(null);

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(actual);
    }

    //[TestMethod]
    //public async Task ExecuteAsync()
    //{
    //    // Arrange
    //    List<EventViewModel> expected = new()
    //    {
    //        new EventViewModel(new EventModel(
    //            new DateOnly(2022, 5, 28),
    //            new TimeOnly(8, 33, 0),
    //            TimeZoneInfo.Utc,
    //            "....",
    //            "WEBHIST",
    //            "MSIE Cache File URL record",
    //            "Expiration Time",
    //            "-",
    //            "PC1-5DFC89FB1E0",
    //            "Location: Visited: PC1@http://www.google.com",
    //            "Location: Visited: PC1@http://www.google.com Number of hits: 1 Cached file size: 0",
    //            2,
    //            "TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/index.dat",
    //            "10536",
    //            "-",
    //            "msiecf",
    //            new Dictionary<string, string>() { { "cache_directory_index", "-2" }, { "recovered", "False" }, { "sha256_hash", "243645de118fab85ae3e5f4f820ee50717ddf478f17f7a678c88aa5d437a7e70" } })) { IsVisible = true, Colour = Brushes.Red },
    //        new EventViewModel(new EventModel(
    //            new DateOnly(2022, 5, 28),
    //            new TimeOnly(8, 33, 0),
    //            TimeZoneInfo.Utc,
    //            "....",
    //            "WEBHIST",
    //            "MSIE Cache File URL record",
    //            "Expiration Time",
    //            "-",
    //            "PC1-5DFC89FB1E0",
    //            "Location: :2022050220220503: PC1@http://www.google.com",
    //            "Location: :2022050220220503: PC1@http://www.google.com Number of hits: 1 Cached file size: 0",
    //            2,
    //            "TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/MSHist012022050220220503/index.dat",
    //            "10762",
    //            "-",
    //            "msiecf",
    //            new Dictionary<string, string>() { { "cache_directory_index", "-2" }, { "recovered", "False" }, { "sha256_hash", "243645de118fab85ae3e5f4f820ee50717ddf478f17f7a678c88aa5d437a7e70" } })) { IsVisible = true, Colour = Brushes.Red },
    //        new EventViewModel(new EventModel(
    //            new DateOnly(2022, 5, 28),
    //            new TimeOnly(8, 33, 6),
    //            TimeZoneInfo.Utc,
    //            "....",
    //            "WEBHIST",
    //            "MSIE Cache File URL record",
    //            "Expiration Time",
    //            "-",
    //            "PC1-5DFC89FB1E0",
    //            "Location: Visited: PC1@http://www.google.com/search?hl=lt&source=hp&q=reddit&...",
    //            "Location: Visited: PC1@http://www.google.com/search?hl=lt&source=hp&q=reddit&iflsig=AJiK0e8AAAAAYm-lUNEs2vr06qDmWQdRc7vnmJPbTNpu&gbv=1 Number of hits: 2 Cached file size: 0",
    //            2,
    //            "TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/index.dat",
    //            "10536",
    //            "-",
    //            "msiecf",
    //            new Dictionary<string, string>() { { "cache_directory_index", "-2" }, { "recovered", "False" }, { "sha256_hash", "228b35c5c30b9314ab588e2d8a147c24c93c53fd2ae392808456e850d7e83ae3" } })) { IsVisible = true, Colour = Brushes.Black }
    //    };
    //    _loadWorkViewModel.FileName = @"Load.csv";

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
