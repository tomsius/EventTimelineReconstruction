using System.ComponentModel;
using System.Windows;
using EventTimelineReconstruction.Commands;
using System.Windows.Input;
using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.Validators;
using EventTimelineReconstruction.ViewModels;
using Moq;

namespace EventTimelineReconstruction.UnitTests.ViewModels;

[TestClass]
public class IntegrityViewModelTests
{
    private readonly Mock<IEventsStore> _iEventsStoreMock;
    private readonly Mock<IHashCalculator> _iHashCalculatorMock;
    private readonly Mock<IEventsImporter> _iEventsImporterMock;
    private readonly Mock<ITimeValidator> _iTimeValidatorMock;
    private readonly Mock<IErrorsViewModel> _iErrorsViewModelMock;
    private readonly Mock<IDateTimeProvider> _iDateTimeProvider;
    private readonly IntegrityViewModel _integrityViewModel;

    public IntegrityViewModelTests()
    {
        _iEventsStoreMock = new();
        _iHashCalculatorMock = new();
        _iEventsImporterMock = new();
        _iTimeValidatorMock = new();
        _iErrorsViewModelMock = new();
        _iDateTimeProvider = new();
        _integrityViewModel = new(_iEventsStoreMock.Object, _iHashCalculatorMock.Object, _iEventsImporterMock.Object, _iTimeValidatorMock.Object, _iErrorsViewModelMock.Object, _iDateTimeProvider.Object);
        _integrityViewModel.PropertyChanged += this.ChangeEventFlag;
    }

    private bool _hasFileNameEventFired;
    private bool _hasFromDateEventFired;
    private bool _hasToDateEventFired;
    private bool _hasFromHoursEventFired;
    private bool _hasFromMinutesEventFired;
    private bool _hasToHoursEventFired;
    private bool _hasToMinutesEventFired;
    private bool _hasHashValueEventFired;
    private bool _hasIsCheckingEventFired;

    private void ChangeEventFlag(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(IntegrityViewModel.FileName):
                _hasFileNameEventFired = true;
                break;
            case nameof(IntegrityViewModel.FromDate):
                _hasFromDateEventFired = true;
                break;
            case nameof(IntegrityViewModel.ToDate):
                _hasToDateEventFired = true;
                break;
            case nameof(IntegrityViewModel.FromHours):
                _hasFromHoursEventFired = true;
                break;
            case nameof(IntegrityViewModel.FromMinutes):
                _hasFromMinutesEventFired = true;
                break;
            case nameof(IntegrityViewModel.ToHours):
                _hasToHoursEventFired = true;
                break;
            case nameof(IntegrityViewModel.ToMinutes):
                _hasToMinutesEventFired = true;
                break;
            case nameof(IntegrityViewModel.HashValue):
                _hasHashValueEventFired = true;
                break;
            case nameof(IntegrityViewModel.IsChecking):
                _hasIsCheckingEventFired = true;
                break;
        }
    }

    [ClassInitialize]
    public static void Initialize(TestContext context)
    {
        var app = new Application();
    }

    [TestInitialize]
    public void Setup()
    {
        _hasFileNameEventFired = false;
        _hasFileNameEventFired = false;
        _hasFromDateEventFired = false;
        _hasToDateEventFired = false;
        _hasFromHoursEventFired = false;
        _hasFromMinutesEventFired = false;
        _hasToHoursEventFired = false;
        _hasToMinutesEventFired = false;
        _hasHashValueEventFired = false;
        _hasIsCheckingEventFired = false;
}

    [TestMethod]
    public void ErrorsViewModel_ShouldReturnObject_WhenObjectIsInitialized()
    {
        // Act
        IErrorsViewModel actual = _integrityViewModel.ErrorsViewModel;

        // Assert
        Assert.IsNotNull(actual);
    }

    [TestMethod]
    public void FileName_ShouldReturnNull_WhenObjectIsInitialized()
    {
        // Act
        string actual = _integrityViewModel.FileName;

        // Assert
        Assert.IsNull(actual);
    }

    [TestMethod]
    public void FileName_ShouldAddError_WhenFileNameValueIsNull()
    {
        // Arrange
        string? fileName = null;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.FileName))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(IntegrityViewModel.FileName), It.IsAny<string>())).Verifiable();

        // Act
        _integrityViewModel.FileName = fileName;

        // Assert
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.FileName)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(IntegrityViewModel.FileName), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasFileNameEventFired);
    }

    [TestMethod]
    public void FileName_ShouldAddError_WhenFileNameValueIsEmptyString()
    {
        // Arrange
        string fileName = "";
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.FileName))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(IntegrityViewModel.FileName), It.IsAny<string>())).Verifiable();

        // Act
        _integrityViewModel.FileName = fileName;

        // Assert
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.FileName)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(IntegrityViewModel.FileName), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasFileNameEventFired);
    }

    [TestMethod]
    public void FileName_ShouldSetFileNameValue_WhenFileNameValueIsValid()
    {
        // Arrange
        string fileName = "Test";
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.FileName))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(IntegrityViewModel.FileName), It.IsAny<string>())).Verifiable();

        // Act
        _integrityViewModel.FileName = fileName;

        // Assert
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.FileName)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(IntegrityViewModel.FileName), It.IsAny<string>()), Times.Never());
        Assert.IsTrue(_hasFileNameEventFired);
    }

    [TestMethod]
    public void FromDate_ShouldBeSetToToday_WhenObjectIsInitialized()
    {
        // Arrange
        DateTime expected = new(2022, 10, 12);
        _iDateTimeProvider.SetupGet(mock => mock.Now).Returns(expected);
        IntegrityViewModel integrityViewModel = new(_iEventsStoreMock.Object, _iHashCalculatorMock.Object, _iEventsImporterMock.Object, _iTimeValidatorMock.Object, _iErrorsViewModelMock.Object, _iDateTimeProvider.Object);

        // Act
        DateTime actual = integrityViewModel.FromDate;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void FromDate_ShouldAddErrors_WhenFullFromDateIsLaterWhenFullToDate()
    {
        // Arrange
        DateTime expected = new(2022, 10, 12);
        _iDateTimeProvider.SetupGet(mock => mock.Now).Returns(expected);
        IntegrityViewModel integrityViewModel = new(_iEventsStoreMock.Object, _iHashCalculatorMock.Object, _iEventsImporterMock.Object, _iTimeValidatorMock.Object, _iErrorsViewModelMock.Object, _iDateTimeProvider.Object);
        integrityViewModel.PropertyChanged += this.ChangeEventFlag;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(IntegrityViewModel.FromDate), It.IsAny<string>())).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(IntegrityViewModel.ToDate), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(false);

        // Act
        integrityViewModel.FromDate = expected;
        DateTime actual = integrityViewModel.FromDate;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(IntegrityViewModel.FromDate), It.IsAny<string>()), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(IntegrityViewModel.ToDate), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasFromDateEventFired);
    }

    [TestMethod]
    public void FromDate_ShouldSetDate_WhenFromDateIsValid()
    {
        // Arrange
        DateTime expected = new(2022, 10, 12);
        _iDateTimeProvider.SetupGet(mock => mock.Now).Returns(expected);
        IntegrityViewModel integrityViewModel = new(_iEventsStoreMock.Object, _iHashCalculatorMock.Object, _iEventsImporterMock.Object, _iTimeValidatorMock.Object, _iErrorsViewModelMock.Object, _iDateTimeProvider.Object);
        integrityViewModel.PropertyChanged += this.ChangeEventFlag;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(IntegrityViewModel.FromDate), It.IsAny<string>())).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(IntegrityViewModel.ToDate), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(true);

        // Act
        integrityViewModel.FromDate = expected;
        DateTime actual = integrityViewModel.FromDate;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(IntegrityViewModel.FromDate), It.IsAny<string>()), Times.Never());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(IntegrityViewModel.ToDate), It.IsAny<string>()), Times.Never());
        Assert.IsTrue(_hasFromDateEventFired);
    }

    [TestMethod]
    public void FromHours_ShouldBeSetToZero_WhenObjectIsInitialized()
    {
        // Arrange
        int expected = 0;

        // Act
        int actual = _integrityViewModel.FromHours;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void FromHours_ShouldAddError_WhenHoursAreInvalid()
    {
        // Arrange
        int expected = 25;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromHours))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(IntegrityViewModel.FromHours), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreHoursValid(It.IsAny<int>())).Returns(false);

        // Act
        _integrityViewModel.FromHours = expected;
        int actual = _integrityViewModel.FromHours;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromHours)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(IntegrityViewModel.FromHours), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasFromHoursEventFired);
    }

    [TestMethod]
    public void FromHours_ShouldAddError_WhenDatesAreInvalid()
    {
        // Arrange
        int expected = 25;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromHours))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(IntegrityViewModel.FromHours), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreHoursValid(It.IsAny<int>())).Returns(false);
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(false);

        // Act
        _integrityViewModel.FromHours = expected;
        int actual = _integrityViewModel.FromHours;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromHours)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(IntegrityViewModel.FromHours), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasFromHoursEventFired);
    }

    [TestMethod]
    public void FromHours_ShouldSetHours_WhenFromHoursIsValid()
    {
        // Arrange
        int expected = 20;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromHours))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(IntegrityViewModel.FromHours), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreHoursValid(It.IsAny<int>())).Returns(true);
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(true);

        // Act
        _integrityViewModel.FromHours = expected;
        int actual = _integrityViewModel.FromHours;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromHours)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(IntegrityViewModel.FromHours), It.IsAny<string>()), Times.Never());
        Assert.IsTrue(_hasFromHoursEventFired);
    }

    [TestMethod]
    public void FromMinutes_ShouldBeSetToZero_WhenObjectIsInitialized()
    {
        // Arrange
        int expected = 0;

        // Act
        int actual = _integrityViewModel.FromMinutes;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void FromMinutes_ShouldAddError_WhenMinutesAreInvalid()
    {
        // Arrange
        int expected = 61;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromMinutes))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(IntegrityViewModel.FromMinutes), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreMinutesValid(It.IsAny<int>())).Returns(false);

        // Act
        _integrityViewModel.FromMinutes = expected;
        int actual = _integrityViewModel.FromMinutes;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromMinutes)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(IntegrityViewModel.FromMinutes), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasFromMinutesEventFired);
    }

    [TestMethod]
    public void FromMinutes_ShouldAddError_WhenDatesAreInvalid()
    {
        // Arrange
        int expected = 61;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromMinutes))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(IntegrityViewModel.FromMinutes), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreMinutesValid(It.IsAny<int>())).Returns(false);
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(false);

        // Act
        _integrityViewModel.FromMinutes = expected;
        int actual = _integrityViewModel.FromMinutes;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromMinutes)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(IntegrityViewModel.FromMinutes), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasFromMinutesEventFired);
    }

    [TestMethod]
    public void FromMinutes_ShouldSetMinutes_WhenFromMinutesIsValid()
    {
        // Arrange
        int expected = 20;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromMinutes))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(IntegrityViewModel.FromMinutes), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreMinutesValid(It.IsAny<int>())).Returns(true);
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(true);

        // Act
        _integrityViewModel.FromMinutes = expected;
        int actual = _integrityViewModel.FromMinutes;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromMinutes)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(IntegrityViewModel.FromMinutes), It.IsAny<string>()), Times.Never());
        Assert.IsTrue(_hasFromMinutesEventFired);
    }

    [TestMethod]
    public void ToDate_ShouldBeSetToToday_WhenObjectIsInitialized()
    {
        // Arrange
        DateTime expected = new(2022, 10, 12);
        _iDateTimeProvider.SetupGet(mock => mock.Now).Returns(expected);
        IntegrityViewModel integrityViewModel = new(_iEventsStoreMock.Object, _iHashCalculatorMock.Object, _iEventsImporterMock.Object, _iTimeValidatorMock.Object, _iErrorsViewModelMock.Object, _iDateTimeProvider.Object);

        // Act
        DateTime actual = integrityViewModel.ToDate;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ToDate_ShouldAddErrors_WhenFullToDateIsEarlierWhenFullFromDate()
    {
        // Arrange
        DateTime expected = new(2022, 10, 12);
        _iDateTimeProvider.SetupGet(mock => mock.Now).Returns(expected);
        IntegrityViewModel integrityViewModel = new(_iEventsStoreMock.Object, _iHashCalculatorMock.Object, _iEventsImporterMock.Object, _iTimeValidatorMock.Object, _iErrorsViewModelMock.Object, _iDateTimeProvider.Object);
        integrityViewModel.PropertyChanged += this.ChangeEventFlag;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(IntegrityViewModel.FromDate), It.IsAny<string>())).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(IntegrityViewModel.ToDate), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(false);

        // Act
        integrityViewModel.ToDate = expected;
        DateTime actual = integrityViewModel.ToDate;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(IntegrityViewModel.FromDate), It.IsAny<string>()), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(IntegrityViewModel.ToDate), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasToDateEventFired);
    }

    [TestMethod]
    public void ToDate_ShouldSetDate_WhenToDateIsValid()
    {
        // Arrange
        DateTime expected = new(2022, 10, 12);
        _iDateTimeProvider.SetupGet(mock => mock.Now).Returns(expected);
        IntegrityViewModel integrityViewModel = new(_iEventsStoreMock.Object, _iHashCalculatorMock.Object, _iEventsImporterMock.Object, _iTimeValidatorMock.Object, _iErrorsViewModelMock.Object, _iDateTimeProvider.Object);
        integrityViewModel.PropertyChanged += this.ChangeEventFlag;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(IntegrityViewModel.FromDate), It.IsAny<string>())).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(IntegrityViewModel.ToDate), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(true);

        // Act
        integrityViewModel.ToDate = expected;
        DateTime actual = integrityViewModel.ToDate;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(IntegrityViewModel.FromDate), It.IsAny<string>()), Times.Never());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(IntegrityViewModel.ToDate), It.IsAny<string>()), Times.Never());
        Assert.IsTrue(_hasToDateEventFired);
    }

    [TestMethod]
    public void ToHours_ShouldBeSetToZero_WhenObjectIsInitialized()
    {
        // Arrange
        int expected = 0;

        // Act
        int actual = _integrityViewModel.ToHours;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ToHours_ShouldAddError_WhenHoursAreInvalid()
    {
        // Arrange
        int expected = 25;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToHours))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(IntegrityViewModel.ToHours), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreHoursValid(It.IsAny<int>())).Returns(false);

        // Act
        _integrityViewModel.ToHours = expected;
        int actual = _integrityViewModel.ToHours;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToHours)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(IntegrityViewModel.ToHours), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasToHoursEventFired);
    }

    [TestMethod]
    public void ToHours_ShouldAddError_WhenDatesAreInvalid()
    {
        // Arrange
        int expected = 25;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToHours))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(IntegrityViewModel.ToHours), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreHoursValid(It.IsAny<int>())).Returns(false);
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(false);

        // Act
        _integrityViewModel.ToHours = expected;
        int actual = _integrityViewModel.ToHours;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToHours)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(IntegrityViewModel.ToHours), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasToHoursEventFired);
    }

    [TestMethod]
    public void ToHours_ShouldSetHours_WhenFromHoursIsValid()
    {
        // Arrange
        int expected = 20;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToHours))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(IntegrityViewModel.ToHours), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreHoursValid(It.IsAny<int>())).Returns(true);
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(true);

        // Act
        _integrityViewModel.ToHours = expected;
        int actual = _integrityViewModel.ToHours;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToHours)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(IntegrityViewModel.ToHours), It.IsAny<string>()), Times.Never());
        Assert.IsTrue(_hasToHoursEventFired);
    }

    [TestMethod]
    public void ToMinutes_ShouldBeSetToZero_WhenObjectIsInitialized()
    {
        // Arrange
        int expected = 0;

        // Act
        int actual = _integrityViewModel.ToMinutes;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ToMinutes_ShouldAddError_WhenMinutesAreInvalid()
    {
        // Arrange
        int expected = 61;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToMinutes))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(IntegrityViewModel.ToMinutes), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreMinutesValid(It.IsAny<int>())).Returns(false);

        // Act
        _integrityViewModel.ToMinutes = expected;
        int actual = _integrityViewModel.ToMinutes;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToMinutes)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(IntegrityViewModel.ToMinutes), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasToMinutesEventFired);
    }

    [TestMethod]
    public void ToMinutes_ShouldAddError_WhenDatesAreInvalid()
    {
        // Arrange
        int expected = 61;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToMinutes))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(IntegrityViewModel.ToMinutes), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreMinutesValid(It.IsAny<int>())).Returns(false);
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(false);

        // Act
        _integrityViewModel.ToMinutes = expected;
        int actual = _integrityViewModel.ToMinutes;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToMinutes)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(IntegrityViewModel.ToMinutes), It.IsAny<string>()), Times.Once());
        Assert.IsTrue(_hasToMinutesEventFired);
    }

    [TestMethod]
    public void ToMinutes_ShouldSetMinutes_WhenFromMinutesIsValid()
    {
        // Arrange
        int expected = 20;
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToMinutes))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToDate))).Verifiable();
        _iErrorsViewModelMock.Setup(mock => mock.AddError(nameof(IntegrityViewModel.ToMinutes), It.IsAny<string>())).Verifiable();
        _iTimeValidatorMock.Setup(mock => mock.AreMinutesValid(It.IsAny<int>())).Returns(true);
        _iTimeValidatorMock.Setup(mock => mock.AreDatesValid(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(true);

        // Act
        _integrityViewModel.ToMinutes = expected;
        int actual = _integrityViewModel.ToMinutes;

        // Assert
        Assert.AreEqual(expected, actual);
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToMinutes)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.FromDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.ClearErrors(nameof(IntegrityViewModel.ToDate)), Times.Once());
        _iErrorsViewModelMock.Verify(mock => mock.AddError(nameof(IntegrityViewModel.ToMinutes), It.IsAny<string>()), Times.Never());
        Assert.IsTrue(_hasToMinutesEventFired);
    }

    [TestMethod]
    public void HashValue_ShouldReturnNull_WhenObjectIsInitialized()
    {
        // Act
        string actual = _integrityViewModel.HashValue;

        // Assert
        Assert.IsNull(actual);
    }

    [TestMethod]
    public void HashValue_ShouldSetHashValue_WhenPropertyIsSet()
    {
        // Arrange
        string expected = "Test";

        // Act
        _integrityViewModel.HashValue = expected;
        string actual = _integrityViewModel.HashValue;

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(_hasHashValueEventFired);
    }

    [TestMethod]
    public void IsChecking_ShouldReturnFalse_WhenObjectIsInitialized()
    {
        // Act
        bool actual = _integrityViewModel.IsChecking;

        // Assert
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void IsChecking_ShouldReturnTrue_WhenPropertyIsSetToTrue()
    {
        // Arrange
        bool expected = true;

        // Act
        _integrityViewModel.IsChecking = expected;
        bool actual = _integrityViewModel.IsChecking;

        // Assert
        Assert.IsTrue(actual);
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(_hasIsCheckingEventFired);
    }

    [TestMethod]
    public void IsChecking_ShouldReturnFalse_WhenPropertyIsSetToFalse()
    {
        // Arrange
        bool expected = false;

        // Act
        _integrityViewModel.IsChecking = expected;
        bool actual = _integrityViewModel.IsChecking;

        // Assert
        Assert.IsFalse(actual);
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(_hasIsCheckingEventFired);
    }

    [TestMethod]
    public void HasErrors_ShouldReturnTrue_WhenThereAreErrors()
    {
        // Arrange
        bool expected = true;
        _iErrorsViewModelMock.Setup(mock => mock.HasErrors).Returns(expected);

        // Act
        bool actual = _integrityViewModel.HasErrors;

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
        bool actual = _integrityViewModel.HasErrors;

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
        ICommand command = _integrityViewModel.ChooseFileCommand;
        Type actual = command.GetType();

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(command is ChooseLoadFileCommand);
    }

    [TestMethod]
    public void CheckCommand_ShouldReturnCommand_WhenObjectIsInitialized()
    {
        // Arrange
        Type expected = typeof(CheckIntegrityCommand);

        // Act
        ICommand command = _integrityViewModel.CheckCommand;
        Type actual = command.GetType();

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(command is CheckIntegrityCommand);
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
        List<string> actual = _integrityViewModel.GetErrors(key).Cast<string>().ToList();

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
        List<string> actual = _integrityViewModel.GetErrors(key).Cast<string>().ToList();

        // Assert
        Assert.AreEqual(expected.Count, actual.Count);
    }

    [TestMethod]
    public void FullFromDate_ShouldReturnDateWithTime_WhenPropertyIsCalled()
    {
        // Arrange
        DateTime expected = new(2022, 10, 12, 12, 15, 0);
        _iDateTimeProvider.SetupGet(mock => mock.Now).Returns(expected);
        IntegrityViewModel integrityViewModel = new(_iEventsStoreMock.Object, _iHashCalculatorMock.Object, _iEventsImporterMock.Object, _iTimeValidatorMock.Object, _iErrorsViewModelMock.Object, _iDateTimeProvider.Object)
        {
            FromHours = 12,
            FromMinutes = 15
        };

        // Act
        DateTime actual = integrityViewModel.FullFromDate;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void FullToDate_ShouldReturnDateWithTime_WhenPropertyIsCalled()
    {
        // Arrange
        DateTime expected = new(2022, 10, 12, 12, 15, 0);
        _iDateTimeProvider.SetupGet(mock => mock.Now).Returns(expected);
        IntegrityViewModel integrityViewModel = new(_iEventsStoreMock.Object, _iHashCalculatorMock.Object, _iEventsImporterMock.Object, _iTimeValidatorMock.Object, _iErrorsViewModelMock.Object, _iDateTimeProvider.Object)
        {
            ToHours = 12,
            ToMinutes = 15
        };

        // Act
        DateTime actual = integrityViewModel.FullToDate;

        // Assert
        Assert.AreEqual(expected, actual);
    }
}
