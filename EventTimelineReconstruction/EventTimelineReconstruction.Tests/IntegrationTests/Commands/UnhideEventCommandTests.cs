using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Tests.IntegrationTests.Commands;

[TestClass]
public class UnhideEventCommandTests
{
    private readonly HiddenEventsViewModel _hiddenEventsViewModel;
    private readonly EventTreeViewModel _eventTreeViewModel;
    private readonly UnhideEventCommand _command;

    public UnhideEventCommandTests()
    {
        IDragDropUtils dragDropUtils = new DragDropUtils();
        IFilteringStore filteringStore = new FilteringStore();
        IEventsImporter eventsImporter = new L2tCSVEventsImporter();
        IEventsStore eventsStore = new EventsStore(eventsImporter);
        EventDetailsViewModel eventDetailsViewModel = new();
        ChangeColourViewModel changeColourViewModel = new(eventDetailsViewModel);
        _eventTreeViewModel = new(eventDetailsViewModel, filteringStore, changeColourViewModel, dragDropUtils);
        _hiddenEventsViewModel = new(eventsStore, _eventTreeViewModel);

        _command = new(_hiddenEventsViewModel, _eventTreeViewModel);
    }

    [TestMethod]
    public void CanExecute_ShouldReturnFalse_WhenNoEventIsSelected()
    {
        // Arrange
        bool expected = false;
        _hiddenEventsViewModel.SelectedHiddenEvent = null;

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
        _hiddenEventsViewModel.SelectedHiddenEvent = new(new EventModel(new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "Source", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "1"));

        // Act
        bool actual = _command.CanExecute(null);

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(actual);
    }

    [TestMethod]
    public void Execute_ShouldUnhideSelectedEvent_WhenCommandIsExecuted()
    {
        // Arrange
        EventViewModel hiddenEvent = new(new EventModel(new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "Source", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "1")) { IsVisible = false };
        EventViewModel otherEvent = new(new EventModel(new DateOnly(2000, 1, 2), new TimeOnly(17, 55), TimeZoneInfo.Utc, "MACB2", "Source2", "Source Type2", "Type2", "Username2", "Hostname2", "Short Description2", "Full Description2", 2.5, "Filename2", "iNode number2", "Notes2", "Format2", new Dictionary<string, string>() { { "Key1", "Value1" } }, "2"));
        List<EventViewModel> expected = new() { hiddenEvent, otherEvent };
        expected = expected.OrderBy(e => e.FullDate).ThenBy(e => e.Filename).ToList();
        _eventTreeViewModel.LoadEvents(expected);
        _hiddenEventsViewModel.SelectedHiddenEvent = hiddenEvent;

        // Act
        _command.Execute(null);
        List<EventViewModel> actual = _eventTreeViewModel.EventsView.Cast<EventViewModel>().ToList();

        // Assert
        Assert.IsTrue(hiddenEvent.IsVisible);
        Assert.IsFalse(_hiddenEventsViewModel.HiddenEvents.Contains(hiddenEvent));
        Assert.IsNull(_hiddenEventsViewModel.SelectedHiddenEvent);
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
}
