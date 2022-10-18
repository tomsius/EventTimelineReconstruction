using System.Windows.Media;
using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Tests.IntegrationTests.Commands;

[TestClass]
public class ApplyColoursCommandTests
{
    private readonly IColouringStore _colouringStore;
    private readonly EventTreeViewModel _eventTreeViewModel;
    private readonly ApplyColoursCommand _command;

    public ApplyColoursCommandTests()
    {
        _colouringStore = new ColouringStore();
        IFilteringStore filteringStore = new FilteringStore();
        IDragDropUtils dragDropUtils = new DragDropUtils();
        EventDetailsViewModel eventDetailsViewModel = new();
        ChangeColourViewModel changeColourViewModel = new(eventDetailsViewModel);
        _eventTreeViewModel = new(eventDetailsViewModel, filteringStore, changeColourViewModel, dragDropUtils);

        _command = new(_colouringStore, _eventTreeViewModel);
    }

    [TestMethod]
    public void CanExecute_ShouldReturnFalse_WhenThereAreNoEvents()
    {
        // Arrange
        bool expected = false;

        // Act
        bool actual = _command.CanExecute(null);

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void CanExecute_ShouldReturnTrue_WhenThereAreEvents()
    {
        // Arrange
        EventViewModel eventViewModel = new(new EventModel(DateOnly.MinValue, TimeOnly.MinValue, TimeZoneInfo.Utc, "", "", "", "", "", "", "", "", 0, "", "", "", "", new Dictionary<string, string>()));
        _eventTreeViewModel.AddEvent(eventViewModel);
        bool expected = true;

        // Act
        bool actual = _command.CanExecute(null);

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(actual);
    }

    [TestMethod]
    public void Execute_ShouldSetEventsColour_WhenCommandIsExecuted()
    {
        // Arrange
        EventViewModel parent = new(new EventModel(new DateOnly(2022, 10, 18), new TimeOnly(13, 10, 0), TimeZoneInfo.Utc, "MACB1", "Source1", "Source Type1", "Created", "User1", "Host1", "Short Description1", "Description1", 1, "Filename1", "iNode1", "Notes1", "Format1", new Dictionary<string, string>()));
        EventViewModel child = new(new EventModel(new DateOnly(2022, 10, 18), new TimeOnly(13, 12, 0), TimeZoneInfo.Utc, "MACB2", "Source2", "Source Type2", "Modified", "User2", "Host2", "Short Description2", "Description2", 2, "Filename2", "iNode2", "Notes2", "Format2", new Dictionary<string, string>()));
        parent.AddChild(child);
        _eventTreeViewModel.AddEvent(parent);
        Dictionary<string, Brush> coloursByType = new() { { "Created", Brushes.Red }, { "Deleted", Brushes.Blue }, { "Modified", Brushes.Green } };
        _colouringStore.SetColoursByType(coloursByType);

        // Act
        _command.Execute(null);

        // Assert
        Assert.AreEqual(coloursByType[parent.Type].ToString(), parent.Colour.ToString());
        Assert.AreEqual(coloursByType[child.Type].ToString(), child.Colour.ToString());
    }
}
