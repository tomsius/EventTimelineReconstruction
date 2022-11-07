using System.ComponentModel;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Tests.UnitTests.ViewModels;

[TestClass]
public class ErrorsViewModelTests
{
    private bool _hasEventFired;

    private void ChangeEventFlag(object? sender, DataErrorsChangedEventArgs e)
    {
        _hasEventFired = true;
    }

    [TestInitialize]
    public void Setup()
    {
        _hasEventFired = false;
    }

    [TestMethod]
    public void Errors_ShouldReturnEmptyList_WhenObjectIsInitialized()
    {
        // Arrange
        ErrorsViewModel errorsViewModel = new();
        int expected = 0;

        // Act
        int actual = errorsViewModel.Errors.Count;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void AddError_ShouldAddNewErrorMessage_WhenMethodIsCalled()
    {
        // Arrange
        ErrorsViewModel errorsViewModel = new();
        errorsViewModel.ErrorsChanged += this.ChangeEventFlag;
        string key = "Error";
        string expectedValue = "Test Error";
        int expectedCount = 1;

        // Act
        errorsViewModel.AddError(key, expectedValue);
        int actualCount = errorsViewModel.Errors.Count;
        string actualValue = errorsViewModel.Errors[0];

        // Assert
        Assert.AreEqual(expectedCount, actualCount);
        Assert.AreEqual(expectedValue, actualValue);
        Assert.IsTrue(_hasEventFired);
    }

    [TestMethod]
    public void HasErrors_ShouldReturnTrue_WhenThereAreErrors()
    {
        // Arrange
        ErrorsViewModel errorsViewModel = new();
        string key = "Error";
        string value = "Test Error";
        errorsViewModel.AddError(key, value);

        // Act
        bool actual = errorsViewModel.HasErrors;

        // Assert
        Assert.IsTrue(actual);
    }

    [TestMethod]
    public void HasErrors_ShouldReturnFalse_WhenThereAreNoErrors()
    {
        // Arrange
        ErrorsViewModel errorsViewModel = new();

        // Act
        bool actual = errorsViewModel.HasErrors;

        // Assert
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void GetErrors_ShouldReturnErrorMessages_WhenThereAreErrorsForGivenKey()
    {
        // Arrange
        ErrorsViewModel errorsViewModel = new();
        string key = "Error";
        string expectedValue = "Test Error";
        errorsViewModel.AddError(key, expectedValue);
        int expectedCount = 1;

        // Act
        List<string> errorMessages = errorsViewModel.GetErrors(key).Cast<string>().ToList();
        int actualCount = errorMessages.Count;
        string actualValue = errorMessages[0];

        // Assert
        Assert.AreEqual(expectedCount, actualCount);
        Assert.AreEqual(expectedValue, actualValue);
        Assert.IsFalse(_hasEventFired);
    }

    [TestMethod]
    public void GetErrors_ShouldReturnEmptyList_WhenThereAreNoErrorsForGivenKey()
    {
        // Arrange
        ErrorsViewModel errorsViewModel = new();
        string key = "Error";
        int expectedCount = 0;

        // Act
        List<string> errorMessages = errorsViewModel.GetErrors(key).Cast<string>().ToList();
        int actualCount = errorMessages.Count;

        // Assert
        Assert.AreEqual(expectedCount, actualCount);
        Assert.IsFalse(_hasEventFired);
    }

    [TestMethod]
    public void ClearErrors_ShouldRemoveErrorMessages_WhenGivenKeyHasErrors()
    {
        // Arrange
        ErrorsViewModel errorsViewModel = new();
        errorsViewModel.AddError("Error1", "Test Error1");
        errorsViewModel.AddError("Error1", "Test Error2");
        errorsViewModel.AddError("Error2", "Test Error3");
        int expectedCount = 1;

        // Act
        errorsViewModel.ClearErrors("Error1");
        int actualCount = errorsViewModel.Errors.Count;

        // Assert
        Assert.AreEqual(expectedCount, actualCount);
    }

    [TestMethod]
    public void ClearErrors_ShouldDoNothing_WhenGivenKeyDoesNotHaveErrors()
    {
        // Arrange
        ErrorsViewModel errorsViewModel = new();
        errorsViewModel.ErrorsChanged += this.ChangeEventFlag;
        errorsViewModel.AddError("Error1", "Test Error1");
        errorsViewModel.AddError("Error1", "Test Error2");
        errorsViewModel.AddError("Error2", "Test Error3");
        int expectedCount = 3;

        // Act
        errorsViewModel.ClearErrors("Error3");
        int actualCount = errorsViewModel.Errors.Count;

        // Assert
        Assert.AreEqual(expectedCount, actualCount);
        Assert.IsTrue(_hasEventFired);
    }
}
