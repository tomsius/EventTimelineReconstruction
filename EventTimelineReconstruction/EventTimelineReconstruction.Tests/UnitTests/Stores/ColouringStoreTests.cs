using System.Windows.Media;
using EventTimelineReconstruction.Stores;

namespace EventTimelineReconstruction.Tests.UnitTests.Stores;

[TestClass]
public class ColouringStoreTests
{
    [TestMethod]
    public void SetColoursByType_AddsAllColoursByType_WhenTypeDoesNotExistInDictionary()
    {
        // Arrange
        ColouringStore colouringStore = new();
        Dictionary<string, Brush> coloursByType = new()
        {
            { "First", Brushes.Red },
            { "Second", Brushes.Wheat },
            { "Third", Brushes.Black },
            { "Fourth", Brushes.Orange },
            { "Fifth", Brushes.Yellow }
        };

        // Act
        colouringStore.SetColoursByType(coloursByType);

        // Assert
        Assert.AreEqual(coloursByType.Count, colouringStore.ColoursByType.Count);

        foreach (KeyValuePair<string, Brush> kvp in coloursByType)
        {
            string expectedKey = kvp.Key;
            Brush expectedValue = kvp.Value;

            Assert.IsTrue(colouringStore.ColoursByType.ContainsKey(expectedKey));
            Assert.AreEqual(expectedValue, colouringStore.ColoursByType[expectedKey]);
        }
    }

    [TestMethod]
    public void SetColoursByType_ChangeColourOfTypes_WhenTypeExistsInDictionary()
    {
        // Arrange
        ColouringStore colouringStore = new();
        Dictionary<string, Brush> coloursByType = new()
        {
            { "First", Brushes.Red },
            { "Second", Brushes.Wheat },
            { "Third", Brushes.Black },
            { "Fourth", Brushes.Orange },
            { "Fifth", Brushes.Yellow }
        };
        Dictionary<string, Brush> newColoursByType = new()
        {
            { "First", Brushes.Green },
            { "Second", Brushes.Black },
            { "Third", Brushes.Cyan },
            { "Fourth", Brushes.Gray },
            { "Fifth", Brushes.Azure }
        };

        // Act
        colouringStore.SetColoursByType(coloursByType);
        colouringStore.SetColoursByType(newColoursByType);

        // Assert
        Assert.AreEqual(newColoursByType.Count, colouringStore.ColoursByType.Count);

        foreach (KeyValuePair<string, Brush> kvp in newColoursByType)
        {
            string expectedKey = kvp.Key;
            Brush expectedValue = kvp.Value;

            Assert.IsTrue(colouringStore.ColoursByType.ContainsKey(expectedKey));
            Assert.AreEqual(expectedValue, colouringStore.ColoursByType[expectedKey]);
        }
    }

    [TestMethod]
    public void SetColoursByType_AddColourByTypeOrChangeColourOfType_WhenSomeTypesExistInDictionary()
    {
        // Arrange
        ColouringStore colouringStore = new();
        Dictionary<string, Brush> coloursByType = new()
        {
            { "First", Brushes.Red },
            { "Second", Brushes.Wheat },
            { "Third", Brushes.Black },
            { "Fourth", Brushes.Orange },
            { "Fifth", Brushes.Yellow }
        };
        Dictionary<string, Brush> newColoursByType = new()
        {
            { "First", Brushes.Green },
            { "Second", Brushes.Black },
            { "Third", Brushes.Cyan },
            { "Six", Brushes.Gray },
            { "Seventh", Brushes.Azure }
        };
        Dictionary<string, Brush> expectedColoursByType = new()
        {
            { "First", Brushes.Green },
            { "Second", Brushes.Black },
            { "Third", Brushes.Cyan },
            { "Fourth", Brushes.Orange },
            { "Fifth", Brushes.Yellow },
            { "Six", Brushes.Gray },
            { "Seventh", Brushes.Azure }
        };

        // Act
        colouringStore.SetColoursByType(coloursByType);
        colouringStore.SetColoursByType(newColoursByType);

        // Assert
        Assert.AreEqual(expectedColoursByType.Count, colouringStore.ColoursByType.Count);

        foreach (KeyValuePair<string, Brush> kvp in expectedColoursByType)
        {
            string expectedKey = kvp.Key;
            Brush expectedValue = kvp.Value;

            Assert.IsTrue(colouringStore.ColoursByType.ContainsKey(expectedKey));
            Assert.AreEqual(expectedValue, colouringStore.ColoursByType[expectedKey]);
        }
    }
}
