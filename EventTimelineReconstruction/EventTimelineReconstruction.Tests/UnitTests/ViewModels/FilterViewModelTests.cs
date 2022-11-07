using System.ComponentModel;
using EventTimelineReconstruction.Commands;
using System.Windows.Input;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.Validators;
using EventTimelineReconstruction.ViewModels;
using Moq;

namespace EventTimelineReconstruction.Tests.UnitTests.ViewModels;

[TestClass]
public class FilterViewModelTests
{
    private readonly Mock<IFilteringStore> _iFilteringStoreMock;
    private readonly Mock<EventTreeViewModel> _eventTreeViewModel;
    private readonly Mock<ITimeValidator> _iTimeValidatorMock;
    private readonly Mock<IFilteringUtils> _iFilteringUtils;
    private readonly Mock<IErrorsViewModel> _iErrorsViewModelMock;
    private readonly Mock<IDateTimeProvider> _iDateTimeProvider;
    private readonly FilterViewModel _filterViewModel;

    public FilterViewModelTests()
    {
        _iFilteringStoreMock = new();
        Mock<EventDetailsViewModel> eventDetailsViewModelMock = new();
        Mock<ChangeColourViewModel> changeColourViewModelMock = new(eventDetailsViewModelMock.Object);
        Mock<IDragDropUtils> iDragDropUtilsMock = new();
        _eventTreeViewModel = new(eventDetailsViewModelMock.Object, _iFilteringStoreMock.Object, changeColourViewModelMock.Object, iDragDropUtilsMock.Object);
        _iTimeValidatorMock = new();
        _iFilteringUtils = new();
        _iErrorsViewModelMock = new();
        _iDateTimeProvider = new();
        _filterViewModel = new(_iFilteringStoreMock.Object, _eventTreeViewModel.Object, _iTimeValidatorMock.Object, _iFilteringUtils.Object, _iErrorsViewModelMock.Object, _iDateTimeProvider.Object);
        _filterViewModel.PropertyChanged += this.ChangeEventFlag;
    }

    private bool _hasFromDateEventFired;
    private bool _hasToDateEventFired;
    private bool _hasFromHoursEventFired;
    private bool _hasFromMinutesEventFired;
    private bool _hasToHoursEventFired;
    private bool _hasToMinutesEventFired;
    private bool _hasAreAllFiltersAppliedEventFired;
    private bool _hasChosenEventTypesEventFired;
    private bool _hasKeywordEventFired;

    private void ChangeEventFlag(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(FilterViewModel.FromDate):
                _hasFromDateEventFired = true;
                break;
            case nameof(FilterViewModel.ToDate):
                _hasToDateEventFired = true;
                break;
            case nameof(FilterViewModel.FromHours):
                _hasFromHoursEventFired = true;
                break;
            case nameof(FilterViewModel.FromMinutes):
                _hasFromMinutesEventFired = true;
                break;
            case nameof(FilterViewModel.ToHours):
                _hasToHoursEventFired = true;
                break;
            case nameof(FilterViewModel.ToMinutes):
                _hasToMinutesEventFired = true;
                break;
            case nameof(FilterViewModel.AreAllFiltersApplied):
                _hasAreAllFiltersAppliedEventFired = true;
                break;
            case nameof(FilterViewModel.ChosenEventTypes):
                _hasChosenEventTypesEventFired = true;
                break;
            case nameof(FilterViewModel.Keyword):
                _hasKeywordEventFired = true;
                break;
        }
    }

    [TestInitialize]
    public void Setup()
    {
        _hasFromDateEventFired = false;
        _hasToDateEventFired = false;
        _hasFromHoursEventFired = false;
        _hasFromMinutesEventFired = false;
        _hasToHoursEventFired = false;
        _hasToMinutesEventFired = false;
        _hasAreAllFiltersAppliedEventFired = false;
        _hasChosenEventTypesEventFired = false;
        _hasKeywordEventFired = false;
    }

    [TestMethod]
    public void ErrorsViewModel_ShouldReturnObject_WhenObjectIsInitialized()
    {
        // Act
        IErrorsViewModel actual = _filterViewModel.ErrorsViewModel;

        // Assert
        Assert.IsNotNull(actual);
    }

    [TestMethod]
    public void AreAllFiltersApplied_ShouldReturnFalse_WhenObjectIsInitialized()
    {
        // Act
        bool actual = _filterViewModel.AreAllFiltersApplied;

        // Assert
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void AreAllFiltersApplied_ShouldReturnTrue_WhenPropertyIsSetToTrue()
    {
        // Arrange
        bool expected = true;

        // Act
        _filterViewModel.AreAllFiltersApplied = expected;
        bool actual = _filterViewModel.AreAllFiltersApplied;

        // Assert
        Assert.IsTrue(actual);
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(_hasAreAllFiltersAppliedEventFired);
    }

    [TestMethod]
    public void AreAllFiltersApplied_ShouldReturnFalse_WhenPropertyIsSetToFalse()
    {
        // Arrange
        bool expected = false;

        // Act
        _filterViewModel.AreAllFiltersApplied = expected;
        bool actual = _filterViewModel.AreAllFiltersApplied;

        // Assert
        Assert.IsFalse(actual);
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(_hasAreAllFiltersAppliedEventFired);
    }

    [TestMethod]
    public void ChosenEventTypes_ShouldReturnEmptyDictionary_WhenObjectIsInitialized()
    {
        // Arrange
        int expected = 0;

        // Act
        int actual = _filterViewModel.ChosenEventTypes.Count;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Keyword_ShouldReturnNull_WhenObjectIsInitialized()
    {
        // Arrange
        string expected = "";

        // Act
        string actual = _filterViewModel.Keyword;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Keyword_ShouldReturnStringValue_WhenPropertyWasSet()
    {
        // Arrange
        string expected = "Test Keyword";
        _filterViewModel.Keyword = expected;

        // Act
        string actual = _filterViewModel.Keyword;

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(_hasKeywordEventFired);
    }

    [TestMethod]
    public void FromDate_ShouldBeSetToToday_WhenObjectIsInitialized()
    {
        // Arrange
        DateTime expected = new(2022, 10, 12);
        _iDateTimeProvider.SetupGet(mock => mock.Now).Returns(expected);
        FilterViewModel filterViewModel = new(_iFilteringStoreMock.Object, _eventTreeViewModel.Object, _iTimeValidatorMock.Object, _iFilteringUtils.Object, _iErrorsViewModelMock.Object, _iDateTimeProvider.Object);

        // Act
        DateTime actual = filterViewModel.FromDate;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void FromDate_ShouldAddErrors_WhenFullFromDateIsLaterWhenFullToDate()
    {
        // Arrange
        DateTime expected = new(2022, 10, 12);
        _iDateTimeProvider.SetupGet(mock => mock.Now).Returns(expected);
        FilterViewModel filterViewModel = new(_iFilteringStoreMock.Object, _eventTreeViewModel.Object, _iTimeValidatorMock.Object, _iFilteringUtils.Object, _iErrorsViewModelMock.Object, _iDateTimeProvider.Object);
        filterViewModel.PropertyChanged += this.ChangeEventFlag;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(FilterViewModel.FromDate), It.IsAny<string>())).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(FilterViewModel.ToDate), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(false);

        // Act
        filterViewModel.FromDate = expected;
        DateTime actual = filterViewModel.FromDate;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(FilterViewModel.FromDate), It.IsAny<string>()), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(FilterViewModel.ToDate), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasFromDateEventFired);
    }

    [TestMethod]
    public void FromDate_ShouldSetDate_WhenFromDateIsValid()
    {
        // Arrange
        DateTime expected = new(2022, 10, 12);
        _iDateTimeProvider.SetupGet(mock => mock.Now).Returns(expected);
        FilterViewModel filterViewModel = new(_iFilteringStoreMock.Object, _eventTreeViewModel.Object, _iTimeValidatorMock.Object, _iFilteringUtils.Object, _iErrorsViewModelMock.Object, _iDateTimeProvider.Object);
        filterViewModel.PropertyChanged += this.ChangeEventFlag;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(FilterViewModel.FromDate), It.IsAny<string>())).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(FilterViewModel.ToDate), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(true);

        // Act
        filterViewModel.FromDate = expected;
        DateTime actual = filterViewModel.FromDate;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(FilterViewModel.FromDate), It.IsAny<string>()), Times.Never());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(FilterViewModel.ToDate), It.IsAny<string>()), Times.Never());
        Assert.IsTrue(_hasFromDateEventFired);
    }

    [TestMethod]
    public void FromHours_ShouldBeSetToZero_WhenObjectIsInitialized()
    {
        // Arrange
        int expected = 0;

        // Act
        int actual = _filterViewModel.FromHours;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void FromHours_ShouldAddError_WhenHoursAreInvalid()
    {
        // Arrange
        int expected = 25;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.FromHours))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(FilterViewModel.FromHours), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreHoursValid(It.IsAny<int>())).Returns(false);

        // Act
        _filterViewModel.FromHours = expected;
        int actual = _filterViewModel.FromHours;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.FromHours)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(FilterViewModel.FromHours), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasFromHoursEventFired);
    }

    [TestMethod]
    public void FromHours_ShouldAddError_WhenDatesAreInvalid()
    {
        // Arrange
        int expected = 25;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.FromHours))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(FilterViewModel.FromHours), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreHoursValid(It.IsAny<int>())).Returns(false);
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(false);

        // Act
        _filterViewModel.FromHours = expected;
        int actual = _filterViewModel.FromHours;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.FromHours)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(FilterViewModel.FromHours), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasFromHoursEventFired);
    }

    [TestMethod]
    public void FromHours_ShouldSetHours_WhenFromHoursIsValid()
    {
        // Arrange
        int expected = 20;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.FromHours))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(FilterViewModel.FromHours), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreHoursValid(It.IsAny<int>())).Returns(true);
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(true);

        // Act
        _filterViewModel.FromHours = expected;
        int actual = _filterViewModel.FromHours;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.FromHours)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(FilterViewModel.FromHours), It.IsAny<string>()), Times.Never());
        Assert.IsTrue(_hasFromHoursEventFired);
    }

    [TestMethod]
    public void FromMinutes_ShouldBeSetToZero_WhenObjectIsInitialized()
    {
        // Arrange
        int expected = 0;

        // Act
        int actual = _filterViewModel.FromMinutes;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void FromMinutes_ShouldAddError_WhenMinutesAreInvalid()
    {
        // Arrange
        int expected = 61;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.FromMinutes))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(FilterViewModel.FromMinutes), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreMinutesValid(It.IsAny<int>())).Returns(false);

        // Act
        _filterViewModel.FromMinutes = expected;
        int actual = _filterViewModel.FromMinutes;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.FromMinutes)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(FilterViewModel.FromMinutes), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasFromMinutesEventFired);
    }

    [TestMethod]
    public void FromMinutes_ShouldAddError_WhenDatesAreInvalid()
    {
        // Arrange
        int expected = 61;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.FromMinutes))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(FilterViewModel.FromMinutes), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreMinutesValid(It.IsAny<int>())).Returns(false);
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(false);

        // Act
        _filterViewModel.FromMinutes = expected;
        int actual = _filterViewModel.FromMinutes;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.FromMinutes)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(FilterViewModel.FromMinutes), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasFromMinutesEventFired);
    }

    [TestMethod]
    public void FromMinutes_ShouldSetMinutes_WhenFromMinutesIsValid()
    {
        // Arrange
        int expected = 20;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.FromMinutes))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(FilterViewModel.FromMinutes), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreMinutesValid(It.IsAny<int>())).Returns(true);
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(true);

        // Act
        _filterViewModel.FromMinutes = expected;
        int actual = _filterViewModel.FromMinutes;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.FromMinutes)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(FilterViewModel.FromMinutes), It.IsAny<string>()), Times.Never());
        Assert.IsTrue(_hasFromMinutesEventFired);
    }

    [TestMethod]
    public void ToDate_ShouldBeSetToToday_WhenObjectIsInitialized()
    {
        // Arrange
        DateTime expected = new(2022, 10, 12);
        _iDateTimeProvider.SetupGet(mock => mock.Now).Returns(expected);
        FilterViewModel filterViewModel = new(_iFilteringStoreMock.Object, _eventTreeViewModel.Object, _iTimeValidatorMock.Object, _iFilteringUtils.Object, _iErrorsViewModelMock.Object, _iDateTimeProvider.Object);
        // Act
        DateTime actual = filterViewModel.ToDate;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ToDate_ShouldAddErrors_WhenFullToDateIsEarlierWhenFullFromDate()
    {
        // Arrange
        DateTime expected = new(2022, 10, 12);
        _iDateTimeProvider.SetupGet(mock => mock.Now).Returns(expected);
        FilterViewModel filterViewModel = new(_iFilteringStoreMock.Object, _eventTreeViewModel.Object, _iTimeValidatorMock.Object, _iFilteringUtils.Object, _iErrorsViewModelMock.Object, _iDateTimeProvider.Object);
        filterViewModel.PropertyChanged += this.ChangeEventFlag;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(FilterViewModel.FromDate), It.IsAny<string>())).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(FilterViewModel.ToDate), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(false);

        // Act
        filterViewModel.ToDate = expected;
        DateTime actual = filterViewModel.ToDate;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(FilterViewModel.FromDate), It.IsAny<string>()), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(FilterViewModel.ToDate), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasToDateEventFired);
    }

    [TestMethod]
    public void ToDate_ShouldSetDate_WhenToDateIsValid()
    {
        // Arrange
        DateTime expected = new(2022, 10, 12);
        _iDateTimeProvider.SetupGet(mock => mock.Now).Returns(expected);
        FilterViewModel filterViewModel = new(_iFilteringStoreMock.Object, _eventTreeViewModel.Object, _iTimeValidatorMock.Object, _iFilteringUtils.Object, _iErrorsViewModelMock.Object, _iDateTimeProvider.Object);
        filterViewModel.PropertyChanged += this.ChangeEventFlag;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(FilterViewModel.FromDate), It.IsAny<string>())).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(FilterViewModel.ToDate), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(true);

        // Act
        filterViewModel.ToDate = expected;
        DateTime actual = filterViewModel.ToDate;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(FilterViewModel.FromDate), It.IsAny<string>()), Times.Never());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(FilterViewModel.ToDate), It.IsAny<string>()), Times.Never());
        Assert.IsTrue(_hasToDateEventFired);
    }

    [TestMethod]
    public void ToHours_ShouldBeSetToZero_WhenObjectIsInitialized()
    {
        // Arrange
        int expected = 0;

        // Act
        int actual = _filterViewModel.ToHours;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ToHours_ShouldAddError_WhenHoursAreInvalid()
    {
        // Arrange
        int expected = 25;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.ToHours))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(FilterViewModel.ToHours), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreHoursValid(It.IsAny<int>())).Returns(false);

        // Act
        _filterViewModel.ToHours = expected;
        int actual = _filterViewModel.ToHours;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.ToHours)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(FilterViewModel.ToHours), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasToHoursEventFired);
    }

    [TestMethod]
    public void ToHours_ShouldAddError_WhenDatesAreInvalid()
    {
        // Arrange
        int expected = 25;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.ToHours))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(FilterViewModel.ToHours), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreHoursValid(It.IsAny<int>())).Returns(false);
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(false);

        // Act
        _filterViewModel.ToHours = expected;
        int actual = _filterViewModel.ToHours;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.ToHours)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(FilterViewModel.ToHours), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasToHoursEventFired);
    }

    [TestMethod]
    public void ToHours_ShouldSetHours_WhenFromHoursIsValid()
    {
        // Arrange
        int expected = 20;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.ToHours))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(FilterViewModel.ToHours), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreHoursValid(It.IsAny<int>())).Returns(true);
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(true);

        // Act
        _filterViewModel.ToHours = expected;
        int actual = _filterViewModel.ToHours;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.ToHours)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(FilterViewModel.ToHours), It.IsAny<string>()), Times.Never());
        Assert.IsTrue(_hasToHoursEventFired);
    }

    [TestMethod]
    public void ToMinutes_ShouldBeSetToZero_WhenObjectIsInitialized()
    {
        // Arrange
        int expected = 0;

        // Act
        int actual = _filterViewModel.ToMinutes;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ToMinutes_ShouldAddError_WhenMinutesAreInvalid()
    {
        // Arrange
        int expected = 61;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.ToMinutes))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(FilterViewModel.ToMinutes), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreMinutesValid(It.IsAny<int>())).Returns(false);

        // Act
        _filterViewModel.ToMinutes = expected;
        int actual = _filterViewModel.ToMinutes;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.ToMinutes)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(FilterViewModel.ToMinutes), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasToMinutesEventFired);
    }

    [TestMethod]
    public void ToMinutes_ShouldAddError_WhenDatesAreInvalid()
    {
        // Arrange
        int expected = 61;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.ToMinutes))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(FilterViewModel.ToMinutes), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreMinutesValid(It.IsAny<int>())).Returns(false);
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(false);

        // Act
        _filterViewModel.ToMinutes = expected;
        int actual = _filterViewModel.ToMinutes;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.ToMinutes)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(FilterViewModel.ToMinutes), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasToMinutesEventFired);
    }

    [TestMethod]
    public void ToMinutes_ShouldSetMinutes_WhenFromMinutesIsValid()
    {
        // Arrange
        int expected = 20;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.ToMinutes))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(FilterViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(FilterViewModel.ToMinutes), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreMinutesValid(It.IsAny<int>())).Returns(true);
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(true);

        // Act
        _filterViewModel.ToMinutes = expected;
        int actual = _filterViewModel.ToMinutes;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.ToMinutes)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(FilterViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(FilterViewModel.ToMinutes), It.IsAny<string>()), Times.Never());
        Assert.IsTrue(_hasToMinutesEventFired);
    }

    [TestMethod]
    public void HasErrors_ShouldReturnTrue_WhenThereAreErrors()
    {
        // Arrange
        bool expected = true;
        _iErrorsViewModelMock.Setup(mock => mock.HasErrors).Returns(expected);

        // Act
        bool actual = _filterViewModel.HasErrors;

        // Assert
        Assert.IsTrue(actual);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void HasErrors_ShouldReturnFalse_WhenThereAreNoErrors()
    {
        // Arrange
        bool expected = false;
        _iErrorsViewModelMock.Setup(mock => mock.HasErrors).Returns(expected);

        // Act
        bool actual = _filterViewModel.HasErrors;

        // Assert
        Assert.IsFalse(actual);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void InitializeCommand_ShouldReturnCommand_WhenObjectIsInitialized()
    {
        // Arrange
        Type expected = typeof(InitializeEventTypesCommand);

        // Act
        ICommand command = _filterViewModel.InitializeCommand;
        Type actual = command.GetType();

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(command is InitializeEventTypesCommand);
    }

    [TestMethod]
    public void FilterChangedCommand_ShouldReturnCommand_WhenObjectIsInitialized()
    {
        // Arrange
        Type expected = typeof(FilterTypeChangedCommand);

        // Act
        ICommand command = _filterViewModel.FilterChangedCommand;
        Type actual = command.GetType();

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(command is FilterTypeChangedCommand);
    }

    [TestMethod]
    public void FilterCheckedCommand_ShouldReturnCommand_WhenObjectIsInitialized()
    {
        // Arrange
        Type expected = typeof(FilterTypeCheckedCommand);

        // Act
        ICommand command = _filterViewModel.FilterCheckedCommand;
        Type actual = command.GetType();

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(command is FilterTypeCheckedCommand);
    }

    [TestMethod]
    public void ApplyCommand_ShouldReturnCommand_WhenObjectIsInitialized()
    {
        // Arrange
        Type expected = typeof(ApplyFilterOptionsCommand);

        // Act
        ICommand command = _filterViewModel.ApplyCommand;
        Type actual = command.GetType();

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(command is ApplyFilterOptionsCommand);
    }

    [TestMethod]
    public void FilterCommand_ShouldReturnCommand_WhenObjectIsInitialized()
    {
        // Arrange
        Type expected = typeof(ApplyFiltersCommand);

        // Act
        ICommand command = _filterViewModel.FilterCommand;
        Type actual = command.GetType();

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(command is ApplyFiltersCommand);
    }

    [TestMethod]
    public void GetErrors_ShouldReturnErrorMessages_WhenThereAreErrorsForGivenKey()
    {
        // Arrange
        List<string> expected = new()
        {
            "Error1",
            "Error2"
        };
        string key = "Test";
        _iErrorsViewModelMock.Setup(mock => mock.GetErrors(key)).Returns(expected);

        // Act
        List<string> actual = _filterViewModel.GetErrors(key).Cast<string>().ToList();

        // Assert
        Assert.AreEqual(expected.Count, actual.Count);

        for (int i = 0; i < expected.Count; i++)
        {
            Assert.AreEqual(expected[i], actual[i]);
        }
    }

    [TestMethod]
    public void GetErrors_ShouldReturnEmptyList_WhenThereAreNoErrorsForGivenKey()
    {
        // Arrange
        List<string> expected = new();
        string key = "Test";
        _iErrorsViewModelMock.Setup(mock => mock.GetErrors(key)).Returns(expected);

        // Act
        List<string> actual = _filterViewModel.GetErrors(key).Cast<string>().ToList();

        // Assert
        Assert.AreEqual(expected.Count, actual.Count);
    }

    [TestMethod]
    public void UpdateChosenEventType_ShouldAddNewChosenType_WhenKeyDoesNotExist()
    {
        // Arrange
        string key = "Created";
        bool value = true;
        int expectedCount = 1;

        // Act
        _filterViewModel.UpdateChosenEventType(key, value);
        int actualCount = _filterViewModel.ChosenEventTypes.Count;
        bool isInDictionary = _filterViewModel.ChosenEventTypes.TryGetValue(key, out bool actualValue);

        // Assert
        Assert.AreEqual(expectedCount, actualCount);
        Assert.IsTrue(isInDictionary);
        Assert.AreEqual(value, actualValue);
        Assert.IsTrue(_hasChosenEventTypesEventFired);
    }

    [TestMethod]
    public void UpdateChosenEventType_ShouldUpdateExistingChosenType_WhenKeyExists()
    {
        // Arrange
        string key = "Created";
        bool value = true;
        _filterViewModel.UpdateChosenEventType(key, value);
        bool newValue = false;
        int expectedCount = 1;

        // Act
        _filterViewModel.UpdateChosenEventType(key, newValue);
        int actualCount = _filterViewModel.ChosenEventTypes.Count;
        bool isInDictionary = _filterViewModel.ChosenEventTypes.TryGetValue(key, out bool actualValue);

        // Assert
        Assert.AreEqual(expectedCount, actualCount);
        Assert.IsTrue(isInDictionary);
        Assert.AreEqual(newValue, actualValue);
        Assert.IsTrue(_hasChosenEventTypesEventFired);
    }
}
