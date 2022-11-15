using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Tests.UnitTests.ViewModels;

[TestClass]
public class EventDetailsViewModelTests
{
    [TestMethod]
    public void SelectedEvent_ShouldReturnNull_WhenObjectIsInitialized()
    {
        // Arrange
        EventDetailsViewModel eventDetailsViewModel = new();
        EventViewModel? expected = null;

        // Act
        EventViewModel actual = eventDetailsViewModel.SelectedEvent;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void SelectedEvent_ShouldReturnObject_WhenPropertyWasSet()
    {
        // Arrange
        EventDetailsViewModel eventDetailsViewModel = new();
        EventViewModel expected = new(
            new EventModel(
                DateOnly.MaxValue,
                TimeOnly.MaxValue,
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
        eventDetailsViewModel.SelectedEvent = expected;
        EventViewModel actual = eventDetailsViewModel.SelectedEvent;

        // Assert
        Assert.AreEqual(expected, actual);
    }
}
