using System.ComponentModel;
using System.Windows.Data;
using EventTimelineReconstruction.Commands;
using System.Windows.Input;
using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;
using Moq;

namespace EventTimelineReconstruction.Tests.UnitTests.ViewModels;

[TestClass]
public class HiddenEventsViewModelTests
{
    private readonly Mock<IEventsStore> _eventsStoreMock;
    private readonly Mock<EventTreeViewModel> _eventTreeViewModelMock;
    private readonly HiddenEventsViewModel _hiddenEventsViewModel;

    public HiddenEventsViewModelTests()
    {
        Mock<EventDetailsViewModel> eventDetailsViewModelMock = new();
        Mock<IFilteringStore> iFilteringStoreMock = new();
        Mock<ChangeColourViewModel> changeColourViewModelMock = new(eventDetailsViewModelMock.Object);
        Mock<IDragDropUtils> iDragDropUtilsMock = new();
        _eventTreeViewModelMock = new(eventDetailsViewModelMock.Object, iFilteringStoreMock.Object, changeColourViewModelMock.Object, iDragDropUtilsMock.Object);
        _eventsStoreMock = new();
        _hiddenEventsViewModel = new(_eventsStoreMock.Object, _eventTreeViewModelMock.Object);
    }

    [TestMethod]
    public void HiddenEvents_ReturnsEmptyList_WhenThereAreNoHiddenEventsInStore()
    {
        // Arrange
        List<EventViewModel> expected = new(0);
        _eventsStoreMock.Setup(s => s.GetAllHiddenEvents()).Returns(expected);

        // Act
        _hiddenEventsViewModel.Initialize(null, new PropertyChangedEventArgs(nameof(EventTreeViewModel.Events)));
        List<EventViewModel> actual = _hiddenEventsViewModel.HiddenEvents.ToList();

        // Assert
        Assert.AreEqual(expected.Count, actual.Count);
    }

    [TestMethod]
    public void HiddenEvents_ReturnsHiddenEvents_WhenThereAreHiddenEventsInStore()
    {
        // Arrange
        List<EventViewModel> expected = new()
        {
            new EventViewModel(
                new EventModel(
                        new DateOnly(2022, 10, 14),
                        new TimeOnly(10, 52),
                        TimeZoneInfo.Local,
                        "MACB",
                        "Source",
                        "Source Type",
                        "Type",
                        "Username",
                        "Hostname",
                        "Short Description",
                        "Full Description",
                        2.5,
                        "Filename",
                        "iNode number",
                        "Notes",
                        "Format",
                        new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                        "1")
                ),
            new EventViewModel(
                new EventModel(
                        new DateOnly(2018, 3, 4),
                        new TimeOnly(22, 11),
                        TimeZoneInfo.Local,
                        "MACB2",
                        "Source3",
                        "Source Type4",
                        "Type5",
                        "Username2",
                        "Hostname4",
                        "Short Description8",
                        "Full Description7",
                        7.4,
                        "Filename7",
                        "iNode number4",
                        "Notes1",
                        "Format2",
                        new Dictionary<string, string>() { { "Key12", "Value12" }, { "Key25", "Value25" } },
                        "2"))

        };
        _eventsStoreMock.Setup(store => store.GetAllHiddenEvents()).Returns(expected);

        // Act
        _hiddenEventsViewModel.Initialize(null, new PropertyChangedEventArgs(nameof(EventTreeViewModel.Events)));
        List<EventViewModel> actual = _hiddenEventsViewModel.HiddenEvents.ToList();

        // Assert
        Assert.AreEqual(expected.Count, actual.Count);

        for (int i = 0; i < expected.Count; i++)
        {
            Assert.AreEqual(expected[i].Children.Count, actual[i].Children.Count);
            Assert.AreEqual(expected[i].FullDate, actual[i].FullDate);
            Assert.AreEqual(expected[i].Timezone, actual[i].Timezone);
            Assert.AreEqual(expected[i].MACB, actual[i].MACB);
            Assert.AreEqual(expected[i].Source, actual[i].Source);
            Assert.AreEqual(expected[i].SourceType, actual[i].SourceType);
            Assert.AreEqual(expected[i].Type, actual[i].Type);
            Assert.AreEqual(expected[i].User, actual[i].User);
            Assert.AreEqual(expected[i].Host, actual[i].Host);
            Assert.AreEqual(expected[i].Short, actual[i].Short);
            Assert.AreEqual(expected[i].Description, actual[i].Description);
            Assert.AreEqual(expected[i].Version, actual[i].Version);
            Assert.AreEqual(expected[i].Filename, actual[i].Filename);
            Assert.AreEqual(expected[i].INode, actual[i].INode);
            Assert.AreEqual(expected[i].Notes, actual[i].Notes);
            Assert.AreEqual(expected[i].Format, actual[i].Format);
            Assert.AreEqual(expected[i].Extra.Count, actual[i].Extra.Count);

            foreach (KeyValuePair<string, string> kvp in expected[i].Extra)
            {
                string expectedKey = kvp.Key;
                string expectedValue = kvp.Value;

                Assert.IsTrue(actual[i].Extra.ContainsKey(expectedKey));
                Assert.AreEqual(expectedValue, actual[i].Extra[expectedKey]);
            }

            Assert.AreEqual(expected[i].IsVisible, actual[i].IsVisible);
            Assert.AreEqual(expected[i].Colour.ToString(), actual[i].Colour.ToString());
            Assert.AreEqual(expected[i].SourceLine, actual[i].SourceLine);
        }
    }

    [TestMethod]
    public void HiddenEventsView_ReturnsEmptyView_WhenObjectIsInitialized()
    {
        // Arrange
        int expectedCount = 0;

        // Act
        CollectionView actualValue = _hiddenEventsViewModel.HiddenEventsView;
        int actualCount = actualValue.Count;

        // Assert
        Assert.AreEqual(expectedCount, actualCount);
        Assert.IsNotNull(actualValue);
    }

    [TestMethod]
    public void HiddenEventsView_ReturnsNonEmptyView_WhenThereAreHiddenEvents()
    {
        // Arrange
        List<EventViewModel> expected = new()
        {
            new EventViewModel(
                new EventModel(
                        new DateOnly(2018, 10, 14),
                        new TimeOnly(10, 52),
                        TimeZoneInfo.Local,
                        "MACB",
                        "Source",
                        "Source Type",
                        "Type",
                        "Username",
                        "Hostname",
                        "Short Description",
                        "Full Description",
                        2.5,
                        "Filename",
                        "iNode number",
                        "Notes",
                        "Format",
                        new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                        "1")
                ),
            new EventViewModel(
                new EventModel(
                        new DateOnly(2022, 3, 4),
                        new TimeOnly(22, 11),
                        TimeZoneInfo.Local,
                        "MACB2",
                        "Source3",
                        "Source Type4",
                        "Type5",
                        "Username2",
                        "Hostname4",
                        "Short Description8",
                        "Full Description7",
                        7.4,
                        "Filename7",
                        "iNode number4",
                        "Notes1",
                        "Format2",
                        new Dictionary<string, string>() { { "Key12", "Value12" }, { "Key25", "Value25" } },
                        "2"))

        };
        _eventsStoreMock.Setup(store => store.GetAllHiddenEvents()).Returns(expected);
        _hiddenEventsViewModel.Initialize(null, new PropertyChangedEventArgs(nameof(EventTreeViewModel.Events)));
        int expectedCount = 2;

        // Act
        CollectionView actualValue = _hiddenEventsViewModel.HiddenEventsView;
        int actualCount = actualValue.Count;

        // Assert
        List<EventViewModel> actual = actualValue.Cast<EventViewModel>().ToList();
        Assert.IsNotNull(actualValue);
        Assert.AreEqual(expectedCount, actualValue.Count);

        for (int i = 0; i < expected.Count; i++)
        {
            Assert.AreEqual(expected[i].Children.Count, actual[i].Children.Count);
            Assert.AreEqual(expected[i].FullDate, actual[i].FullDate);
            Assert.AreEqual(expected[i].Timezone, actual[i].Timezone);
            Assert.AreEqual(expected[i].MACB, actual[i].MACB);
            Assert.AreEqual(expected[i].Source, actual[i].Source);
            Assert.AreEqual(expected[i].SourceType, actual[i].SourceType);
            Assert.AreEqual(expected[i].Type, actual[i].Type);
            Assert.AreEqual(expected[i].User, actual[i].User);
            Assert.AreEqual(expected[i].Host, actual[i].Host);
            Assert.AreEqual(expected[i].Short, actual[i].Short);
            Assert.AreEqual(expected[i].Description, actual[i].Description);
            Assert.AreEqual(expected[i].Version, actual[i].Version);
            Assert.AreEqual(expected[i].Filename, actual[i].Filename);
            Assert.AreEqual(expected[i].INode, actual[i].INode);
            Assert.AreEqual(expected[i].Notes, actual[i].Notes);
            Assert.AreEqual(expected[i].Format, actual[i].Format);
            Assert.AreEqual(expected[i].Extra.Count, actual[i].Extra.Count);

            foreach (KeyValuePair<string, string> kvp in expected[i].Extra)
            {
                string expectedKey = kvp.Key;
                string expectedValue = kvp.Value;

                Assert.IsTrue(actual[i].Extra.ContainsKey(expectedKey));
                Assert.AreEqual(expectedValue, actual[i].Extra[expectedKey]);
            }

            Assert.AreEqual(expected[i].IsVisible, actual[i].IsVisible);
            Assert.AreEqual(expected[i].Colour.ToString(), actual[i].Colour.ToString());
            Assert.AreEqual(expected[i].SourceLine, actual[i].SourceLine);
        }
    }

    [TestMethod]
    public void SelectedHiddenEvent_ShouldReturnNull_WhenObjectIsInitialized()
    {
        // Arrange
        EventViewModel? expected = null;

        // Act
        EventViewModel actual = _hiddenEventsViewModel.SelectedHiddenEvent;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void SelectedHiddenEvent_ShouldReturnObject_WhenPropertyWasSet()
    {
        // Arrange
        EventViewModel expected = new(
                new EventModel(
                        new DateOnly(2022, 10, 14),
                        new TimeOnly(10, 52),
                        TimeZoneInfo.Local,
                        "MACB",
                        "Source",
                        "Source Type",
                        "Type",
                        "Username",
                        "Hostname",
                        "Short Description",
                        "Full Description",
                        2.5,
                        "Filename",
                        "iNode number",
                        "Notes",
                        "Format",
                        new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                        "1")
                );

        // Act
        _hiddenEventsViewModel.SelectedHiddenEvent = expected;
        EventViewModel actual = _hiddenEventsViewModel.SelectedHiddenEvent;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void UnhideCommand_ShouldReturnCommand_WhenObjectIsInitialized()
    {
        // Arrange
        Type expected = typeof(UnhideEventCommand);

        // Act
        ICommand command = _hiddenEventsViewModel.UnhideCommand;
        Type actual = command.GetType();

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(command is UnhideEventCommand);
    }

    [TestMethod]
    public void AddHiddenEvent_ShouldAddEvent_WhenMethodIsCalled()
    {
        // Arrange
        EventViewModel expected = new(
                new EventModel(
                        new DateOnly(2022, 10, 14),
                        new TimeOnly(10, 52),
                        TimeZoneInfo.Local,
                        "MACB",
                        "Source",
                        "Source Type",
                        "Type",
                        "Username",
                        "Hostname",
                        "Short Description",
                        "Full Description",
                        2.5,
                        "Filename",
                        "iNode number",
                        "Notes",
                        "Format",
                        new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                        "1")
                );
        int expectedCount = 1;

        // Act
        _hiddenEventsViewModel.AddHiddenEvent(expected);
        List<EventViewModel> actualValue = _hiddenEventsViewModel.HiddenEvents.ToList();
        int actualCount = actualValue.Count;

        // Assert
        Assert.AreEqual(expectedCount, actualCount);

        Assert.AreEqual(expected.Children.Count, actualValue[0].Children.Count);
        Assert.AreEqual(expected.FullDate, actualValue[0].FullDate);
        Assert.AreEqual(expected.Timezone, actualValue[0].Timezone);
        Assert.AreEqual(expected.MACB, actualValue[0].MACB);
        Assert.AreEqual(expected.Source, actualValue[0].Source);
        Assert.AreEqual(expected.SourceType, actualValue[0].SourceType);
        Assert.AreEqual(expected.Type, actualValue[0].Type);
        Assert.AreEqual(expected.User, actualValue[0].User);
        Assert.AreEqual(expected.Host, actualValue[0].Host);
        Assert.AreEqual(expected.Short, actualValue[0].Short);
        Assert.AreEqual(expected.Description, actualValue[0].Description);
        Assert.AreEqual(expected.Version, actualValue[0].Version);
        Assert.AreEqual(expected.Filename, actualValue[0].Filename);
        Assert.AreEqual(expected.INode, actualValue[0].INode);
        Assert.AreEqual(expected.Notes, actualValue[0].Notes);
        Assert.AreEqual(expected.Format, actualValue[0].Format);
        Assert.AreEqual(expected.Extra.Count, actualValue[0].Extra.Count);

        foreach (KeyValuePair<string, string> kvp in expected.Extra)
        {
            string expectedKey = kvp.Key;
            string expectedValue = kvp.Value;

            Assert.IsTrue(actualValue[0].Extra.ContainsKey(expectedKey));
            Assert.AreEqual(expectedValue, actualValue[0].Extra[expectedKey]);
        }

        Assert.AreEqual(expected.IsVisible, actualValue[0].IsVisible);
        Assert.AreEqual(expected.Colour.ToString(), actualValue[0].Colour.ToString());
        Assert.AreEqual(expected.SourceLine, actualValue[0].SourceLine);
    }

    [TestMethod]
    public void RemoveHiddenEvent_ShouldRemoveEvent_WhenGiveEventIExists()
    {
        // Arrange
        EventViewModel eventViewModel = new(
                new EventModel(
                        new DateOnly(2022, 10, 14),
                        new TimeOnly(10, 52),
                        TimeZoneInfo.Local,
                        "MACB",
                        "Source",
                        "Source Type",
                        "Type",
                        "Username",
                        "Hostname",
                        "Short Description",
                        "Full Description",
                        2.5,
                        "Filename",
                        "iNode number",
                        "Notes",
                        "Format",
                        new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                        "1")
                );
        _hiddenEventsViewModel.AddHiddenEvent(eventViewModel);
        int expected = 0;

        // Act
        _hiddenEventsViewModel.RemoveHiddenEvent(eventViewModel);
        int actual = _hiddenEventsViewModel.HiddenEvents.Count();

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void RemoveHiddenEvent_ShouldDoNothing_WhenGivenEventDoesNotExist()
    {
        // Arrange
        EventViewModel originalViewModel = new(
                new EventModel(
                        new DateOnly(2022, 10, 14),
                        new TimeOnly(10, 52),
                        TimeZoneInfo.Local,
                        "MACB",
                        "Source",
                        "Source Type",
                        "Type",
                        "Username",
                        "Hostname",
                        "Short Description",
                        "Full Description",
                        2.5,
                        "Filename",
                        "iNode number",
                        "Notes",
                        "Format",
                        new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                        "1")
                );
        EventViewModel otherViewModel = new(
                new EventModel(
                        new DateOnly(2018, 1, 5),
                        new TimeOnly(15, 14),
                        TimeZoneInfo.Local,
                        "MACB2",
                        "Source2",
                        "Source Type2",
                        "Type2",
                        "Username2",
                        "Hostname2",
                        "Short Description2",
                        "Full Description2",
                        4.5,
                        "Filename2",
                        "iNode number2",
                        "Notes2",
                        "Format2",
                        new Dictionary<string, string>() { { "Key12", "Value12" }, { "Key21", "Value21" } },
                        "2")
                );
        _hiddenEventsViewModel.AddHiddenEvent(originalViewModel);
        int expected = 1;

        // Act
        _hiddenEventsViewModel.RemoveHiddenEvent(otherViewModel);
        int actual = _hiddenEventsViewModel.HiddenEvents.Count();

        // Assert
        Assert.AreEqual(expected, actual);
    }
}
