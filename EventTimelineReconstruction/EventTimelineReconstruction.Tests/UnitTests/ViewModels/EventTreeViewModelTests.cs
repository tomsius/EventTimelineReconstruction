using System.Windows.Controls;
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
public class EventTreeViewModelTests
{
    private readonly Mock<IFilteringStore> _iFilteringStore;
    private readonly EventTreeViewModel _eventTreeViewModel;

    public EventTreeViewModelTests()
    {
        Mock<EventDetailsViewModel> eventDetailsViewModelMock = new();
        _iFilteringStore = new();
        Mock<ChangeColourViewModel> changeColourViewModelMock = new(eventDetailsViewModelMock.Object);
        Mock<IDragDropUtils> iDragDropUtilsMock = new();
        _eventTreeViewModel = new(eventDetailsViewModelMock.Object, _iFilteringStore.Object, changeColourViewModelMock.Object, iDragDropUtilsMock.Object);
    }

    private static IEnumerable<object[]> DifferentObjects
    {
        get
        {
            return new[]
            {
                new object[]
                {
                    new EventViewModel(
                        new EventModel(
                            new DateOnly(2000, 10, 14),
                            new TimeOnly(10, 52),
                            TimeZoneInfo.Local,
                            "MACB",
                            "Source",
                            "Source Type",
                            "Type2",
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
                            1
                        )
                    )
                },
                new object[]
                {
                    new EventViewModel(
                        new EventModel(
                            new DateOnly(2000, 10, 14),
                            new TimeOnly(10, 52),
                            TimeZoneInfo.Local,
                            "MACB",
                            "Source",
                            "KEYWORD",
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
                            2
                        )
                    )
                },
                new object[]
                {
                    new EventViewModel(
                        new EventModel(
                            new DateOnly(2019, 10, 14),
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
                            3
                        )
                    )
                }
            };
        }
    }

    [TestMethod]
    public void Events_ReturnsEmptyList_WhenObjectIsInitialized()
    {
        // Arrange
        List<EventViewModel> expected = new(0);

        // Act
        List<EventViewModel> actual = _eventTreeViewModel.Events.ToList();

        // Assert
        Assert.AreEqual(expected.Count, actual.Count);
    }

    [TestMethod]
    public void EventsView_ReturnsEmptyView_WhenObjectIsInitialized()
    {
        // Arrange
        int expectedCount = 0;

        // Act
        CollectionView actualValue = (CollectionView)_eventTreeViewModel.EventsView;
        int actualCount = actualValue.Count;

        // Assert
        Assert.IsNotNull(actualValue);
        Assert.AreEqual(expectedCount, actualCount);
    }

    [TestMethod]
    public void DraggedItem_ReturnsNull_WhenObjectIsInitialized()
    {
        // Act
        EventViewModel actual = _eventTreeViewModel.DraggedItem;

        // Assert
        Assert.IsNull(actual);
    }

    [TestMethod]
    public void DraggedItem_ReturnsObject_WhenPropertyWasSet()
    {
        // Arrange
        EventViewModel expected = new(new EventModel(
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
                        1));

        // Act
        _eventTreeViewModel.DraggedItem = expected;
        EventViewModel actual = _eventTreeViewModel.DraggedItem;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Target_ReturnsNull_WhenObjectIsInitialized()
    {
        // Act
        EventViewModel actual = _eventTreeViewModel.Target;

        // Assert
        Assert.IsNull(actual);
    }

    [TestMethod]
    public void Target_ReturnsObject_WhenPropertyWasSet()
    {
        // Arrange
        EventViewModel expected = new(new EventModel(
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
                        1));

        // Act
        _eventTreeViewModel.Target = expected;
        EventViewModel actual = _eventTreeViewModel.Target;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void DraggedItemElement_ReturnsNull_WhenObjectIsInitialized()
    {
        // Act
        TreeViewItem actual = _eventTreeViewModel.DraggedItemElement;

        // Assert
        Assert.IsNull(actual);
    }

    [STATestMethod]
    public void DraggedItemElement_ReturnsObject_WhenPropertyWasSet()
    {
        // Arrange
        TreeViewItem expected = new() { Width = 10, Height = 10, AllowDrop = true };

        // Act
        _eventTreeViewModel.DraggedItemElement = expected;
        TreeViewItem actual = _eventTreeViewModel.DraggedItemElement;

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.AreEqual(expected.Width, actual.Width);
        Assert.AreEqual(expected.Height, actual.Height);
        Assert.AreEqual(expected.AllowDrop, actual.AllowDrop);
    }

    [TestMethod]
    public void MyAdornment_ReturnsNull_WhenObjectIsInitialized()
    {
        // Act
        DraggableAdorner actual = _eventTreeViewModel.MyAdornment;

        // Assert
        Assert.IsNull(actual);
    }

    [STATestMethod]
    public void MyAdornment_ReturnsObject_WhenPropertyWasSet()
    {
        // Arrange
        EventViewModel eventViewModel = new(new EventModel(
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
                        1));
        TreeViewItem uiElement = new() { Header = eventViewModel };
        DraggableAdorner expected = new(uiElement, uiElement);

        // Act
        _eventTreeViewModel.MyAdornment = expected;
        DraggableAdorner actual = _eventTreeViewModel.MyAdornment;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ShowDetailsCommand_ShouldReturnCommand_WhenObjectIsInitialized()
    {
        // Arrange
        Type expected = typeof(ShowEventDetailsCommand);

        // Act
        ICommand command = _eventTreeViewModel.ShowDetailsCommand;
        Type actual = command.GetType();

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(command is ShowEventDetailsCommand);
    }

    [TestMethod]
    public void DragOverCommand_ShouldReturnCommand_WhenObjectIsInitialized()
    {
        // Arrange
        Type expected = typeof(DragOverEventCommand);

        // Act
        ICommand command = _eventTreeViewModel.DragOverCommand;
        Type actual = command.GetType();

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(command is DragOverEventCommand);
    }

    [TestMethod]
    public void DropCommand_ShouldReturnCommand_WhenObjectIsInitialized()
    {
        // Arrange
        Type expected = typeof(DropEventCommand);

        // Act
        ICommand command = _eventTreeViewModel.DropCommand;
        Type actual = command.GetType();

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(command is DropEventCommand);
    }

    [TestMethod]
    public void MouseMoveCommand_ShouldReturnCommand_WhenObjectIsInitialized()
    {
        // Arrange
        Type expected = typeof(MouseMoveEventCommand);

        // Act
        ICommand command = _eventTreeViewModel.MouseMoveCommand;
        Type actual = command.GetType();

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(command is MouseMoveEventCommand);
    }

    [TestMethod]
    public void GiveFeedbackCommand_ShouldReturnCommand_WhenObjectIsInitialized()
    {
        // Arrange
        Type expected = typeof(GiveEventFeedbackCommand);

        // Act
        ICommand command = _eventTreeViewModel.GiveFeedbackCommand;
        Type actual = command.GetType();

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(command is GiveEventFeedbackCommand);
    }

    [TestMethod]
    public void AddEvent_ShouldAddEvent_WhenMethodIsCalled()
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
                        1)
                );
        int expectedCount = 1;

        // Act
        _eventTreeViewModel.AddEvent(expected);
        List<EventViewModel> actualValue = _eventTreeViewModel.Events.ToList();
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
    public void RemoveEvent_ShouldRemoveEvent_WhenGiveEventIExists()
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
                        1)
                );
        _eventTreeViewModel.AddEvent(eventViewModel);
        int expected = 0;

        // Act
        _eventTreeViewModel.RemoveEvent(eventViewModel);
        int actual = _eventTreeViewModel.Events.Count();

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void RemoveEvent_ShouldDoNothing_WhenGivenEventDoesNotExist()
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
                        1)
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
                        2)
                );
        _eventTreeViewModel.AddEvent(originalViewModel);
        int expected = 1;

        // Act
        _eventTreeViewModel.RemoveEvent(otherViewModel);
        int actual = _eventTreeViewModel.Events.Count();

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ApplyFilters_ShouldReturnVisibleElements_WhenOtherFiltersAreDisabled()
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
                        1)
                )
        {
            IsVisible = true
        };
        EventViewModel hiddenEventViewModel = new(
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
                        2)
                )
        {
            IsVisible = false
        };
        _eventTreeViewModel.AddEvent(expected);
        _eventTreeViewModel.AddEvent(hiddenEventViewModel);
        _eventTreeViewModel.AddEvent(null);
        _iFilteringStore.Setup(mock => mock.IsEnabled).Returns(false);
        int expectedCount = 1;

        // Act
        _eventTreeViewModel.ApplyFilters();
        List<EventViewModel> actualValue = _eventTreeViewModel.EventsView.Cast<EventViewModel>().ToList();
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

    [DataTestMethod]
    [DataRow("source type3")]
    [DataRow("rname3")]
    [DataRow("tname3")]
    [DataRow("full description3")]
    [DataRow("ename3")]
    [DataRow("notes3")]
    [DataRow("y123")]
    [DataRow("value213")]
    public void ApplyFilters_ShouldReturnVisibleFilteredElements_WhenChosenEventTypeAndKeywordAndDateIntervalMatch(string keyword)
    {
        // Arrange
        EventViewModel firstEventViewModel = new(
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
                        1)
                )
        {
            IsVisible = true
        };
        EventViewModel hiddenEventViewModel = new(
                new EventModel(
                        new DateOnly(2003, 1, 5),
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
                        2)
                )
        {
            IsVisible = false
        };
        EventViewModel otherHiddenEventViewModel = new(
                new EventModel(
                        new DateOnly(2003, 1, 5),
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
                        3)
                )
        {
            IsVisible = false
        };
        EventViewModel expected = new(
                new EventModel(
                        new DateOnly(2019, 3, 20),
                        new TimeOnly(20, 35),
                        TimeZoneInfo.Local,
                        "MACB3",
                        "Source3",
                        "Source Type3",
                        "Type3",
                        "Username3",
                        "Hostname3",
                        "Short Description3",
                        "Full Description3",
                        3.8,
                        "Filename3",
                        "iNode number3",
                        "Notes3",
                        "Format3",
                        new Dictionary<string, string>() { { "Key123", "Value123" }, { "Key213", "Value213" } },
                        4)
                )
        {
            IsVisible = true
        };
        hiddenEventViewModel.Children.Add(otherHiddenEventViewModel);
        firstEventViewModel.Children.Add(hiddenEventViewModel);
        _eventTreeViewModel.AddEvent(firstEventViewModel);
        _eventTreeViewModel.AddEvent(expected);
        _eventTreeViewModel.AddEvent(null);
        _iFilteringStore.Setup(mock => mock.IsEnabled).Returns(true);
        _iFilteringStore.Setup(mock => mock.AreAllFiltersApplied).Returns(true);
        _iFilteringStore.Setup(mock => mock.ChosenEventTypes).Returns(new Dictionary<string, bool>() { { "Type", false }, { "Type2", true }, { "Type3", true } });
        _iFilteringStore.Setup(mock => mock.Keyword).Returns(keyword);
        _iFilteringStore.Setup(mock => mock.FromDate).Returns(new DateTime(2010, 1, 1));
        _iFilteringStore.Setup(mock => mock.ToDate).Returns(new DateTime(2023, 1, 1));

        int expectedCount = 1;

        // Act
        _eventTreeViewModel.ApplyFilters();
        List<EventViewModel> actualValue = _eventTreeViewModel.EventsView.Cast<EventViewModel>().ToList();
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
    [DynamicData(nameof(DifferentObjects))]
    public void ApplyFilters_ShouldReturnVisibleFilteredElements_WhenChosenEventTypeOrKeywordOrDateIntervalMatch(EventViewModel expected)
    {
        // Arrange
        _eventTreeViewModel.AddEvent(expected);
        _iFilteringStore.Setup(mock => mock.IsEnabled).Returns(true);
        _iFilteringStore.Setup(mock => mock.AreAllFiltersApplied).Returns(false);
        _iFilteringStore.Setup(mock => mock.ChosenEventTypes).Returns(new Dictionary<string, bool>() { { "Type", false }, { "Type2", true } });
        _iFilteringStore.Setup(mock => mock.Keyword).Returns("keyword");
        _iFilteringStore.Setup(mock => mock.FromDate).Returns(new DateTime(2010, 1, 1));
        _iFilteringStore.Setup(mock => mock.ToDate).Returns(new DateTime(2023, 1, 1));

        int expectedCount = 1;

        // Act
        _eventTreeViewModel.ApplyFilters();
        List<EventViewModel> actualValue = _eventTreeViewModel.EventsView.Cast<EventViewModel>().ToList();
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
}
