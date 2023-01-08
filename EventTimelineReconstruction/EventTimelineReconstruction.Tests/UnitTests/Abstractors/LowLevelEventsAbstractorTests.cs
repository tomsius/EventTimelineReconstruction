using EventTimelineReconstruction.Abstractors;
using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;
using Moq;

namespace EventTimelineReconstruction.Tests.UnitTests.Abstractors;

[TestClass]
public class LowLevelEventsAbstractorTests
{
    private readonly LowLevelEventsAbstractor _abstractor;
    private readonly List<EventViewModel> _events;
    private readonly Mock<IHighLevelEventsAbstractorUtils> _highLevelUtils;
    private readonly Mock<ILowLevelEventsAbstractorUtils> _lowLevelUtils;

    public LowLevelEventsAbstractorTests()
    {
        _highLevelUtils = new();
        _lowLevelUtils = new();
        _abstractor = new(_highLevelUtils.Object, _lowLevelUtils.Object);

        _events = GetStoredEvents();

        _highLevelUtils.Setup(u => u.IsValidWebhistLine(_events[0])).Returns(false);

        _highLevelUtils.Setup(u => u.IsValidWebhistLine(_events[1])).Returns(true);
        _lowLevelUtils.Setup(u => u.GetFirefoxExtraFromWebhistSource(_events[1].Extra)).Returns("https://www.google.com/search?client=firefox-b-ab&q=ekiga&oq=ekiga&aqs=heirloom-srp..0l5");
        _highLevelUtils.Setup(u => u.IsWebhistDownloadEvent(_events[1])).Returns(true);
        _highLevelUtils.Setup(u => u.GetDownloadedFileName(_events[1].Description)).Returns("C:\\Documents and Settings\\PC1\\My Documents\\Downloads\\Timeline2GUI A Log2Timeline CSV parser and training scenarios.pdf");

        _highLevelUtils.Setup(u => u.IsValidWebhistLine(_events[2])).Returns(true);
        _highLevelUtils.Setup(u => u.IsWebhistDownloadEvent(_events[2])).Returns(false);
        _highLevelUtils.Setup(u => u.IsWebhistMailEvent(_events[2])).Returns(true);
        _highLevelUtils.Setup(u => u.GetMailUrl(It.IsAny<string>())).Returns("https://mail.google.com");
        _lowLevelUtils.Setup(u => u.AddMailUser("-", _events[2].Description)).Returns("testas@gmail.com");

        _highLevelUtils.Setup(u => u.IsValidWebhistLine(_events[3])).Returns(true);
        _highLevelUtils.Setup(u => u.IsWebhistDownloadEvent(_events[3])).Returns(false);
        _highLevelUtils.Setup(u => u.IsWebhistMailEvent(_events[3])).Returns(true);
        _lowLevelUtils.Setup(u => u.AddMailUser("-", _events[3].Description)).Returns("testas@gmail.com");

        _highLevelUtils.Setup(u => u.IsValidWebhistLine(_events[4])).Returns(true);
        _highLevelUtils.Setup(u => u.IsWebhistDownloadEvent(_events[4])).Returns(false);
        _highLevelUtils.Setup(u => u.IsWebhistMailEvent(_events[4])).Returns(true);
        _lowLevelUtils.Setup(u => u.AddMailUser("-", _events[4].Description)).Returns("testas@gmail.com");

        _highLevelUtils.Setup(u => u.IsValidWebhistLine(_events[5])).Returns(true);
        _highLevelUtils.Setup(u => u.IsWebhistDownloadEvent(_events[5])).Returns(false);
        _highLevelUtils.Setup(u => u.IsWebhistMailEvent(_events[5])).Returns(false);
        _highLevelUtils.Setup(u => u.IsWebhistNamingActivityEvent(_events[5])).Returns(true);
        _highLevelUtils.Setup(u => u.GenerateVisitValue(_events[5].Description)).Returns("LINK");
        _lowLevelUtils.Setup(u => u.GetUrl(_events[5].Short)).Returns("https://www.google.com/mail/");

        _highLevelUtils.Setup(u => u.IsValidWebhistLine(_events[6])).Returns(true);
        _highLevelUtils.Setup(u => u.IsWebhistDownloadEvent(_events[6])).Returns(false);
        _highLevelUtils.Setup(u => u.IsWebhistMailEvent(_events[6])).Returns(false);
        _highLevelUtils.Setup(u => u.IsWebhistNamingActivityEvent(_events[6])).Returns(false);

        _lowLevelUtils.Setup(u => u.GetKeywordsFromExtra(_events[7].Extra, _events[7].Short)).Returns("number_of_paragraphs: 3 total_time: 1");

        _lowLevelUtils.Setup(u => u.IsValidRegEvent(_events[8])).Returns(true);
        _lowLevelUtils.Setup(u => u.GetSummaryFromShort(_events[8].Description)).Returns("{645FF040-5081-101B-9F08-00AA002F954E}");
        _lowLevelUtils.Setup(u => u.GetExtraTillSha256(_events[8].Extra)).Returns("something1: something1");

        _lowLevelUtils.Setup(u => u.IsValidRegEvent(_events[9])).Returns(false);

        _highLevelUtils.Setup(u => u.IsValidPeEvent(_events[10])).Returns(true);

        _highLevelUtils.Setup(u => u.IsValidPeEvent(_events[11])).Returns(false);

        _lowLevelUtils.Setup(u => u.GetExtraTillSha256(_events[12].Extra)).Returns("drive_number: 2 file_size: 4096");

        _highLevelUtils.Setup(u => u.GetShort(_events[16].Description)).Returns("C:\\Program Files\\Mozilla Firefox\\firefox.exe");
        _lowLevelUtils.Setup(u => u.GetFilename(_events[15])).Returns(_events[15].Short);
        _lowLevelUtils.Setup(u => u.GetFilename(_events[16])).Returns("firefox.exe");

        _highLevelUtils.Setup(u => u.GetShort(_events[17].Description)).Returns("C:\\Program Files\\Mozilla Firefox\\firefox.exe");
        _lowLevelUtils.Setup(u => u.GetFilename(_events[17])).Returns("firefox.exe");


        _highLevelUtils.Setup(u => u.GetShort(_events[18].Description)).Returns("C:\\Program Files\\Mozilla Firefox\\firefox2.exe");
        _lowLevelUtils.Setup(u => u.GetFilename(_events[18])).Returns("firefox2.exe");

        _lowLevelUtils.Setup(u => u.GetShort(_events[19].Description)).Returns("2016.exe");
        _lowLevelUtils.Setup(u => u.GetFilename(_events[19])).Returns("2016.exe");

        _lowLevelUtils.Setup(u => u.IsValidFileEvent(_events[20])).Returns(false);
        _lowLevelUtils.Setup(u => u.GetFilename(_events[20])).Returns("testas1.txt");
        _lowLevelUtils.Setup(u => u.GetExtraTillSha256(_events[20].Extra)).Returns("something5: somehing5");

        _lowLevelUtils.Setup(u => u.IsValidFileEvent(_events[21])).Returns(true);
        _lowLevelUtils.Setup(u => u.GetFilename(_events[21])).Returns("testas2.txt");
        _lowLevelUtils.Setup(u => u.GetExtraTillSha256(_events[21].Extra)).Returns("something6: somehing6");

        _lowLevelUtils.Setup(u => u.IsValidFileEvent(_events[22])).Returns(true);
        _lowLevelUtils.Setup(u => u.GetFilename(_events[22])).Returns("calc1.exe");
        _lowLevelUtils.Setup(u => u.GetExtraTillSha256(_events[22].Extra)).Returns("something7: somehing7");

        _lowLevelUtils.Setup(u => u.IsValidFileEvent(_events[23])).Returns(true);
        _lowLevelUtils.Setup(u => u.GetFilename(_events[23])).Returns("calc2.exe");
        _lowLevelUtils.Setup(u => u.GetExtraTillSha256(_events[23].Extra)).Returns("something8: somehing8");

        _lowLevelUtils.Setup(u => u.IsValidFileEvent(_events[24])).Returns(true);
        _lowLevelUtils.Setup(u => u.GetFilename(_events[24])).Returns("calc3.exe");
        _lowLevelUtils.Setup(u => u.GetExtraTillSha256(_events[24].Extra)).Returns("something9: somehing9");

        _lowLevelUtils.Setup(u => u.IsValidFileEvent(_events[25])).Returns(true);
        _lowLevelUtils.Setup(u => u.GetFilename(_events[25])).Returns("calc4.exe");
        _lowLevelUtils.Setup(u => u.GetExtraTillSha256(_events[25].Extra)).Returns("something10: somehing10");

        _lowLevelUtils.Setup(u => u.IsValidFileEvent(_events[26])).Returns(true);
        _lowLevelUtils.Setup(u => u.GetFilename(_events[26])).Returns("testas1.txt");
        _lowLevelUtils.Setup(u => u.GetExtraTillSha256(_events[26].Extra)).Returns("something11: somehing11");

        _lowLevelUtils.Setup(u => u.IsValidFileEvent(_events[27])).Returns(false);

        _lowLevelUtils.Setup(u => u.IsValidFileEvent(_events[28])).Returns(false);

        _lowLevelUtils.Setup(u => u.GetShort(_events[29].Description)).Returns("test.exe");
        _lowLevelUtils.Setup(u => u.GetFilename(_events[29])).Returns("test1.exe");

        _highLevelUtils.Setup(u => u.GetShort(_events[30].Description)).Returns("test.exe");
        _lowLevelUtils.Setup(u => u.GetFilename(_events[30])).Returns("test2.exe");

        _lowLevelUtils.Setup(u => u.GetShort(_events[31].Description)).Returns("test.exe");
        _lowLevelUtils.Setup(u => u.GetFilename(_events[31])).Returns("test1.exe");

        _highLevelUtils.Setup(u => u.GetShort(_events[32].Description)).Returns("test.exe");
        _lowLevelUtils.Setup(u => u.GetFilename(_events[32])).Returns("test2.exe");

        _lowLevelUtils.Setup(u => u.GetShort(_events[33].Description)).Returns("test.exe");
        _lowLevelUtils.Setup(u => u.GetFilename(_events[33])).Returns("test1.exe");

        _highLevelUtils.Setup(u => u.GetShort(_events[34].Description)).Returns("test.exe");
        _lowLevelUtils.Setup(u => u.GetFilename(_events[34])).Returns("test2.exe");

        _lowLevelUtils.Setup(u => u.GetShort(_events[35].Description)).Returns("test.exe");
        _lowLevelUtils.Setup(u => u.GetFilename(_events[35])).Returns("test1.exe");

        _highLevelUtils.Setup(u => u.GetShort(_events[36].Description)).Returns("test.exe");
        _lowLevelUtils.Setup(u => u.GetFilename(_events[36])).Returns("test2.exe");

        _lowLevelUtils.Setup(u => u.IsValidRegEvent(_events[37])).Returns(true);
        _lowLevelUtils.Setup(u => u.GetSummaryFromShort(_events[37].Description)).Returns("{645FF040-5081-101B-9F08-00AA002F954E}");
        _lowLevelUtils.Setup(u => u.GetExtraTillSha256(_events[37].Extra)).Returns("something13: something13");

        _lowLevelUtils.Setup(u => u.IsValidRegEvent(_events[38])).Returns(true);
        _lowLevelUtils.Setup(u => u.GetSummaryFromShort(_events[38].Description)).Returns("{645FF040-5081-101B-9F08-00AA002F954E}");
        _lowLevelUtils.Setup(u => u.GetExtraTillSha256(_events[38].Extra)).Returns("something14: something14");

        _lowLevelUtils.Setup(u => u.IsValidRegEvent(_events[39])).Returns(true);
        _lowLevelUtils.Setup(u => u.GetSummaryFromShort(_events[39].Description)).Returns("test");
        _lowLevelUtils.Setup(u => u.GetExtraTillSha256(_events[39].Extra)).Returns("something15: something15");

        _lowLevelUtils.Setup(u => u.GetShort(_events[40].Description)).Returns("test");
        _lowLevelUtils.Setup(u => u.GetFilename(_events[40])).Returns("test1");

        _lowLevelUtils.Setup(u => u.GetShort(_events[41].Description)).Returns("test");
        _lowLevelUtils.Setup(u => u.GetFilename(_events[41])).Returns("test1");

        _lowLevelUtils.Setup(u => u.IsValidRegEvent(_events[42])).Returns(true);
        _lowLevelUtils.Setup(u => u.GetSummaryFromShort(_events[42].Description)).Returns("test");
        _lowLevelUtils.Setup(u => u.GetExtraTillSha256(_events[42].Extra)).Returns("something16: something16");

        _lowLevelUtils.Setup(u => u.GetExtraTillSha256(_events[43].Extra)).Returns("something17: something17");

        _lowLevelUtils.Setup(u => u.GetExtraTillSha256(_events[44].Extra)).Returns("something18: something18");

        _lowLevelUtils.Setup(u => u.GetShort(_events[45].Description)).Returns("Deleted file: C:\\Documents and Settings\\PC1\\Desktop\\not secret anymore.txt");
        _lowLevelUtils.Setup(u => u.GetFilename(_events[45])).Returns("test2");

        _lowLevelUtils.Setup(u => u.GetExtraTillSha256(_events[46].Extra)).Returns("something19: something19");

        _lowLevelUtils.Setup(u => u.IsValidFileEvent(_events[47])).Returns(true);
        _lowLevelUtils.Setup(u => u.GetFilename(_events[47])).Returns("calc5.exe");
        _lowLevelUtils.Setup(u => u.GetExtraTillSha256(_events[47].Extra)).Returns("something20: somehing20");

        _lowLevelUtils.Setup(u => u.IsValidFileEvent(_events[48])).Returns(true);
        _lowLevelUtils.Setup(u => u.GetFilename(_events[48])).Returns("calc6.exe");
        _lowLevelUtils.Setup(u => u.GetExtraTillSha256(_events[48].Extra)).Returns("something21: somehing21");
    }

    private List<EventViewModel> GetStoredEvents()
    {
        return new List<EventViewModel>()
        {
            new EventViewModel(new EventModel(new DateOnly(2000, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "WEBHIST", "Firefox Cookies", "Last Access Time", "User", "Host", "www.mozilla.org (moz-stub-attribution-code)", "http://www.mozilla.org/ (moz-stub-attribution-code) Flags: [HTTP only]: False", 2, "TSK:/Documents and Settings/PC1/Application Data/Mozilla/Firefox/Profiles/obcflyez.default/cookies.sqlite", "13452", "-", "sqlite/firefox_cookies", new Dictionary<string, string>(), 0)),
            new EventViewModel(new EventModel(new DateOnly(2000, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "WEBHIST", "Firefox History", "File downloaded", "User", "Host", "C:\\Documents and Settings\\PC1\\My Documents\\Downloads\\Timeline2GUI A Log2Timel...", "https://mail-attachment.googleusercontent.com/attachment/... (C:\\Documents and Settings\\PC1\\My Documents\\Downloads\\Timeline2GUI A Log2Timeline CSV parser and training scenarios.pdf). Received: 1116192 bytes out of: 1116192 bytes.", 2, "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", "13452", "-", "sqlite/firefox_history", new Dictionary<string, string>() { { "extra", "['visited from: https://www.google.com/search?client=firefox-b-ab&q=ekiga&oq=ekiga&aqs=heirloom-srp..0l5 (www.google.com)'  '(URL not typed directly)'  'Transition: LINK']" }, { "schema_match", "False" }, { "sha256_hash", "a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741" }, { "visit_type", "1" } }, 1)),
            new EventViewModel(new EventModel(new DateOnly(2000, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "WEBHIST", "Google History", "Last Visited Time", "User", "Host", "https://www.mail.google.com/ (Google)", "https://mail.google.com/mail/u/0/#inbox (Gautieji (170) - testas@gmail.com - Gmail) [count: 0] Visit from: https://mail.google.com/mail/u/0/ (Gmail) Type: [LINK - User clicked a link] (URL not typed directly - no typed count)", 2, "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", "13452", "-", "sqlite/chrome_27_history", new Dictionary<string, string>(), 2)),
            new EventViewModel(new EventModel(new DateOnly(2000, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "WEBHIST", "Google History", "Last Visited Time", "User", "Host", "https://www.mail.google.com/ (Google)", "https://mail.google.com/mail/u/0/#inbox (Gautieji (170) - testas@gmail.com - Gmail) [count: 0] Visit from: https://mail.google.com/mail/u/0/ (Gmail) Type: [LINK - User clicked a link] (URL not typed directly - no typed count)", 2, "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", "13452", "-", "sqlite/chrome_27_history", new Dictionary<string, string>(), 3)),
            new EventViewModel(new EventModel(new DateOnly(2000, 1, 1), new TimeOnly(12, 30, 0), TimeZoneInfo.Utc, "B...", "WEBHIST", "Google History", "Last Visited Time", "User", "Host", "https://www.mail.google.com/ (Google)", "https://mail.google.com/mail/u/0/#inbox (Gautieji (170) - testas@gmail.com - Gmail) [count: 0] Visit from: https://mail.google.com/mail/u/0/ (Gmail) Type: [LINK - User clicked a link] (URL not typed directly - no typed count)", 2, "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", "13452", "-", "sqlite/chrome_27_history", new Dictionary<string, string>(), 4)),
            new EventViewModel(new EventModel(new DateOnly(2001, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "WEBHIST", "Google History", "Last Visited Time", "User", "Host", "https://google.com/mail/u/0/?tab=rm&ogbl (Gmail)", "https://google.com/mail/u/0/?tab=rm&ogbl (Gmail) [count: 0] Visit from: https://google.com/mail/?tab=rm&ogbl (Gmail) Type: [LINK - User clicked a link] (URL not typed directly - no typed count)", 2, "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", "13452", "-", "sqlite/chrome_27_history", new Dictionary<string, string>(), 5)),
            new EventViewModel(new EventModel(new DateOnly(2001, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "WEBHIST", "Google History", "Last Visited Time", "User", "Host", "https://google.com/mail/u/0/?tab=rm&ogbl (Gmail)", "https://google.com/mail/u/0/?tab=rm&ogbl (Gmail) [count: 0] Visit from: https://google.com/mail/?tab=rm&ogbl (Gmail) Type: [LINK - User clicked a link] (URL not typed directly - no typed count)", 2, "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", "13452", "-", "sqlite/chrome_27_history", new Dictionary<string, string>(), 6)),
            new EventViewModel(new EventModel(new DateOnly(2002, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "META", "System", "Creation Time", "User", "Host", "Short Description", "Full Description", 2, "testas.docx", "13452", "-", "Format", new Dictionary<string, string>() { { "number_of_paragraphs", "3" }, { "total_time", "1" } }, 7)),
            new EventViewModel(new EventModel(new DateOnly(2003, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, ".A..", "REG", "Registry Key: UserAssist", "Last Time Executed", "User", "Host", "UEME_RUNPATH:::{645FF040-5081-101B-9F08-00AA002F954E} Count: 1", "[HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\UserAssist\\{75048700-EF1F-11D0-9888-006097DEACF9}\\Count] Value name: UEME_RUNPATH:::{645FF040-5081-101B-9F08-00AA002F954E} Count: 1", 2, "TSK:/Documents and Settings/PC1/NTUSER.DAT", "13452", "-", "winreg/userassist", new Dictionary<string, string>() { { "something1", "something1" } }, 8)),
            new EventViewModel(new EventModel(new DateOnly(2003, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, ".A..", "REG", "Registry Key: UserAssist", "Last Time Executed", "User", "Host", "UEME_RUNPATH:::{645FF040-5081-101B-9F08-00AA002F954E} Count: 1", "[HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\UserAssist\\{75048700-EF1F-11D0-9888-006097DEACF9}\\Count] Value name: UEME_RUNPATH:::{645FF040-5081-101B-9F08-00AA002F954E} Count: 1", 2, "TSK:/Documents and Settings/PC1/NTUSER.DAT", "13452", "-", "winreg/userassist", new Dictionary<string, string>(), 9)),
            new EventViewModel(new EventModel(new DateOnly(2004, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "PE", "PE Compilation time", "Creation Time", "User", "Host", "pe_type", "Something something", 2, "test.exe", "13452", "-", "pe", new Dictionary<string, string>(), 10)),
            new EventViewModel(new EventModel(new DateOnly(2004, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "PE", "PE Compilation time", "Creation Time", "User", "Host", "Something something", "Something something", 2, "TSK:/WINDOWS/system32/wmimgmt.msc", "13452", "-", "pe", new Dictionary<string, string>(), 11)),
            new EventViewModel(new EventModel(new DateOnly(2005, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "RECBIN", "Recycle Bin", "Content Deletion Time", "User", "Host", "Deleted file: C:\\Documents and Settings\\PC1\\Desktop\\not secret anymore.txt", "DC4 -> C:\\Documents and Settings\\PC1\\Desktop\\not secret anymore.txt (from drive: C)", 2, "TSK:/RECYCLER/S-1-5-21-1292428093-484763869-854245398-1003/INFO2", "13452", "-", "recycle_bin_info2", new Dictionary<string, string>() { { "drive_number", "2" }, { "file_size", "4096" }, { "sha256_hash", "37047395e0bfec4a6bdec6feee3bd2b262340c26349fc946b843a1bc6fdcbb4e" } }, 12)),
            new EventViewModel(new EventModel(new DateOnly(2005, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "EVT", "PE Compilation time", "Creation Time", "User", "Host", "pe_type", "Something something", 2, "test.exe", "13452", "-", "pe", new Dictionary<string, string>(), 13)),
            new EventViewModel(new EventModel(new DateOnly(2006, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B..M", "OLECF", "OLECF Item", "Creation Time", "User", "Host", "Name: view", "Something something", 2, "TSK:/WINDOWS/system32/wmimgmt.msc", "13451", "-", "olecf/olecf_default", new Dictionary<string, string>(), 14)),
            new EventViewModel(new EventModel(new DateOnly(2006, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "OLECF", "OLECF Item", "Creation Time", "User", "Host", "Name: data", "Something something", 2, "TSK:/WINDOWS/system32/wmimgmt.msc", "13452", "-", "olecf/olecf_default", new Dictionary<string, string>(), 15)),
            new EventViewModel(new EventModel(new DateOnly(2007, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B..M", "LNK", "Windows Shortcut", "Creation Time", "User", "Host", "[Empty description] C:\\Program Files\\Mozilla Firefox\\firefox.exe", "[Empty description] C:\\Program Files\\Mozilla Firefox\\firefox.exe", 2, "TSK:/Documents and Settings/All Users/Start Menu/Programs/Mozilla Firefox.lnk", "13451", "-", "lnk", new Dictionary<string, string>(), 16)),
            new EventViewModel(new EventModel(new DateOnly(2007, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B..M", "LNK", "Windows Shortcut", "Creation Time", "User", "Host", "[Empty description] C:\\Program Files\\Mozilla Firefox\\firefox.exe", "[Empty description] C:\\Program Files\\Mozilla Firefox\\firefox.exe", 2, "TSK:/Documents and Settings/All Users/Start Menu/Programs/Mozilla Firefox.lnk", "13451", "-", "lnk", new Dictionary<string, string>(), 17)),
            new EventViewModel(new EventModel(new DateOnly(2007, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B..M", "LNK", "Windows Shortcut", "Creation Time", "User", "Host", "[Empty description] C:\\Program Files\\Mozilla Firefox\\firefox2.exe", "[Empty description] C:\\Program Files\\Mozilla Firefox\\firefox2.exe", 2, "TSK:/Documents and Settings/All Users/Start Menu/Programs/Mozilla Firefox.lnk", "13451", "-", "lnk", new Dictionary<string, string>(), 18)),
            new EventViewModel(new EventModel(new DateOnly(2008, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "LOG", "System", "Creation Time", "User", "Host", "3ec2b817-c405-11e7-8ac5-a0afbdac1ec0 Origin: 2016.lnk", "Unpinned Path: 2016.exe", 2, "2016.lnk", "13452", "-", "lnk", new Dictionary<string, string>(), 19)),
            new EventViewModel(new EventModel(new DateOnly(2009, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "FILE", "NTFS Creation Time", "OS Last Access Time", "User", "Host", "C:\\Users\\User\\AppData\\testas1.txt", "TSK:/WINDOWS/system32/spool/drivers/color/testas1.txt Type: file", 2, "TSK:/WINDOWS/system32/spool/drivers/color/testas1.txt", "13452", "-", "filestat", new Dictionary<string, string>() { { "something5", "something5" } }, 20)),
            new EventViewModel(new EventModel(new DateOnly(2009, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "FILE", "NTFS Creation Time", "Creation Time", "User", "Host", "C:\\Users\\User\\testas2.txt", "C:\\Users\\User\\testas2.txt", 2, "TSK:\\Users\\User\\testas2.txt", "13452", "-", "filestat", new Dictionary<string, string>() { { "something6", "something6" } }, 21)),
            new EventViewModel(new EventModel(new DateOnly(2010, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "FILE", "File entry shell item", "Content Modification Time", "User", "Host", "Name: calc1.exe Origin: Calculator.lnk", "Name: calc1.exe Long name: calc1.exe Shell item path: <My Computer> C:\\WINDOWS\\system32\\calc1.exe Origin: Calculator.lnk", 2, "TSK:/Documents and Settings/All Users/Start Menu/Programs/Accessories/Calculator.lnk", "13452", "-", "lnk/shell_items", new Dictionary<string, string>() { { "something7", "something7" } }, 22)),
            new EventViewModel(new EventModel(new DateOnly(2010, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "FILE", "File entry shell item", "Content Modification Time", "User", "Host", "Name: calc2.exe Origin: Calculator.lnk", "Name: calc2.exe Long name: calc2.exe Shell item path: <My Computer> C:\\WINDOWS\\system32\\calc2.exe Origin: Calculator.lnk", 2, "TSK:/Documents and Settings/All Users/Start Menu/Programs/Accessories/Calculator.lnk", "13452", "-", "lnk/shell_items", new Dictionary<string, string>() { { "something8", "something8" } }, 23)),
            new EventViewModel(new EventModel(new DateOnly(2010, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "FILE", "File entry shell item", "Content Modification Time", "User", "Host", "Name: calc3.exe Origin: Calculator.lnk", "Name: calc3.exe Long name: calc3.exe Shell item path: <My Computer> C:\\WINDOWS\\system32\\calc3.exe Origin: Calculator.lnk", 2, "TSK:/Documents and Settings/All Users/Start Menu/Programs/Accessories/Calculator.lnk", "13452", "-", "winreg/explorer_programscache/shell_items", new Dictionary<string, string>() { { "something9", "something9" } }, 24)),
            new EventViewModel(new EventModel(new DateOnly(2011, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "FILE", "File entry shell item", "Content Modification Time", "User", "Host", "Name: calc4.exe Origin: Calculator.lnk", "Name: calc4.exe Long name: calc4.exe Shell item path: <My Computer> C:\\WINDOWS\\system32\\calc4.exe Origin: Calculator.lnk", 2, "TSK:/Documents and Settings/All Users/Start Menu/Programs/Accessories/Calculator.lnk", "13452", "-", "winreg/explorer_programscache/shell_items", new Dictionary<string, string>() { { "something10", "something10" } }, 25)),
            new EventViewModel(new EventModel(new DateOnly(2012, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "FILE", "NTFS Creation Time", "Creation Time", "User", "Host", "C:\\Users\\User\\AppData\\testas1.txt", "TSK:/WINDOWS/system32/spool/drivers/color/testas1.txt Type: file", 2, "TSK:/WINDOWS/system32/spool/drivers/color/testas1.txt", "13452", "-", "filestat", new Dictionary<string, string>(), 26)),
            new EventViewModel(new EventModel(new DateOnly(2012, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "FILE", "NTFS Creation Time", "Creation Time", "User", "Host", "C:\\Users\\User\\testas2.txt", "C:\\Users\\User\\testas2.txt", 2, "TSK:\\Users\\User\\testas2.txt", "13452", "-", "filestat", new Dictionary<string, string>() { { "something11", "something11" } }, 27)),
            new EventViewModel(new EventModel(new DateOnly(2012, 1, 1), new TimeOnly(12, 0, 5), TimeZoneInfo.Utc, "B...", "FILE", "NTFS Creation Time", "OS Last Access Time", "User", "Host", "C:\\Users\\User\\AppData\\testas2.txt", "C:\\Users\\User\\testas2.txt", 2, "TSK:\\Users\\User\\testas2.txt", "13452", "-", "filestat", new Dictionary<string, string>() { { "something12", "something12" } }, 28)),
            new EventViewModel(new EventModel(new DateOnly(2013, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "LOG", "System", "Creation Time", "User", "Host", "3ec2b817-c405-11e7-8ac5-a0afbdac1ec0 Origin: test.lnk", "Unpinned Path: test.exe", 2, "test.lnk", "13452", "-", "lnk", new Dictionary<string, string>(), 29)),
            new EventViewModel(new EventModel(new DateOnly(2013, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "...M", "LNK", "System", "Creation Time", "User", "Host", "3ec2b817-c405-11e7-8ac5-a0afbdac1ec0 Origin: test.lnk", "Unpinned Path: test.exe", 2, "test.lnk", "13452", "-", "lnk", new Dictionary<string, string>(), 30)),
            new EventViewModel(new EventModel(new DateOnly(2014, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "...M", "LOG", "System", "Creation Time", "User", "Host", "3ec2b817-c405-11e7-8ac5-a0afbdac1ec0 Origin: test.lnk", "Unpinned Path: test.exe", 2, "test.lnk", "13452", "-", "lnk", new Dictionary<string, string>(), 31)),
            new EventViewModel(new EventModel(new DateOnly(2014, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "LNK", "System", "Creation Time", "User", "Host", "3ec2b817-c405-11e7-8ac5-a0afbdac1ec0 Origin: test.lnk", "Unpinned Path: test.exe", 2, "test.lnk", "13452", "-", "lnk", new Dictionary<string, string>(), 32)),
            new EventViewModel(new EventModel(new DateOnly(2015, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "...M", "LOG", "System", "Creation Time", "User", "Host", "3ec2b817-c405-11e7-8ac5-a0afbdac1ec0 Origin: test.lnk", "Unpinned Path: test.exe", 2, "test.lnk", "13452", "-", "lnk", new Dictionary<string, string>(), 33)),
            new EventViewModel(new EventModel(new DateOnly(2015, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "...M", "LNK", "System", "Creation Time", "User", "Host", "3ec2b817-c405-11e7-8ac5-a0afbdac1ec0 Origin: test.lnk", "Unpinned Path: test.exe", 2, "test.txt", "13452", "-", "lnk", new Dictionary<string, string>(), 34)),
            new EventViewModel(new EventModel(new DateOnly(2016, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "LOG", "System", "Creation Time", "User", "Host", "3ec2b817-c405-11e7-8ac5-a0afbdac1ec0 Origin: test.lnk", "Unpinned Path: test.exe", 2, "test.txt", "13452", "-", "lnk", new Dictionary<string, string>(), 35)),
            new EventViewModel(new EventModel(new DateOnly(2016, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "LNK", "System", "Creation Time", "User", "Host", "3ec2b817-c405-11e7-8ac5-a0afbdac1ec0 Origin: test.lnk", "Unpinned Path: test.exe", 2, "test.lnk", "13452", "-", "lnk", new Dictionary<string, string>(), 36)),
            new EventViewModel(new EventModel(new DateOnly(2017, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, ".A..", "REG", "Registry Key: UserAssist", "Last Time Executed", "User", "Host", "UEME_RUNPATH:::{645FF040-5081-101B-9F08-00AA002F954E} Count: 1", "[HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\UserAssist\\{75048700-EF1F-11D0-9888-006097DEACF9}\\Count] Value name: UEME_RUNPATH:::{645FF040-5081-101B-9F08-00AA002F954E} Count: 1", 2, "TSK:/Documents and Settings/PC1/NTUSER.lnk", "13452", "-", "winreg/userassist", new Dictionary<string, string>() { { "something13", "something13" } }, 37)),
            new EventViewModel(new EventModel(new DateOnly(2017, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, ".A..", "REG", "Registry Key: UserAssist", "Last Time Executed", "User", "Host", "UEME_RUNPATH:::{645FF040-5081-101B-9F08-00AA002F954E} Count: 1", "[HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\UserAssist\\{75048700-EF1F-11D0-9888-006097DEACF9}\\Count] Value name: UEME_RUNPATH:::{645FF040-5081-101B-9F08-00AA002F954E} Count: 1", 2, "TSK:/Documents and Settings/PC1/NTUSER.lnk", "13452", "-", "winreg/userassist", new Dictionary<string, string>() { { "something14", "something14" } }, 38)),
            new EventViewModel(new EventModel(new DateOnly(2018, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, ".A..", "REG", "Registry Key: UserAssist", "Last Time Executed", "User", "Host", "UEME_RUNPATH:::{645FF040-5081-101B-9F08-00AA002F954A} Count: 1", "[HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\UserAssist\\{75048700-EF1F-11D0-9888-006097DEACF8}\\Count] Value name: UEME_RUNPATH:::{645FF040-5081-101B-9F08-00AA002F954E} Count: 1", 2, "TSK:/Documents and Settings/PC1/NTUSER.DAT", "13452", "-", "winreg/userassist", new Dictionary<string, string>() { { "something15", "something15" } }, 39)),
            new EventViewModel(new EventModel(new DateOnly(2018, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "...M", "LOG", "System", "Creation Time", "User", "Host", "3ec2b817-c405-11e7-8ac5-a0afbdac1ec0 Origin: test.lnk", "Unpinned Path: test", 2, "test.txt", "13452", "-", "lnk", new Dictionary<string, string>(), 40)),
            new EventViewModel(new EventModel(new DateOnly(2019, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "...M", "LOG", "System", "Creation Time", "User", "Host", "3ec2b817-c405-11e7-8ac5-a0afbdac1ec0 Origin: test.lnk", "Unpinned Path: test2", 2, "test.txt", "13452", "-", "lnk", new Dictionary<string, string>(), 41)),
            new EventViewModel(new EventModel(new DateOnly(2019, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, ".A..", "REG", "Registry Key: UserAssist", "Last Time Executed", "User", "Host", "UEME_RUNPATH:::{645FF040-5081-101B-9F08-00AA002F954A} Count: 1", "[HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\UserAssist\\{75048700-EF1F-11D0-9888-006097DEACF7}\\Count] Value name: UEME_RUNPATH:::{645FF040-5081-101B-9F08-00AA002F954E} Count: 1", 2, "TSK:/Documents and Settings/PC1/NTUSER.DAT", "13452", "-", "winreg/userassist", new Dictionary<string, string>() { { "something16", "something16" } }, 42)),
            new EventViewModel(new EventModel(new DateOnly(2020, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "RECBIN", "Recycle Bin", "Content Deletion Time", "User", "Host", "Deleted file: C:\\Documents and Settings\\PC1\\Desktop\\not secret anymore.txt", "DC4 -> C:\\Documents and Settings\\PC1\\Desktop\\not secret anymore.txt (from drive: C)", 2, "TSK:/RECYCLER/S-1-5-21-1292428093-484763869-854245398-1003/INFO2", "13452", "-", "recycle_bin_info2", new Dictionary<string, string>() { { "something17", "something17" } }, 43)),
            new EventViewModel(new EventModel(new DateOnly(2020, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "RECBIN", "Recycle Bin", "Content Deletion Time", "User", "Host", "Deleted file: C:\\Documents and Settings\\PC1\\Desktop\\not secret anymore.txt", "DC4 -> C:\\Documents and Settings\\PC1\\Desktop\\not secret anymore.txt (from drive: C)", 2, "TSK:/RECYCLER/S-1-5-21-1292428093-484763869-854245398-1003/INFO2", "13452", "-", "recycle_bin_info2", new Dictionary<string, string>() { { "something18", "something18" } }, 44)),
            new EventViewModel(new EventModel(new DateOnly(2021, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "...M", "LOG", "System", "Creation Time", "User", "Host", "3ec2b817-c405-11e7-8ac5-a0afbdac1ec0 Origin: test.lnk", "Unpinned Path: test3", 2, "test.txt", "13452", "-", "lnk", new Dictionary<string, string>(), 45)),
            new EventViewModel(new EventModel(new DateOnly(2021, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "...M", "RECBIN", "Recycle Bin", "Content Deletion Time", "User", "Host", "Deleted file: C:\\Documents and Settings\\PC1\\Desktop\\not secret anymore.txt", "DC4 -> C:\\Documents and Settings\\PC1\\Desktop\\not secret anymore.txt (from drive: C)", 2, "TSK:/RECYCLER/S-1-5-21-1292428093-484763869-854245398-1003/INFO2", "13452", "-", "recycle_bin_info2", new Dictionary<string, string>() { { "something19", "something19" } }, 46)),
            new EventViewModel(new EventModel(new DateOnly(2022, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "FILE", "File entry shell item", "Content Modification Time", "User", "Host", "Name: calc5.exe Origin: Calculator.lnk", "Name: calc1.exe Long name: calc5.exe Shell item path: <My Computer> C:\\WINDOWS\\system32\\calc5.exe Origin: Calculator.lnk", 2, "TSK:/Documents and Settings/All Users/Start Menu/Programs/Accessories/Calculator.lnk", "13452", "-", "lnk/shell_items", new Dictionary<string, string>() { { "something20", "something20" } }, 47)),
            new EventViewModel(new EventModel(new DateOnly(2022, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "FILE", "File entry shell item", "Content Modification Time", "User", "Host", "Name: calc6.exe Origin: Calculator.lnk", "Name: calc2.exe Long name: calc6.exe Shell item path: <My Computer> C:\\WINDOWS\\system32\\calc6.exe Origin: Calculator.lnk", 2, "TSK:/Documents and Settings/All Users/Start Menu/Programs/Accessories/Calculator.lnk", "13452", "-", "lnk/shell_items", new Dictionary<string, string>() { { "something21", "something21" } }, 48))
        };
    }

    private List<LowLevelEventViewModel> GetExpectedEvents()
    {
        return new List<LowLevelEventViewModel>()
        {
            new LowLevelEventViewModel() { Date = new DateOnly(2000, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "WEBHIST", Short = "C:\\Documents and Settings\\PC1\\My Documents\\Downloads\\Timeline2GUI A Log2Timeline CSV parser and training scenarios.pdf", Reference = 1, Visit = "Download", Extra = "https://www.google.com/search?client=firefox-b-ab&q=ekiga&oq=ekiga&aqs=heirloom-srp..0l5" },
            new LowLevelEventViewModel() { Date = new DateOnly(2000, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "WEBHIST", Short = "https://mail.google.com", Reference = 2, Visit = "Mail", Extra = "testas@gmail.com" },
            new LowLevelEventViewModel() { Date = new DateOnly(2000, 1, 1), Time = new TimeOnly(12, 30, 0), Source = "WEBHIST", Short = "https://mail.google.com", Reference = 4, Visit = "Mail", Extra = "testas@gmail.com" },
            new LowLevelEventViewModel() { Date = new DateOnly(2001, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "WEBHIST", Short = "https://www.google.com/mail/", Reference = 5, Visit = "LINK" },
            new LowLevelEventViewModel() { Date = new DateOnly(2002, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "META", Short = "testas.docx", Reference = 7, Extra = "number_of_paragraphs: 3 total_time: 1" },
            new LowLevelEventViewModel() { Date = new DateOnly(2003, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "REG", Short = "{645FF040-5081-101B-9F08-00AA002F954E}", Reference = 8, Extra = "something1: something1" },
            new LowLevelEventViewModel() { Date = new DateOnly(2004, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "PE", Short = "test.exe", Reference = 10 },
            new LowLevelEventViewModel() { Date = new DateOnly(2005, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "RECBIN", Short = "Deleted file: C:\\Documents and Settings\\PC1\\Desktop\\not secret anymore.txt", Reference = 12, Extra = "drive_number: 2 file_size: 4096" },
            new LowLevelEventViewModel() { Date = new DateOnly(2006, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "OLECF", Short = "TSK:/WINDOWS/system32/wmimgmt.msc", Reference = 15, Extra = "Name: data" },
            new LowLevelEventViewModel() { Date = new DateOnly(2007, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "LNK", Short = "C:\\Program Files\\Mozilla Firefox\\firefox.exe", Reference = 16 },
            new LowLevelEventViewModel() { Date = new DateOnly(2007, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "LNK", Short = "C:\\Program Files\\Mozilla Firefox\\firefox2.exe", Reference = 18 },
            new LowLevelEventViewModel() { Date = new DateOnly(2008, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "LOG", Short = "2016.exe", Reference = 19 },
            new LowLevelEventViewModel() { Date = new DateOnly(2009, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "FILE", Short = "testas2.txt", Reference = 21, Extra = "something6: somehing6" },
            new LowLevelEventViewModel() { Date = new DateOnly(2010, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "FILE", Short = "calc2.exe", Reference = 23, Extra = "something8: somehing8" },
            new LowLevelEventViewModel() { Date = new DateOnly(2011, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "FILE", Short = "calc4.exe", Reference = 25, Extra = "something10: somehing10" },
            new LowLevelEventViewModel() { Date = new DateOnly(2012, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "FILE", Short = "testas1.txt", Reference = 26, Extra = "something11: somehing11" },
            new LowLevelEventViewModel() { Date = new DateOnly(2013, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "LOG", Short = "test.exe", Reference = 29 },
            new LowLevelEventViewModel() { Date = new DateOnly(2014, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "LNK", Short = "test.exe", Reference = 32 },
            new LowLevelEventViewModel() { Date = new DateOnly(2015, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "LOG", Short = "test.exe", Reference = 33 },
            new LowLevelEventViewModel() { Date = new DateOnly(2016, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "LNK", Short = "test.exe", Reference = 36 },
            new LowLevelEventViewModel() { Date = new DateOnly(2017, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "REG", Short = "{645FF040-5081-101B-9F08-00AA002F954E}", Reference = 37, Extra = "something13: something13" },
            new LowLevelEventViewModel() { Date = new DateOnly(2018, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "LOG", Short = "test", Reference = 40 },
            new LowLevelEventViewModel() { Date = new DateOnly(2019, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "LOG", Short = "test", Reference = 41 },
            new LowLevelEventViewModel() { Date = new DateOnly(2020, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "RECBIN", Short = "Deleted file: C:\\Documents and Settings\\PC1\\Desktop\\not secret anymore.txt", Reference = 43, Extra = "something17: something17" },
            new LowLevelEventViewModel() { Date = new DateOnly(2021, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "LOG", Short = "Deleted file: C:\\Documents and Settings\\PC1\\Desktop\\not secret anymore.txt", Reference = 45 },
            new LowLevelEventViewModel() { Date = new DateOnly(2022, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "FILE", Short = "calc6.exe", Reference = 48, Extra = "something21: somehing21" }
        };
    }

    [TestMethod]
    public void FormLowLevelEvents_ShouldReturnLowLevelEvents_WhenMethodIsCalled()
    {
        // Arrange
        List<LowLevelEventViewModel> expected = GetExpectedEvents();

        // Act
        List<LowLevelEventViewModel> actual = _abstractor.FormLowLevelEvents(_events);

        // Assert
        Assert.AreEqual(expected.Count, actual.Count);

        for (int i = 0; i < expected.Count; i++)
        {
            Assert.AreEqual(expected[i].Reference, actual[i].Reference);
            Assert.AreEqual(expected[i].Date, actual[i].Date);
            Assert.AreEqual(expected[i].Time, actual[i].Time);
            Assert.AreEqual(expected[i].Source, actual[i].Source);
            Assert.AreEqual(expected[i].Short, actual[i].Short);
            Assert.AreEqual(expected[i].Visit, actual[i].Visit);
            Assert.AreEqual(expected[i].Extra, actual[i].Extra);
        }
    }

    [TestMethod]
    public void FormLowLevelEvents_ShouldReturnLastFileSourceEvent_WhenMethodIsCalled()
    {
        // Arrange
        List<EventViewModel> events = new()
        {
            _events[20],
            _events[21]
        };
        List<LowLevelEventViewModel> expected = new()
        {
            GetExpectedEvents()[12]
        };

        // Act
        List<LowLevelEventViewModel> actual = _abstractor.FormLowLevelEvents(events);

        // Assert
        Assert.AreEqual(expected.Count, actual.Count);

        for (int i = 0; i < expected.Count; i++)
        {
            Assert.AreEqual(expected[i].Reference, actual[i].Reference);
            Assert.AreEqual(expected[i].Date, actual[i].Date);
            Assert.AreEqual(expected[i].Time, actual[i].Time);
            Assert.AreEqual(expected[i].Source, actual[i].Source);
            Assert.AreEqual(expected[i].Short, actual[i].Short);
            Assert.AreEqual(expected[i].Visit, actual[i].Visit);
            Assert.AreEqual(expected[i].Extra, actual[i].Extra);
        }
    }

    [TestMethod]
    public void FormLowLevelEvents_ShouldReturnFirstOlecfSourceEvent_WhenMethodIsCalled()
    {
        // Arrange
        List<EventViewModel> events = new()
        {
            _events[14],
            _events[15]
        };
        List<LowLevelEventViewModel> expected = new()
        {
            GetExpectedEvents()[8]
        };

        // Act
        List<LowLevelEventViewModel> actual = _abstractor.FormLowLevelEvents(events);

        // Assert
        Assert.AreEqual(expected.Count, actual.Count);

        for (int i = 0; i < expected.Count; i++)
        {
            Assert.AreEqual(expected[i].Reference, actual[i].Reference);
            Assert.AreEqual(expected[i].Date, actual[i].Date);
            Assert.AreEqual(expected[i].Time, actual[i].Time);
            Assert.AreEqual(expected[i].Source, actual[i].Source);
            Assert.AreEqual(expected[i].Short, actual[i].Short);
            Assert.AreEqual(expected[i].Visit, actual[i].Visit);
            Assert.AreEqual(expected[i].Extra, actual[i].Extra);
        }
    }
}
