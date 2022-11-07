using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.ViewModels;
using Moq;

namespace EventTimelineReconstruction.Tests.UnitTests.Stores;

[TestClass]
public class EventsStoreTests
{
    private List<EventModel> GetEventModels()
    {
        List<EventModel> events = new()
        {
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
                        new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }),
            new EventModel(
                        new DateOnly(2021, 10, 14),
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
                        new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }),
            new EventModel(
                        new DateOnly(2022, 11, 14),
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
                        new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }),
            new EventModel(
                        new DateOnly(2022, 10, 15),
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
                        new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }),
            new EventModel(
                        new DateOnly(2022, 10, 14),
                        new TimeOnly(20, 52),
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
                        new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }),
            new EventModel(
                        new DateOnly(2022, 10, 14),
                        new TimeOnly(10, 15),
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
                        new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }),
            new EventModel(
                        new DateOnly(2022, 12, 14),
                        new TimeOnly(10, 52),
                        TimeZoneInfo.Utc,
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
                        new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }),
            new EventModel(
                        new DateOnly(2022, 9, 16),
                        new TimeOnly(10, 52),
                        TimeZoneInfo.Local,
                        "BCAM",
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
                        new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }),
            new EventModel(
                        new DateOnly(2020, 2, 1),
                        new TimeOnly(10, 52),
                        TimeZoneInfo.Local,
                        "MACB",
                        "Source2",
                        "Source Type",
                        "Type",
                        "Username",
                        "Hostname",
                        "Short Description",
                        "Full Description",
                        2.5,
                        "Filename123",
                        "iNode number",
                        "Notes",
                        "Format",
                        new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }),
            new EventModel(
                        new DateOnly(2020, 2, 1),
                        new TimeOnly(10, 52),
                        TimeZoneInfo.Local,
                        "MACB",
                        "Source",
                        "Source Type2",
                        "Type",
                        "Username",
                        "Hostname",
                        "Short Description",
                        "Full Description",
                        2.5,
                        "Filename321",
                        "iNode number",
                        "Notes",
                        "Format",
                        new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } })
        };

        return events;
    }

    private List<EventViewModel> GetEventViewModels()
    {
        List<EventViewModel> eventViewModels = this.GetEventModels().Select(e => new EventViewModel(e)).ToList();

        EventModel eventModel = new(
            new DateOnly(2018, 8, 5),
            new TimeOnly(12, 32),
            TimeZoneInfo.Local,
            "MACB8",
            "Source1",
            "Source Type8",
            "Type5",
            "Username4",
            "Hostname6",
            "Short Description3",
            "Full Description5",
            2.5,
            "Filename7",
            "iNode number8",
            "Notes5",
            "Format1",
            new Dictionary<string, string>() { { "Key10", "Value10" }, { "Key21", "Value21" } });
        EventViewModel childEventViewModel = new(eventModel) { IsVisible = false };

        eventViewModels[^1].AddChild(childEventViewModel);

        eventViewModels[0].IsVisible = false;
        eventViewModels[1].IsVisible = false;

        return eventViewModels;
    }

    private List<EventViewModel> GetHiddenEventViewModels()
    {
        List<EventViewModel> eventViewModels = this.GetEventViewModels();
        List<EventViewModel> hiddenEvents = new()
        {
            eventViewModels[0],
            eventViewModels[1],
            eventViewModels[^1].Children[0]
        };

        return hiddenEvents;
    }

    [TestMethod]
    public async Task Import_ShouldPopulateListWithEvents_WhenThereAreEventsInFile()
    {
        // Arrange
        Mock<IEventsImporter> mock = new();
        mock.Setup(importer => importer.Import(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(this.GetEventModels);
        EventsStore eventsStore = new(mock.Object);

        string path = "FilePath";
        DateTime from = DateTime.MinValue;
        DateTime to = DateTime.MaxValue;
        List<EventViewModel> expectedEvents = this.GetEventModels().Select(e => new EventViewModel(e)).ToList().OrderBy(e => e.FullDate).ThenBy(e => e.Filename).ToList();

        // Act
        await eventsStore.Import(path, from, to);

        // Assert
        Assert.AreEqual(expectedEvents.Count, eventsStore.Events.Count);

        for (int i = 0; i < expectedEvents.Count; i++)
        {
            Assert.AreEqual(expectedEvents[i].Children.Count, eventsStore.Events[i].Children.Count);
            Assert.AreEqual(expectedEvents[i].FullDate, eventsStore.Events[i].FullDate);
            Assert.AreEqual(expectedEvents[i].Timezone, eventsStore.Events[i].Timezone);
            Assert.AreEqual(expectedEvents[i].MACB, eventsStore.Events[i].MACB);
            Assert.AreEqual(expectedEvents[i].Source, eventsStore.Events[i].Source);
            Assert.AreEqual(expectedEvents[i].SourceType, eventsStore.Events[i].SourceType);
            Assert.AreEqual(expectedEvents[i].Type, eventsStore.Events[i].Type);
            Assert.AreEqual(expectedEvents[i].User, eventsStore.Events[i].User);
            Assert.AreEqual(expectedEvents[i].Host, eventsStore.Events[i].Host);
            Assert.AreEqual(expectedEvents[i].Short, eventsStore.Events[i].Short);
            Assert.AreEqual(expectedEvents[i].Description, eventsStore.Events[i].Description);
            Assert.AreEqual(expectedEvents[i].Version, eventsStore.Events[i].Version);
            Assert.AreEqual(expectedEvents[i].Filename, eventsStore.Events[i].Filename);
            Assert.AreEqual(expectedEvents[i].INode, eventsStore.Events[i].INode);
            Assert.AreEqual(expectedEvents[i].Notes, eventsStore.Events[i].Notes);
            Assert.AreEqual(expectedEvents[i].Format, eventsStore.Events[i].Format);
            Assert.AreEqual(expectedEvents[i].Extra.Count, eventsStore.Events[i].Extra.Count);

            foreach (KeyValuePair<string, string> kvp in expectedEvents[i].Extra)
            {
                string expectedKey = kvp.Key;
                string expectedValue = kvp.Value;

                Assert.IsTrue(eventsStore.Events[i].Extra.ContainsKey(expectedKey));
                Assert.AreEqual(expectedValue, eventsStore.Events[i].Extra[expectedKey]);
            }

            Assert.AreEqual(expectedEvents[i].IsVisible, eventsStore.Events[i].IsVisible);
            Assert.AreEqual(expectedEvents[i].Colour, eventsStore.Events[i].Colour);
        }
    }

    [TestMethod]
    public void LoadEvents_ShouldRepopulateListWithGivenEvents_WhenNewEventsAreGiven()
    {
        // Arrange
        Mock<IEventsImporter> mock = new();
        EventsStore eventsStore = new(mock.Object);

        List<EventViewModel> expectedEvents = this.GetEventModels().Select(e => new EventViewModel(e)).ToList();

        // Act
        eventsStore.LoadEvents(expectedEvents);

        // Assert
        Assert.AreEqual(expectedEvents.Count, eventsStore.Events.Count);

        for (int i = 0; i < expectedEvents.Count; i++)
        {
            Assert.AreEqual(expectedEvents[i].Children.Count, eventsStore.Events[i].Children.Count);
            Assert.AreEqual(expectedEvents[i].FullDate, eventsStore.Events[i].FullDate);
            Assert.AreEqual(expectedEvents[i].Timezone, eventsStore.Events[i].Timezone);
            Assert.AreEqual(expectedEvents[i].MACB, eventsStore.Events[i].MACB);
            Assert.AreEqual(expectedEvents[i].Source, eventsStore.Events[i].Source);
            Assert.AreEqual(expectedEvents[i].SourceType, eventsStore.Events[i].SourceType);
            Assert.AreEqual(expectedEvents[i].Type, eventsStore.Events[i].Type);
            Assert.AreEqual(expectedEvents[i].User, eventsStore.Events[i].User);
            Assert.AreEqual(expectedEvents[i].Host, eventsStore.Events[i].Host);
            Assert.AreEqual(expectedEvents[i].Short, eventsStore.Events[i].Short);
            Assert.AreEqual(expectedEvents[i].Description, eventsStore.Events[i].Description);
            Assert.AreEqual(expectedEvents[i].Version, eventsStore.Events[i].Version);
            Assert.AreEqual(expectedEvents[i].Filename, eventsStore.Events[i].Filename);
            Assert.AreEqual(expectedEvents[i].INode, eventsStore.Events[i].INode);
            Assert.AreEqual(expectedEvents[i].Notes, eventsStore.Events[i].Notes);
            Assert.AreEqual(expectedEvents[i].Format, eventsStore.Events[i].Format);
            Assert.AreEqual(expectedEvents[i].Extra.Count, eventsStore.Events[i].Extra.Count);

            foreach (KeyValuePair<string, string> kvp in expectedEvents[i].Extra)
            {
                string expectedKey = kvp.Key;
                string expectedValue = kvp.Value;

                Assert.IsTrue(eventsStore.Events[i].Extra.ContainsKey(expectedKey));
                Assert.AreEqual(expectedValue, eventsStore.Events[i].Extra[expectedKey]);
            }

            Assert.AreEqual(expectedEvents[i].IsVisible, eventsStore.Events[i].IsVisible);
            Assert.AreEqual(expectedEvents[i].Colour, eventsStore.Events[i].Colour);
        }
    }

    [TestMethod]
    public void GetAllHiddenEvents_ShouldReturnOnlyHiddenEvents_WhenThereAreHiddenEvents()
    {
        // Arrange
        Mock<IEventsImporter> mock = new();
        EventsStore eventsStore = new(mock.Object);

        List<EventViewModel> expectedEvents = this.GetHiddenEventViewModels();
        eventsStore.LoadEvents(this.GetEventViewModels());

        // Act
        List<EventViewModel> actualEvents = eventsStore.GetAllHiddenEvents();

        // Assert
        Assert.AreEqual(expectedEvents.Count, actualEvents.Count);

        for (int i = 0; i < expectedEvents.Count; i++)
        {
            Assert.AreEqual(expectedEvents[i].Children.Count, actualEvents[i].Children.Count);
            Assert.AreEqual(expectedEvents[i].FullDate, actualEvents[i].FullDate);
            Assert.AreEqual(expectedEvents[i].Timezone, actualEvents[i].Timezone);
            Assert.AreEqual(expectedEvents[i].MACB, actualEvents[i].MACB);
            Assert.AreEqual(expectedEvents[i].Source, actualEvents[i].Source);
            Assert.AreEqual(expectedEvents[i].SourceType, actualEvents[i].SourceType);
            Assert.AreEqual(expectedEvents[i].Type, actualEvents[i].Type);
            Assert.AreEqual(expectedEvents[i].User, actualEvents[i].User);
            Assert.AreEqual(expectedEvents[i].Host, actualEvents[i].Host);
            Assert.AreEqual(expectedEvents[i].Short, actualEvents[i].Short);
            Assert.AreEqual(expectedEvents[i].Description, actualEvents[i].Description);
            Assert.AreEqual(expectedEvents[i].Version, actualEvents[i].Version);
            Assert.AreEqual(expectedEvents[i].Filename, actualEvents[i].Filename);
            Assert.AreEqual(expectedEvents[i].INode, actualEvents[i].INode);
            Assert.AreEqual(expectedEvents[i].Notes, actualEvents[i].Notes);
            Assert.AreEqual(expectedEvents[i].Format, actualEvents[i].Format);
            Assert.AreEqual(expectedEvents[i].Extra.Count, actualEvents[i].Extra.Count);

            foreach (KeyValuePair<string, string> kvp in expectedEvents[i].Extra)
            {
                string expectedKey = kvp.Key;
                string expectedValue = kvp.Value;

                Assert.IsTrue(actualEvents[i].Extra.ContainsKey(expectedKey));
                Assert.AreEqual(expectedValue, actualEvents[i].Extra[expectedKey]);
            }

            Assert.AreEqual(expectedEvents[i].IsVisible, actualEvents[i].IsVisible);
            Assert.AreEqual(expectedEvents[i].Colour, actualEvents[i].Colour);
        }
    }

    [TestMethod]
    public void GetStoredEventModels_ShouldReturnEventModels_WhenThereAreStoredEvents()
    {
        // Arrange
        Mock<IEventsImporter> mock = new();
        EventsStore eventsStore = new(mock.Object);

        List<EventViewModel> storedEvents = this.GetEventViewModels();
        eventsStore.LoadEvents(storedEvents);

        List<EventModel> expectedEvents = this.GetEventModels();

        EventViewModel childEvent = storedEvents[^1].Children[0];
        EventModel childEventModel = new(
                new DateOnly(childEvent.FullDate.Year, childEvent.FullDate.Month, childEvent.FullDate.Day),
                new TimeOnly(childEvent.FullDate.Hour, childEvent.FullDate.Minute, childEvent.FullDate.Second),
                childEvent.Timezone,
                childEvent.MACB,
                childEvent.Source,
                childEvent.SourceType,
                childEvent.Type,
                childEvent.User,
                childEvent.Host,
                childEvent.Short,
                childEvent.Description,
                childEvent.Version,
                childEvent.Filename,
                childEvent.INode,
                childEvent.Notes,
                childEvent.Format,
                childEvent.Extra
                );
        expectedEvents.Add(childEventModel);

        // Act
        List<EventModel> actualEvents = eventsStore.GetStoredEventModels();

        // Assert
        Assert.AreEqual(expectedEvents.Count, actualEvents.Count);

        for (int i = 0; i < expectedEvents.Count; i++)
        {
            Assert.AreEqual(expectedEvents[i].Date, actualEvents[i].Date);
            Assert.AreEqual(expectedEvents[i].Time, actualEvents[i].Time);
            Assert.AreEqual(expectedEvents[i].Timezone, actualEvents[i].Timezone);
            Assert.AreEqual(expectedEvents[i].MACB, actualEvents[i].MACB);
            Assert.AreEqual(expectedEvents[i].Source, actualEvents[i].Source);
            Assert.AreEqual(expectedEvents[i].SourceType, actualEvents[i].SourceType);
            Assert.AreEqual(expectedEvents[i].Type, actualEvents[i].Type);
            Assert.AreEqual(expectedEvents[i].User, actualEvents[i].User);
            Assert.AreEqual(expectedEvents[i].Host, actualEvents[i].Host);
            Assert.AreEqual(expectedEvents[i].Short, actualEvents[i].Short);
            Assert.AreEqual(expectedEvents[i].Description, actualEvents[i].Description);
            Assert.AreEqual(expectedEvents[i].Version, actualEvents[i].Version);
            Assert.AreEqual(expectedEvents[i].Filename, actualEvents[i].Filename);
            Assert.AreEqual(expectedEvents[i].INode, actualEvents[i].INode);
            Assert.AreEqual(expectedEvents[i].Notes, actualEvents[i].Notes);
            Assert.AreEqual(expectedEvents[i].Format, actualEvents[i].Format);
            Assert.AreEqual(expectedEvents[i].Extra.Count, actualEvents[i].Extra.Count);

            foreach (KeyValuePair<string, string> kvp in expectedEvents[i].Extra)
            {
                string expectedKey = kvp.Key;
                string expectedValue = kvp.Value;

                Assert.IsTrue(actualEvents[i].Extra.ContainsKey(expectedKey));
                Assert.AreEqual(expectedValue, actualEvents[i].Extra[expectedKey]);
            }
        }
    }
}
