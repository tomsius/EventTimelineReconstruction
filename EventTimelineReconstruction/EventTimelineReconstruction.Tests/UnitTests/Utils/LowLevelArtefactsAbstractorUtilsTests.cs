using EventTimelineReconstruction.Utils;

namespace EventTimelineReconstruction.Tests.UnitTests.Utils;

[TestClass]
public class LowLevelArtefactsAbstractorUtilsTests
{
    [DataTestMethod]
    [DataRow("http://www.mozilla.org/", "mozilla.org/")]
    [DataRow("http://mozilla.org/", "mozilla.org/")]
    public void GetAddress_ShouldReturnAddress_WhenWebhistDescriptionIsGiven(string description, string expected)
    {
        // Arrange
        LowLevelArtefactsAbstractorUtils utils = new();

        // Act
        string actual = utils.GetAddress(description);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void GetExtraValue_ShouldReturnFormattedExtraValues_WhenExtraDictionaryIsGiven()
    {
        // Arrange
        Dictionary<string, string> extra = new() { { "page_transition_type", "1" }, { "schema_match", "False" }, { "sha256_hash", "a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741" }, { "visit_type", "5" } };
        string expected = "page_transition_type: 1 schema_match: False sha256_hash: a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741 visit_type: 5";
        LowLevelArtefactsAbstractorUtils utils = new();

        // Act
        string actual = utils.GetExtraValue(extra);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [DataTestMethod]
    [DataRow("Firefox History", "Something")]
    [DataRow("google history", "something")]
    [DataRow("Cookies", "Something")]
    public void IsValidWebhistLine_ShouldReturnTrue_WhenWebhistIsValid(string sourceType, string type)
    {
        // Arrange
        LowLevelArtefactsAbstractorUtils utils = new();

        // Act
        bool actual = utils.IsValidWebhistLine(sourceType, type);

        // Assert
        Assert.IsTrue(actual);
    }

    [DataTestMethod]
    [DataRow("Cookies", "Cookie Expires")]
    [DataRow("cookies", "expiration time")]
    [DataRow("something", "something")]
    public void IsValidWebhistLine_ShouldReturnFalse_WhenWebhistIsNotValid(string sourceType, string type)
    {
        // Arrange
        LowLevelArtefactsAbstractorUtils utils = new();

        // Act
        bool actual = utils.IsValidWebhistLine(sourceType, type);

        // Assert
        Assert.IsFalse(actual);
    }
}
