using System.Windows.Controls;
using System.Windows;
using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.Validators;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Tests.IntegrationTests.Commands;

[TestClass]
public class InitializeEventTypesCommandTests
{
    private readonly FilterViewModel _filterViewModel;
    private readonly InitializeEventTypesCommand _command;

    public InitializeEventTypesCommandTests()
    {
        IFilteringUtils filteringUtils = new FilteringUtils();
        ITimeValidator timeValidator = new TimeValidator();
        IDateTimeProvider dateTimeProvider = new DateTimeProvider();
        IDragDropUtils dragDropUtils = new DragDropUtils();
        IErrorsViewModel errorsViewModel = new ErrorsViewModel();
        IFilteringStore filteringStore = new FilteringStore();
        EventDetailsViewModel eventDetailsViewModel = new();
        ChangeColourViewModel changeColourViewModel = new(eventDetailsViewModel);
        EventTreeViewModel eventTreeViewModel = new(eventDetailsViewModel, filteringStore, changeColourViewModel, dragDropUtils);
        _filterViewModel = new(filteringStore, eventTreeViewModel, timeValidator, filteringUtils, errorsViewModel, dateTimeProvider);

        _command = new(_filterViewModel, filteringUtils);
    }

    [STATestMethod]
    public void Execute_ShouldInitializeEventTypes_WhenCommandIsExecuted()
    {
        // Arrange
        DockPanel root = new();
        TextBlock rootTextBlock = new();
        StackPanel rootPanel = new();
        CheckBox rootCheckBox = new() { IsChecked = null };
        StackPanel childPanel = new();
        CheckBox childCheckBox1 = new() { Content = "Created", IsChecked = true };
        CheckBox childCheckBox2 = new() { Content = "Deleted", IsChecked = false };
        CheckBox childCheckBox3 = new() { Content = "Modified", IsChecked = null };
        List<CheckBox> checkBoxes = new() { childCheckBox1, childCheckBox2, childCheckBox3 };

        childPanel.Children.Add(childCheckBox1);
        childPanel.Children.Add(childCheckBox2);
        childPanel.Children.Add(childCheckBox3);
        rootPanel.Children.Add(rootCheckBox);
        rootPanel.Children.Add(childPanel);
        root.Children.Add(rootTextBlock);
        root.Children.Add(rootPanel);

        RoutedEventArgs args = new(CheckBox.LoadedEvent, rootCheckBox);
        object parameter = (object)args;

        // Act
        _command.Execute(parameter);

        // Assert
        Assert.AreEqual(checkBoxes.Count, _filterViewModel.ChosenEventTypes.Count);

        for (int i = 0; i < checkBoxes.Count; i++)
        {
            string expectedKey = (string)checkBoxes[i].Content;
            bool expectedValue = checkBoxes[i].IsChecked == true;

            Assert.IsTrue(_filterViewModel.ChosenEventTypes.ContainsKey(expectedKey));
            Assert.AreEqual(expectedValue, _filterViewModel.ChosenEventTypes[expectedKey]);
        }
    }
}
