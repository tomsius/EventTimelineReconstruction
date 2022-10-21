using System.Windows;
using System.Windows.Controls;
using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.Validators;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Tests.IntegrationTests.Commands;

[TestClass]
public class FilterTypeCheckedCommandTests
{
    private readonly FilterViewModel _filterViewModel;
    private readonly FilterTypeCheckedCommand _command;

    public FilterTypeCheckedCommandTests()
    {
        IFilteringStore filteringStore = new FilteringStore();
        IDragDropUtils dragDropUtils = new DragDropUtils();
        ITimeValidator timeValidator = new TimeValidator();
        IFilteringUtils filteringUtils = new FilteringUtils();
        IErrorsViewModel errorsViewModel = new ErrorsViewModel();
        IDateTimeProvider dateTimeProvider = new DateTimeProvider();
        EventDetailsViewModel eventDetailsViewModel = new();
        ChangeColourViewModel changeColourViewModel = new(eventDetailsViewModel);
        EventTreeViewModel eventTreeViewModel = new(eventDetailsViewModel, filteringStore, changeColourViewModel, dragDropUtils);
        _filterViewModel = new(filteringStore, eventTreeViewModel, timeValidator, filteringUtils, errorsViewModel, dateTimeProvider);
        _command = new(_filterViewModel, filteringUtils);
    }

    [STATestMethod]
    public void Execute_ShouldSetChildrenCheckBoxesToFalseAndUpdateFilterTypes_WhenRootBoxIsSetToNull()
    {
        // Arrange
        DockPanel root = new();
        TextBlock rootTextBlock = new();
        StackPanel rootPanel = new();
        CheckBox rootCheckBox = new() { IsChecked = null };
        StackPanel childPanel = new();
        CheckBox childCheckBox1 = new() { Content = "Created" };
        CheckBox childCheckBox2 = new() { Content = "Deleted" };
        CheckBox childCheckBox3 = new() { Content = "Modified" };
        List<CheckBox> checkBoxes = new() { childCheckBox1, childCheckBox2, childCheckBox3 };

        childPanel.Children.Add(childCheckBox1);
        childPanel.Children.Add(childCheckBox2);
        childPanel.Children.Add(childCheckBox3);
        rootPanel.Children.Add(rootCheckBox);
        rootPanel.Children.Add(childPanel);
        root.Children.Add(rootTextBlock);
        root.Children.Add(rootPanel);

        RoutedEventArgs args = new(CheckBox.ClickEvent, rootCheckBox);
        object parameter = (object)args;

        // Act
        _command.Execute(parameter);

        // Assert
        for (int i = 0; i < checkBoxes.Count; i++)
        {
            Assert.IsFalse(checkBoxes[i].IsChecked);
            Assert.IsTrue(_filterViewModel.ChosenEventTypes.ContainsKey(checkBoxes[i].Content as string));
            Assert.IsFalse(_filterViewModel.ChosenEventTypes[checkBoxes[i].Content as string]);
        }
    }

    [STATestMethod]
    public void Execute_ShouldSetChildrenCheckBoxesToFalseAndUpdateFilterTypes_WhenRootBoxIsSetToFalse()
    {
        // Arrange
        DockPanel root = new();
        TextBlock rootTextBlock = new();
        StackPanel rootPanel = new();
        CheckBox rootCheckBox = new() { IsChecked = false };
        StackPanel childPanel = new();
        CheckBox childCheckBox1 = new() { Content = "Created" };
        CheckBox childCheckBox2 = new() { Content = "Deleted" };
        CheckBox childCheckBox3 = new() { Content = "Modified" };
        List<CheckBox> checkBoxes = new() { childCheckBox1, childCheckBox2, childCheckBox3 };

        childPanel.Children.Add(childCheckBox1);
        childPanel.Children.Add(childCheckBox2);
        childPanel.Children.Add(childCheckBox3);
        rootPanel.Children.Add(rootCheckBox);
        rootPanel.Children.Add(childPanel);
        root.Children.Add(rootTextBlock);
        root.Children.Add(rootPanel);

        RoutedEventArgs args = new(CheckBox.ClickEvent, rootCheckBox);
        object parameter = (object)args;

        // Act
        _command.Execute(parameter);

        // Assert
        for (int i = 0; i < checkBoxes.Count; i++)
        {
            Assert.IsFalse(checkBoxes[i].IsChecked);
            Assert.IsTrue(_filterViewModel.ChosenEventTypes.ContainsKey(checkBoxes[i].Content as string));
            Assert.IsFalse(_filterViewModel.ChosenEventTypes[checkBoxes[i].Content as string]);
        }
    }

    [STATestMethod]
    public void Execute_ShouldSetChildrenCheckBoxesToTrueAndUpdateFilterTypes_WhenRootBoxIsSetToTrue()
    {
        // Arrange
        DockPanel root = new();
        TextBlock rootTextBlock = new();
        StackPanel rootPanel = new();
        CheckBox rootCheckBox = new() { IsChecked = true };
        StackPanel childPanel = new();
        CheckBox childCheckBox1 = new() { Content = "Created" };
        CheckBox childCheckBox2 = new() { Content = "Deleted" };
        CheckBox childCheckBox3 = new() { Content = "Modified" };
        List<CheckBox> checkBoxes = new() { childCheckBox1, childCheckBox2, childCheckBox3 };

        childPanel.Children.Add(childCheckBox1);
        childPanel.Children.Add(childCheckBox2);
        childPanel.Children.Add(childCheckBox3);
        rootPanel.Children.Add(rootCheckBox);
        rootPanel.Children.Add(childPanel);
        root.Children.Add(rootTextBlock);
        root.Children.Add(rootPanel);

        RoutedEventArgs args = new(CheckBox.ClickEvent, rootCheckBox);
        object parameter = (object)args;

        // Act
        _command.Execute(parameter);

        // Assert
        for (int i = 0; i < checkBoxes.Count; i++)
        {
            Assert.IsTrue(checkBoxes[i].IsChecked);
            Assert.IsTrue(_filterViewModel.ChosenEventTypes.ContainsKey(checkBoxes[i].Content as string));
            Assert.IsTrue(_filterViewModel.ChosenEventTypes[checkBoxes[i].Content as string]);
        }
    }

    [STATestMethod]
    public void Execute_ShouldSetRootCheckBoxToNullAndUpdateFilterTypes_WhenSomeChildrenCheckBoxesAreSetToTrueAndSomeAreSetToFalse()
    {
        // Arrange
        DockPanel root = new();
        TextBlock rootTextBlock = new();
        StackPanel rootPanel = new();
        CheckBox rootCheckBox = new();
        StackPanel childPanel = new();
        CheckBox childCheckBox1 = new() { Content = "Created", IsChecked = true };
        CheckBox childCheckBox2 = new() { Content = "Deleted", IsChecked = true };
        CheckBox childCheckBox3 = new() { Content = "Modified", IsChecked = false };
        List<CheckBox> checkBoxes = new() { childCheckBox1, childCheckBox2, childCheckBox3 };

        childPanel.Children.Add(childCheckBox1);
        childPanel.Children.Add(childCheckBox2);
        childPanel.Children.Add(childCheckBox3);
        rootPanel.Children.Add(rootCheckBox);
        rootPanel.Children.Add(childPanel);
        root.Children.Add(rootTextBlock);
        root.Children.Add(rootPanel);

        RoutedEventArgs args = new(CheckBox.ClickEvent, childCheckBox1);
        object parameter = (object)args;

        // Act
        _command.Execute(parameter);

        // Assert
        Assert.IsNull(rootCheckBox.IsChecked);

        for (int i = 0; i < checkBoxes.Count; i++)
        {
            Assert.IsTrue(_filterViewModel.ChosenEventTypes.ContainsKey(checkBoxes[i].Content as string));
            Assert.AreEqual(checkBoxes[i].IsChecked, _filterViewModel.ChosenEventTypes[checkBoxes[i].Content as string]);
        }
    }

    [STATestMethod]
    public void Execute_ShouldSetRootCheckBoxToFalseAndUpdateFilterTypes_WhenAllChildrenCheckBoxesAreSetToFalse()
    {
        // Arrange
        DockPanel root = new();
        TextBlock rootTextBlock = new();
        StackPanel rootPanel = new();
        CheckBox rootCheckBox = new();
        StackPanel childPanel = new();
        CheckBox childCheckBox1 = new() { Content = "Created", IsChecked = false };
        CheckBox childCheckBox2 = new() { Content = "Deleted", IsChecked = false };
        CheckBox childCheckBox3 = new() { Content = "Modified", IsChecked = false };
        List<CheckBox> checkBoxes = new() { childCheckBox1, childCheckBox2, childCheckBox3 };

        childPanel.Children.Add(childCheckBox1);
        childPanel.Children.Add(childCheckBox2);
        childPanel.Children.Add(childCheckBox3);
        rootPanel.Children.Add(rootCheckBox);
        rootPanel.Children.Add(childPanel);
        root.Children.Add(rootTextBlock);
        root.Children.Add(rootPanel);

        RoutedEventArgs args = new(CheckBox.ClickEvent, childCheckBox1);
        object parameter = (object)args;

        // Act
        _command.Execute(parameter);

        // Assert
        Assert.IsFalse(rootCheckBox.IsChecked);

        for (int i = 0; i < checkBoxes.Count; i++)
        {
            Assert.IsTrue(_filterViewModel.ChosenEventTypes.ContainsKey(checkBoxes[i].Content as string));
            Assert.AreEqual(checkBoxes[i].IsChecked, _filterViewModel.ChosenEventTypes[checkBoxes[i].Content as string]);
        }
    }

    [STATestMethod]
    public void Execute_ShouldSetRootCheckBoxToTrueAndUpdateFilterTypes_WhenAllChildrenCheckBoxesAreSetToTrue()
    {
        // Arrange
        DockPanel root = new();
        TextBlock rootTextBlock = new();
        StackPanel rootPanel = new();
        CheckBox rootCheckBox = new();
        StackPanel childPanel = new();
        CheckBox childCheckBox1 = new() { Content = "Created", IsChecked = true };
        CheckBox childCheckBox2 = new() { Content = "Deleted", IsChecked = true };
        CheckBox childCheckBox3 = new() { Content = "Modified", IsChecked = true };
        List<CheckBox> checkBoxes = new() { childCheckBox1, childCheckBox2, childCheckBox3 };

        childPanel.Children.Add(childCheckBox1);
        childPanel.Children.Add(childCheckBox2);
        childPanel.Children.Add(childCheckBox3);
        rootPanel.Children.Add(rootCheckBox);
        rootPanel.Children.Add(childPanel);
        root.Children.Add(rootTextBlock);
        root.Children.Add(rootPanel);

        RoutedEventArgs args = new(CheckBox.ClickEvent, childCheckBox1);
        object parameter = (object)args;

        // Act
        _command.Execute(parameter);

        // Assert
        Assert.IsTrue(rootCheckBox.IsChecked);

        for (int i = 0; i < checkBoxes.Count; i++)
        {
            Assert.IsTrue(_filterViewModel.ChosenEventTypes.ContainsKey(checkBoxes[i].Content as string));
            Assert.AreEqual(checkBoxes[i].IsChecked, _filterViewModel.ChosenEventTypes[checkBoxes[i].Content as string]);
        }
    }
}
