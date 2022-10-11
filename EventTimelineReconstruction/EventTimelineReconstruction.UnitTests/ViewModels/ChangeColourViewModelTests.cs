using System.Windows.Input;
using System.Windows.Media;
using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.ViewModels;
using Moq;

namespace EventTimelineReconstruction.UnitTests.ViewModels;

[TestClass]
public class ChangeColourViewModelTests
{
    [TestMethod]
    public void SelectedColour_ShouldReturnTransparentColour_WhenColourIsNotSet()
    {
        // Arrange
        Mock<EventDetailsViewModel> mock = new();
        ChangeColourViewModel changeColourViewModel = new(mock.Object);
        Color expected = new();

        // Act
        Color actual = changeColourViewModel.SelectedColour;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void SetBrushColour_ShouldSetColour_WhenBrushIsGiven()
    {
        // Arrange
        Mock<EventDetailsViewModel> mock = new();
        ChangeColourViewModel changeColourViewModel = new(mock.Object);
        Brush brushColour = Brushes.Red;
        Color expected = ((SolidColorBrush)brushColour).Color;

        // Act
        changeColourViewModel.SetBrushColour(brushColour);
        Color actual = changeColourViewModel.SelectedColour;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ApplyCommand_ShouldReturnCommand_WhenObjectIsInitialized()
    {
        // Arrange
        Mock<EventDetailsViewModel> mock = new();
        ChangeColourViewModel changeColourViewModel = new(mock.Object);
        Type expected = typeof(ChangeEventColourCommand);

        // Act
        ICommand command = changeColourViewModel.ApplyCommand;
        Type actual = command.GetType();

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(command is ChangeEventColourCommand);
    }
}
