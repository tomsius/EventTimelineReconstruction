using EventTimelineReconstruction.Abstractors;
using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;
using Moq;

namespace EventTimelineReconstruction.Tests.UnitTests.Abstractors;

[TestClass]
public class HighLevelEventsAbstractorTests
{
    private readonly HighLevelEventsAbstractor _abstractor;
    private readonly List<EventViewModel> _events;

    public HighLevelEventsAbstractorTests()
    {
        Mock<IHighLevelEventsAbstractorUtils> utils = new();
        _abstractor = new(utils.Object);

        _events = GetStoredEvents();

        utils.Setup(u => u.IsValidWebhistLine(_events[0])).Returns(true);
        utils.Setup(u => u.IsWebhistDownloadEvent(_events[0])).Returns(false);
        utils.Setup(u => u.IsWebhistMailEvent(_events[0])).Returns(true);
        utils.Setup(u => u.GetMacAddress("3ec2b817-c405-11e7-8ac5-a0afbdac1ec0 Origin: 2015.lnk")).Returns("a0afbdac1ec0");
        utils.Setup(u => u.GetOrigin("3ec2b817-c405-11e7-8ac5-a0afbdac1ec0 Origin: 2015.lnk")).Returns("2015.lnk");
        utils.Setup(u => u.GetMacAddress("3ec2b817-c405-11e7-8ac5-a0afbdac1ec0 Origin: 2016.lnk")).Returns("a0afbdac1ec0");
        utils.Setup(u => u.GetOrigin("3ec2b817-c405-11e7-8ac5-a0afbdac1ec0 Origin: 2016.lnk")).Returns("2016.lnk");
        utils.Setup(u => u.GetMacAddress("3ec2b817-c405-11e7-8ac5-a0afbdac1ec0 Origin: 2017.lnk")).Returns("a0afbdac1ec0");
        utils.Setup(u => u.GetOrigin("3ec2b817-c405-11e7-8ac5-a0afbdac1ec0 Origin: 2017.lnk")).Returns("2017.lnk");
        utils.Setup(u => u.GetShort("[Empty description] C:\\Program Files\\Mozilla Firefox\\firefox.exe")).Returns("C:\\Program Files\\Mozilla Firefox\\firefox.exe");
        utils.Setup(u => u.IsValidPeEvent(_events[10])).Returns(true);
        utils.Setup(u => u.IsValidPeEvent(_events[11])).Returns(true);
        utils.Setup(u => u.IsValidPeEvent(_events[12])).Returns(false);
        utils.Setup(u => u.IsValidWebhistLine(_events[13])).Returns(false);
        utils.Setup(u => u.IsValidWebhistLine(_events[14])).Returns(true);
        utils.Setup(u => u.IsWebhistDownloadEvent(_events[14])).Returns(false);
        utils.Setup(u => u.IsWebhistMailEvent(_events[14])).Returns(false);
        utils.Setup(u => u.IsWebhistNamingActivityEvent(_events[14])).Returns(false);
        utils.Setup(u => u.IsValidWebhistLine(_events[15])).Returns(true);
        utils.Setup(u => u.IsWebhistDownloadEvent(_events[15])).Returns(true);
        utils.Setup(u => u.IsWebhistMailEvent(_events[15])).Returns(false);
        utils.Setup(u => u.IsWebhistNamingActivityEvent(_events[15])).Returns(false);
        utils.Setup(u => u.GetDownloadedFileName("https://mail-attachment.googleusercontent.com/attachment/... (C:\\Documents and Settings\\PC1\\My Documents\\Downloads\\Timeline2GUI A Log2Timeline CSV parser and training scenarios.pdf). Received: 1116192 bytes out of: 1116192 bytes.")).Returns("C:\\Documents and Settings\\PC1\\My Documents\\Downloads\\Timeline2GUI A Log2Timeline CSV parser and training scenarios.pdf");
        utils.Setup(u => u.IsValidWebhistLine(_events[16])).Returns(true);
        utils.Setup(u => u.IsWebhistDownloadEvent(_events[16])).Returns(false);
        utils.Setup(u => u.IsWebhistMailEvent(_events[16])).Returns(true);
        utils.Setup(u => u.IsWebhistNamingActivityEvent(_events[16])).Returns(false);
        utils.Setup(u => u.GetMailUrl(It.IsAny<string>())).Returns("https://mail.google.com");
        utils.Setup(u => u.IsValidWebhistLine(_events[17])).Returns(true);
        utils.Setup(u => u.IsWebhistDownloadEvent(_events[17])).Returns(false);
        utils.Setup(u => u.IsWebhistMailEvent(_events[17])).Returns(false);
        utils.Setup(u => u.IsWebhistNamingActivityEvent(_events[17])).Returns(true);
        utils.Setup(u => u.GetUrlHost("https://google.com/mail/u/0/?tab=rm&ogbl (Gmail)")).Returns("https://www.google.com/");
        utils.Setup(u => u.GenerateVisitValue("https://google.com/mail/u/0/?tab=rm&ogbl (Gmail) [count: 0] Visit from: https://google.com/mail/?tab=rm&ogbl (Gmail) Type: [LINK - User clicked a link] (URL not typed directly - no typed count)")).Returns("LINK");
        utils.Setup(u => u.IsValidWebhistLine(_events[18])).Returns(true);
        utils.Setup(u => u.IsWebhistDownloadEvent(_events[18])).Returns(false);
        utils.Setup(u => u.IsWebhistMailEvent(_events[18])).Returns(false);
        utils.Setup(u => u.IsWebhistNamingActivityEvent(_events[18])).Returns(true);
        utils.Setup(u => u.IsValidWebhistLine(_events[19])).Returns(true);
        utils.Setup(u => u.IsWebhistDownloadEvent(_events[19])).Returns(false);
        utils.Setup(u => u.IsWebhistMailEvent(_events[19])).Returns(false);
        utils.Setup(u => u.IsWebhistNamingActivityEvent(_events[19])).Returns(true);
    }

    private List<EventViewModel> GetStoredEvents()
    {
        return new List<EventViewModel>()
        {
            new EventViewModel(new EventModel(new DateOnly(2018, 10, 21), new TimeOnly(18, 11, 1), TimeZoneInfo.Utc, "B...", "WEBHIST", "Firefox History", "Last Visited Time", "User", "Host", "https://google.com/mail/u/0/?tab=rm&ogbl (Gmail)", "https://google.com/mail/u/0/?tab=rm&ogbl (Gmail) [count: 0] Visit from: https://google.com/mail/?tab=rm&ogbl (Gmail) Type: [LINK - User clicked a link] (URL not typed directly - no typed count)", 2, "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", "13452", "-", "sqlite/chrome_27_history", new Dictionary<string, string>(), 0)),
            new EventViewModel(new EventModel(new DateOnly(2022, 10, 14), new TimeOnly(15, 4, 25), TimeZoneInfo.Utc, "B..M", "LOG", "System", "Creation Time", "User", "Host", "3ec2b817-c405-11e7-8ac5-a0afbdac1ec0 Origin: 2015.lnk", "3ec2b817-c405-11e7-8ac5-a0afbdac1ec0 Origin: 2015.lnk", 2, "2016.ln", "13451", "-", "lnk", new Dictionary<string, string>(), 1)),
            new EventViewModel(new EventModel(new DateOnly(2022, 10, 14), new TimeOnly(15, 4, 25), TimeZoneInfo.Utc, "B...", "LOG", "System", "Creation Time", "User", "Host", "3ec2b817-c405-11e7-8ac5-a0afbdac1ec0 Origin: 2016.lnk", "3ec2b817-c405-11e7-8ac5-a0afbdac1ec0 Origin: 2016.lnk", 2, "2016.ln", "13452", "-", "lnk", new Dictionary<string, string>(), 2)),
            new EventViewModel(new EventModel(new DateOnly(2022, 10, 15), new TimeOnly(12, 8, 11), TimeZoneInfo.Utc, "B...", "LOG", "System", "Creation Time", "User", "Host", "3ec2b817-c405-11e7-8ac5-a0afbdac1ec0 Origin: 2017.lnk", "3ec2b817-c405-11e7-8ac5-a0afbdac1ec0 Origin: 2017.lnk", 2, "2017.ln", "13452", "-", "lnk", new Dictionary<string, string>(), 3)),
            new EventViewModel(new EventModel(new DateOnly(2022, 10, 15), new TimeOnly(12, 8, 11), TimeZoneInfo.Utc, "B..M", "LNK", "Windows Shortcut", "Creation Time", "User", "Host", "[Empty description] C:\\Program Files\\Mozilla Firefox\\firefox.exe", "[Empty description] C:\\Program Files\\Mozilla Firefox\\firefox.exe", 2, "TSK:/Documents and Settings/All Users/Start Menu/Programs/Mozilla Firefox.lnk", "13451", "-", "lnk", new Dictionary<string, string>(), 4)),
            new EventViewModel(new EventModel(new DateOnly(2022, 10, 15), new TimeOnly(12, 8, 11), TimeZoneInfo.Utc, "B...", "LNK", "Windows Shortcut", "Creation Time", "User", "Host", "[Empty description] C:\\Program Files\\Mozilla Firefox\\firefox.exe", "[Empty description] C:\\Program Files\\Mozilla Firefox\\firefox.exe", 2, "TSK:/Documents and Settings/All Users/Start Menu/Programs/Mozilla Firefox.lnk", "13452", "-", "lnk", new Dictionary<string, string>(), 5)),
            new EventViewModel(new EventModel(new DateOnly(2022, 10, 16), new TimeOnly(5, 18, 15), TimeZoneInfo.Utc, "B..M", "META", "System", "Creation Time", "User", "Host", "Something something", "Something something", 2, "C:\\Program Files\\Mozilla Firefox\\firefox.exe", "13451", "-", "meta", new Dictionary<string, string>(), 6)),
            new EventViewModel(new EventModel(new DateOnly(2022, 10, 16), new TimeOnly(5, 18, 15), TimeZoneInfo.Utc, "B...", "META", "System", "Creation Time", "User", "Host", "Something something", "Something something", 2, "C:\\Program Files\\Mozilla Firefox\\chrome.exe", "13452", "-", "meta", new Dictionary<string, string>(), 7)),
            new EventViewModel(new EventModel(new DateOnly(2022, 10, 17), new TimeOnly(10, 12, 57), TimeZoneInfo.Utc, "B..M", "OLECF", "OLECF Item", "Creation Time", "User", "Host", "Something something", "Something something", 2, "TSK:/WINDOWS/system32/wmimgmt.msc", "13451", "-", "olecf/olecf_default", new Dictionary<string, string>(), 8)),
            new EventViewModel(new EventModel(new DateOnly(2022, 10, 17), new TimeOnly(10, 12, 57), TimeZoneInfo.Utc, "B...", "OLECF", "OLECF Item", "Creation Time", "User", "Host", "Something something", "Something something", 2, "TSK:/WINDOWS/system32/wmimgmt.msc", "13452", "-", "olecf/olecf_default", new Dictionary<string, string>(), 9)),
            new EventViewModel(new EventModel(new DateOnly(2022, 10, 18), new TimeOnly(17, 2, 33), TimeZoneInfo.Utc, "B..M", "PE", "PE Compilation time", "Creation Time", "User", "Host", "pe_type", "Something something", 2, "test.exe", "13451", "-", "pe", new Dictionary<string, string>(), 10)),
            new EventViewModel(new EventModel(new DateOnly(2022, 10, 18), new TimeOnly(17, 2, 33), TimeZoneInfo.Utc, "B...", "PE", "PE Compilation time", "Creation Time", "User", "Host", "pe_type", "Something something", 2, "test.exe", "13452", "-", "pe", new Dictionary<string, string>(), 11)),
            new EventViewModel(new EventModel(new DateOnly(2022, 10, 18), new TimeOnly(20, 45, 13), TimeZoneInfo.Utc, "B...", "PE", "PE Compilation time", "Creation Time", "User", "Host", "Something something", "Something something", 2, "TSK:/WINDOWS/system32/wmimgmt.msc", "13452", "-", "pe", new Dictionary<string, string>(), 12)),
            new EventViewModel(new EventModel(new DateOnly(2022, 10, 18), new TimeOnly(20, 45, 13), TimeZoneInfo.Utc, "B...", "FILE", "NTFS Content Modification Time", "Creation Time", "User", "Host", "Something something", "Something something", 2, "TSK:/WINDOWS/system32/drivers/cpqdap01.sys", "13452", "-", "filestat", new Dictionary<string, string>(), 13)),
            new EventViewModel(new EventModel(new DateOnly(2022, 10, 18), new TimeOnly(20, 45, 13), TimeZoneInfo.Utc, "B...", "WEBHIST", "MSIE Cache File leak record", "Not a time", "User", "Host", "Something something", "Something something", 2, "TSK:/Documents and Settings/PC1/Local Settings/Temporary Internet Files/Content.IE5/index.dat", "13452", "-", "msiecf", new Dictionary<string, string>(), 14)),
            new EventViewModel(new EventModel(new DateOnly(2022, 10, 19), new TimeOnly(17, 21, 1), TimeZoneInfo.Utc, "B...", "WEBHIST", "Firefox History", "File downloaded", "User", "Host", "C:\\Documents and Settings\\PC1\\My Documents\\Downloads\\Timeline2GUI A Log2Timel...", "https://mail-attachment.googleusercontent.com/attachment/... (C:\\Documents and Settings\\PC1\\My Documents\\Downloads\\Timeline2GUI A Log2Timeline CSV parser and training scenarios.pdf). Received: 1116192 bytes out of: 1116192 bytes.", 2, "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", "13452", "-", "sqlite/firefox_history", new Dictionary<string, string>(), 15)),
            new EventViewModel(new EventModel(new DateOnly(2022, 10, 20), new TimeOnly(17, 21, 1), TimeZoneInfo.Utc, "B...", "WEBHIST", "Google History", "Last Visited Time", "User", "Host", "https://www.mail.google.com/ (Google)", "https://www.mail.google.com/ (Google) [count: 0]", 2, "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", "13452", "-", "sqlite/chrome_27_history", new Dictionary<string, string>(), 16)),
            new EventViewModel(new EventModel(new DateOnly(2022, 10, 21), new TimeOnly(17, 21, 1), TimeZoneInfo.Utc, "B...", "WEBHIST", "Firefox History", "Last Visited Time", "User", "Host", "https://google.com/mail/u/0/?tab=rm&ogbl (Gmail)", "https://google.com/mail/u/0/?tab=rm&ogbl (Gmail) [count: 0] Visit from: https://google.com/mail/?tab=rm&ogbl (Gmail) Type: [LINK - User clicked a link] (URL not typed directly - no typed count)", 2, "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", "13452", "-", "sqlite/chrome_27_history", new Dictionary<string, string>(), 17)),
            new EventViewModel(new EventModel(new DateOnly(2022, 10, 21), new TimeOnly(17, 30, 15), TimeZoneInfo.Utc, "B...", "WEBHIST", "Firefox History", "Last Visited Time", "User", "Host", "https://google.com/mail/u/0/?tab=rm&ogbl (Gmail)", "https://google.com/mail/u/0/?tab=rm&ogbl (Gmail) [count: 0] Visit from: https://google.com/mail/?tab=rm&ogbl (Gmail) Type: [LINK - User clicked a link] (URL not typed directly - no typed count)", 2, "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", "13452", "-", "sqlite/chrome_27_history", new Dictionary<string, string>(), 18)),
            new EventViewModel(new EventModel(new DateOnly(2022, 10, 21), new TimeOnly(18, 11, 1), TimeZoneInfo.Utc, "B...", "WEBHIST", "Firefox History", "Last Visited Time", "User", "Host", "https://google.com/mail/u/0/?tab=rm&ogbl (Gmail)", "https://google.com/mail/u/0/?tab=rm&ogbl (Gmail) [count: 0] Visit from: https://google.com/mail/?tab=rm&ogbl (Gmail) Type: [LINK - User clicked a link] (URL not typed directly - no typed count)", 2, "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", "13452", "-", "sqlite/chrome_27_history", new Dictionary<string, string>(), 19))
        };
    }

    private List<HighLevelEventViewModel> GetExpectedEvents()
    {
        return new List<HighLevelEventViewModel>()
        {
            new HighLevelEventViewModel() { Date = new DateOnly(2018, 10, 21), Time = new TimeOnly(18, 11, 1), Source = "WEBHIST", Short = "https://mail.google.com", Reference = 0, Visit = "Mail" },
            new HighLevelEventViewModel() { Date = new DateOnly(2022, 10, 14), Time = new TimeOnly(15, 4, 25), Source = "LOG", Short = "MAC Address: a0afbdac1ec0. Origin: 2016.lnk.", Reference = 2 },
            new HighLevelEventViewModel() { Date = new DateOnly(2022, 10, 15), Time = new TimeOnly(12, 8, 11), Source = "LOG", Short = "MAC Address: a0afbdac1ec0. Origin: 2017.lnk.", Reference = 3 },
            new HighLevelEventViewModel() { Date = new DateOnly(2022, 10, 15), Time = new TimeOnly(12, 8, 11), Source = "LNK", Short = "C:\\Program Files\\Mozilla Firefox\\firefox.exe", Reference = 5 },
            new HighLevelEventViewModel() { Date = new DateOnly(2022, 10, 16), Time = new TimeOnly(5, 18, 15), Source = "META", Short = "C:\\Program Files\\Mozilla Firefox\\firefox.exe", Reference = 6 },
            new HighLevelEventViewModel() { Date = new DateOnly(2022, 10, 17), Time = new TimeOnly(10, 12, 57), Source = "OLECF", Short = "TSK:/WINDOWS/system32/wmimgmt.msc", Reference = 8 },
            new HighLevelEventViewModel() { Date = new DateOnly(2022, 10, 18), Time = new TimeOnly(17, 2, 33), Source = "PE", Short = "test.exe", Reference = 10 },
            new HighLevelEventViewModel() { Date = new DateOnly(2022, 10, 19), Time = new TimeOnly(17, 21, 1), Source = "WEBHIST", Short = "C:\\Documents and Settings\\PC1\\My Documents\\Downloads\\Timeline2GUI A Log2Timeline CSV parser and training scenarios.pdf", Reference = 15, Visit = "Download" },
            new HighLevelEventViewModel() { Date = new DateOnly(2022, 10, 20), Time = new TimeOnly(17, 21, 1), Source = "WEBHIST", Short = "https://mail.google.com", Reference = 16, Visit = "Mail" },
            new HighLevelEventViewModel() { Date = new DateOnly(2022, 10, 21), Time = new TimeOnly(17, 21, 1), Source = "WEBHIST", Short = "https://www.google.com/", Reference = 17, Visit = "LINK" },
            new HighLevelEventViewModel() { Date = new DateOnly(2022, 10, 21), Time = new TimeOnly(18, 11, 1), Source = "WEBHIST", Short = "https://www.google.com/", Reference = 19, Visit = "LINK" }
        };
    }

    [TestMethod]
    public void FormHighLevelEvents_ShouldReturnHighLevelEvents_WhenMethodIsCalled()
    {
        // Arrange
        List<HighLevelEventViewModel> expected = GetExpectedEvents();

        // Act
        List<HighLevelEventViewModel> actual = _abstractor.FormHighLevelEvents(_events).Cast<HighLevelEventViewModel>().ToList();

        // Assert
        Assert.AreEqual(expected.Count, actual.Count);

        for (int i = 0; i < expected.Count; i++)
        {
            Assert.AreEqual(expected[i].Date, actual[i].Date);
            Assert.AreEqual(expected[i].Time, actual[i].Time);
            Assert.AreEqual(expected[i].Source, actual[i].Source);
            Assert.AreEqual(expected[i].Short, actual[i].Short);
            Assert.AreEqual(expected[i].Reference, actual[i].Reference);
            Assert.AreEqual(expected[i].Visit, actual[i].Visit);
        }
    }
}
