using EventTimelineReconstruction.Stores;

namespace EventTimelineReconstruction.Tests.UnitTests.Stores;

[TestClass]
public class FilteringStoreTests
{
    [TestMethod]
    public void FilteringStore_ShouldInitializeObject_WhenConstructorIsCalled()
    {
        // Arrange
        int expectedDictionarySize = 0;
        DateTime expectedFromDate = DateTime.MinValue;
        DateTime expectedToDate = DateTime.MinValue;

        // Act
        FilteringStore filteringStore = new();

        // Assert
        Assert.IsFalse(filteringStore.IsEnabled);
        Assert.IsFalse(filteringStore.AreAllFiltersApplied);
        Assert.IsNotNull(filteringStore.ChosenEventTypes);
        Assert.AreEqual(expectedDictionarySize, filteringStore.ChosenEventTypes.Count);
        Assert.IsNotNull(filteringStore.Keyword);
        Assert.AreEqual(expectedFromDate, filteringStore.FromDate);
        Assert.AreEqual(expectedToDate, filteringStore.ToDate);
    }

    [TestMethod]
    public void SetEventTypes_AddsAllChosenEventTypes_WhenTypeDoesNotExistInDictionary()
    {
        // Arrange
        FilteringStore filteringStore = new();
        Dictionary<string, bool> chosenEvents = new()
        {
            { "First", true },
            { "Second", true },
            { "Third", false },
            { "Fourth", true },
            { "Fifth", false }
        };

        // Act
        filteringStore.SetEventTypes(chosenEvents);

        // Assert
        Assert.AreEqual(chosenEvents.Count, filteringStore.ChosenEventTypes.Count);

        foreach (KeyValuePair<string, bool> kvp in chosenEvents)
        {
            string expectedKey = kvp.Key;
            bool expectedValue = kvp.Value;

            Assert.IsTrue(filteringStore.ChosenEventTypes.ContainsKey(expectedKey));
            Assert.AreEqual(expectedValue, filteringStore.ChosenEventTypes[expectedKey]);
        }
    }

    [TestMethod]
    public void SetEventTypes_ChangeChosenEventType_WhenTypeExistsInDictionary()
    {
        // Arrange
        FilteringStore filteringStore = new();
        Dictionary<string, bool> chosenEvents = new()
        {
            { "First", true },
            { "Second", true },
            { "Third", false },
            { "Fourth", true },
            { "Fifth", false }
        };
        Dictionary<string, bool> newChosenEvents = new()
        {
            { "First", false },
            { "Second", false },
            { "Third", true },
            { "Fourth", false },
            { "Fifth", true }
        };

        // Act
        filteringStore.SetEventTypes(chosenEvents);
        filteringStore.SetEventTypes(newChosenEvents);

        // Assert
        Assert.AreEqual(newChosenEvents.Count, filteringStore.ChosenEventTypes.Count);

        foreach (KeyValuePair<string, bool> kvp in newChosenEvents)
        {
            string expectedKey = kvp.Key;
            bool expectedValue = kvp.Value;

            Assert.IsTrue(filteringStore.ChosenEventTypes.ContainsKey(expectedKey));
            Assert.AreEqual(expectedValue, filteringStore.ChosenEventTypes[expectedKey]);
        }
    }

    [TestMethod]
    public void SetEventTypes_AddChosenEventTypeOrChangeChosenEventByType_WhenSomeTypesExistInDictionary()
    {
        // Arrange
        FilteringStore filteringStore = new();
        Dictionary<string, bool> chosenEvents = new()
        {
            { "First", true },
            { "Second", true },
            { "Third", false },
            { "Fourth", true },
            { "Fifth", false }
        };
        Dictionary<string, bool> newChosenEvents = new()
        {
            { "First", false },
            { "Second", false },
            { "Third", true },
            { "Six", true },
            { "Seventh", false }
        };
        Dictionary<string, bool> expectedChosenEvents = new()
        {
            { "First", false },
            { "Second", false },
            { "Third", true },
            { "Fourth", true },
            { "Fifth", false },
            { "Six", true },
            { "Seventh", false }
        };

        // Act
        filteringStore.SetEventTypes(chosenEvents);
        filteringStore.SetEventTypes(newChosenEvents);

        // Assert
        Assert.AreEqual(expectedChosenEvents.Count, filteringStore.ChosenEventTypes.Count);

        foreach (KeyValuePair<string, bool> kvp in expectedChosenEvents)
        {
            string expectedKey = kvp.Key;
            bool expectedValue = kvp.Value;

            Assert.IsTrue(filteringStore.ChosenEventTypes.ContainsKey(expectedKey));
            Assert.AreEqual(expectedValue, filteringStore.ChosenEventTypes[expectedKey]);
        }
    }
}
