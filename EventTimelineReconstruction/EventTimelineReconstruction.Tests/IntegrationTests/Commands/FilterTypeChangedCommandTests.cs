using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.Validators;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Tests.IntegrationTests.Commands;

[TestClass]
public class FilterTypeChangedCommandTests
{
    private readonly FilterViewModel _filterViewModel;
    private readonly FilterTypeChangedCommand _command;

    public FilterTypeChangedCommandTests()
    {
        ITimeValidator timeValidator = new TimeValidator();
        IFilteringUtils filteringUtils = new FilteringUtils();
        IDateTimeProvider dateTimeProvider = new DateTimeProvider();
        IDragDropUtils dragDropUtils = new DragDropUtils();
        IErrorsViewModel errorsViewModel = new ErrorsViewModel();
        IFilteringStore filteringStore = new FilteringStore();
        EventDetailsViewModel eventDetailsViewModel = new();
        ChangeColourViewModel changeColourViewModel = new(eventDetailsViewModel);
        EventTreeViewModel eventTreeViewModel = new(eventDetailsViewModel, filteringStore, changeColourViewModel, dragDropUtils);
        _filterViewModel = new(filteringStore, eventTreeViewModel, timeValidator, filteringUtils, errorsViewModel, dateTimeProvider);

        _command = new(_filterViewModel);
    }

    [DataTestMethod]
    [DataRow(true, false)]
    [DataRow(false, true)]
    public void Execute_ShouldInvertProperty_WhenCommandIsExecuted(bool initialValue, bool expected)
    {
        // Arrange
        _filterViewModel.AreAllFiltersApplied = initialValue;

        // Act
        _command.Execute(null);

        // Assert
        Assert.AreEqual(expected, _filterViewModel.AreAllFiltersApplied);
        Assert.AreNotEqual(initialValue, _filterViewModel.AreAllFiltersApplied);
    }
}
