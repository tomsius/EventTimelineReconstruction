using System.Windows.Media;
using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Tests.IntegrationTests.Commands;

[TestClass]
public class ApplyColourOptionsCommandTests
{
    private readonly IColouringStore _colouringStore;
    private readonly ColourViewModel _colourViewModel;
    private readonly ApplyColourOptionsCommand _command;

    public ApplyColourOptionsCommandTests()
    {
        IFilteringStore filteringStore = new FilteringStore();
        IDragDropUtils dragDropUtils = new DragDropUtils();
        IColouringUtils colouringUtils = new ColouringUtils();
        _colouringStore = new ColouringStore();
        EventDetailsViewModel eventDetailsViewModel = new();
        ChangeColourViewModel changeColourViewModel = new(eventDetailsViewModel);
        EventTreeViewModel eventTreeViewModel = new(eventDetailsViewModel, filteringStore, changeColourViewModel, dragDropUtils);
        _colourViewModel = new(_colouringStore, eventTreeViewModel, colouringUtils);

        _command = new(_colourViewModel, _colouringStore);
    }

    [TestMethod]
    public void CanExecute_ShouldReturnFalse_WhenTransparentColourIsSelected()
    {
        // Arrange
        _colourViewModel.UpdateColourByType("CanExecuteTest1", Brushes.Transparent);
        _colourViewModel.UpdateColourByType("CanExecuteTest2", Brushes.Red);
        bool expected = false;

        // Act
        bool actual = _command.CanExecute(null);

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void CanExecute_ShouldReturnTrue_WhenNoTransparentColourIsSelected()
    {
        // Arrange
        _colourViewModel.UpdateColourByType("CanExecuteTest1", Brushes.Red);
        _colourViewModel.UpdateColourByType("CanExecuteTest2", Brushes.Blue);
        bool expected = true;

        // Act
        bool actual = _command.CanExecute(null);

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(actual);
    }

    [TestMethod]
    public void Execute_ShouldSetColoursByTypeInStore_WhenCommandIsExecuted()
    {
        // Arrange
        Dictionary<string, Brush> expectedColoursByType = new() { { "CanExecuteTest1", Brushes.Red }, {"CanExecuteTest2", Brushes.Blue }};
        foreach (KeyValuePair<string, Brush> expectedItem in expectedColoursByType)
        {
            _colourViewModel.UpdateColourByType(expectedItem.Key, expectedItem.Value);
        }

        // Act
        _command.Execute(null);

        // Assert
        Assert.AreEqual(expectedColoursByType.Count, _colouringStore.ColoursByType.Count);

        foreach (KeyValuePair<string, Brush> expectedItem in expectedColoursByType)
        {
            Assert.IsTrue(_colouringStore.ColoursByType.ContainsKey(expectedItem.Key));
            Assert.AreEqual(expectedItem.Value, _colouringStore.ColoursByType[expectedItem.Key]);
        }
    }
}
