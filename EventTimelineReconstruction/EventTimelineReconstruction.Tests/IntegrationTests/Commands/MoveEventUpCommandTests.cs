using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Tests.IntegrationTests.Commands;

[TestClass]
public class MoveEventUpCommandTests
{
    private readonly EventDetailsViewModel _eventDetailsViewModel;
    private readonly EventTreeViewModel _eventTreeViewModel;
    private readonly MoveEventUpCommand _command;

    public MoveEventUpCommandTests()
    {
        IDragDropUtils dragDropUtils = new DragDropUtils();
        IFilteringStore filteringStore = new FilteringStore();
        _eventDetailsViewModel = new();
        ChangeColourViewModel changeColourViewModel = new(_eventDetailsViewModel);
        _eventTreeViewModel = new(_eventDetailsViewModel, filteringStore, changeColourViewModel, dragDropUtils);

        _command = new(_eventTreeViewModel, _eventDetailsViewModel);
    }

    private static IEnumerable<object[]> Data
    {
        get
        {
            EventModel parent = new(new DateOnly(2012, 5, 4), new TimeOnly(15, 21), TimeZoneInfo.Local, "MACB1", "Source1", "Source Type1", "Type1", "Username1", "Hostname1", "Short Description1", "Full Description1", 2.5, "Filename1", "iNode number1", "Notes1", "Format1", new Dictionary<string, string>() { { "Key1", "Value1" } }, 1);
            EventModel child1 = new(new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB2", "Source2", "Source Type2", "Type2", "Username2", "Hostname2", "Short Description2", "Full Description2", 2.5, "Filename2", "iNode number2", "Notes2", "Format2", new Dictionary<string, string>() { { "Key2", "Value2" } }, 2);
            EventModel child2 = new(new DateOnly(2022, 12, 22), new TimeOnly(21, 3), TimeZoneInfo.Local, "MACB3", "Source3", "Source Type3", "Type3", "Username3", "Hostname3", "Short Description3", "Full Description3", 2.5, "Filename3", "iNode number3", "Notes3", "Format3", new Dictionary<string, string>() { { "Key3", "Value3" } }, 3);
            EventModel child3 = new(new DateOnly(2022, 12, 25), new TimeOnly(21, 3), TimeZoneInfo.Local, "MACB4", "Source4", "Source Type4", "Type3", "Username4", "Hostname4", "Short Description4", "Full Description4", 2.5, "Filename4", "iNode number4", "Notes4", "Format4", new Dictionary<string, string>() { { "Key4", "Value4" } }, 4);
            EventModel child4 = new(new DateOnly(2022, 12, 30), new TimeOnly(23, 13), TimeZoneInfo.Local, "MACB5", "Source5", "Source Type5", "Type5", "Username5", "Hostname5", "Short Description5", "Full Description5", 2.5, "Filename5", "iNode number5", "Notes5", "Format5", new Dictionary<string, string>() { { "Key5", "Value5" } }, 5);

            EventViewModel firstParent = new(parent);
            EventViewModel firstChild1 = new(child1);
            EventViewModel firstChild2 = new(child2);
            EventViewModel firstChild3 = new(child3);
            EventViewModel firstChild4 = new(child4);
            firstChild3.AddChild(firstChild4);
            firstChild2.AddChild(firstChild3);
            firstChild1.AddChild(firstChild2);
            firstParent.AddChild(firstChild1);

            EventViewModel secondParent = new(parent);
            EventViewModel secondChild1 = new(child1);
            secondParent.AddChild(secondChild1);

            return new[]
            {
                new object[]
                {
                        new List<EventViewModel>()
                        {
                            firstParent
                        },
                        firstChild4,
                        1,
                        1,
                        1,
                        2
                },
                new object[]
                {
                        new List<EventViewModel>()
                        {
                            secondParent
                        },
                        secondChild1,
                        2,
                        0,
                        0,
                        0
                }
            };
        }
    }

    [TestMethod]
    public void CanExecute_ShouldReturnFalse_WhenThereAreNoEvents()
    {
        // Arrange
        bool expected = false;
        _eventDetailsViewModel.SelectedEvent = new(new EventModel(new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "Source", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), 1));

        // Act
        bool actual = _command.CanExecute(null);

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void CanExecute_ShouldReturnFalse_WhenNoEventIsSelected()
    {
        // Arrange
        bool expected = false;
        EventViewModel eventViewModel = new(new EventModel(new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "Source", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), 1));
        _eventTreeViewModel.AddEvent(eventViewModel);
        _eventDetailsViewModel.SelectedEvent = null;

        // Act
        bool actual = _command.CanExecute(null);

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void CanExecute_ShouldReturnTrue_WhenThereAreEventsAndEventIsSelected()
    {
        // Arrange
        bool expected = true;
        EventViewModel eventViewModel = new(new EventModel(new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "Source", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), 1));
        _eventTreeViewModel.AddEvent(eventViewModel);
        _eventDetailsViewModel.SelectedEvent = eventViewModel;

        // Act
        bool actual = _command.CanExecute(null);

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(actual);
    }

    [TestMethod]
    [DynamicData(nameof(Data))]
    public void Execute_ShouldMoveEventOneLevelUp_WhenCommandIsExecuted(List<EventViewModel> events, EventViewModel selectedEvent, int expectedFirstLevelCount, int expectedSecondLevelCount, int expectedThirdLevelCount, int expectedFourthLevelCount)
    {
        // Arrange
        _eventTreeViewModel.LoadEvents(events);
        _eventDetailsViewModel.SelectedEvent = selectedEvent;

        // Act
        _command.Execute(null);
        List<EventViewModel> actual = _eventTreeViewModel.Events.ToList();

        // Assert
        Assert.AreEqual(expectedFirstLevelCount, actual.Count);

        for (int i = 0; i < actual.Count; i++)
        {
            Assert.AreEqual(expectedSecondLevelCount, actual[i].Children.Count);

            for (int j = 0; j < actual[i].Children.Count; j++)
            {
                Assert.AreEqual(expectedThirdLevelCount, actual[i].Children[j].Children.Count);

                for (int k = 0; k < actual[i].Children[j].Children.Count; k++)
                {
                    Assert.AreEqual(expectedFourthLevelCount, actual[i].Children[j].Children[k].Children.Count);
                }
            }
        }
    }
}
