using EventTimelineReconstruction.Commands;
using System.Windows.Input;
using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.Validators;
using EventTimelineReconstruction.ViewModels;
using Moq;

namespace EventTimelineReconstruction.UnitTests.ViewModels;

[TestClass]
public class MainWindowViewModelTests
{
    private readonly Mock<EventDetailsViewModel> _eventDetailsViewModelMock;
    private readonly Mock<EventTreeViewModel> _eventTreeViewModelMock;
    private readonly MainWindowViewModel _mainWindowViewModel;

    public MainWindowViewModelTests()
    {
        Mock<FilteringStore> filteringStoreMock = new();
        _eventDetailsViewModelMock = new();
        Mock<ChangeColourViewModel> changeColourViewModelMock = new(_eventDetailsViewModelMock.Object);
        Mock<IDragDropUtils> iDragDropUtilsMock = new();
        _eventTreeViewModelMock = new(_eventDetailsViewModelMock.Object, filteringStoreMock.Object, changeColourViewModelMock.Object, iDragDropUtilsMock.Object);
        Mock<IEventsImporter> iEventsImporterMock = new();
        Mock<EventsStore> eventsStoreMock = new(iEventsImporterMock.Object);
        Mock<HiddenEventsViewModel> hiddenEventsViewModelMock = new(eventsStoreMock.Object, _eventTreeViewModelMock.Object);
        Mock<ITimeValidator> iTimeValidatorMock = new();
        Mock<ImportViewModel> importViewModelMock = new(_eventTreeViewModelMock.Object, eventsStoreMock.Object, iTimeValidatorMock.Object);
        Mock<IFilteringUtils> iFilteringUtilsMock = new();
        Mock<FilterViewModel> filterViewModelMock = new(filteringStoreMock.Object, _eventTreeViewModelMock.Object, iTimeValidatorMock.Object, iFilteringUtilsMock.Object);
        Mock<IHashCalculator> iHashCalculatorMock = new();
        Mock<IntegrityViewModel> integrityViewModelMock = new(eventsStoreMock.Object, iHashCalculatorMock.Object, iEventsImporterMock.Object, iTimeValidatorMock.Object);
        Mock<IFileUtils> fileUtilsMock = new();
        Mock<IResourcesUtils> resourcesUtilsMock = new();
        _mainWindowViewModel = new(
            _eventTreeViewModelMock.Object,
            _eventDetailsViewModelMock.Object,
            hiddenEventsViewModelMock.Object,
            importViewModelMock.Object,
            filterViewModelMock.Object,
            integrityViewModelMock.Object,
            fileUtilsMock.Object,
            resourcesUtilsMock.Object
            );
    }

    [TestMethod]
    public void EventTreeViewModel_ShouldReturnObject_WhenPropertyIsCalled()
    {
        // Act
        EventTreeViewModel actual = _mainWindowViewModel.EventTreeViewModel;

        // Assert
        Assert.AreEqual(_eventTreeViewModelMock.Object, actual);
    }

    [TestMethod]
    public void EventDetailsViewModel_ShouldReturnObject_WhenPropertyIsCalled()
    {
        // Act
        EventDetailsViewModel actual = _mainWindowViewModel.EventDetailsViewModel;

        // Assert
        Assert.AreEqual(_eventDetailsViewModelMock.Object, actual);
    }

    [TestMethod]
    public void InitializeCommand_ShouldReturnCommand_WhenObjectIsInitialized()
    {
        // Arrange
        Type expected = typeof(InitializeLanguagesCommand);

        // Act
        ICommand command = _mainWindowViewModel.InitializeCommand;
        Type actual = command.GetType();

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(command is InitializeLanguagesCommand);
    }

    [TestMethod]
    public void MoveEventCommand_ShouldReturnCommand_WhenObjectIsInitialized()
    {
        // Arrange
        Type expected = typeof(MoveEventUpCommand);

        // Act
        ICommand command = _mainWindowViewModel.MoveEventCommand;
        Type actual = command.GetType();

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(command is MoveEventUpCommand);
    }

    [TestMethod]
    public void HideCommand_ShouldReturnCommand_WhenObjectIsInitialized()
    {
        // Arrange
        Type expected = typeof(HideEventCommand);

        // Act
        ICommand command = _mainWindowViewModel.HideCommand;
        Type actual = command.GetType();

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(command is HideEventCommand);
    }
}
