using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Tests.IntegrationTests.Commands;

[TestClass]
public class HideEventCommandTests
{
    private readonly EventTreeViewModel _eventTreeViewModel;
    private readonly EventDetailsViewModel _eventDetailsViewModel;
    private readonly HiddenEventsViewModel _hiddenEventsViewModel;
    private readonly HideEventCommand _command;

    public HideEventCommandTests()
    {
        IDragDropUtils dragDropUtils = new DragDropUtils();
        IFilteringStore filteringStore = new FilteringStore();
        IEventsImporter eventsImporter = new L2tCSVEventsImporter();
        IEventsStore eventsStore = new EventsStore(eventsImporter);
        _eventDetailsViewModel = new();
        ChangeColourViewModel changeColourViewModel = new(_eventDetailsViewModel);
        _eventTreeViewModel = new(_eventDetailsViewModel, filteringStore, changeColourViewModel, dragDropUtils);
        _hiddenEventsViewModel = new(eventsStore, _eventTreeViewModel);
        _command = new(_eventTreeViewModel, _eventDetailsViewModel, _hiddenEventsViewModel);
    }

    [TestMethod]
    public void CanExecute_ShouldReturnFalse_WhenNoEventIsSelected()
    {
        // Arrange
        bool expected = false;
        _eventDetailsViewModel.SelectedEvent = null;

        // Act
        bool actual = _command.CanExecute(null);

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void CanExecute_ShouldReturnTrue_WhenEventIsSelected()
    {
        // Arrange
        bool expected = true;
        _eventDetailsViewModel.SelectedEvent = new(new EventModel(new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "Source", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>()));

        // Act
        bool actual = _command.CanExecute(null);

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(actual);
    }

    [TestMethod]
    public void Execute_ShouldHideSelectedEvent_WhenCommandIsExecuted()
    {
        // Arrange
        EventViewModel hiddenEvent = new(new EventModel(new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "Source", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>()));
        EventViewModel expected = new(new EventModel(new DateOnly(2000, 1, 2), new TimeOnly(17, 55), TimeZoneInfo.Utc, "MACB2", "Source2", "Source Type2", "Type2", "Username2", "Hostname2", "Short Description2", "Full Description2", 2.5, "Filename2", "iNode number2", "Notes2", "Format2", new Dictionary<string, string>() { { "Key1", "Value1" } }));
        List<EventViewModel> events = new() { hiddenEvent, expected };
        _eventTreeViewModel.LoadEvents(events);
        _eventDetailsViewModel.SelectedEvent = hiddenEvent;

        // Act
        _command.Execute(null);
        List<EventViewModel> filteredEvents = _eventTreeViewModel.EventsView.Cast<EventViewModel>().ToList();
        EventViewModel? actual = filteredEvents.FirstOrDefault();

        // Assert
        Assert.IsFalse(hiddenEvent.IsVisible);
        Assert.IsTrue(_hiddenEventsViewModel.HiddenEvents.Contains(hiddenEvent));
        Assert.IsNull(_eventDetailsViewModel.SelectedEvent);

        Assert.IsNotNull(actual);
        Assert.AreEqual(expected.Children.Count, actual.Children.Count);
        Assert.AreEqual(expected.FullDate, actual.FullDate);
        Assert.AreEqual(expected.Timezone, actual.Timezone);
        Assert.AreEqual(expected.MACB, actual.MACB);
        Assert.AreEqual(expected.Source, actual.Source);
        Assert.AreEqual(expected.SourceType, actual.SourceType);
        Assert.AreEqual(expected.Type, actual.Type);
        Assert.AreEqual(expected.User, actual.User);
        Assert.AreEqual(expected.Host, actual.Host);
        Assert.AreEqual(expected.Short, actual.Short);
        Assert.AreEqual(expected.Description, actual.Description);
        Assert.AreEqual(expected.Version, actual.Version);
        Assert.AreEqual(expected.Filename, actual.Filename);
        Assert.AreEqual(expected.INode, actual.INode);
        Assert.AreEqual(expected.Notes, actual.Notes);
        Assert.AreEqual(expected.Format, actual.Format);
        Assert.AreEqual(expected.Extra.Count, actual.Extra.Count);

        foreach (KeyValuePair<string, string> kvp in expected.Extra)
        {
            string expectedKey = kvp.Key;
            string expectedValue = kvp.Value;

            Assert.IsTrue(actual.Extra.ContainsKey(expectedKey));
            Assert.AreEqual(expectedValue, actual.Extra[expectedKey]);
        }

        Assert.AreEqual(expected.IsVisible, actual.IsVisible);
        Assert.AreEqual(expected.Colour, actual.Colour);
    }
}
