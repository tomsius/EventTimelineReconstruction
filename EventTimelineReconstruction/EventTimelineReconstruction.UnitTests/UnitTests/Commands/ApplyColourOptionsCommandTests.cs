using System.Windows.Media;
using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;
using Moq;

namespace EventTimelineReconstruction.UnitTests.Commands;

[TestClass]
public class ApplyColourOptionsCommandTests
{
    private readonly ApplyColourOptionsCommand _applyColourOptionsCommand;
    private readonly Mock<IColouringStore> _iColouringStoreMock;

    public ApplyColourOptionsCommandTests()
    {
        _iColouringStoreMock = new();
        Mock<IColouringUtils> iColouringUtilsMock = new();
        Mock<EventDetailsViewModel> eventDetailsViewModelMock = new();
        Mock<IFilteringStore> iFilteringStore = new();
        Mock<ChangeColourViewModel> changeColourViewModelMock = new(eventDetailsViewModelMock.Object);
        Mock<IDragDropUtils> iDragDropUtilsMock = new();
        Mock<EventTreeViewModel> eventTreeViewModelMock = new(eventDetailsViewModelMock.Object, iFilteringStore.Object, changeColourViewModelMock.Object, iDragDropUtilsMock.Object);
        Mock<ColourViewModel> colourViewModelMock = new(_iColouringStoreMock.Object, eventTreeViewModelMock.Object, iColouringUtilsMock.Object);
        _applyColourOptionsCommand = new(colourViewModelMock.Object, _iColouringStoreMock.Object);

        _iColouringStoreMock.Setup(mock => mock.SetColoursByType(It.IsAny<Dictionary<string, Brush>>())).Verifiable();
    }

    [TestMethod]
    public void Execute_ShouldCallSetColoursByType_WhenCommandIsExecuted()
    {
        // Arrange
        object? parameter = null;

        // Act
        _applyColourOptionsCommand.Execute(parameter);

        // Assert
        _iColouringStoreMock.Verify(mock => mock.SetColoursByType(It.IsAny<Dictionary<string, Brush>>()), Times.Once());
    }
}
