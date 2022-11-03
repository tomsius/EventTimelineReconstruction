using System.Windows.Input;
using System.Windows.Media;
using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.ViewModels;
using Moq;

namespace EventTimelineReconstruction.Tests.UnitTests.ViewModels;

[TestClass]
public class ChangeColourViewModelTests
{
    private readonly ChangeColourViewModel _changeColourViewModel;

    public ChangeColourViewModelTests()
    {
        Mock<EventDetailsViewModel> mock = new();
        _changeColourViewModel = new(mock.Object);
    }

    [TestMethod]
    public void SelectedColour_ShouldReturnDefaultColour_WhenColourIsNotSet()
    {
        // Arrange
        Color expected = new();

        // Act
        Color actual = _changeColourViewModel.SelectedColour;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void SetBrushColour_ShouldSetColour_WhenBrushIsGiven()
    {
        // Arrange
        Brush brushColour = Brushes.Red;
        Color expected = ((SolidColorBrush)brushColour).Color;

        // Act
        _changeColourViewModel.SetBrushColour(brushColour);
        Color actual = _changeColourViewModel.SelectedColour;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ApplyCommand_ShouldReturnCommand_WhenObjectIsInitialized()
    {
        // Arrange
        Type expected = typeof(ChangeEventColourCommand);

        // Act
        ICommand command = _changeColourViewModel.ApplyCommand;
        Type actual = command.GetType();

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(command is ChangeEventColourCommand);
    }
}
