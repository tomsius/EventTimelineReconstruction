using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Tests.UnitTests.Utils;

[TestClass]
public class LowLevelEventsAbstractorUtilsTests
{
    private static IEnumerable<object[]> FirefoxWebhistExtra
    {
        get
        {
            return new[]
            {
                new object[]
                {
                    new Dictionary<string, string>() { { "extra", "['visited from: http://mail.google.com/ (mail.google.com)'  '(URL not typed directly)'  'Transition: REDIRECT_PERMANENT']" }, { "schema_match", "False" }, { "sha256_hash", "a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741" }, { "visit_type", "5" } },
                    "http://mail.google.com/"
                },
                new object[]
                {
                    new Dictionary<string, string>() { { "extra", "['visited from: https://www.google.com/url?esrc=s&q=&username=testas&rct=j&sa=U&url=https://www.ekiga.org/&ved=2ahUKEwjuuN_AtsD3AhXSRPEDHRXrAd0QFnoECAoQAg&usg=AOvVaw0N9Qq_LLtIk92eAMdY3Rbn (www.google.com)'  '(URL not typed directly)'  'Transition: LINK']" }, { "schema_match", "False" }, { "sha256_hash", "a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741" }, { "visit_type", "5" } },
                    "https://www.google.com/url?esrc=s&q=&username=testas&rct=j&sa=U&url=https://www.ekiga.org/&ved=2ahUKEwjuuN_AtsD3AhXSRPEDHRXrAd0QFnoECAoQAg&usg=AOvVaw0N9Qq_LLtIk92eAMdY3Rbn username=testas"
                },
                new object[]
                {
                    new Dictionary<string, string>() { { "extra", "['visited from: https://www.google.com/url?esrc=s&q=&rct=j&sa=U&url=https://www.ekiga.org/&ved=2ahUKEwjuuN_AtsD3AhXSRPEDHRXrAd0QFnoECAoQAg&usg=AOvVaw0N9Qq_LLtIk92eAMdY3Rbn&username=testas (www.google.com)'  '(URL not typed directly)'  'Transition: LINK']" }, { "schema_match", "False" }, { "sha256_hash", "a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741" }, { "visit_type", "5" } },
                    "https://www.google.com/url?esrc=s&q=&username=testas&rct=j&sa=U&url=https://www.ekiga.org/&ved=2ahUKEwjuuN_AtsD3AhXSRPEDHRXrAd0QFnoECAoQAg&usg=AOvVaw0N9Qq_LLtIk92eAMdY3Rbn username=testas"
                },
                new object[]
                {
                    new Dictionary<string, string>() { { "host", "." }, { "visit_type", "5" }, { "schema_match", "False" }, { "sha256_hash", "a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741" } },
                    "host: . visit_type: 5"
                },
                new object[]
                {
                    new Dictionary<string, string>() { { "extra", "['visited from: https://www.google.com/search?q=chrome&ie=utf-8&oe=utf-8&client=firefox-b (www.google.com)'  '(URL not typed directly)'  'Transition: LINK']" }, { "schema_match", "False" }, { "sha256_hash", "4eb3f81bf5801eb3f96b796c4f5b2b68a187a5165893e3a7957ae347a07c4fb7" }, { "visit_type", "1" } },
                    "https://www.google.com/search?q=chrome&ie=utf-8&oe=utf-8&client=firefox-b"
                }
            };
        }
    }

    private static IEnumerable<object[]> MetaExtra
    {
        get
        {
            return new[]
            {
                new object[]
                {
                    new Dictionary<string, string>() { { "number_of_paragraphs", "1" }, { "total_time", "3" }, { "something", "else" } },
                    "something something",
                    "number_of_paragraphs: 1 total_time: 3"
                },
                new object[]
                {
                    new Dictionary<string, string>() { { "total_time", "3" }, { "something", "else" } },
                    "something something",
                    "total_time: 3"
                },
                new object[]
                {
                    new Dictionary<string, string>() { { "number_of_paragraphs", "1" }, { "something", "else" } },
                    "something something",
                    "number_of_paragraphs: 1"
                },
                new object[]
                {
                    new Dictionary<string, string>() { { "something", "else" } },
                    "Testas",
                    "Testas"
                },
                new object[]
                {
                    new Dictionary<string, string>() { { "number_of_paragraphs", "1" }, { "total_time", "3" } },
                    "Creating App: Microsoft Office Word App...",
                    "Creating App number_of_paragraphs: 1 total_time: 3"
                }
            };
        }
    }

    private static IEnumerable<object[]> EventsWithFilename
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
                            "FILE",
                            "Source Type",
                            "Type",
                            "Username",
                            "Hostname",
                            "Name: testas 1.txt Origin: testas1.lnk",
                            "Full Description",
                            2.5,
                            "Filename",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            1
                        )
                    ),
                    "testas 1.txt"
                },
                new object[]
                {
                    new EventViewModel(
                        new EventModel(
                            new DateOnly(2022, 10, 14),
                            new TimeOnly(10, 52),
                            TimeZoneInfo.Local,
                            "MACB",
                            "LNK",
                            "Source Type",
                            "Type",
                            "Username",
                            "Hostname",
                            "Short Description",
                            "[Empty description] File size: 20 File attribute flags: 0x00000020 Drive type: 3 Drive serial number: 0x6c91ef9f Local path: C:\\Documents and Settings\\PC1\\Desktop\\testas2.txt Relative path: ..\\Desktop\\testas2.txt Working dir: C:\\Documents and Settings\\PC1\\Desktop Link target: testas 2.txt",
                            2.5,
                            "Filename",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            1
                        )
                    ),
                    "testas 2.txt"
                },
                new object[]
                {
                    new EventViewModel(
                        new EventModel(
                            new DateOnly(2022, 10, 14),
                            new TimeOnly(10, 52),
                            TimeZoneInfo.Local,
                            "MACB",
                            "LOG",
                            "Source Type",
                            "Type",
                            "Username",
                            "Hostname",
                            "Short Description",
                            "Entry: 36 Pin status: Unpinned Path: C:\\temp\\testas 3.txt",
                            2.5,
                            "Filename",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            1
                        )
                    ),
                    "testas 3.txt"
                },
                new object[]
                {
                    new EventViewModel(
                        new EventModel(
                            new DateOnly(2022, 10, 14),
                            new TimeOnly(10, 52),
                            TimeZoneInfo.Local,
                            "MACB",
                            "REG",
                            "Source Type",
                            "Type",
                            "Username",
                            "Hostname",
                            "Short Description",
                            "Full Description",
                            2.5,
                            "Filename",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            1
                        )
                    ),
                    ""
                },
                new object[]
                {
                    new EventViewModel(
                        new EventModel(
                            new DateOnly(2022, 10, 14),
                            new TimeOnly(10, 52),
                            TimeZoneInfo.Local,
                            "MACB",
                            "FILE",
                            "Source Type",
                            "Type",
                            "Username",
                            "Hostname",
                            "something\\temp\\testas 4.txt...",
                            "Full Description",
                            2.5,
                            "Filename",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            1
                        )
                    ),
                    "testas 4.txt..."
                },
                new object[]
                {
                    new EventViewModel(
                        new EventModel(
                            new DateOnly(2022, 10, 14),
                            new TimeOnly(10, 52),
                            TimeZoneInfo.Local,
                            "MACB",
                            "FILE",
                            "Source Type",
                            "Type",
                            "Username",
                            "Hostname",
                            "Name: testas 5.txt",
                            "Full Description",
                            2.5,
                            "Filename",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            1
                        )
                    ),
                    "testas 5.txt"
                },
                new object[]
                {
                    new EventViewModel(
                        new EventModel(
                            new DateOnly(2022, 10, 14),
                            new TimeOnly(10, 52),
                            TimeZoneInfo.Local,
                            "MACB",
                            "FILE",
                            "Source Type",
                            "Type",
                            "Username",
                            "Hostname",
                            "testas 6.txt",
                            "Full Description",
                            2.5,
                            "Filename",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            1
                        )
                    ),
                    "testas 6.txt"
                }
            };
        }
    }

    private static IEnumerable<object[]> InvalidFileEvents
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
                            "OS Last Access Time",
                            "Type",
                            "Username",
                            "Hostname",
                            "C:\\Users\\User\\AppData\\something",
                            "Full Description",
                            2.5,
                            "Filename",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            1
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
                            "OS Metadata Modification Time",
                            "Type",
                            "Username",
                            "Hostname",
                            "C:\\Users\\User\\AppData\\something",
                            "Full Description",
                            2.5,
                            "Filename",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            1
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
                            "OS Content Modification Time",
                            "Type",
                            "Username",
                            "Hostname",
                            "C:\\Users\\User\\AppData\\something",
                            "Full Description",
                            2.5,
                            "Filename",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            1
                        )
                    )
                }
            };
        }
    }

    private static IEnumerable<object[]> ValidFileEvents
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
                            "Creation Time",
                            "Type",
                            "Username",
                            "Hostname",
                            "C:\\Users\\User\\AppData\\something",
                            "Full Description",
                            2.5,
                            "Filename",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            1
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
                            "OS Metadata Modification Time",
                            "Type",
                            "Username",
                            "Hostname",
                            "something something",
                            "Full Description",
                            2.5,
                            "Filename",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            1
                        )
                    )
                }
            };
        }
    }

    private static IEnumerable<object[]> InvalidRegEvents
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
                            "Creation Time",
                            "Type",
                            "Username",
                            "Hostname",
                            "Something something",
                            "Full Description",
                            2.5,
                            "Filename",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            1
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
                            "Registry Key: UserAssist",
                            "Type",
                            "Username",
                            "Hostname",
                            "Something something",
                            "Full Description",
                            2.5,
                            "Filename",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            1
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
                            "Creation Time",
                            "Type",
                            "Username",
                            "Hostname",
                            "UEME_RUNPIDL:::{2559A1F4-21D7-11D4-BDAF-00C04F60B9F0}\\Windows NT\\Accessories\\wordpad.exe Cou...",
                            "Full Description",
                            2.5,
                            "Filename",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            1
                        )
                    )
                }
            };
        }
    }

    [DataTestMethod]
    [DataRow("-", "https://mail.google.com/mail/u/0/#inbox (Gautieji (170) - testmail@gmail.com - Gmail) [count: 2] Host: mail.google.com (URL not typed directly) Transition: LINK", "Mail User: testmail@gmail.com")]
    [DataRow("Something extra", "https://mail.google.com/mail/u/0/#inbox (Gautieji (170) - testmail@gmail.com - Gmail) [count: 2] Host: mail.google.com (URL not typed directly) Transition: LINK", "Something extra Mail User: testmail@gmail.com")]
    public void AddMailUser_ShouldAppendMailUser_WhenGmailKeywordExists(string extra, string description, string expected)
    {
        // Arrange
        LowLevelEventsAbstractorUtils utils = new();

        // Act
        string actual = utils.AddMailUser(extra, description);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [DataTestMethod]
    [DataRow("-", "https://mail.google.com/mail/u/0/#inbox (Gautieji (170) - Gmail) [count: 2] Host: mail.google.com (URL not typed directly) Transition: LINK", "-")]
    [DataRow("Something extra", "https://mail.google.com/mail/u/0/#inbox (Gautieji (170) - Gmail) [count: 2] Host: mail.google.com (URL not typed directly) Transition: LINK", "Something extra")]
    public void AddMailUser_ShouldReturnExtra_WhenGmailKeywordDoesNotExist(string extra, string description, string expected)
    {
        // Arrange
        LowLevelEventsAbstractorUtils utils = new();

        // Act
        string actual = utils.AddMailUser(extra, description);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void GetExtraFromFileSource_ShouldReturnFormattedKeyValuePairs_WhenMethodIsCalled()
    {
        // Arrange
        Dictionary<string, string> extra = new() { { "file_size", "4450" }, { "file_system_type", "OS" }, { "is_allocated", "True" }, { "sha256_hash", "9a449ed20986127ce1d7e000bc5525b10f47839742c85b3833d02cf884717061" } };
        string expected = "file_size: 4450 file_system_type: OS is_allocated: True";
        LowLevelEventsAbstractorUtils utils = new();

        // Act
        string actual = utils.GetExtraTillSha256(extra);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DynamicData(nameof(FirefoxWebhistExtra))]
    public void GetFirefoxExtraFromWebhistSource_ShouldExtractVisits_WhenExtraKeyIsInExtraColumn(Dictionary<string, string> extra, string expected)
    {
        // Arrange
        LowLevelEventsAbstractorUtils utils = new();

        // Act
        string actual = utils.GetFirefoxExtraFromWebhistSource(extra);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DynamicData(nameof(MetaExtra))]
    public void GetKeywordsFromExtra_ShouldExtractExtraFields_WhenExtraHasRequiredKeys(Dictionary<string, string> extra, string shortDescription, string expected)
    {
        // Arrange
        LowLevelEventsAbstractorUtils utils = new();

        // Act
        string actual = utils.GetKeywordsFromExtra(extra, shortDescription);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DynamicData(nameof(EventsWithFilename))]
    public void GetFilename_ShouldReturnFilename_WhenMethodIsCalled(EventViewModel eventViewModel, string expected)
    {
        // Arrange
        LowLevelEventsAbstractorUtils utils = new();

        // Act
        string actual = utils.GetFilename(eventViewModel);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void GetShort_ShouldReturnPathValue_IfPathKeywordExists()
    {
        // Arrange
        string description = "Entry: 36 Pin status: Unpinned Path: C:\\temp\\testas 1.txt";
        string expected = "C:\\temp\\testas 1.txt";
        LowLevelEventsAbstractorUtils utils = new();

        // Act
        string actual = utils.GetShort(description);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void GetShort_ShouldReturnOriginValue_IfOriginKeywordExists()
    {
        // Arrange
        string description = "Soemthing something Origin: C:\\temp\\testas 1.txt";
        string expected = "C:\\temp\\testas 1.txt";
        LowLevelEventsAbstractorUtils utils = new();

        // Act
        string actual = utils.GetShort(description);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void GetShort_ShouldReturnGivenValue_IfNoKeywordExists()
    {
        // Arrange
        string description = "Prefetch [SPOOLSV.EXE] was executed - run count 1 path: \\WINDOWS\\SYSTEM32\\SPOOLSV.EXE hash: 0x282F76A7 volume: 1 [serial number: 0x6C91EF9F  device path: \\DEVICE\\HARDDISKVOLUME1]";
        string expected = "Prefetch [SPOOLSV.EXE] was executed - run count 1 path: \\WINDOWS\\SYSTEM32\\SPOOLSV.EXE hash: 0x282F76A7 volume: 1 [serial number: 0x6C91EF9F  device path: \\DEVICE\\HARDDISKVOLUME1]";
        LowLevelEventsAbstractorUtils utils = new();

        // Act
        string actual = utils.GetShort(description);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [DataTestMethod]
    [DataRow("UEME_RUNPIDL:::{2559A1F4-21D7-11D4-BDAF-00C04F60B9F0}\\Windows NT\\Accessories\\wordpad.exe Cou...", "{2559A1F4-21D7-11D4-BDAF-00C04F60B9F0}\\Windows NT\\Accessories\\wordpad.exe")]
    [DataRow("UEME_RUNPIDL:::{2559A1F4-21D7-11D4-BDAF-00C04F60B9F0}\\Windows NT\\Accessories\\wordpad 2.exe", "{2559A1F4-21D7-11D4-BDAF-00C04F60B9F0}\\Windows NT\\Accessories\\wordpad 2.exe")]
    [DataRow("UEME_RUNPIDL:::{2559A1F4-21D7-11D4-BDAF-00C04F60B9F0} Cou...", "{2559A1F4-21D7-11D4-BDAF-00C04F60B9F0}")]
    [DataRow("UEME_RUNPIDL:::{2559A1F4-21D7-11D4-BDAF-00C04F60B9F0}", "{2559A1F4-21D7-11D4-BDAF-00C04F60B9F0}")]
    [DataRow("UEME_RUNPIDL:::{2559A1F4-21D7-11D4-BDAF-00C04F60B9F0}\\Windows\\Accessories\\wordpad3.exe", "{2559A1F4-21D7-11D4-BDAF-00C04F60B9F0}\\Windows\\Accessories\\wordpad3.exe")]
    [DataRow("Something else", "Something else")]
    public void GetSummaryFromShort_ShouldReturnFormattedShortDescription_WhenMethodIsCalled(string description, string expected)
    {
        // Arrange
        LowLevelEventsAbstractorUtils utils = new();

        // Act
        string actual = utils.GetSummaryFromShort(description);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [DataTestMethod]
    [DataRow("URL: https://www.youtube.com/", "https://www.youtube.com/")]
    [DataRow("URL: https://www.liveaction.com/voip/", "https://www.liveaction.com/voip/")]
    [DataRow("URL: https://ethereal.en.uptodown.com/windows/download", "https://ethereal.en.uptodown.com/windows/")]
    [DataRow("URL: https://ethereal.en.uptodown.com/windows", "https://ethereal.en.uptodown.com/windows")]
    public void GetUrl_ShouldReturnFormattedUrl_WhenMethodIsCalled(string description, string expected)
    {
        // Arrange
        LowLevelEventsAbstractorUtils utils = new();

        // Act
        string actual = utils.GetUrl(description);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DynamicData(nameof(ValidFileEvents))]
    public void IsValidFileEvent_ShouldReturnTrue_WhenEventIsValid(EventViewModel eventViewModel)
    {
        // Arrange
        LowLevelEventsAbstractorUtils utils = new();

        // Act
        bool actual = utils.IsValidFileEvent(eventViewModel);

        // Assert
        Assert.IsTrue(actual);
    }

    [TestMethod]
    [DynamicData(nameof(InvalidFileEvents))]
    public void IsValidFileEvent_ShouldReturnFalse_WhenEventIsInvalid(EventViewModel eventViewModel)
    {
        // Arrange
        LowLevelEventsAbstractorUtils utils = new();

        // Act
        bool actual = utils.IsValidFileEvent(eventViewModel);

        // Assert
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void IsValidRegEvent_ShouldReturnTrue_WhenEventIsValid()
    {
        // Arrange
        EventViewModel eventViewModel = new(new EventModel(new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "Source", "Registry Key: UserAssist", "Type", "Username", "Hostname", "UEME_RUNPIDL:::{2559A1F4-21D7-11D4-BDAF-00C04F60B9F0}\\Windows NT\\Accessories\\wordpad.exe Cou...", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }, 1 ));
        LowLevelEventsAbstractorUtils utils = new();

        // Act
        bool actual = utils.IsValidRegEvent(eventViewModel);

        // Assert
        Assert.IsTrue(actual);
    }

    [TestMethod]
    [DynamicData(nameof(InvalidRegEvents))]
    public void IsValidRegEvent_ShouldReturnFalse_WhenEventIsInvalid(EventViewModel eventViewModel)
    {
        // Arrange
        LowLevelEventsAbstractorUtils utils = new();

        // Act
        bool actual = utils.IsValidRegEvent(eventViewModel);

        // Assert
        Assert.IsFalse(actual);
    }
}
