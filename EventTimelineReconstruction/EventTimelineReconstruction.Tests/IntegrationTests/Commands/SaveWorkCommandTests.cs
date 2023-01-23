using System.Windows.Media;
using EventTimelineReconstruction.Abstractors;
using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Tests.IntegrationTests.Commands;

[TestClass]
public class SaveWorkCommandTests
{
    private readonly SaveWorkViewModel _saveWorkViewModel;
    private readonly EventTreeViewModel _eventTreeViewModel;
    private readonly AbstractedEventsViewModel _abstractedEventsViewModel;
    private readonly IWorkSaver _workSaver;
    private readonly SaveWorkCommand _command;

    public SaveWorkCommandTests()
    {
        IDragDropUtils dragDropUtils = new DragDropUtils();
        IFilteringStore filteringStore = new FilteringStore();
        IHighLevelEventsAbstractorUtils highLevelEventsAbstractorUtils = new HighLevelEventsAbstractorUtils();
        ILowLevelEventsAbstractorUtils lowLevelEventsAbstractorUtils = new LowLevelEventsAbstractorUtils();
        IHighLevelArtefactsAbstractorUtils highLevelArtefactsAbstractorUtils = new HighLevelArtefactsAbstractorUtils();
        ILowLevelArtefactsAbstractorUtils lowLevelArtefactsAbstractorUtils = new LowLevelArtefactsAbstractorUtils();
        IHighLevelEventsAbstractor highLevelEventsAbstractor = new HighLevelEventsAbstractor(highLevelEventsAbstractorUtils);
        ILowLevelEventsAbstractor lowLevelEventsAbstractor = new LowLevelEventsAbstractor(highLevelEventsAbstractorUtils, lowLevelEventsAbstractorUtils);
        IHighLevelArtefactsAbstractor highLevelArtefactsAbstractor = new HighLevelArtefactsAbstractor(highLevelEventsAbstractorUtils, lowLevelEventsAbstractorUtils, highLevelArtefactsAbstractorUtils);
        ILowLevelArtefactsAbstractor lowLevelArtefactsAbstractor = new LowLevelArtefactsAbstractor(lowLevelArtefactsAbstractorUtils);
        IEventsImporter eventsImporter = new L2tCSVEventsImporter();
        IEventsStore _eventsStore = new EventsStore(eventsImporter);
        IErrorsViewModel errorsViewModel = new ErrorsViewModel();
        EventDetailsViewModel eventDetailsViewModel = new();
        ChangeColourViewModel changeColourViewModel = new(eventDetailsViewModel);
        _eventTreeViewModel = new(eventDetailsViewModel, filteringStore, changeColourViewModel, dragDropUtils);
        _workSaver = new FileWorkSaver();
        _abstractedEventsViewModel = new(_eventsStore, highLevelEventsAbstractor, lowLevelEventsAbstractor, highLevelArtefactsAbstractor, lowLevelArtefactsAbstractor, errorsViewModel);
        _saveWorkViewModel = new(_eventTreeViewModel, _abstractedEventsViewModel, _workSaver);

        _command = new(_saveWorkViewModel, _eventTreeViewModel, _abstractedEventsViewModel, _workSaver);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        File.Delete(@"Save.csv");
    }

    [TestMethod]
    public void CanExecute_ShouldReturnFalse_WhenFileNameIsEmptyString()
    {
        // Arrange
        bool expected = false;
        _saveWorkViewModel.FileName = string.Empty;

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
        _saveWorkViewModel.FileName = null;

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
        _saveWorkViewModel.FileName = "Test";

        // Act
        bool actual = _command.CanExecute(null);

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(actual);
    }

    [TestMethod]
    public async Task ExecuteAsync_ShouldSaveEventsToFile_WhenCommandIsExecuted()
    {
        // Arrange
        List<EventViewModel> events = new()
            {
                new EventViewModel(new EventModel(
                    new DateOnly(2022, 5, 28),
                    new TimeOnly(8, 33, 0),
                    TimeZoneInfo.Utc,
                    "....",
                    "WEBHIST",
                    "MSIE Cache File URL record",
                    "Expiration Time",
                    "-",
                    "PC1-5DFC89FB1E0",
                    "Location: Visited: PC1@http://www.google.com",
                    "Location: Visited: PC1@http://www.google.com Number of hits: 1 Cached file size: 0",
                    2,
                    "TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/index.dat",
                    "10536",
                    "-",
                    "msiecf",
                    new Dictionary<string, string>() { { "cache_directory_index", "-2" }, { "recovered", "False" }, { "sha256_hash", "243645de118fab85ae3e5f4f820ee50717ddf478f17f7a678c88aa5d437a7e70" } },
                    1)) { IsVisible = true, Colour = Brushes.Red },
                new EventViewModel(new EventModel(
                    new DateOnly(2022, 5, 28),
                    new TimeOnly(8, 33, 0),
                    TimeZoneInfo.Utc,
                    "....",
                    "WEBHIST",
                    "MSIE Cache File URL record",
                    "Expiration Time",
                    "-",
                    "PC1-5DFC89FB1E0",
                    "Location: :2022050220220503: PC1@http://www.google.com",
                    "Location: :2022050220220503: PC1@http://www.google.com Number of hits: 1 Cached file size: 0",
                    2,
                    "TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/MSHist012022050220220503/index.dat",
                    "10762",
                    "-",
                    "msiecf",
                    new Dictionary<string, string>() { { "cache_directory_index", "-2" }, { "recovered", "False" }, { "sha256_hash", "228b35c5c30b9314ab588e2d8a147c24c93c53fd2ae392808456e850d7e83ae3" } },
                    2)) { IsVisible = true, Colour = Brushes.Red },
                new EventViewModel(new EventModel(
                    new DateOnly(2022, 5, 28),
                    new TimeOnly(8, 33, 6),
                    TimeZoneInfo.Utc,
                    "....",
                    "WEBHIST",
                    "MSIE Cache File URL record",
                    "Expiration Time",
                    "-",
                    "PC1-5DFC89FB1E0",
                    "Location: Visited: PC1@http://www.google.com/search?hl=lt&source=hp&q=reddit&...",
                    "Location: Visited: PC1@http://www.google.com/search?hl=lt&source=hp&q=reddit&iflsig=AJiK0e8AAAAAYm-lUNEs2vr06qDmWQdRc7vnmJPbTNpu&gbv=1 Number of hits: 2 Cached file size: 0",
                    2,
                    "TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/index.dat",
                    "10536",
                    "-",
                    "msiecf",
                    new Dictionary<string, string>() { { "cache_directory_index", "-2" }, { "recovered", "False" }, { "sha256_hash", "243645de118fab85ae3e5f4f820ee50717ddf478f17f7a678c88aa5d437a7e70" } },
                    3)) { IsVisible = true, Colour = Brushes.Black }
            };
        _eventTreeViewModel.LoadEvents(events);
        _saveWorkViewModel.FileName = @"Save.csv";
        string[] expected = new string[]
        {
            @"2022,5,28,8,33,0,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,....,WEBHIST,MSIE Cache File URL record,Expiration Time,-,PC1-5DFC89FB1E0,Location: Visited: PC1@http://www.google.com,Location: Visited: PC1@http://www.google.com Number of hits: 1 Cached file size: 0,2,TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/index.dat,10536,-,msiecf,cache_directory_index:-2;recovered:False;sha256_hash:243645de118fab85ae3e5f4f820ee50717ddf478f17f7a678c88aa5d437a7e70,1,True,#FFFF0000",
            @"2022,5,28,8,33,0,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,....,WEBHIST,MSIE Cache File URL record,Expiration Time,-,PC1-5DFC89FB1E0,Location: :2022050220220503: PC1@http://www.google.com,Location: :2022050220220503: PC1@http://www.google.com Number of hits: 1 Cached file size: 0,2,TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/MSHist012022050220220503/index.dat,10762,-,msiecf,cache_directory_index:-2;recovered:False;sha256_hash:228b35c5c30b9314ab588e2d8a147c24c93c53fd2ae392808456e850d7e83ae3,2,True,#FFFF0000",
            @"2022,5,28,8,33,6,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,....,WEBHIST,MSIE Cache File URL record,Expiration Time,-,PC1-5DFC89FB1E0,Location: Visited: PC1@http://www.google.com/search?hl=lt&source=hp&q=reddit&...,Location: Visited: PC1@http://www.google.com/search?hl=lt&source=hp&q=reddit&iflsig=AJiK0e8AAAAAYm-lUNEs2vr06qDmWQdRc7vnmJPbTNpu&gbv=1 Number of hits: 2 Cached file size: 0,2,TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/index.dat,10536,-,msiecf,cache_directory_index:-2;recovered:False;sha256_hash:243645de118fab85ae3e5f4f820ee50717ddf478f17f7a678c88aa5d437a7e70,3,True,#FF000000",
            "",
            "",
            "",
            ""
        };

        // Act
        await _command.ExecuteAsync(null);
        string[] actual = File.ReadAllLines(@"Save.csv");

        // Assert
        Assert.AreEqual(expected.Length, actual.Length);

        for (int i = 0; i < expected.Length; i++)
        {
            Assert.AreEqual(expected[i], actual[i]);
        }
    }
}
