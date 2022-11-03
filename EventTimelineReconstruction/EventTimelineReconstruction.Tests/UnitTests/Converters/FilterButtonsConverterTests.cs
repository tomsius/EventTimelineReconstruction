using EventTimelineReconstruction.Converters;

namespace EventTimelineReconstruction.Tests.UnitTests.Converters;

[TestClass]
public class FilterButtonsConverterTests
{
    private static IEnumerable<object[]> Values
    {
        get
        {
            return new[]
            {
                new object[]
                {
                    new object[] { 1, "string", 'c', 2.5, false }
                },
                new object[]
                {
                    new object[] { "test", ';', 5.0, false, 7 }
                },
                new object[]
                {
                    new object[] { 14, 'g', 4.8, "String", true }
                }
            };
        }
    }

    [TestMethod]
    [DynamicData(nameof(Values))]
    public void Convert_ShouldReturnClonedArray_WhenValuesAreGiven(object[] values)
    {
        // Arrange
        FilterButtonsConverter filterButtonsConverter = new();

        // Act
        object result = filterButtonsConverter.Convert(values, null, null, null);
        object[] actualResult = (object[])result;

        // Assert
        Assert.IsFalse(ReferenceEquals(values, actualResult));

        for (int i = 0; i < actualResult.Length; i++)
        {
            Assert.AreEqual(values[i], actualResult[i]);
        }
    }

    [DataTestMethod]
    [DataRow(1)]
    [DataRow(2.3)]
    [DataRow("string")]
    [DataRow('c')]
    [DataRow(true)]
    public void ConvertBack_ShouldReturnEmptyArray_WhenAnyObjectIsGiven(object objectValue)
    {
        // Arrange
        FilterButtonsConverter filterButtonsConverter = new();

        // Act
        object[] result = filterButtonsConverter.ConvertBack(objectValue, null, null, null);

        // Assert
        Assert.IsTrue(result.Length == 0);
    }
}
