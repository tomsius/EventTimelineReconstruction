using EventTimelineReconstruction.Commands;
using System.Windows.Input;
using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;
using Moq;
using EventTimelineReconstruction.Abstractors;

namespace EventTimelineReconstruction.Tests.UnitTests.ViewModels;

[TestClass]
public class SaveWorkViewModelTests
{
    private readonly SaveWorkViewModel _saveWorkViewModel;

    public SaveWorkViewModelTests()
    {
        Mock<EventDetailsViewModel> eventDetailsViewModelMock = new();
        Mock<IFilteringStore> iFilteringStoreMock = new();
        Mock<ChangeColourViewModel> changeColourViewModelMock = new(eventDetailsViewModelMock.Object);
        Mock<IDragDropUtils> iDragDropUtilsMock = new();
        Mock<EventTreeViewModel> eventTreeViewModelMock = new(eventDetailsViewModelMock.Object, iFilteringStoreMock.Object, changeColourViewModelMock.Object, iDragDropUtilsMock.Object);
        Mock<IEventsStore> iEventsStoreMock = new();
        Mock<IHighLevelEventsAbstractor> iHighLevelEventsAbstractor = new();
        Mock<ILowLevelEventsAbstractor> iLowLevelEventsAbstractor = new();
        Mock<IHighLevelArtefactsAbstractor> iHighLevelArtefactsAbstractor = new();
        Mock<ILowLevelArtefactsAbstractor> iLowLevelArtefactsAbstractor = new();
        Mock<IErrorsViewModel> iErrorsViewModelMock = new();
        Mock<AbstractedEventsViewModel> abstractedEventsViewModelMock = new(iEventsStoreMock.Object, iHighLevelEventsAbstractor.Object, iLowLevelEventsAbstractor.Object, iHighLevelArtefactsAbstractor.Object, iLowLevelArtefactsAbstractor.Object, iErrorsViewModelMock.Object);
        Mock<IWorkSaver> iWorkSaver = new();
        _saveWorkViewModel = new(eventTreeViewModelMock.Object, abstractedEventsViewModelMock.Object, iWorkSaver.Object);
    }

    [TestMethod]
    public void FileName_ShouldReturnNull_WhenObjectIsInitialized()
    {
        // Arrange
        string? expected = null;

        // Act
        string actual = _saveWorkViewModel.FileName;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void FileName_ShouldReturnStringValue_WhenPropertyWasSet()
    {
        // Arrange
        string expected = "Test Filename";
        _saveWorkViewModel.FileName = expected;

        // Act
        string actual = _saveWorkViewModel.FileName;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void IsSaving_ShouldReturnFalse_WhenObjectIsInitialized()
    {
        // Arrange
        bool expected = false;

        // Act
        bool actual = _saveWorkViewModel.IsSaving;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void IsSaving_ShouldReturnTrue_WhenPropertyWasSetToTrue()
    {
        // Arrange
        bool expected = true;
        _saveWorkViewModel.IsSaving = expected;

        // Act
        bool actual = _saveWorkViewModel.IsSaving;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void IsSaving_ShouldReturnFalse_WhenPropertyWasSetToFalse()
    {
        // Arrange
        bool expected = false;
        _saveWorkViewModel.IsSaving = expected;

        // Act
        bool actual = _saveWorkViewModel.IsSaving;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ChooseFileCommand_ShouldReturnCommand_WhenObjectIsInitialized()
    {
        // Arrange
        Type expected = typeof(ChooseSaveFileCommand);

        // Act
        ICommand command = _saveWorkViewModel.ChooseFileCommand;
        Type actual = command.GetType();

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(command is ChooseSaveFileCommand);
    }

    [TestMethod]
    public void SaveCommand_ShouldReturnCommand_WhenObjectIsInitialized()
    {
        // Arrange
        Type expected = typeof(SaveWorkCommand);

        // Act
        ICommand command = _saveWorkViewModel.SaveCommand;
        Type actual = command.GetType();

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(command is SaveWorkCommand);
    }
}
