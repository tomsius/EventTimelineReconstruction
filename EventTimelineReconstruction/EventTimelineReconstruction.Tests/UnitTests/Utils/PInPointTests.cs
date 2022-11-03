using System.Windows;
using EventTimelineReconstruction.Utils;

namespace EventTimelineReconstruction.Tests.UnitTests.Utils;

[TestClass]
public class PInPointTests
{
    private static IEnumerable<object[]> Offsets
    {
        get
        {
            return new[]
            {
                new object[]
                {
                    new Point(0, 0),
                    new Point(0, 0)
                },
                new object[]
                {
                    new Point(10, 10),
                    new Point(10, 10)
                },
                new object[]
                {
                    new Point(-10, -10),
                    new Point(-10, -10)
                }
            };
        }
    }

    [TestMethod]
    [DynamicData(nameof(Offsets))]
    public void GetPoint_ShouldReturnNewPoint_WhenOffsetIsGiven(Point offset, Point expected)
    {
        // Arrange
        PInPoint original = new() { X = 0, Y = 0 };

        // Act
        Point actual = original.GetPoint(offset);

        // Assert
        Assert.AreEqual(expected.X, actual.X);
        Assert.AreEqual(expected.Y, actual.Y);
    }
}
