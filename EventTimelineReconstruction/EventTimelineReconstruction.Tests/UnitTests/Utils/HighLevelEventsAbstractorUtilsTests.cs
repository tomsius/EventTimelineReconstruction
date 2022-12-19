using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Tests.UnitTests.Utils;

[TestClass]
public class HighLevelEventsAbstractorUtilsTests
{
    private static IEnumerable<object[]> InvalidPeEvents
    {
        get
        {
            return new[]
            {
                new object[]
                {
                    new EventViewModel(
                        new EventModel(
                            new DateOnly(2022, 10, 14),
                            new TimeOnly(10, 52),
                            TimeZoneInfo.Local,
                            "MACB",
                            "Source",
                            "Source Type",
                            "Type",
                            "Username",
                            "Hostname",
                            "Short Description",
                            "Full Description",
                            2.5,
                            "Filename.exe",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            "1"
                        )
                    )
                },
                new object[]
                {
                    new EventViewModel(
                        new EventModel(
                            new DateOnly(2022, 10, 14),
                            new TimeOnly(10, 52),
                            TimeZoneInfo.Local,
                            "MACB",
                            "Source",
                            "Source Type",
                            "Type",
                            "Username",
                            "Hostname",
                            "pe_type",
                            "Full Description",
                            2.5,
                            "Filename.txt",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            "1"
                        )
                    )
                },
                new object[]
                {
                    new EventViewModel(
                        new EventModel(
                            new DateOnly(2022, 10, 14),
                            new TimeOnly(10, 52),
                            TimeZoneInfo.Local,
                            "MACB",
                            "Source",
                            "Source Type",
                            "Type",
                            "Username",
                            "Hostname",
                            "something",
                            "Full Description",
                            2.5,
                            "Filename.txt",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            "1"
                        )
                    )
                }
            };
        }
    }

    [DataTestMethod]
    [DataRow("Type: [GENERATED - something] (type count 1)", "GENERATED 1")]
    [DataRow("Type: [FORM_SUBMIT - something] (type count 31)", "FORM_SUBMIT 31")]
    [DataRow("(type count 0) Type: [RELOAD - something]", "RELOAD")]
    [DataRow("Type: [TYPED - something]", "TYPED")]
    [DataRow("Transition: DOWNLOAD [count: 2]", "DOWNLOAD 2")]
    [DataRow("Transition: REDIRECT_TEMPORARY [count: 25]", "REDIRECT_TEMPORARY 25")]
    [DataRow("Transition: REDIRECT_PERMANENT [count: 0]", "REDIRECT_PERMANENT")]
    [DataRow("test.pdf Transition: LINK", "PDF")]
    public void GenerateVisitValue_ShouldReturnFormattedVisitValue_WhenInputIsGiven(string input, string expected)
    {
        // Arrange
        HighLevelEventsAbstractorUtils utils = new();

        // Act
        string actual = utils.GenerateVisitValue(input);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [DataTestMethod]
    [DataRow("(test.pdf)", "test.pdf")]
    [DataRow("something (test.docx)", "test.docx")]
    [DataRow("(test.txt) something", "test.txt")]
    [DataRow("something (test.pptx) something", "test.pptx")]
    public void GetDownloadedFileName_ShouldReturnFilename_WhenInputIsGiven(string input, string expected)
    {
        // Arrange
        HighLevelEventsAbstractorUtils utils = new();

        // Act
        string actual = utils.GetDownloadedFileName(input);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [DataTestMethod]
    [DataRow("3ec2b817-c405-11e7-8ac5-a0afbdac1ec0 Origin: Access 2016.lnk", "a0afbdac1ec0")]
    [DataRow("Origin: Access 2016.lnk", "-")]
    public void GetMacAddress_ShouldReturnMACAddress_WhenInputIsGiven(string input, string expected)
    {
        // Arrange
        HighLevelEventsAbstractorUtils utils = new();

        // Act
        string actual = utils.GetMacAddress(input);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void GetMailUrl_ShouldReturnGMailUrl_WhenMethodIsCalled()
    {
        // Arrange
        HighLevelEventsAbstractorUtils utils = new();
        string input = "";
        string expected = "https://mail.google.com";

        // Act
        string actual = utils.GetMailUrl(input);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [DataTestMethod]
    [DataRow("3ec2b817-c405-11e7-8ac5-a0afbdac1ec0 Origin: Access 2016.lnk", "Access 2016.lnk")]
    [DataRow("Origin: Access 2016.lnk", "Access 2016.lnk")]
    public void GetOrigin_ShouldReturnOriginFilename_WhenInputIsGiven(string input, string expected)
    {
        // Arrange
        HighLevelEventsAbstractorUtils utils = new();

        // Act
        string actual = utils.GetOrigin(input);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [DataTestMethod]
    [DataRow("[Empty description] something test.txt", "something test.txt")]
    [DataRow("[Empty description] something...", "something...")]
    [DataRow("[Process RAR  ZIP and other archive formats] something something test.zip", "something something test.zip")]
    [DataRow("[Create beautiful documents easily work with others and enjoy the read.] C:...", "Create beautiful documents easily work with others and enjoy the read.")]
    [DataRow("[Use your computer to connect to a computer that is located elsewhere and run...", "Use your computer to connect to a computer that is located elsewhere and run...")]
    public void GetShort_ShouldReturnShortValue_WhenInputIsGiven(string input, string expected)
    {
        // Arrange
        HighLevelEventsAbstractorUtils utils = new();

        // Act
        string actual = utils.GetShort(input);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [DataTestMethod]
    [DataRow("https://s.com/help/", "https://s.com/")]
    [DataRow("http://s.com/help/", "http://s.com/")]
    [DataRow("http://itc.ktu.lt/help/", "http://itc.ktu.lt/")]
    [DataRow("Bookmark URL Help and Tutorials (https://www.mozilla.org/en-US/firefox/help/) visit count 0", "https://www.mozilla.org/")]
    [DataRow("https://www.google.com/?gws_rd=ssl (Google) [count: 0] Visit from: https://consent.google.com/ml?continue=https://www.google.com/%3Fgws_rd%3Dssl&gl=LT&m=0&pc=shp&uxe=none&hl=lt&src=1 Type: [LINK - User clicked a link] (URL not typed directly - no typed count)", "https://www.google.com/")]
    public void GetUrlHost_ShouldReturnHost_WhenInputIsGiven(string input, string expected)
    {
        // Arrange
        HighLevelEventsAbstractorUtils utils = new();

        // Act
        string actual = utils.GetUrlHost(input);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void IsValidPeEvent_ShouldReturnTrue_WhenValidPeEventIsGiven()
    {
        // Arrange
        HighLevelEventsAbstractorUtils utils = new();
        EventViewModel eventViewModel = new(
                        new EventModel(
                            new DateOnly(2022, 10, 14),
                            new TimeOnly(10, 52),
                            TimeZoneInfo.Local,
                            "MACB",
                            "Source",
                            "Source Type",
                            "Type",
                            "Username",
                            "Hostname",
                            "pe_type",
                            "Full Description",
                            2.5,
                            "test.exe",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            "1"
                        )
                    );

        // Act
        bool actual = utils.IsValidPeEvent(eventViewModel);

        // Assert
        Assert.IsTrue(actual);
    }

    [TestMethod]
    [DynamicData(nameof(InvalidPeEvents))]
    public void IsValidPeEvent_ShouldReturnFalse_WhenInvalidPeEventIsGiven(EventViewModel eventViewModel)
    {
        // Arrange
        HighLevelEventsAbstractorUtils utils = new();

        // Act
        bool actual = utils.IsValidPeEvent(eventViewModel);

        // Assert
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void IsValidWebhistLine_ShouldReturnTrue_WhenWebhistLineIsValid()
    {
        // Arrange
        HighLevelEventsAbstractorUtils utils = new();
        EventViewModel eventViewModel = new(
                        new EventModel(
                            new DateOnly(2022, 10, 14),
                            new TimeOnly(10, 52),
                            TimeZoneInfo.Local,
                            "MACB",
                            "Source",
                            "History",
                            "Type",
                            "Username",
                            "Hostname",
                            "pe_type",
                            "Full Description",
                            2.5,
                            "test.exe",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            "1"
                        )
                    );

        // Act
        bool actual = utils.IsValidWebhistLine(eventViewModel);

        // Assert
        Assert.IsTrue(actual);
    }

    [TestMethod]
    public void IsValidWebhistLine_ShouldReturnFalse_WhenWebhistLineIsInvalid()
    {
        // Arrange
        HighLevelEventsAbstractorUtils utils = new();
        EventViewModel eventViewModel = new(
                        new EventModel(
                            new DateOnly(2022, 10, 14),
                            new TimeOnly(10, 52),
                            TimeZoneInfo.Local,
                            "MACB",
                            "Source",
                            "Cache",
                            "Type",
                            "Username",
                            "Hostname",
                            "pe_type",
                            "Full Description",
                            2.5,
                            "test.exe",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            "1"
                        )
                    );

        // Act
        bool actual = utils.IsValidWebhistLine(eventViewModel);

        // Assert
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void IsWebhistDownloadEvent_ShouldReturnTrue_WhenWebhistLineIsAboutDownloadedFile()
    {
        // Arrange
        HighLevelEventsAbstractorUtils utils = new();
        EventViewModel eventViewModel = new(
                        new EventModel(
                            new DateOnly(2022, 10, 14),
                            new TimeOnly(10, 52),
                            TimeZoneInfo.Local,
                            "MACB",
                            "Source",
                            "History",
                            "File Downloaded",
                            "Username",
                            "Hostname",
                            "pe_type",
                            "Full Description",
                            2.5,
                            "test.exe",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            "1"
                        )
                    );

        // Act
        bool actual = utils.IsWebhistDownloadEvent(eventViewModel);

        // Assert
        Assert.IsTrue(actual);
    }

    [TestMethod]
    public void IsWebhistDownloadEvent_ShouldReturnFalse_WhenWebhistLineIsNotAboutDownloadedFile()
    {
        // Arrange
        HighLevelEventsAbstractorUtils utils = new();
        EventViewModel eventViewModel = new(
                        new EventModel(
                            new DateOnly(2022, 10, 14),
                            new TimeOnly(10, 52),
                            TimeZoneInfo.Local,
                            "MACB",
                            "Source",
                            "Cache",
                            "Type",
                            "Username",
                            "Hostname",
                            "pe_type",
                            "Full Description",
                            2.5,
                            "test.exe",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            "1"
                        )
                    );

        // Act
        bool actual = utils.IsWebhistDownloadEvent(eventViewModel);

        // Assert
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void IsWebhistMailEvent_ShouldReturnTrue_WhenWebhistLineIsAboutMail()
    {
        // Arrange
        HighLevelEventsAbstractorUtils utils = new();
        EventViewModel eventViewModel = new(
                        new EventModel(
                            new DateOnly(2022, 10, 14),
                            new TimeOnly(10, 52),
                            TimeZoneInfo.Local,
                            "MACB",
                            "Source",
                            "History",
                            "File Downloaded",
                            "Username",
                            "Hostname",
                            "mail",
                            "Full Description",
                            2.5,
                            "test.exe",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            "1"
                        )
                    );

        // Act
        bool actual = utils.IsWebhistMailEvent(eventViewModel);

        // Assert
        Assert.IsTrue(actual);
    }

    [TestMethod]
    public void IsWebhistMailEvent_ShouldReturnFalse_WhenWebhistLineIsNotAboutMail()
    {
        // Arrange
        HighLevelEventsAbstractorUtils utils = new();
        EventViewModel eventViewModel = new(
                        new EventModel(
                            new DateOnly(2022, 10, 14),
                            new TimeOnly(10, 52),
                            TimeZoneInfo.Local,
                            "MACB",
                            "Source",
                            "Cache",
                            "Type",
                            "Username",
                            "Hostname",
                            "pe_type",
                            "Full Description",
                            2.5,
                            "test.exe",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            "1"
                        )
                    );

        // Act
        bool actual = utils.IsWebhistMailEvent(eventViewModel);

        // Assert
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void IsWebhistNamingActivityEvent_ShouldReturnTrue_WhenWebhistLineIsNamingActivity()
    {
        // Arrange
        HighLevelEventsAbstractorUtils utils = new();
        EventViewModel eventViewModel = new(
                        new EventModel(
                            new DateOnly(2022, 10, 14),
                            new TimeOnly(10, 52),
                            TimeZoneInfo.Local,
                            "MACB",
                            "Source",
                            "History",
                            "File Downloaded",
                            "Username",
                            "Hostname",
                            "http://test.com",
                            "Full Description",
                            2.5,
                            "test.exe",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            "1"
                        )
                    );

        // Act
        bool actual = utils.IsWebhistNamingActivityEvent(eventViewModel);

        // Assert
        Assert.IsTrue(actual);
    }

    [TestMethod]
    public void IsWebhistNamingActivityEvent_ShouldReturnFalse_WhenWebhistLineIsNotNamingActivity()
    {
        // Arrange
        HighLevelEventsAbstractorUtils utils = new();
        EventViewModel eventViewModel = new(
                        new EventModel(
                            new DateOnly(2022, 10, 14),
                            new TimeOnly(10, 52),
                            TimeZoneInfo.Local,
                            "MACB",
                            "Source",
                            "Cache",
                            "Type",
                            "Username",
                            "Hostname",
                            "pe_type",
                            "Full Description",
                            2.5,
                            "test.exe",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            "1"
                        )
                    );

        // Act
        bool actual = utils.IsWebhistNamingActivityEvent(eventViewModel);

        // Assert
        Assert.IsFalse(actual);
    }
}
