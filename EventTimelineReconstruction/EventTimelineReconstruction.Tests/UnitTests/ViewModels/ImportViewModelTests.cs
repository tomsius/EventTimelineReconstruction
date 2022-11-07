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
public class ImportViewModelTests
{
    private readonly Mock<EventTreeViewModel> _eventTreeViewModel;
    private readonly Mock<IEventsStore> _iEventsStoreMock;
    private readonly Mock<ITimeValidator> _iTimeValidatorMock;
    private readonly Mock<IErrorsViewModel> _iErrorsViewModelMock;
    private readonly Mock<IDateTimeProvider> _iDateTimeProvider;
    private readonly ImportViewModel _importViewModel;

    public ImportViewModelTests()
    {
        Mock<EventDetailsViewModel> eventDetailsViewModelMock = new();
        Mock<IFilteringStore> iFilteringStoreMock = new();
        Mock<ChangeColourViewModel> changeColourViewModelMock = new(eventDetailsViewModelMock.Object);
        Mock<IDragDropUtils> iDragDropUtilsMock = new();
        _eventTreeViewModel = new(eventDetailsViewModelMock.Object, iFilteringStoreMock.Object, changeColourViewModelMock.Object, iDragDropUtilsMock.Object);
        _iEventsStoreMock = new();
        _iTimeValidatorMock = new();
        _iErrorsViewModelMock = new();
        _iDateTimeProvider = new();
        _importViewModel = new(_eventTreeViewModel.Object, _iEventsStoreMock.Object, _iTimeValidatorMock.Object, _iErrorsViewModelMock.Object, _iDateTimeProvider.Object);
        _importViewModel.PropertyChanged += this.ChangeEventFlag;
    }

    private bool _hasFileNameEventFired;
    private bool _hasFromDateEventFired;
    private bool _hasToDateEventFired;
    private bool _hasFromHoursEventFired;
    private bool _hasFromMinutesEventFired;
    private bool _hasToHoursEventFired;
    private bool _hasToMinutesEventFired;
    private bool _hasIsImportingEventFired;

    private void ChangeEventFlag(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(ImportViewModel.FileName):
                _hasFileNameEventFired = true;
                break;
            case nameof(ImportViewModel.FromDate):
                _hasFromDateEventFired = true;
                break;
            case nameof(ImportViewModel.ToDate):
                _hasToDateEventFired = true;
                break;
            case nameof(ImportViewModel.FromHours):
                _hasFromHoursEventFired = true;
                break;
            case nameof(ImportViewModel.FromMinutes):
                _hasFromMinutesEventFired = true;
                break;
            case nameof(ImportViewModel.ToHours):
                _hasToHoursEventFired = true;
                break;
            case nameof(ImportViewModel.ToMinutes):
                _hasToMinutesEventFired = true;
                break;
            case nameof(ImportViewModel.IsImporting):
                _hasIsImportingEventFired = true;
                break;
        }
    }

    [TestInitialize]
    public void Setup()
    {
        _hasFileNameEventFired = false;
        _hasFromDateEventFired = false;
        _hasToDateEventFired = false;
        _hasFromHoursEventFired = false;
        _hasFromMinutesEventFired = false;
        _hasToHoursEventFired = false;
        _hasToMinutesEventFired = false;
        _hasIsImportingEventFired = false;
    }

    [TestMethod]
    public void ErrorsViewModel_ShouldReturnObject_WhenObjectIsInitialized()
    {
        // Act
        IErrorsViewModel actual = _importViewModel.ErrorsViewModel;

        // Assert
        Assert.IsNotNull(actual);
    }

    [TestMethod]
    public void FileName_ShouldReturnNull_WhenObjectIsInitialized()
    {
        // Act
        string actual = _importViewModel.FileName;

        // Assert
        Assert.IsNull(actual);
    }

    [TestMethod]
    public void FileName_ShouldAddError_WhenFileNameValueIsNull()
    {
        // Arrange
        string? fileName = null;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.FileName))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(ImportViewModel.FileName), It.IsAny<string>())).Verifiable();

        // Act
        _importViewModel.FileName = fileName;

        // Assert
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.FileName)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(ImportViewModel.FileName), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasFileNameEventFired);
    }

    [TestMethod]
    public void FileName_ShouldAddError_WhenFileNameValueIsEmptyString()
    {
        // Arrange
        string fileName = "";
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.FileName))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(ImportViewModel.FileName), It.IsAny<string>())).Verifiable();

        // Act
        _importViewModel.FileName = fileName;

        // Assert
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.FileName)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(ImportViewModel.FileName), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasFileNameEventFired);
    }

    [TestMethod]
    public void FileName_ShouldSetFileNameValue_WhenFileNameValueIsValid()
    {
        // Arrange
        string fileName = "Test";
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.FileName))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(ImportViewModel.FileName), It.IsAny<string>())).Verifiable();

        // Act
        _importViewModel.FileName = fileName;

        // Assert
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.FileName)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(ImportViewModel.FileName), It.IsAny<string>()), Times.Never());
        Assert.IsTrue(_hasFileNameEventFired);
    }

    [TestMethod]
    public void FromDate_ShouldBeSetToToday_WhenObjectIsInitialized()
    {
        // Arrange
        DateTime expected = new(2022, 10, 12);
        _iDateTimeProvider.SetupGet(mock => mock.Now).Returns(expected);
        ImportViewModel importViewModel = new(_eventTreeViewModel.Object, _iEventsStoreMock.Object, _iTimeValidatorMock.Object, _iErrorsViewModelMock.Object, _iDateTimeProvider.Object);

        // Act
        DateTime actual = importViewModel.FromDate;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void FromDate_ShouldAddErrors_WhenFullFromDateIsLaterWhenFullToDate()
    {
        // Arrange
        DateTime expected = new(2022, 10, 12);
        _iDateTimeProvider.SetupGet(mock => mock.Now).Returns(expected);
        ImportViewModel importViewModel = new(_eventTreeViewModel.Object, _iEventsStoreMock.Object, _iTimeValidatorMock.Object, _iErrorsViewModelMock.Object, _iDateTimeProvider.Object);
        importViewModel.PropertyChanged += this.ChangeEventFlag;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(ImportViewModel.FromDate), It.IsAny<string>())).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(ImportViewModel.ToDate), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(false);

        // Act
        importViewModel.FromDate = expected;
        DateTime actual = importViewModel.FromDate;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(ImportViewModel.FromDate), It.IsAny<string>()), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(ImportViewModel.ToDate), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasFromDateEventFired);
    }

    [TestMethod]
    public void FromDate_ShouldSetDate_WhenFromDateIsValid()
    {
        // Arrange
        DateTime expected = new(2022, 10, 12);
        _iDateTimeProvider.SetupGet(mock => mock.Now).Returns(expected);
        ImportViewModel importViewModel = new(_eventTreeViewModel.Object, _iEventsStoreMock.Object, _iTimeValidatorMock.Object, _iErrorsViewModelMock.Object, _iDateTimeProvider.Object);
        importViewModel.PropertyChanged += this.ChangeEventFlag;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(ImportViewModel.FromDate), It.IsAny<string>())).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(ImportViewModel.ToDate), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(true);

        // Act
        importViewModel.FromDate = expected;
        DateTime actual = importViewModel.FromDate;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(ImportViewModel.FromDate), It.IsAny<string>()), Times.Never());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(ImportViewModel.ToDate), It.IsAny<string>()), Times.Never());
        Assert.IsTrue(_hasFromDateEventFired);
    }

    [TestMethod]
    public void FromHours_ShouldBeSetToZero_WhenObjectIsInitialized()
    {
        // Arrange
        int expected = 0;

        // Act
        int actual = _importViewModel.FromHours;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void FromHours_ShouldAddError_WhenHoursAreInvalid()
    {
        // Arrange
        int expected = 25;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.FromHours))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(ImportViewModel.FromHours), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreHoursValid(It.IsAny<int>())).Returns(false);

        // Act
        _importViewModel.FromHours = expected;
        int actual = _importViewModel.FromHours;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.FromHours)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(ImportViewModel.FromHours), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasFromHoursEventFired);
    }

    [TestMethod]
    public void FromHours_ShouldAddError_WhenDatesAreInvalid()
    {
        // Arrange
        int expected = 25;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.FromHours))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(ImportViewModel.FromHours), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreHoursValid(It.IsAny<int>())).Returns(false);
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(false);

        // Act
        _importViewModel.FromHours = expected;
        int actual = _importViewModel.FromHours;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.FromHours)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(ImportViewModel.FromHours), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasFromHoursEventFired);
    }

    [TestMethod]
    public void FromHours_ShouldSetHours_WhenFromHoursIsValid()
    {
        // Arrange
        int expected = 20;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.FromHours))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(ImportViewModel.FromHours), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreHoursValid(It.IsAny<int>())).Returns(true);
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(true);

        // Act
        _importViewModel.FromHours = expected;
        int actual = _importViewModel.FromHours;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.FromHours)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(ImportViewModel.FromHours), It.IsAny<string>()), Times.Never());
        Assert.IsTrue(_hasFromHoursEventFired);
    }

    [TestMethod]
    public void FromMinutes_ShouldBeSetToZero_WhenObjectIsInitialized()
    {
        // Arrange
        int expected = 0;

        // Act
        int actual = _importViewModel.FromMinutes;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void FromMinutes_ShouldAddError_WhenMinutesAreInvalid()
    {
        // Arrange
        int expected = 61;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.FromMinutes))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(ImportViewModel.FromMinutes), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreMinutesValid(It.IsAny<int>())).Returns(false);

        // Act
        _importViewModel.FromMinutes = expected;
        int actual = _importViewModel.FromMinutes;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.FromMinutes)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(ImportViewModel.FromMinutes), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasFromMinutesEventFired);
    }

    [TestMethod]
    public void FromMinutes_ShouldAddError_WhenDatesAreInvalid()
    {
        // Arrange
        int expected = 61;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.FromMinutes))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(ImportViewModel.FromMinutes), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreMinutesValid(It.IsAny<int>())).Returns(false);
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(false);

        // Act
        _importViewModel.FromMinutes = expected;
        int actual = _importViewModel.FromMinutes;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.FromMinutes)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(ImportViewModel.FromMinutes), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasFromMinutesEventFired);
    }

    [TestMethod]
    public void FromMinutes_ShouldSetMinutes_WhenFromMinutesIsValid()
    {
        // Arrange
        int expected = 20;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.FromMinutes))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(ImportViewModel.FromMinutes), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreMinutesValid(It.IsAny<int>())).Returns(true);
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(true);

        // Act
        _importViewModel.FromMinutes = expected;
        int actual = _importViewModel.FromMinutes;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.FromMinutes)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(ImportViewModel.FromMinutes), It.IsAny<string>()), Times.Never());
        Assert.IsTrue(_hasFromMinutesEventFired);
    }

    [TestMethod]
    public void ToDate_ShouldBeSetToToday_WhenObjectIsInitialized()
    {
        // Arrange
        DateTime expected = new(2022, 10, 12);
        _iDateTimeProvider.SetupGet(mock => mock.Now).Returns(expected);
        ImportViewModel importViewModel = new(_eventTreeViewModel.Object, _iEventsStoreMock.Object, _iTimeValidatorMock.Object, _iErrorsViewModelMock.Object, _iDateTimeProvider.Object);
        // Act
        DateTime actual = importViewModel.ToDate;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ToDate_ShouldAddErrors_WhenFullToDateIsEarlierWhenFullFromDate()
    {
        // Arrange
        DateTime expected = new(2022, 10, 12);
        _iDateTimeProvider.SetupGet(mock => mock.Now).Returns(expected);
        ImportViewModel importViewModel = new(_eventTreeViewModel.Object, _iEventsStoreMock.Object, _iTimeValidatorMock.Object, _iErrorsViewModelMock.Object, _iDateTimeProvider.Object);
        importViewModel.PropertyChanged += this.ChangeEventFlag;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(ImportViewModel.FromDate), It.IsAny<string>())).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(ImportViewModel.ToDate), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(false);

        // Act
        importViewModel.ToDate = expected;
        DateTime actual = importViewModel.ToDate;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(ImportViewModel.FromDate), It.IsAny<string>()), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(ImportViewModel.ToDate), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasToDateEventFired);
    }

    [TestMethod]
    public void ToDate_ShouldSetDate_WhenToDateIsValid()
    {
        // Arrange
        DateTime expected = new(2022, 10, 12);
        _iDateTimeProvider.SetupGet(mock => mock.Now).Returns(expected);
        ImportViewModel importViewModel = new(_eventTreeViewModel.Object, _iEventsStoreMock.Object, _iTimeValidatorMock.Object, _iErrorsViewModelMock.Object, _iDateTimeProvider.Object);
        importViewModel.PropertyChanged += this.ChangeEventFlag;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(ImportViewModel.FromDate), It.IsAny<string>())).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(ImportViewModel.ToDate), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(true);

        // Act
        importViewModel.ToDate = expected;
        DateTime actual = importViewModel.ToDate;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(ImportViewModel.FromDate), It.IsAny<string>()), Times.Never());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(ImportViewModel.ToDate), It.IsAny<string>()), Times.Never());
        Assert.IsTrue(_hasToDateEventFired);
    }

    [TestMethod]
    public void ToHours_ShouldBeSetToZero_WhenObjectIsInitialized()
    {
        // Arrange
        int expected = 0;

        // Act
        int actual = _importViewModel.ToHours;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ToHours_ShouldAddError_WhenHoursAreInvalid()
    {
        // Arrange
        int expected = 25;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.ToHours))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(ImportViewModel.ToHours), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreHoursValid(It.IsAny<int>())).Returns(false);

        // Act
        _importViewModel.ToHours = expected;
        int actual = _importViewModel.ToHours;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.ToHours)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(ImportViewModel.ToHours), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasToHoursEventFired);
    }

    [TestMethod]
    public void ToHours_ShouldAddError_WhenDatesAreInvalid()
    {
        // Arrange
        int expected = 25;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.ToHours))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(ImportViewModel.ToHours), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreHoursValid(It.IsAny<int>())).Returns(false);
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(false);

        // Act
        _importViewModel.ToHours = expected;
        int actual = _importViewModel.ToHours;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.ToHours)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(ImportViewModel.ToHours), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasToHoursEventFired);
    }

    [TestMethod]
    public void ToHours_ShouldSetHours_WhenFromHoursIsValid()
    {
        // Arrange
        int expected = 20;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.ToHours))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(ImportViewModel.ToHours), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreHoursValid(It.IsAny<int>())).Returns(true);
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(true);

        // Act
        _importViewModel.ToHours = expected;
        int actual = _importViewModel.ToHours;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.ToHours)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(ImportViewModel.ToHours), It.IsAny<string>()), Times.Never());
        Assert.IsTrue(_hasToHoursEventFired);
    }

    [TestMethod]
    public void ToMinutes_ShouldBeSetToZero_WhenObjectIsInitialized()
    {
        // Arrange
        int expected = 0;

        // Act
        int actual = _importViewModel.ToMinutes;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ToMinutes_ShouldAddError_WhenMinutesAreInvalid()
    {
        // Arrange
        int expected = 61;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.ToMinutes))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(ImportViewModel.ToMinutes), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreMinutesValid(It.IsAny<int>())).Returns(false);

        // Act
        _importViewModel.ToMinutes = expected;
        int actual = _importViewModel.ToMinutes;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.ToMinutes)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(ImportViewModel.ToMinutes), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasToMinutesEventFired);
    }

    [TestMethod]
    public void ToMinutes_ShouldAddError_WhenDatesAreInvalid()
    {
        // Arrange
        int expected = 61;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.ToMinutes))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(ImportViewModel.ToMinutes), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreMinutesValid(It.IsAny<int>())).Returns(false);
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(false);

        // Act
        _importViewModel.ToMinutes = expected;
        int actual = _importViewModel.ToMinutes;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.ToMinutes)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(ImportViewModel.ToMinutes), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasToMinutesEventFired);
    }

    [TestMethod]
    public void ToMinutes_ShouldSetMinutes_WhenFromMinutesIsValid()
    {
        // Arrange
        int expected = 20;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.ToMinutes))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(ImportViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(ImportViewModel.ToMinutes), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreMinutesValid(It.IsAny<int>())).Returns(true);
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(true);

        // Act
        _importViewModel.ToMinutes = expected;
        int actual = _importViewModel.ToMinutes;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.ToMinutes)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(ImportViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(ImportViewModel.ToMinutes), It.IsAny<string>()), Times.Never());
        Assert.IsTrue(_hasToMinutesEventFired);
    }

    [TestMethod]
    public void IsImporting_ShouldReturnFalse_WhenObjectIsInitialized()
    {
        // Act
        bool actual = _importViewModel.IsImporting;

        // Assert
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void IsImporting_ShouldReturnTrue_WhenPropertyIsSetToTrue()
    {
        // Arrange
        bool expected = true;

        // Act
        _importViewModel.IsImporting = expected;
        bool actual = _importViewModel.IsImporting;

        // Assert
        Assert.IsTrue(actual);
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(_hasIsImportingEventFired);
    }

    [TestMethod]
    public void IsImporting_ShouldReturnFalse_WhenPropertyIsSetToFalse()
    {
        // Arrange
        bool expected = false;

        // Act
        _importViewModel.IsImporting = expected;
        bool actual = _importViewModel.IsImporting;

        // Assert
        Assert.IsFalse(actual);
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(_hasIsImportingEventFired);
    }

    [TestMethod]
    public void HasErrors_ShouldReturnTrue_WhenThereAreErrors()
    {
        // Arrange
        bool expected = true;
        _iErrorsViewModelMock.Setup(mock => mock.HasErrors).Returns(expected);

        // Act
        bool actual = _importViewModel.HasErrors;

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
        bool actual = _importViewModel.HasErrors;

        // Assert
        Assert.IsFalse(actual);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ChooseFileCommand_ShouldReturnCommand_WhenObjectIsInitialized()
    {
        // Arrange
        Type expected = typeof(ChooseLoadFileCommand);

        // Act
        ICommand command = _importViewModel.ChooseFileCommand;
        Type actual = command.GetType();

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(command is ChooseLoadFileCommand);
    }

    [TestMethod]
    public void ImportCommand_ShouldReturnCommand_WhenObjectIsInitialized()
    {
        // Arrange
        Type expected = typeof(ImportEventsCommand);

        // Act
        ICommand command = _importViewModel.ImportCommand;
        Type actual = command.GetType();

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(command is ImportEventsCommand);
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
        List<string> actual = _importViewModel.GetErrors(key).Cast<string>().ToList();

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
        List<string> actual = _importViewModel.GetErrors(key).Cast<string>().ToList();

        // Assert
        Assert.AreEqual(expected.Count, actual.Count);
    }

    [TestMethod]
    public void FullFromDate_ShouldReturnDateWithTime_WhenPropertyIsCalled()
    {
        // Arrange
        DateTime expected = new(2022, 10, 12, 12, 15, 0);
        _iDateTimeProvider.SetupGet(mock => mock.Now).Returns(expected);
        ImportViewModel importViewModel = new(_eventTreeViewModel.Object, _iEventsStoreMock.Object, _iTimeValidatorMock.Object, _iErrorsViewModelMock.Object, _iDateTimeProvider.Object)
        {
            FromHours = 12,
            FromMinutes = 15
        };

        // Act
        DateTime actual = importViewModel.FullFromDate;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void FullToDate_ShouldReturnDateWithTime_WhenPropertyIsCalled()
    {
        // Arrange
        DateTime expected = new(2022, 10, 12, 12, 15, 0);
        _iDateTimeProvider.SetupGet(mock => mock.Now).Returns(expected);
        ImportViewModel importViewModel = new(_eventTreeViewModel.Object, _iEventsStoreMock.Object, _iTimeValidatorMock.Object, _iErrorsViewModelMock.Object, _iDateTimeProvider.Object)
        {
            ToHours = 12,
            ToMinutes = 15
        };

        // Act
        DateTime actual = importViewModel.FullToDate;

        // Assert
        Assert.AreEqual(expected, actual);
    }
}
