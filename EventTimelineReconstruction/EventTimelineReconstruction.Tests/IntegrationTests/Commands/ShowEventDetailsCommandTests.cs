using System.Windows;
using System.Windows.Media;
using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Tests.IntegrationTests.Commands;

[TestClass]
public class ShowEventDetailsCommandTests
{
    private readonly EventDetailsViewModel _eventDetailsViewModel;
    private readonly ChangeColourViewModel _changeColourViewModel;
    private readonly ShowEventDetailsCommand _command;

    public ShowEventDetailsCommandTests()
    {
        _eventDetailsViewModel = new();
        _changeColourViewModel = new(_eventDetailsViewModel);
        _command = new(_eventDetailsViewModel, _changeColourViewModel);
    }

    [TestMethod]
    public void Execute_ShouldSetSelectedEventAndSelectedColour_WhenCommandIsExecuted()
    {
        // Arrange
        EventViewModel expected = new(new EventModel(new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "Source", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "1")) { Colour = Brushes.Green };
        RoutedPropertyChangedEventArgs<object?> e = new(null, expected);
        object parameter = (object)e;

        // Act
        _command.Execute(parameter);

        // Assert
        Assert.AreEqual(expected, _eventDetailsViewModel.SelectedEvent);
        Assert.AreEqual(expected.Colour.ToString(), _changeColourViewModel.SelectedColour.ToString());
    }

    [TestMethod]
    public void Execute_ShouldDoNothing_WhenCommandIsExecutedNotOnEventViewModel()
    {
        // Arrange
        Color expectedColour = new() { A = 0, B = 0, G = 0, R = 0 };
        EventModel newValue = new(new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "Source", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "1");
        RoutedPropertyChangedEventArgs<object?> e = new(null, newValue);
        object parameter = (object)e;

        // Act
        _command.Execute(parameter);

        // Assert
        Assert.IsNull(_eventDetailsViewModel.SelectedEvent);
        Assert.AreEqual(expectedColour.ToString(), _changeColourViewModel.SelectedColour.ToString());
    }
}
