using System.Windows.Media;
using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Tests.IntegrationTests.Commands;

[TestClass]
public class ChangeEventColourCommandTests
{
    private readonly ChangeColourViewModel _changeColourViewModel;
    private readonly EventDetailsViewModel _eventDetailsViewModel;
    private readonly ChangeEventColourCommand _command;

    public ChangeEventColourCommandTests()
    {
        _eventDetailsViewModel = new();
        _changeColourViewModel = new(_eventDetailsViewModel);

        _command = new(_changeColourViewModel, _eventDetailsViewModel);
    }

    [TestMethod]
    public void CanExecute_ShouldReturnFalse_WhenTransparentColourIsSelected()
    {
        // Arrange
        _eventDetailsViewModel.SelectedEvent = new(new EventModel(new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "Source", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>()));
        _changeColourViewModel.SelectedColour = Colors.Transparent;
        bool expected = false;

        // Act
        bool actual = _command.CanExecute(null);

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void CanExecute_ShouldReturnFalse_WhenEventIsNotSelected()
    {
        // Arrange
        _eventDetailsViewModel.SelectedEvent = null;
        _changeColourViewModel.SelectedColour = Colors.Red;
        bool expected = false;

        // Act
        bool actual = _command.CanExecute(null);

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void CanExecute_ShouldReturnTrue_WhenEventIsSelectedAndSelectedColourIsNotTransparent()
    {
        // Arrange
        _eventDetailsViewModel.SelectedEvent = new(new EventModel(new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "Source", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>()));
        _changeColourViewModel.SelectedColour = Colors.Red;
        bool expected = true;

        // Act
        bool actual = _command.CanExecute(null);

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(actual);
    }

    [TestMethod]
    public void Execute_ShouldChangeEventColour_WhenCommandIsExecuted()
    {
        // Arrange
        Color expected = Colors.Green;
        _eventDetailsViewModel.SelectedEvent = new(new EventModel(new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "Source", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>()));
        _changeColourViewModel.SelectedColour = expected;

        // Act
        _command.Execute(null);
        Brush brush = _eventDetailsViewModel.SelectedEvent.Colour;
        SolidColorBrush solidBrush = (SolidColorBrush)brush;
        Color actual = solidBrush.Color;

        // Assert
        Assert.AreEqual(expected.ToString(), actual.ToString());
    }
}
