using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;
using Xceed.Wpf.Toolkit;

namespace EventTimelineReconstruction.Tests.IntegrationTests.Commands;

[TestClass]
public class SelectedColourChangedCommandTests
{
    private readonly ColourViewModel _colourViewModel;
    private readonly SelectedColourChangedCommand _command;

    public SelectedColourChangedCommandTests()
    {
        IColouringStore colouringStore = new ColouringStore();
        IColouringUtils colouringUtils = new ColouringUtils();
        IDragDropUtils dragDropUtils = new DragDropUtils();
        IFilteringStore filteringStore = new FilteringStore();
        EventDetailsViewModel eventDetailsViewModel = new();
        ChangeColourViewModel changeColourViewModel = new(eventDetailsViewModel);
        EventTreeViewModel eventTreeViewModel = new(eventDetailsViewModel, filteringStore, changeColourViewModel, dragDropUtils);
        _colourViewModel = new(colouringStore, eventTreeViewModel, colouringUtils);

        _command = new(_colourViewModel, colouringUtils);
    }

    [STATestMethod]
    public void Execute_ShouldUpdateColourByType_WhenCommandIsExecuted()
    {
        // Arrange
        string expectedType = "Created";
        Color expectedColour = Colors.Green;
        DockPanel dockPanel = new();
        TextBlock textBlock = new() { Text = expectedType };
        ColorPicker colorPicker = new() { SelectedColor = expectedColour };
        dockPanel.Children.Add(textBlock);
        dockPanel.Children.Add(colorPicker);
        RoutedEventArgs e = new(ColorPicker.SelectedColorChangedEvent, colorPicker);
        object parameter = (object)e;

        // Act
        _command.Execute(parameter);

        // Assert
        Assert.IsTrue(_colourViewModel.ColoursByType.ContainsKey(expectedType));
        Assert.AreEqual(expectedColour.ToString(), _colourViewModel.ColoursByType[expectedType].ToString());
    }

    [STATestMethod]
    public void Execute_ShouldUpdateColourByTypeWithBlackColour_WhenCommandIsExecutedWithNoSelectedColour()
    {
        // Arrange
        string expectedType = "Created";
        Color expectedColour = Colors.Black;
        DockPanel dockPanel = new();
        TextBlock textBlock = new() { Text = expectedType };
        ColorPicker colorPicker = new() { SelectedColor = null };
        dockPanel.Children.Add(textBlock);
        dockPanel.Children.Add(colorPicker);
        RoutedEventArgs e = new(ColorPicker.SelectedColorChangedEvent, colorPicker);
        object parameter = (object)e;

        // Act
        _command.Execute(parameter);

        // Assert
        Assert.IsTrue(_colourViewModel.ColoursByType.ContainsKey(expectedType));
        Assert.AreEqual(expectedColour.ToString(), _colourViewModel.ColoursByType[expectedType].ToString());
    }
}
