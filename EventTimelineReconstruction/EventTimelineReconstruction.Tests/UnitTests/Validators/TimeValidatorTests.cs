using EventTimelineReconstruction.Validators;

namespace EventTimelineReconstruction.Tests.UnitTests.Validators;

[TestClass]
public class TimeValidatorTests
{
    [TestMethod]
    public void AreDatesValid_ShouldReturnTrue_WhenDateAreInOrder()
    {
        // Arrange
        TimeValidator timeValidator = new();
        DateTime from = new(1998, 10, 14, 11, 45, 25);
        DateTime to = new(2022, 5, 9, 23, 11, 58);

        // Act
        bool result = timeValidator.AreDatesValid(from, to);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void AreDatesValid_ShouldReturnFalse_WhenDateAreOutOfOrder()
    {
        // Arrange
        TimeValidator timeValidator = new();
        DateTime from = new(2022, 5, 9, 23, 11, 58);
        DateTime to = new(1998, 10, 14, 11, 45, 25);

        // Act
        bool result = timeValidator.AreDatesValid(from, to);

        // Assert
        Assert.IsFalse(result);
    }

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(12)]
    [DataRow(23)]
    public void AreHoursValid_ShouldReturnTrue_WhenHoursAreBetween0And23(int hours)
    {
        // Arrange
        TimeValidator timeValidator = new();

        // Act
        bool result = timeValidator.AreHoursValid(hours);

        // Assert
        Assert.IsTrue(result);
    }

    [DataTestMethod]
    [DataRow(-10)]
    [DataRow(-1)]
    [DataRow(24)]
    [DataRow(40)]
    public void AreHoursValid_ShouldReturnFalse_WhenHoursAreNotBetween0And23(int hours)
    {
        // Arrange
        TimeValidator timeValidator = new();

        // Act
        bool result = timeValidator.AreHoursValid(hours);

        // Assert
        Assert.IsFalse(result);
    }

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(33)]
    [DataRow(59)]
    public void AreMinutesValid_ShouldReturnTrue_WhenMinutesAreBetween0And59(int minutes)
    {
        // Arrange
        TimeValidator timeValidator = new();

        // Act
        bool result = timeValidator.AreMinutesValid(minutes);

        // Assert
        Assert.IsTrue(result);
    }

    [DataTestMethod]
    [DataRow(-10)]
    [DataRow(-1)]
    [DataRow(60)]
    [DataRow(125)]
    public void AreMinutesValid_ShouldReturnFalse_WhenMinutesAreNotBetween0And59(int minutes)
    {
        // Arrange
        TimeValidator timeValidator = new();

        // Act
        bool result = timeValidator.AreMinutesValid(minutes);

        // Assert
        Assert.IsFalse(result);
    }
}
