
using System.Windows.Media;
using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;
using Moq;

namespace EventTimelineReconstruction.Tests.IntegrationTests.Commands;

[TestClass]
public class ApplyColourOptionsCommandTests
{
    private readonly EventTreeViewModel _eventTreeViewModel;
    private readonly IColouringUtils _colouringUtils;

    public ApplyColourOptionsCommandTests()
    {
        IFilteringStore filteringStore = new FilteringStore();
        EventDetailsViewModel eventDetailsViewModel = new();
        ChangeColourViewModel changeColourViewModel = new(eventDetailsViewModel);
        IDragDropUtils dragDropUtils = new DragDropUtils();

        _colouringUtils = new ColouringUtils();
        _eventTreeViewModel = new(eventDetailsViewModel, filteringStore, changeColourViewModel, dragDropUtils);
    }

    [TestMethod]
    public void CanExecute_ShouldReturnFalse_WhenTransparentColourIsSelected()
    {
        // Arrange
        IColouringStore colouringStore = new ColouringStore();
        ColourViewModel colourViewModel = new(colouringStore, _eventTreeViewModel, _colouringUtils);
        ApplyColourOptionsCommand command = new(colourViewModel, colouringStore);
        colourViewModel.UpdateColourByType("CanExecuteTest1", Brushes.Transparent);
        colourViewModel.UpdateColourByType("CanExecuteTest2", Brushes.Red);
        bool expected = false;

        // Act
        bool actual = command.CanExecute(null);

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void CanExecute_ShouldReturnTrue_WhenNoTransparentColourIsSelected()
    {
        // Arrange
        IColouringStore colouringStore = new ColouringStore();
        ColourViewModel colourViewModel = new(colouringStore, _eventTreeViewModel, _colouringUtils);
        ApplyColourOptionsCommand command = new(colourViewModel, colouringStore);
        colourViewModel.UpdateColourByType("CanExecuteTest1", Brushes.Red);
        colourViewModel.UpdateColourByType("CanExecuteTest2", Brushes.Blue);
        bool expected = true;

        // Act
        bool actual = command.CanExecute(null);

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(actual);
    }

    [TestMethod]
    public void Execute_ShouldSetColoursByTypeInStore_WhenCommandIsExecuted()
    {
        // Arrange
        IColouringStore colouringStore = new ColouringStore();
        ColourViewModel colourViewModel = new(colouringStore, _eventTreeViewModel, _colouringUtils);
        ApplyColourOptionsCommand command = new(colourViewModel, colouringStore);
        Dictionary<string, Brush> expectedColoursByType = new() { { "CanExecuteTest1", Brushes.Red }, {"CanExecuteTest2", Brushes.Blue }};
        foreach (KeyValuePair<string, Brush> expectedItem in expectedColoursByType)
        {
            colourViewModel.UpdateColourByType(expectedItem.Key, expectedItem.Value);
        }

        // Act
        command.Execute(null);

        // Assert
        Assert.AreEqual(expectedColoursByType.Count, colouringStore.ColoursByType.Count);

        foreach (KeyValuePair<string, Brush> expectedItem in expectedColoursByType)
        {
            Assert.IsTrue(colouringStore.ColoursByType.ContainsKey(expectedItem.Key));
            Assert.AreEqual(expectedItem.Value, colouringStore.ColoursByType[expectedItem.Key]);
        }
    }
}
