using EventTimelineReconstruction.Abstractors;
using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;
using Moq;

namespace EventTimelineReconstruction.Tests.UnitTests.Abstractors;

[TestClass]
public class LowLevelArtefactsAbstractorTests
{
    private readonly Mock<ILowLevelArtefactsAbstractorUtils> _utils;
    private readonly LowLevelArtefactsAbstractor _abstractor;
    private readonly List<EventViewModel> _events;

    public LowLevelArtefactsAbstractorTests()
    {
        _utils = new();
        _abstractor = new(_utils.Object);

        _events = GetStoredEvents();

        _utils.Setup(u => u.IsValidWebhistLine(_events[1].SourceType, _events[1].Type)).Returns(false);

        _utils.Setup(u => u.IsValidWebhistLine(_events[2].SourceType, _events[2].Type)).Returns(true);
        _utils.Setup(u => u.GetExtraValue(_events[2].Extra)).Returns("extra: ['visited from: https://www.google.com/search?client=firefox-b-ab&q=ekiga&oq=ekiga&aqs=heirloom-srp..0l5 (www.google.com)'  '(URL not typed directly)'  'Transition: LINK'] schema_match: False sha256_hash: a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741 visit_type: 1");

        _utils.Setup(u => u.IsValidWebhistLine(_events[3].SourceType, _events[3].Type)).Returns(true);
        _utils.Setup(u => u.GetExtraValue(_events[3].Extra)).Returns("visit_type: 1 schema_match: False sha256_hash: a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741");
        _utils.Setup(u => u.GetAddress(_events[3].Description)).Returns("mail.google.com/");

        _utils.Setup(u => u.IsValidWebhistLine(_events[4].SourceType, _events[4].Type)).Returns(true);
        _utils.Setup(u => u.GetExtraValue(_events[4].Extra)).Returns("visit_type: 2 schema_match: False sha256_hash: a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741");
        _utils.Setup(u => u.GetAddress(_events[4].Description)).Returns("mail.google.com/");

        _utils.Setup(u => u.IsValidWebhistLine(_events[5].SourceType, _events[5].Type)).Returns(true);
        _utils.Setup(u => u.GetExtraValue(_events[5].Extra)).Returns("visit_type: 3 schema_match: False sha256_hash: a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741");
        _utils.Setup(u => u.GetAddress(_events[5].Description)).Returns("google.com/");

        _utils.Setup(u => u.GetExtraValue(_events[6].Extra)).Returns("Something6: Something6");

        _utils.Setup(u => u.IsValidWebhistLine(_events[7].SourceType, _events[7].Type)).Returns(true);
        _utils.Setup(u => u.GetExtraValue(_events[7].Extra)).Returns("extra: ['visited from: https://www.google.com/search?client=firefox-b-ab&q=ekiga&oq=ekiga&aqs=heirloom-srp..0l5 (www.google.com)'  '(URL not typed directly)'  'Transition: LINK'] schema_match: True sha256_hash: a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741 visit_type: 1");
        
        _utils.Setup(u => u.GetExtraValue(_events[8].Extra)).Returns("Something8: Something8");

        _utils.Setup(u => u.GetExtraValue(_events[9].Extra)).Returns("Something9: Something9");

        _utils.Setup(u => u.GetExtraValue(_events[10].Extra)).Returns("schema_match: False sha256_hash: 4eb3f81bf5801eb3f96b796c4f5b2b68a187a5165893e3a7957ae347a07c4fb7");

        _utils.Setup(u => u.GetExtraValue(_events[11].Extra)).Returns("schema_match: True sha256_hash: 4eb3f81bf5801eb3f96b796c4f5b2b68a187a5165893e3a7957ae347a07c4fb7");

        _utils.Setup(u => u.GetExtraValue(_events[12].Extra)).Returns("schema_match: True sha256_hash: 4eb3f81bf5801eb3f96b796c4f5b2b68a187a5165893e3a7957ae347a07c4fb9");

        _utils.Setup(u => u.GetExtraValue(_events[13].Extra)).Returns("Something12: Something12");

        _utils.Setup(u => u.GetExtraValue(_events[14].Extra)).Returns("schema_match: False sha256_hash: 4eb3f81bf5801eb3f96b796c4f5b2b68a187a5165893e3a7957ae347a07c4fb8");

        _utils.Setup(u => u.GetExtraValue(_events[15].Extra)).Returns("schema_match: False sha256_hash: 4eb3f81bf5801eb3f96b796c4f5b2b68a187a5165893e3a7957ae347a07c4fb1");

        _utils.Setup(u => u.GetExtraValue(_events[16].Extra)).Returns("schema_match: False sha256_hash: 4eb3f81bf5801eb3f96b796c4f5b2b68a187a5165893e3a7957ae347a07c4fb2");

        _utils.Setup(u => u.GetExtraValue(_events[17].Extra)).Returns("schema_match: False sha256_hash: 4eb3f81bf5801eb3f96b796c4f5b2b68a187a5165893e3a7957ae347a07c4fb3");

        _utils.Setup(u => u.GetExtraValue(_events[18].Extra)).Returns("schema_match: False sha256_hash: 4eb3f81bf5801eb3f96b796c4f5b2b68a187a5165893e3a7957ae347a07c4fb4");

        _utils.Setup(u => u.GetExtraValue(_events[19].Extra)).Returns("Something19: Something19");

        _utils.Setup(u => u.GetExtraValue(_events[20].Extra)).Returns("schema_match: False sha256_hash: 4eb3f81bf5801eb3f96b796c4f5b2b68a187a5165893e3a7957ae347a07c4fb5");
    }

    private List<EventViewModel> GetStoredEvents()
    {
        return new List<EventViewModel>()
        {
            new EventViewModel(new EventModel(new DateOnly(2000, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "EVT", "NTFS Creation Time", "Creation Time", "User", "Host", "C:\\Users\\User\\testas0.txt", "C:\\Users\\User\\testas0.txt Type: file", 2, "TSK:\\Users\\User\\testas0.txt", "13452", "-", "filestat", new Dictionary<string, string>() { { "something0", "something0" } }, 0)),
            new EventViewModel(new EventModel(new DateOnly(2000, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "WEBHIST", "Firefox History", "Last Visited Time", "User", "Host", "https://google.com/mail/u/0/?tab=rm&ogbl (Gmail)", "https://google.com/mail/u/0/?tab=rm&ogbl (Gmail) [count: 0] Visit from: https://google.com/mail/?tab=rm&ogbl (Gmail) Type: [LINK - User clicked a link] (URL not typed directly - no typed count)", 2, "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", "13452", "-", "sqlite/chrome_27_history", new Dictionary<string, string>(), 1)),
            new EventViewModel(new EventModel(new DateOnly(2000, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "WEBHIST", "Firefox History", "File downloaded", "User", "Host", "C:\\Documents and Settings\\PC1\\My Documents\\Downloads\\Timeline2GUI A Log2Timel...", "https://mail-attachment.googleusercontent.com/attachment/... (C:\\Documents and Settings\\PC1\\My Documents\\Downloads\\Timeline2GUI A Log2Timeline CSV parser and training scenarios.pdf). Received: 1116192 bytes out of: 1116192 bytes.", 2, "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", "13452", "-", "sqlite/firefox_history", new Dictionary<string, string>() { { "extra", "['visited from: https://www.google.com/search?client=firefox-b-ab&q=ekiga&oq=ekiga&aqs=heirloom-srp..0l5 (www.google.com)'  '(URL not typed directly)'  'Transition: LINK']" }, { "schema_match", "False" }, { "sha256_hash", "a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741" }, { "visit_type", "1" } }, 2)),
            new EventViewModel(new EventModel(new DateOnly(2001, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "WEBHIST", "Google Cookies", "Last Visited Time", "User", "Host", "https://www.mail.google.com/ (Google)", "https://mail.google.com/mail/u/0/#inbox (Gautieji (170) - testas@gmail.com - Gmail) [count: 0] Visit from: https://mail.google.com/mail/u/0/ (Gmail) Type: [LINK - User clicked a link] (URL not typed directly - no typed count)", 2, "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", "13452", "-", "sqlite/chrome_27_history", new Dictionary<string, string>() { { "visit_type", "1" }, { "schema_match", "False" }, { "sha256_hash", "a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741" } }, 3)),
            new EventViewModel(new EventModel(new DateOnly(2001, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "WEBHIST", "Google Cookies", "Last Visited Time", "User", "Host", "https://www.mail.google.com/ (Google)", "https://mail.google.com/mail/u/0/#inbox (Gautieji (170) - testas@gmail.com - Gmail) [count: 0] Visit from: https://mail.google.com/mail/u/0/ (Gmail) Type: [LINK - User clicked a link] (URL not typed directly - no typed count)", 2, "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", "13452", "-", "sqlite/chrome_27_history", new Dictionary<string, string>() { { "visit_type", "2" }, { "schema_match", "False" }, { "sha256_hash", "a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741" } }, 4)),
            new EventViewModel(new EventModel(new DateOnly(2001, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "WEBHIST", "Google Cookies", "Last Visited Time", "User", "Host", "https://google.com/mail/u/0/?tab=rm&ogbl (Gmail)", "https://google.com/mail/u/0/?tab=rm&ogbl (Gmail) [count: 0] Visit from: https://google.com/mail/?tab=rm&ogbl (Gmail) Type: [LINK - User clicked a link] (URL not typed directly - no typed count)", 2, "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", "13452", "-", "sqlite/chrome_27_history", new Dictionary<string, string>() { { "visit_type", "3" }, { "schema_match", "False" }, { "sha256_hash", "a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741" } }, 5)),
            new EventViewModel(new EventModel(new DateOnly(2002, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B..M", "LNK", "Windows Shortcut", "Creation Time", "User", "Host", "[Empty description] C:\\Program Files\\Mozilla Firefox\\firefox.exe", "[Empty description] C:\\Program Files\\Mozilla Firefox\\firefox.exe", 2, "TSK:/Documents and Settings/All Users/Start Menu/Programs/Mozilla Firefox.lnk", "13451", "-", "lnk", new Dictionary<string, string>() { { "Something6", "Something6" } }, 6)),
            new EventViewModel(new EventModel(new DateOnly(2003, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "WEBHIST", "Firefox History", "File downloaded", "User", "Host", "C:\\Documents and Settings\\PC1\\My Documents\\Downloads\\Timeline2GUI A Log2Timel...", "https://mail-attachment.googleusercontent.com/attachment/... (C:\\Documents and Settings\\PC1\\My Documents\\Downloads\\Timeline2GUI A Log2Timeline CSV parser and training scenarios.pdf). Received: 1116192 bytes out of: 1116192 bytes.", 2, "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", "13452", "-", "sqlite/firefox_history", new Dictionary<string, string>() { { "extra", "['visited from: https://www.google.com/search?client=firefox-b-ab&q=ekiga&oq=ekiga&aqs=heirloom-srp..0l5 (www.google.com)'  '(URL not typed directly)'  'Transition: LINK']" }, { "schema_match", "True" }, { "sha256_hash", "a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741" }, { "visit_type", "1" } }, 7)),
            new EventViewModel(new EventModel(new DateOnly(2003, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "FILE", "NTFS Creation Time", "Creation Time", "User", "Host", "C:\\Users\\User\\testas.txt", "C:\\Users\\User\\testas.txt Type: file", 2, "TSK:\\Users\\User\\testas.txt", "13452", "-", "filestat", new Dictionary<string, string>() { { "Something8", "Something8" } }, 8)),
            new EventViewModel(new EventModel(new DateOnly(2003, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "FILE", "NTFS Creation Time", "Creation Time", "User", "Host", "C:\\Users\\User\\testas.txt", "C:\\Users\\User\\testas.txt Type: file", 2, "TSK:\\Users\\User\\testas.txt", "13452", "-", "filestat", new Dictionary<string, string>() { { "Something9", "Something9" } }, 9)),
            new EventViewModel(new EventModel(new DateOnly(2004, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "FILE", "OS Content Modification Time", "Content Modification Time", "User", "Host", "/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL", "TSK:/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL Type: file", 2, "TSK:/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL", "13452", "-", "filestat", new Dictionary<string, string>() { { "schema_match", "False" }, {"sha256_hash", "4eb3f81bf5801eb3f96b796c4f5b2b68a187a5165893e3a7957ae347a07c4fb7" } }, 10)),
            new EventViewModel(new EventModel(new DateOnly(2004, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "FILE", "OS Content Modification Time", "Content Modification Time", "User", "Host", "/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL", "TSK:/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL Type: file", 2, "TSK:/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL", "13452", "-", "filestat", new Dictionary<string, string>() { { "schema_match", "True" }, {"sha256_hash", "4eb3f81bf5801eb3f96b796c4f5b2b68a187a5165893e3a7957ae347a07c4fb7" } }, 11)),
            new EventViewModel(new EventModel(new DateOnly(2004, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "FILE", "OS Content Modification Time", "Content Modification Time", "User", "Host", "/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL", "TSK:/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL Type: file", 2, "TSK:/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL", "13452", "-", "filestat", new Dictionary<string, string>() { { "schema_match", "True" }, {"sha256_hash", "4eb3f81bf5801eb3f96b796c4f5b2b68a187a5165893e3a7957ae347a07c4fb9" } }, 12)),
            new EventViewModel(new EventModel(new DateOnly(2005, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "FILE", "NTFS Creation Time", "Creation Time", "User", "Host", "C:\\Users\\User\\testas.txt", "C:\\Users\\User\\testas.txt Type: file", 2, "TSK:\\Users\\User\\testas.txt", "13452", "-", "filestat", new Dictionary<string, string>() { { "Something12", "Something12" } }, 13)),
            new EventViewModel(new EventModel(new DateOnly(2005, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "FILE", "OS Content Modification Time", "Content Modification Time", "User", "Host", "/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL", "TSK:/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL Type: file", 2, "TSK:/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL", "13452", "-", "filestat", new Dictionary<string, string>() { { "schema_match", "False" }, {"sha256_hash", "4eb3f81bf5801eb3f96b796c4f5b2b68a187a5165893e3a7957ae347a07c4fb8" } }, 14)),
            new EventViewModel(new EventModel(new DateOnly(2006, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "FILE", "OS Last Access Time", "Last Access Time", "User", "Host", "/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL", "TSK:/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL Type: file", 2, "TSK:/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL", "13452", "-", "filestat", new Dictionary<string, string>() { { "schema_match", "False" }, {"sha256_hash", "4eb3f81bf5801eb3f96b796c4f5b2b68a187a5165893e3a7957ae347a07c4fb1" } }, 15)),
            new EventViewModel(new EventModel(new DateOnly(2006, 1, 1), new TimeOnly(12, 0, 30), TimeZoneInfo.Utc, "B...", "FILE", "OS Last Access Time", "Last Access Time", "User", "Host", "/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL", "TSK:/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL Type: file", 2, "TSK:/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL", "13452", "-", "filestat", new Dictionary<string, string>() { { "schema_match", "False" }, {"sha256_hash", "4eb3f81bf5801eb3f96b796c4f5b2b68a187a5165893e3a7957ae347a07c4fb2" } }, 16)),
            new EventViewModel(new EventModel(new DateOnly(2006, 1, 1), new TimeOnly(12, 1, 0), TimeZoneInfo.Utc, "B...", "FILE", "OS Metadata Modification Time", "Metadata Modification Time", "User", "Host", "/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL", "TSK:/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL Type: file", 2, "TSK:/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL", "13452", "-", "filestat", new Dictionary<string, string>() { { "schema_match", "False" }, {"sha256_hash", "4eb3f81bf5801eb3f96b796c4f5b2b68a187a5165893e3a7957ae347a07c4fb3" } }, 17)),
            new EventViewModel(new EventModel(new DateOnly(2006, 1, 1), new TimeOnly(12, 1, 1), TimeZoneInfo.Utc, "B...", "FILE", "OS Last Access Time", "Last Access Time", "User", "Host", "/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL", "TSK:/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL Type: file", 2, "TSK:/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL", "13452", "-", "filestat", new Dictionary<string, string>() { { "schema_match", "False" }, {"sha256_hash", "4eb3f81bf5801eb3f96b796c4f5b2b68a187a5165893e3a7957ae347a07c4fb4" } }, 18)),
            new EventViewModel(new EventModel(new DateOnly(2006, 1, 1), new TimeOnly(12, 1, 1), TimeZoneInfo.Utc, "B...", "FILE", "NTFS Creation Time", "Creation Time", "User", "Host", "C:\\Users\\User\\testas.txt", "C:\\Users\\User\\testas.txt Type: file", 2, "TSK:\\Users\\User\\testas.txt", "13452", "-", "filestat", new Dictionary<string, string>() { { "Something19", "Something19" } }, 19)),
            new EventViewModel(new EventModel(new DateOnly(2007, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "FILE", "OS Last Access Time", "Last Access Time", "User", "Host", "/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL", "TSK:/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL Type: file", 2, "TSK:/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL", "13452", "-", "filestat", new Dictionary<string, string>() { { "schema_match", "False" }, {"sha256_hash", "4eb3f81bf5801eb3f96b796c4f5b2b68a187a5165893e3a7957ae347a07c4fb5" } }, 20)),
            new EventViewModel(new EventModel(new DateOnly(2007, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "EVT", "NTFS Creation Time", "Creation Time", "User", "Host", "C:\\Users\\User\\testas0.txt", "C:\\Users\\User\\testas0.txt Type: file", 2, "TSK:\\Users\\User\\testas0.txt", "13452", "-", "filestat", new Dictionary<string, string>() { { "something0", "something0" } }, 21))
        };
    }

    private List<LowLevelArtefactViewModel> GetExpectedEvents()
    {
        return new List<LowLevelArtefactViewModel>()
        {
            new LowLevelArtefactViewModel() { Date = new DateOnly(2000, 1, 1), Time = new TimeOnly(12, 0, 0), Timezone = "(UTC) Coordinated Universal Time", Macb = "B...", Source = "WEBHIST", SourceType = "Firefox History", Type = "File downloaded", User = "User", Host = "Host", Short = "C:\\Documents and Settings\\PC1\\My Documents\\Downloads\\Timeline2GUI A Log2Timel...", Description = "https://mail-attachment.googleusercontent.com/attachment/... (C:\\Documents and Settings\\PC1\\My Documents\\Downloads\\Timeline2GUI A Log2Timeline CSV parser and training scenarios.pdf). Received: 1116192 bytes out of: 1116192 bytes.", Version = "2", Filename = "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", Inode = "13452", Notes = "-", Format = "sqlite/firefox_history", Extra = "extra: ['visited from: https://www.google.com/search?client=firefox-b-ab&q=ekiga&oq=ekiga&aqs=heirloom-srp..0l5 (www.google.com)'  '(URL not typed directly)'  'Transition: LINK'] schema_match: False sha256_hash: a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741 visit_type: 1",  Reference = 2 },
            new LowLevelArtefactViewModel() { Date = new DateOnly(2001, 1, 1), Time = new TimeOnly(12, 0, 0), Timezone = "(UTC) Coordinated Universal Time", Macb = "B...", Source = "WEBHIST", SourceType = "Google Cookies", Type = "Last Visited Time", User = "User", Host = "Host", Short = "https://www.mail.google.com/ (Google)", Description = "https://mail.google.com/mail/u/0/#inbox (Gautieji (170) - testas@gmail.com - Gmail) [count: 0] Visit from: https://mail.google.com/mail/u/0/ (Gmail) Type: [LINK - User clicked a link] (URL not typed directly - no typed count)", Version = "2", Filename = "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", Inode = "13452", Notes = "-", Format = "sqlite/chrome_27_history", Extra = "visit_type: 1 schema_match: False sha256_hash: a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741",  Reference = 3 },
            new LowLevelArtefactViewModel() { Date = new DateOnly(2001, 1, 1), Time = new TimeOnly(12, 0, 0), Timezone = "(UTC) Coordinated Universal Time", Macb = "B...", Source = "WEBHIST", SourceType = "Google Cookies", Type = "Last Visited Time", User = "User", Host = "Host", Short = "https://google.com/mail/u/0/?tab=rm&ogbl (Gmail)", Description = "https://google.com/mail/u/0/?tab=rm&ogbl (Gmail) [count: 0] Visit from: https://google.com/mail/?tab=rm&ogbl (Gmail) Type: [LINK - User clicked a link] (URL not typed directly - no typed count)", Version = "2", Filename = "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", Inode = "13452", Notes = "-", Format = "sqlite/chrome_27_history", Extra = "visit_type: 3 schema_match: False sha256_hash: a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741",  Reference = 5 },
            new LowLevelArtefactViewModel() { Date = new DateOnly(2002, 1, 1), Time = new TimeOnly(12, 0, 0), Timezone = "(UTC) Coordinated Universal Time", Macb = "B..M", Source = "LNK", SourceType = "Windows Shortcut", Type = "Creation Time", User = "User", Host = "Host", Short = "[Empty description] C:\\Program Files\\Mozilla Firefox\\firefox.exe", Description = "[Empty description] C:\\Program Files\\Mozilla Firefox\\firefox.exe", Version = "2", Filename = "TSK:/Documents and Settings/All Users/Start Menu/Programs/Mozilla Firefox.lnk", Inode = "13451", Notes = "-", Format = "lnk", Extra = "Something6: Something6",  Reference = 6 },
            new LowLevelArtefactViewModel() { Date = new DateOnly(2003, 1, 1), Time = new TimeOnly(12, 0, 0), Timezone = "(UTC) Coordinated Universal Time", Macb = "B...", Source = "WEBHIST", SourceType = "Firefox History", Type = "File downloaded", User = "User", Host = "Host", Short = "C:\\Documents and Settings\\PC1\\My Documents\\Downloads\\Timeline2GUI A Log2Timel...", Description = "https://mail-attachment.googleusercontent.com/attachment/... (C:\\Documents and Settings\\PC1\\My Documents\\Downloads\\Timeline2GUI A Log2Timeline CSV parser and training scenarios.pdf). Received: 1116192 bytes out of: 1116192 bytes.", Version = "2", Filename = "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", Inode = "13452", Notes = "-", Format = "sqlite/firefox_history", Extra = "extra: ['visited from: https://www.google.com/search?client=firefox-b-ab&q=ekiga&oq=ekiga&aqs=heirloom-srp..0l5 (www.google.com)'  '(URL not typed directly)'  'Transition: LINK'] schema_match: True sha256_hash: a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741 visit_type: 1",  Reference = 7 },
            new LowLevelArtefactViewModel() { Date = new DateOnly(2003, 1, 1), Time = new TimeOnly(12, 0, 0), Timezone = "(UTC) Coordinated Universal Time", Macb = "B...", Source = "FILE", SourceType = "NTFS Creation Time", Type = "Creation Time", User = "User", Host = "Host", Short = "C:\\Users\\User\\testas.txt", Description = "C:\\Users\\User\\testas.txt Type: file", Version = "2", Filename = "TSK:\\Users\\User\\testas.txt", Inode = "13452", Notes = "-", Format = "filestat", Extra = "Something8: Something8",  Reference = 8 },
            new LowLevelArtefactViewModel() { Date = new DateOnly(2004, 1, 1), Time = new TimeOnly(12, 0, 0), Timezone = "(UTC) Coordinated Universal Time", Macb = "B...", Source = "FILE", SourceType = "OS Content Modification Time", Type = "Content Modification Time", User = "User", Host = "Host", Short = "/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL", Description = "TSK:/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL Type: file", Version = "2", Filename = "TSK:/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL", Inode = "13452", Notes = "-", Format = "filestat", Extra = "schema_match: False sha256_hash: 4eb3f81bf5801eb3f96b796c4f5b2b68a187a5165893e3a7957ae347a07c4fb7",  Reference = 10 },
            new LowLevelArtefactViewModel() { Date = new DateOnly(2005, 1, 1), Time = new TimeOnly(12, 0, 0), Timezone = "(UTC) Coordinated Universal Time", Macb = "B...", Source = "FILE", SourceType = "NTFS Creation Time", Type = "Creation Time", User = "User", Host = "Host", Short = "C:\\Users\\User\\testas.txt", Description = "C:\\Users\\User\\testas.txt Type: file", Version = "2", Filename = "TSK:\\Users\\User\\testas.txt", Inode = "13452", Notes = "-", Format = "filestat", Extra = "Something12: Something12",  Reference = 13 },
            new LowLevelArtefactViewModel() { Date = new DateOnly(2005, 1, 1), Time = new TimeOnly(12, 0, 0), Timezone = "(UTC) Coordinated Universal Time", Macb = "B...", Source = "FILE", SourceType = "OS Content Modification Time", Type = "Content Modification Time", User = "User", Host = "Host", Short = "/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL", Description = "TSK:/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL Type: file", Version = "2", Filename = "TSK:/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL", Inode = "13452", Notes = "-", Format = "filestat", Extra = "schema_match: False sha256_hash: 4eb3f81bf5801eb3f96b796c4f5b2b68a187a5165893e3a7957ae347a07c4fb8",  Reference = 14 },
            new LowLevelArtefactViewModel() { Date = new DateOnly(2006, 1, 1), Time = new TimeOnly(12, 0, 0), Timezone = "(UTC) Coordinated Universal Time", Macb = "B...", Source = "FILE", SourceType = "OS Last Access Time", Type = "Last Access Time", User = "User", Host = "Host", Short = "/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL", Description = "TSK:/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL Type: file", Version = "2", Filename = "TSK:/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL", Inode = "13452", Notes = "-", Format = "filestat", Extra = "schema_match: False sha256_hash: 4eb3f81bf5801eb3f96b796c4f5b2b68a187a5165893e3a7957ae347a07c4fb1",  Reference = 15 },
            new LowLevelArtefactViewModel() { Date = new DateOnly(2006, 1, 1), Time = new TimeOnly(12, 1, 1), Timezone = "(UTC) Coordinated Universal Time", Macb = "B...", Source = "FILE", SourceType = "OS Last Access Time", Type = "Last Access Time", User = "User", Host = "Host", Short = "/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL", Description = "TSK:/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL Type: file", Version = "2", Filename = "TSK:/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL", Inode = "13452", Notes = "-", Format = "filestat", Extra = "schema_match: False sha256_hash: 4eb3f81bf5801eb3f96b796c4f5b2b68a187a5165893e3a7957ae347a07c4fb4",  Reference = 18 },
            new LowLevelArtefactViewModel() { Date = new DateOnly(2006, 1, 1), Time = new TimeOnly(12, 1, 1), Timezone = "(UTC) Coordinated Universal Time", Macb = "B...", Source = "FILE", SourceType = "NTFS Creation Time", Type = "Creation Time", User = "User", Host = "Host", Short = "C:\\Users\\User\\testas.txt", Description = "C:\\Users\\User\\testas.txt Type: file", Version = "2", Filename = "TSK:\\Users\\User\\testas.txt", Inode = "13452", Notes = "-", Format = "filestat", Extra = "Something19: Something19",  Reference = 19 },
            new LowLevelArtefactViewModel() { Date = new DateOnly(2007, 1, 1), Time = new TimeOnly(12, 0, 0), Timezone = "(UTC) Coordinated Universal Time", Macb = "B...", Source = "FILE", SourceType = "OS Last Access Time", Type = "Last Access Time", User = "User", Host = "Host", Short = "/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL", Description = "TSK:/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL Type: file", Version = "2", Filename = "TSK:/Program Files/Common Files/Microsoft Shared/Web Folders/MSOWS409.DLL", Inode = "13452", Notes = "-", Format = "filestat", Extra = "schema_match: False sha256_hash: 4eb3f81bf5801eb3f96b796c4f5b2b68a187a5165893e3a7957ae347a07c4fb5",  Reference = 20 }
        };
    }

    [TestMethod]
    public void FormLowLevelArtefacts_ShouldReturnLowLevelArtefacts_WhenMethodIsCalled()
    {
        // Arrange
        int expectedLinesSkipped = 1;
        int expectedLinesNeglected = 1;
        List<LowLevelArtefactViewModel> expected = GetExpectedEvents();

        // Act
        List<LowLevelArtefactViewModel> actual = _abstractor.FormLowLevelArtefacts(_events);

        // Assert
        Assert.AreEqual(expected.Count, actual.Count);

        for (int i = 0; i < expected.Count; i++)
        {
            Assert.AreEqual(expected[i].Date, actual[i].Date);
            Assert.AreEqual(expected[i].Time, actual[i].Time);
            Assert.AreEqual(expected[i].Timezone, actual[i].Timezone);
            Assert.AreEqual(expected[i].Macb, actual[i].Macb);
            Assert.AreEqual(expected[i].Source, actual[i].Source);
            Assert.AreEqual(expected[i].SourceType, actual[i].SourceType);
            Assert.AreEqual(expected[i].Type, actual[i].Type);
            Assert.AreEqual(expected[i].User, actual[i].User);
            Assert.AreEqual(expected[i].Host, actual[i].Host);
            Assert.AreEqual(expected[i].Short, actual[i].Short);
            Assert.AreEqual(expected[i].Description, actual[i].Description);
            Assert.AreEqual(expected[i].Version, actual[i].Version);
            Assert.AreEqual(expected[i].Filename, actual[i].Filename);
            Assert.AreEqual(expected[i].Inode, actual[i].Inode);
            Assert.AreEqual(expected[i].Notes, actual[i].Notes);
            Assert.AreEqual(expected[i].Format, actual[i].Format);
            Assert.AreEqual(expected[i].Extra, actual[i].Extra);
            Assert.AreEqual(expected[i].Reference, actual[i].Reference);
        }

        Assert.AreEqual(expectedLinesSkipped, _abstractor.LinesSkipped);
        Assert.AreEqual(expectedLinesNeglected, _abstractor.LinesNeglected);
    }
}
