using System.Windows.Input;
using System.Windows.Media;
using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;
using Moq;

namespace EventTimelineReconstruction.UnitTests.ViewModels;

[TestClass]
public class ColourViewModelTests
{
    private readonly ColourViewModel _colourViewModel;

    public ColourViewModelTests()
    {
        Mock<ColouringStore> colouringStoreMock = new();
        Mock<EventDetailsViewModel> eventDetailsViewModelMock = new();
        Mock<FilteringStore> filteringStoreMock = new();
        Mock<ChangeColourViewModel> changeColourViewModelMock = new(eventDetailsViewModelMock.Object);
        Mock<IDragDropUtils> iDragDropUtilsMock = new();
        Mock<EventTreeViewModel> eventTreeViewModelMock = new(eventDetailsViewModelMock.Object, filteringStoreMock.Object, changeColourViewModelMock.Object, iDragDropUtilsMock.Object);
        Mock<IColouringUtils> iColouringUtilsMock = new();
        _colourViewModel = new(colouringStoreMock.Object, eventTreeViewModelMock.Object, iColouringUtilsMock.Object);
    }

    [TestMethod]
    public void ColoursByType_ShouldReturnEmptyDictionary_WhenObjectIsInitialized()
    {
        // Arrange
        int expected = 0;

        // Act
        int actual = _colourViewModel.ColoursByType.Count;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void UpdateColourByType_ShouldAddNewColourByType_WhenKeyDoesNotExist()
    {
        // Arrange
        string key = "Created";
        Brush value = Brushes.Red;
        int expectedCount = 1;

        // Act
        _colourViewModel.UpdateColourByType(key, value);
        int actualCount = _colourViewModel.ColoursByType.Count;
        bool isInDictionary = _colourViewModel.ColoursByType.TryGetValue(key, out Brush? actualValue);

        // Assert
        Assert.AreEqual(expectedCount, actualCount);
        Assert.IsTrue(isInDictionary);
        Assert.AreEqual(value, actualValue);
    }

    [TestMethod]
    public void UpdateColourByType_ShouldUpdateExistingColourByType_WhenKeyExists()
    {
        // Arrange
        string key = "Created";
        Brush value = Brushes.Red;
        _colourViewModel.UpdateColourByType(key, value);
        Brush newValue = Brushes.Green;
        int expectedCount = 1;

        // Act
        _colourViewModel.UpdateColourByType(key, newValue);
        int actualCount = _colourViewModel.ColoursByType.Count;
        bool isInDictionary = _colourViewModel.ColoursByType.TryGetValue(key, out Brush? actualValue);

        // Assert
        Assert.AreEqual(expectedCount, actualCount);
        Assert.IsTrue(isInDictionary);
        Assert.AreEqual(newValue, actualValue);
    }

    [TestMethod]
    public void InitializeCommand_ShouldReturnCommand_WhenObjectIsInitialized()
    {
        // Arrange
        Type expected = typeof(InitializeColoursByTypeCommand);

        // Act
        ICommand command = _colourViewModel.InitializeCommand;
        Type actual = command.GetType();

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(command is InitializeColoursByTypeCommand);
    }

    [TestMethod]
    public void ApplyCommand_ShouldReturnCommand_WhenObjectIsInitialized()
    {
        // Arrange
        Type expected = typeof(ApplyColourOptionsCommand);

        // Act
        ICommand command = _colourViewModel.ApplyCommand;
        Type actual = command.GetType();

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(command is ApplyColourOptionsCommand);
    }

    [TestMethod]
    public void ColourChangedCommand_ShouldReturnCommand_WhenObjectIsInitialized()
    {
        // Arrange
        Type expected = typeof(SelectedColourChangedCommand);

        // Act
        ICommand command = _colourViewModel.ColourChangedCommand;
        Type actual = command.GetType();

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(command is SelectedColourChangedCommand);
    }

    [TestMethod]
    public void ColourCommand_ShouldReturnCommand_WhenObjectIsInitialized()
    {
        // Arrange
        Type expected = typeof(ApplyColoursCommand);

        // Act
        ICommand command = _colourViewModel.ColourCommand;
        Type actual = command.GetType();

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(command is ApplyColoursCommand);
    }
}
