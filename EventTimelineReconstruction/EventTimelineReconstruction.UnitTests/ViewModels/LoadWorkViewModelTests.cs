using EventTimelineReconstruction.Commands;
using System.Windows.Input;
using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;
using Moq;

namespace EventTimelineReconstruction.UnitTests.ViewModels;

[TestClass]
public class LoadWorkViewModelTests
{
    private readonly LoadWorkViewModel _loadWorkViewModel;

    public LoadWorkViewModelTests()
    {
        Mock<EventDetailsViewModel> eventDetailsViewModelMock = new();
        Mock<FilteringStore> filteringStoreMock = new();
        Mock<ChangeColourViewModel> changeColourViewModelMock = new(eventDetailsViewModelMock.Object);
        Mock<IDragDropUtils> iDragDropUtilsMock = new();
        Mock<EventTreeViewModel> eventTreeViewModelMock = new(eventDetailsViewModelMock.Object, filteringStoreMock.Object, changeColourViewModelMock.Object, iDragDropUtilsMock.Object);
        Mock<IEventsStore> iEventsStoreMock = new();
        Mock<IWorkLoader> iWorkLoader = new();
        _loadWorkViewModel = new(eventTreeViewModelMock.Object, iEventsStoreMock.Object, iWorkLoader.Object);
    }

    [TestMethod]
    public void FileName_ShouldReturnNull_WhenObjectIsInitialized()
    {
        // Arrange
        string? expected = null;

        // Act
        string actual = _loadWorkViewModel.FileName;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void FileName_ShouldReturnStringValue_WhenPropertyWasSet()
    {
        // Arrange
        string expected = "Test Filename";
        _loadWorkViewModel.FileName = expected;

        // Act
        string actual = _loadWorkViewModel.FileName;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void IsLoading_ShouldReturnFalse_WhenObjectIsInitialized()
    {
        // Arrange
        bool expected = false;

        // Act
        bool actual = _loadWorkViewModel.IsLoading;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void IsLoading_ShouldReturnTrue_WhenPropertyWasSetToTrue()
    {
        // Arrange
        bool expected = true;
        _loadWorkViewModel.IsLoading = expected;

        // Act
        bool actual = _loadWorkViewModel.IsLoading;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void IsLoading_ShouldReturnFalse_WhenPropertyWasSetToFalse()
    {
        // Arrange
        bool expected = false;
        _loadWorkViewModel.IsLoading = expected;

        // Act
        bool actual = _loadWorkViewModel.IsLoading;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ChooseFileCommand_ShouldReturnCommand_WhenObjectIsInitialized()
    {
        // Arrange
        Type expected = typeof(ChooseLoadFileCommand);

        // Act
        ICommand command = _loadWorkViewModel.ChooseFileCommand;
        Type actual = command.GetType();

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(command is ChooseLoadFileCommand);
    }

    [TestMethod]
    public void LoadCommand_ShouldReturnCommand_WhenObjectIsInitialized()
    {
        // Arrange
        Type expected = typeof(LoadWorkCommand);

        // Act
        ICommand command = _loadWorkViewModel.LoadCommand;
        Type actual = command.GetType();

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(command is LoadWorkCommand);
    }
}
