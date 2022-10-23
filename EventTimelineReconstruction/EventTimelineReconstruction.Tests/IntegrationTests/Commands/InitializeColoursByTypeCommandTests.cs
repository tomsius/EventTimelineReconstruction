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
public class InitializeColoursByTypeCommandTests
{
    private readonly ColourViewModel _colourViewModel;
    private readonly InitializeColoursByTypeCommand _command;

    public InitializeColoursByTypeCommandTests()
    {
        IColouringUtils colouringUtils = new ColouringUtils();
        IColouringStore colouringStore = new ColouringStore();
        IDragDropUtils dragDropUtils = new DragDropUtils();
        EventDetailsViewModel eventDetailsViewModel = new();
        ChangeColourViewModel changeColourViewModel = new(eventDetailsViewModel);
        IFilteringStore filteringStore = new FilteringStore();
        EventTreeViewModel eventTreeViewModel = new(eventDetailsViewModel, filteringStore, changeColourViewModel, dragDropUtils);
        _colourViewModel = new(colouringStore, eventTreeViewModel, colouringUtils);

        _command = new(_colourViewModel, colouringUtils);
    }

    [STATestMethod]
    public void Execute_ShouldInitializeColoursByType_WhenCommandIsExecuted()
    {
        // Arrange
        StackPanel root = new();
        DockPanel child1 = new();
        TextBlock textBlock1 = new() { Text = "Created" };
        ColorPicker colour1 = new() { SelectedColor = Colors.Green };
        child1.Children.Add(textBlock1);
        child1.Children.Add(colour1);
        root.Children.Add(child1);
        DockPanel child2 = new();
        TextBlock textBlock2 = new() { Text = "Deleted" };
        ColorPicker colour2 = new() { SelectedColor = Colors.Red };
        child2.Children.Add(textBlock2);
        child2.Children.Add(colour2);
        root.Children.Add(child2);
        DockPanel child3 = new();
        TextBlock textBlock3 = new() { Text = "Modified" };
        ColorPicker colour3 = new() { SelectedColor = null };
        child3.Children.Add(textBlock3);
        child3.Children.Add(colour3);
        root.Children.Add(child3);
        Dictionary<string, Color> expected = new() { { "Created", Colors.Green }, { "Deleted", Colors.Red }, { "Modified", Colors.Black } };
        RoutedEventArgs args = new(StackPanel.LoadedEvent, root);
        object parameter = (object)args;

        // Act
        _command.Execute(parameter);

        // Assert
        foreach (KeyValuePair<string, Color> item in expected)
        {
            string expectedKey = item.Key;
            Color expectedColour = item.Value;

            Assert.IsTrue(_colourViewModel.ColoursByType.ContainsKey(expectedKey));
            Assert.AreEqual(expectedColour.ToString(), _colourViewModel.ColoursByType[expectedKey].ToString());
        }
    }
}
