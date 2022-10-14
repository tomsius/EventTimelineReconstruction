using EventTimelineReconstruction.Utils;

namespace EventTimelineReconstruction.Tests.UnitTests.Utils;

[TestClass]
public class RangeEnabledObservableCollectionTests
{
    private readonly RangeEnabledObservableCollection<int> _collection;

    public RangeEnabledObservableCollectionTests()
    {
        _collection = new();
    }

    [TestMethod]
    public void AddRange_ShouldAddAllelements_WhenListOfItemsIsGiven()
    {
        // Arrange
        List<int> expected = new() { 1, 5, 2, 8, 6 };

        // Act
        _collection.AddRange(expected);

        // Assert
        Assert.AreEqual(expected.Count, _collection.Count);

        for (int i = 0; i < expected.Count; i++)
        {
            Assert.AreEqual(expected[i], _collection[i]);
        }
    }
}
