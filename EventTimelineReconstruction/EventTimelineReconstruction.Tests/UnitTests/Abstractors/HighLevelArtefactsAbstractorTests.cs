using EventTimelineReconstruction.Abstractors;
using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;
using Moq;

namespace EventTimelineReconstruction.Tests.UnitTests.Abstractors;

[TestClass]
public class HighLevelArtefactsAbstractorTests
{
    private readonly Mock<ILowLevelEventsAbstractorUtils> _lowLevelEventsAbstractorUtils;
    private readonly Mock<IHighLevelArtefactsAbstractorUtils> _highLevelArtefactsAbstractorUtils;
    private readonly HighLevelArtefactsAbstractor _abstractor;
    private readonly List<EventViewModel> _events;

    public HighLevelArtefactsAbstractorTests()
    {
        Mock<IHighLevelEventsAbstractorUtils> highLevelEventsAbstractorUtils = new();
        _lowLevelEventsAbstractorUtils = new();
        _highLevelArtefactsAbstractorUtils = new();
        _abstractor = new(highLevelEventsAbstractorUtils.Object, _lowLevelEventsAbstractorUtils.Object, _highLevelArtefactsAbstractorUtils.Object);

        _events = GetStoredEvents();

        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetFilename(_events[0].Description)).Returns("testas0.txt");
        _lowLevelEventsAbstractorUtils.Setup(u => u.GetExtraTillSha256(_events[0].Extra)).Returns("something0: somehing0");

        highLevelEventsAbstractorUtils.Setup(u => u.IsValidWebhistLine(_events[1])).Returns(false);

        highLevelEventsAbstractorUtils.Setup(u => u.IsValidWebhistLine(_events[2])).Returns(true);
        _lowLevelEventsAbstractorUtils.Setup(u => u.GetFirefoxExtraFromWebhistSource(_events[2].Extra)).Returns("");
        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetOtherBrowserExtraFromWebhistSource(_events[2].Extra)).Returns("");
        highLevelEventsAbstractorUtils.Setup(u => u.IsWebhistDownloadEvent(_events[2])).Returns(false);
        highLevelEventsAbstractorUtils.Setup(u => u.IsWebhistMailEvent(_events[2])).Returns(false);
        highLevelEventsAbstractorUtils.Setup(u => u.IsWebhistNamingActivityEvent(_events[2])).Returns(false);

        highLevelEventsAbstractorUtils.Setup(u => u.IsValidWebhistLine(_events[3])).Returns(true);
        _lowLevelEventsAbstractorUtils.Setup(u => u.GetFirefoxExtraFromWebhistSource(_events[3].Extra)).Returns("https://www.google.com/search?client=firefox-b-ab&q=ekiga&oq=ekiga&aqs=heirloom-srp..0l5");
        highLevelEventsAbstractorUtils.Setup(u => u.IsWebhistDownloadEvent(_events[3])).Returns(true);
        highLevelEventsAbstractorUtils.Setup(u => u.GetDownloadedFileName(_events[3].Description)).Returns("C:\\Documents and Settings\\PC1\\My Documents\\Downloads\\Timeline2GUI A Log2Timeline CSV parser and training scenarios.pdf");

        highLevelEventsAbstractorUtils.Setup(u => u.IsValidWebhistLine(_events[4])).Returns(true);
        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetOtherBrowserExtraFromWebhistSource(_events[4].Extra)).Returns("visit_type: 1");
        highLevelEventsAbstractorUtils.Setup(u => u.IsWebhistDownloadEvent(_events[4])).Returns(false);
        highLevelEventsAbstractorUtils.Setup(u => u.IsWebhistMailEvent(_events[4])).Returns(true);
        highLevelEventsAbstractorUtils.Setup(u => u.GetMailUrl(It.IsAny<string>())).Returns("https://mail.google.com");
        _lowLevelEventsAbstractorUtils.Setup(u => u.AddMailUser("visit_type: 1", _events[4].Description)).Returns("testas@gmail.com");

        highLevelEventsAbstractorUtils.Setup(u => u.IsValidWebhistLine(_events[5])).Returns(true);
        highLevelEventsAbstractorUtils.Setup(u => u.IsWebhistDownloadEvent(_events[5])).Returns(false);
        highLevelEventsAbstractorUtils.Setup(u => u.IsWebhistMailEvent(_events[5])).Returns(true);

        highLevelEventsAbstractorUtils.Setup(u => u.IsValidWebhistLine(_events[6])).Returns(true);
        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetOtherBrowserExtraFromWebhistSource(_events[6].Extra)).Returns("visit_type: 2");
        highLevelEventsAbstractorUtils.Setup(u => u.IsWebhistDownloadEvent(_events[6])).Returns(false);
        highLevelEventsAbstractorUtils.Setup(u => u.IsWebhistMailEvent(_events[6])).Returns(false);
        highLevelEventsAbstractorUtils.Setup(u => u.IsWebhistNamingActivityEvent(_events[6])).Returns(true);
        _lowLevelEventsAbstractorUtils.Setup(u => u.GetUrl(_events[6].Short)).Returns("https://www.google.com/mail/");
        highLevelEventsAbstractorUtils.Setup(u => u.GenerateVisitValue(_events[6].Description)).Returns("LINK");

        _lowLevelEventsAbstractorUtils.Setup(u => u.GetShort(_events[7].Description)).Returns("Prefetch [SPOOLSV0.EXE] was executed - run count 1 path: \\WINDOWS\\SYSTEM32\\SPOOLSV0.EXE hash: 0x282F76A7 volume: 1 [serial number: 0x6C91EF9F  device path: \\DEVICE\\HARDDISKVOLUME0]");
        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetDescriptionFromLogSource(_events[7])).Returns("TSK:/WINDOWS/Prefetch/SPOOLSV0.EXE-282F76A7.pf");

        _lowLevelEventsAbstractorUtils.Setup(u => u.GetShort(_events[8].Description)).Returns("very secret.lnk");
        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetDescriptionFromLogSource(_events[8])).Returns("TSK:/Documents and Settings/PC1/Recent/very secret.lnk");

        _lowLevelEventsAbstractorUtils.Setup(u => u.GetShort(_events[9].Description)).Returns("Prefetch [SPOOLSV1.EXE] was executed - run count 1 path: \\WINDOWS\\SYSTEM32\\SPOOLSV1.EXE hash: 0x282F76A7 volume: 1 [serial number: 0x6C91EF9F  device path: \\DEVICE\\HARDDISKVOLUME1]");
        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetDescriptionFromLogSource(_events[9])).Returns("TSK:/WINDOWS/Prefetch/SPOOLSV1.EXE-282F76A7.pf");

        _lowLevelEventsAbstractorUtils.Setup(u => u.GetShort(_events[10].Description)).Returns("Prefetch [SPOOLSV2.EXE] was executed - run count 1 path: \\WINDOWS\\SYSTEM32\\SPOOLSV2.EXE hash: 0x282F76A7 volume: 1 [serial number: 0x6C91EF9F  device path: \\DEVICE\\HARDDISKVOLUME2]");
        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetDescriptionFromLogSource(_events[10])).Returns("TSK:/WINDOWS/Prefetch/SPOOLSV2.EXE-282F76A7.pf");

        _lowLevelEventsAbstractorUtils.Setup(u => u.GetShort(_events[11].Description)).Returns("Prefetch [SPOOLSV3.EXE] was executed - run count 1 path: \\WINDOWS\\SYSTEM32\\SPOOLSV3.EXE hash: 0x282F76A7 volume: 1 [serial number: 0x6C91EF9F  device path: \\DEVICE\\HARDDISKVOLUME3]");
        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetDescriptionFromLogSource(_events[11])).Returns("TSK:/WINDOWS/Prefetch/SPOOLSV3.EXE-282F76A7.pf");

        _lowLevelEventsAbstractorUtils.Setup(u => u.GetShort(_events[12].Description)).Returns("C:\\december\\testas12.txt");
        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetDescriptionFromLogSource(_events[12])).Returns("");

        _lowLevelEventsAbstractorUtils.Setup(u => u.GetShort(_events[13].Description)).Returns("C:\\december\\testas13.txt");
        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetDescriptionFromLogSource(_events[13])).Returns("");

        _lowLevelEventsAbstractorUtils.Setup(u => u.GetSummaryFromShort(_events[14].Description)).Returns("{645FF040-5081-101B-9F08-00AA002F954E}");
        _lowLevelEventsAbstractorUtils.Setup(u => u.GetExtraTillSha256(_events[14].Extra)).Returns("something14: something14");
        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetDescriptionFromRegSource(_events[14].SourceType, _events[14].Description)).Returns("test14.exe");

        _lowLevelEventsAbstractorUtils.Setup(u => u.GetShort(_events[15].Description)).Returns("C:\\december\\testas15.txt");
        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetDescriptionFromLogSource(_events[15])).Returns("");

        _lowLevelEventsAbstractorUtils.Setup(u => u.GetSummaryFromShort(_events[16].Description)).Returns("");
        _lowLevelEventsAbstractorUtils.Setup(u => u.GetExtraTillSha256(_events[16].Extra)).Returns("something16: something16");
        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetDescriptionFromRegSource(_events[16].SourceType, _events[16].Description)).Returns("");

        _lowLevelEventsAbstractorUtils.Setup(u => u.GetSummaryFromShort(_events[17].Description)).Returns("{645FF040-5081-101B-9F08-00AA002F954A}");
        _lowLevelEventsAbstractorUtils.Setup(u => u.GetExtraTillSha256(_events[17].Extra)).Returns("something17: something17");
        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetDescriptionFromRegSource(_events[17].SourceType, _events[17].Description)).Returns(_events[17].Description);

        _lowLevelEventsAbstractorUtils.Setup(u => u.GetSummaryFromShort(_events[18].Description)).Returns("{645FF040-5081-101B-9F08-00AA002F954C}");
        _lowLevelEventsAbstractorUtils.Setup(u => u.GetExtraTillSha256(_events[18].Extra)).Returns("something18: something18");
        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetDescriptionFromRegSource(_events[18].SourceType, _events[18].Description)).Returns(_events[18].Description);

        _lowLevelEventsAbstractorUtils.Setup(u => u.GetSummaryFromShort(_events[19].Description)).Returns("{645FF040-5081-101B-9F08-00AA002F954B}");
        _lowLevelEventsAbstractorUtils.Setup(u => u.GetExtraTillSha256(_events[19].Extra)).Returns("something19: something19");
        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetDescriptionFromRegSource(_events[19].SourceType, _events[19].Description)).Returns("");

        _lowLevelEventsAbstractorUtils.Setup(u => u.GetSummaryFromShort(_events[20].Description)).Returns("{645FF040-5081-101B-9F08-00AA002F954D}");
        _lowLevelEventsAbstractorUtils.Setup(u => u.GetExtraTillSha256(_events[20].Extra)).Returns("something20: something20");
        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetDescriptionFromRegSource(_events[20].SourceType, _events[20].Description)).Returns("");

        _lowLevelEventsAbstractorUtils.Setup(u => u.GetKeywordsFromExtra(_events[21].Extra, _events[21].Short)).Returns("number_of_paragraphs: 3 total_time: 1");
        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetDescriptionFromMetaSource(_events[21].Description)).Returns(_events[21].Description);

        _highLevelArtefactsAbstractorUtils.Setup(u => u.IsPeLineExecutable(_events[22])).Returns(false);
        _highLevelArtefactsAbstractorUtils.Setup(u => u.IsPeLineValid(_events[22])).Returns(true);

        _highLevelArtefactsAbstractorUtils.Setup(u => u.IsPeLineExecutable(_events[23])).Returns(false);
        _highLevelArtefactsAbstractorUtils.Setup(u => u.IsPeLineValid(_events[23])).Returns(true);

        _highLevelArtefactsAbstractorUtils.Setup(u => u.IsPeLineExecutable(_events[24])).Returns(true);

        _highLevelArtefactsAbstractorUtils.Setup(u => u.IsPeLineExecutable(_events[25])).Returns(false);
        _highLevelArtefactsAbstractorUtils.Setup(u => u.IsPeLineValid(_events[25])).Returns(false);

        _lowLevelEventsAbstractorUtils.Setup(u => u.GetKeywordsFromExtra(_events[26].Extra, _events[26].Short)).Returns("number_of_paragraphs: 3 total_time: 1");
        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetDescriptionFromMetaSource(_events[26].Description)).Returns(_events[26].Description);

        _lowLevelEventsAbstractorUtils.Setup(u => u.GetKeywordsFromExtra(_events[27].Extra, _events[27].Short)).Returns("number_of_paragraphs: 3 total_time: 1");
        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetDescriptionFromMetaSource(_events[27].Description)).Returns(_events[27].Description);

        highLevelEventsAbstractorUtils.Setup(u => u.GetShort(_events[31].Description)).Returns("C:\\Program Files\\Mozilla Firefox\\firefox.exe");

        highLevelEventsAbstractorUtils.Setup(u => u.GetShort(_events[32].Description)).Returns("C:\\Program Files\\Mozilla Firefox\\firefox.exe");

        _highLevelArtefactsAbstractorUtils.Setup(u => u.IsFileDuplicateOfLnk(_events, 32, _events[33])).Returns(true);

        _highLevelArtefactsAbstractorUtils.Setup(u => u.IsFileDuplicateOfLnk(_events, 33, _events[34])).Returns(false);
        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetFileCountInRowAtSameMinute(_events, 34)).Returns(2);
        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetFilename(_events[34].Description)).Returns("testas34.txt");
        _lowLevelEventsAbstractorUtils.Setup(u => u.GetExtraTillSha256(_events[34].Extra)).Returns("something34: somehing34");

        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetFilename(_events[35].Description)).Returns("testas35.txt");
        _lowLevelEventsAbstractorUtils.Setup(u => u.GetExtraTillSha256(_events[35].Extra)).Returns("something35: somehing34");

        _lowLevelEventsAbstractorUtils.Setup(u => u.GetSummaryFromShort(_events[36].Description)).Returns("{645FF040-5081-101B-9F08-00AA002F954G}");
        _lowLevelEventsAbstractorUtils.Setup(u => u.GetExtraTillSha256(_events[36].Extra)).Returns("something36: something36");
        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetDescriptionFromRegSource(_events[36].SourceType, _events[36].Description)).Returns("test36.txt");

        _highLevelArtefactsAbstractorUtils.Setup(u => u.IsFileDuplicateOfLnk(_events, 36, _events[37])).Returns(false);
        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetFileCountInRowAtSameMinute(_events, 37)).Returns(37);
        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetFilename(_events[37].Description)).Returns("testas37.txt");
        _lowLevelEventsAbstractorUtils.Setup(u => u.GetExtraTillSha256(_events[37].Extra)).Returns("something37: somehing37");
        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetValidFileEventIndices(_events, 37, It.IsAny<int>())).Returns(new List<int>() { 37 });
    }

    private List<EventViewModel> GetStoredEvents()
    {
        return new List<EventViewModel>()
        {
            new EventViewModel(new EventModel(new DateOnly(2000, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "FILE", "NTFS Creation Time", "Creation Time", "User", "Host", "C:\\Users\\User\\testas0.txt", "C:\\Users\\User\\testas0.txt Type: file", 2, "TSK:\\Users\\User\\testas0.txt", "13452", "-", "filestat", new Dictionary<string, string>() { { "something0", "something0" } }, 0)),
            new EventViewModel(new EventModel(new DateOnly(2000, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "WEBHIST", "Firefox History", "Last Visited Time", "User", "Host", "https://google.com/mail/u/0/?tab=rm&ogbl (Gmail)", "https://google.com/mail/u/0/?tab=rm&ogbl (Gmail) [count: 0] Visit from: https://google.com/mail/?tab=rm&ogbl (Gmail) Type: [LINK - User clicked a link] (URL not typed directly - no typed count)", 2, "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", "13452", "-", "sqlite/chrome_27_history", new Dictionary<string, string>(), 1)),
            new EventViewModel(new EventModel(new DateOnly(2000, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "WEBHIST", "Firefox History", "Last Visited Time", "User", "Host", "https://google.com/mail/u/0/?tab=rm&ogbl (Gmail)", "https://google.com/mail/u/0/?tab=rm&ogbl (Gmail) [count: 0] Visit from: https://google.com/mail/?tab=rm&ogbl (Gmail) Type: [LINK - User clicked a link] (URL not typed directly - no typed count)", 2, "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", "13452", "-", "sqlite/chrome_27_history", new Dictionary<string, string>(), 2)),
            new EventViewModel(new EventModel(new DateOnly(2000, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "WEBHIST", "Firefox History", "File downloaded", "User", "Host", "C:\\Documents and Settings\\PC1\\My Documents\\Downloads\\Timeline2GUI A Log2Timel...", "https://mail-attachment.googleusercontent.com/attachment/... (C:\\Documents and Settings\\PC1\\My Documents\\Downloads\\Timeline2GUI A Log2Timeline CSV parser and training scenarios.pdf). Received: 1116192 bytes out of: 1116192 bytes.", 2, "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", "13452", "-", "sqlite/firefox_history", new Dictionary<string, string>() { { "extra", "['visited from: https://www.google.com/search?client=firefox-b-ab&q=ekiga&oq=ekiga&aqs=heirloom-srp..0l5 (www.google.com)'  '(URL not typed directly)'  'Transition: LINK']" }, { "schema_match", "False" }, { "sha256_hash", "a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741" }, { "visit_type", "1" } }, 3)),
            new EventViewModel(new EventModel(new DateOnly(2002, 1, 1), new TimeOnly(12, 0, 1), TimeZoneInfo.Utc, "B...", "WEBHIST", "Google History", "Last Visited Time", "User", "Host", "https://www.mail.google.com/ (Google)", "https://mail.google.com/mail/u/0/#inbox (Gautieji (170) - testas@gmail.com - Gmail) [count: 0] Visit from: https://mail.google.com/mail/u/0/ (Gmail) Type: [LINK - User clicked a link] (URL not typed directly - no typed count)", 2, "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", "13452", "-", "sqlite/chrome_27_history", new Dictionary<string, string>() { { "visit_type", "1" }, { "schema_match", "False" }, { "sha256_hash", "a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741" } }, 4)),
            new EventViewModel(new EventModel(new DateOnly(2002, 1, 1), new TimeOnly(12, 0, 1), TimeZoneInfo.Utc, "B...", "WEBHIST", "Google History", "Last Visited Time", "User", "Host", "https://www.mail.google.com/ (Google)", "https://mail.google.com/mail/u/0/#inbox (Gautieji (170) - testas@gmail.com - Gmail) [count: 0] Visit from: https://mail.google.com/mail/u/0/ (Gmail) Type: [LINK - User clicked a link] (URL not typed directly - no typed count)", 2, "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", "13452", "-", "sqlite/chrome_27_history", new Dictionary<string, string>() { { "visit_type", "1" }, { "schema_match", "False" }, { "sha256_hash", "a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741" } }, 5)),
            new EventViewModel(new EventModel(new DateOnly(2003, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "WEBHIST", "Google History", "Last Visited Time", "User", "Host", "https://google.com/mail/u/0/?tab=rm&ogbl (Gmail)", "https://google.com/mail/u/0/?tab=rm&ogbl (Gmail) [count: 0] Visit from: https://google.com/mail/?tab=rm&ogbl (Gmail) Type: [LINK - User clicked a link] (URL not typed directly - no typed count)", 2, "TSK:/Documents and Settings/PC1/Local Settings/Application Data/Google/Chrome/User Data/Default/History", "13452", "-", "sqlite/chrome_27_history", new Dictionary<string, string>() { { "visit_type", "2" }, { "schema_match", "False" }, { "sha256_hash", "a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741" } }, 6)),
            new EventViewModel(new EventModel(new DateOnly(2003, 2, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "LOG", "System", "Creation Time", "User", "Host", "SPOOLSV.EXE was run 1 time(s)", "Prefetch [SPOOLSV0.EXE] was executed - run count 1 path: \\WINDOWS\\SYSTEM32\\SPOOLSV0.EXE hash: 0x282F76A7 volume: 1 [serial number: 0x6C91EF9F  device path: \\DEVICE\\HARDDISKVOLUME0]", 2, "TSK:/WINDOWS/Prefetch/SPOOLSV0.EXE-282F76A7.pf", "13452", "-", "prefetch", new Dictionary<string, string>(), 7)),
            new EventViewModel(new EventModel(new DateOnly(2004, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "LOG", "System", "Creation Time", "User", "Host", "1d12a904-665c-11ed-af7f-080027367a04 Origin: very secret.lnk", "1d12a904-665c-11ed-af7f-080027367a04 MAC address: 08:00:27:36:7a:04 Origin: very secret.lnk", 2, "TSK:/Documents and Settings/PC1/Recent/very secret.lnk", "13452", "-", "lnk", new Dictionary<string, string>(), 8)),
            new EventViewModel(new EventModel(new DateOnly(2005, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "LOG", "System", "Creation Time", "User", "Host", "SPOOLSV.EXE was run 1 time(s)", "Prefetch [SPOOLSV1.EXE] was executed - run count 1 path: \\WINDOWS\\SYSTEM32\\SPOOLSV1.EXE hash: 0x282F76A7 volume: 1 [serial number: 0x6C91EF9F  device path: \\DEVICE\\HARDDISKVOLUME1]", 2, "TSK:/WINDOWS/Prefetch/SPOOLSV1.EXE-282F76A7.pf", "13452", "-", "prefetch", new Dictionary<string, string>(), 9)),
            new EventViewModel(new EventModel(new DateOnly(2005, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "LOG", "System", "Creation Time", "User", "Host", "SPOOLSV.EXE was run 1 time(s)", "Prefetch [SPOOLSV2.EXE] was executed - run count 1 path: \\WINDOWS\\SYSTEM32\\SPOOLSV2.EXE hash: 0x282F76A7 volume: 1 [serial number: 0x6C91EF9F  device path: \\DEVICE\\HARDDISKVOLUME2]", 2, "TSK:/WINDOWS/Prefetch/SPOOLSV2.EXE-282F76A7.pf", "13452", "-", "prefetch", new Dictionary<string, string>(), 10)),
            new EventViewModel(new EventModel(new DateOnly(2006, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "LOG", "System", "Creation Time", "User", "Host", "SPOOLSV.EXE was run 1 time(s)", "Prefetch [SPOOLSV3.EXE] was executed - run count 1 path: \\WINDOWS\\SYSTEM32\\SPOOLSV3.EXE hash: 0x282F76A7 volume: 1 [serial number: 0x6C91EF9F  device path: \\DEVICE\\HARDDISKVOLUME3]", 2, "TSK:/WINDOWS/Prefetch/SPOOLSV3.EXE-282F76A7.pf", "13452", "-", "prefetch", new Dictionary<string, string>(), 11)),
            new EventViewModel(new EventModel(new DateOnly(2007, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B..M", "LOG", "System", "Creation Time", "User", "Host", "Entry: 36 Pin status: Unpinned Path: C:\\december\\testas12.txt", "Something12", 2, "TSK:/WINDOWS/Prefetch/SPOOLSV4.EXE-282F76A7.pf", "13452", "-", "prefetch", new Dictionary<string, string>(), 12)),
            new EventViewModel(new EventModel(new DateOnly(2007, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B..M", "LOG", "System", "Creation Time", "User", "Host", "Entry: 36 Pin status: Unpinned Path: C:\\december\\testas13.txt", "Something13", 2, "TSK:/WINDOWS/Prefetch/SPOOLSV4.EXE-282F76A7.pf", "13452", "-", "prefetch", new Dictionary<string, string>(), 13)),
            new EventViewModel(new EventModel(new DateOnly(2008, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, ".A..", "REG", "Registry Key: UserAssist", "Last Time Executed", "User", "Host", "UEME_RUNPATH:::{645FF040-5081-101B-9F08-00AA002F954E} Count: 1", "[HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\UserAssist\\{75048700-EF1F-11D0-9888-006097DEACF9}\\Count] Value name: UEME_RUNPATH:::{645FF040-5081-101B-9F08-00AA002F954E}\\test14.exe Count: 1", 2, "TSK:/Documents and Settings/PC1/NTUSER.DAT", "13452", "-", "winreg/userassist", new Dictionary<string, string>() { { "something14", "something14" } }, 14)),
            new EventViewModel(new EventModel(new DateOnly(2008, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B..M", "LOG", "System", "Creation Time", "User", "Host", "Entry: 36 Pin status: Unpinned Path: C:\\december\\testas15.txt", "Something15", 2, "TSK:/WINDOWS/Prefetch/SPOOLSV14.EXE-282F76A7.pf", "13452", "-", "prefetch", new Dictionary<string, string>(), 15)),
            new EventViewModel(new EventModel(new DateOnly(2009, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, ".A..", "REG", "Registry Key : BagMRU", "Last Time Executed", "User", "Host", "Something16", "Something16", 2, "Something16", "13452", "-", "winreg/userassist", new Dictionary<string, string>() { { "something16", "something16" } }, 16)),
            new EventViewModel(new EventModel(new DateOnly(2010, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, ".A..", "REG", "Registry Key : Run Key", "Last Time Executed", "User", "Host", "UEME_RUNPATH:::{645FF040-5081-101B-9F08-00AA002F954A} Count: 1", "Something17", 2, "Something17", "13452", "-", "winreg/userassist", new Dictionary<string, string>() { { "something17", "something17" } }, 17)),
            new EventViewModel(new EventModel(new DateOnly(2010, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, ".A..", "REG", "Registry Key : Run Key", "Last Time Executed", "User", "Host", "UEME_RUNPATH:::{645FF040-5081-101B-9F08-00AA002F954C} Count: 1", "Something18", 2, "Something18", "13452", "-", "winreg/userassist", new Dictionary<string, string>() { { "something18", "something18" } }, 18)),
            new EventViewModel(new EventModel(new DateOnly(2010, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, ".A..", "REG", "UNKNOWN", "Last Time Executed", "User", "Host", "UEME_RUNPATH:::{645FF040-5081-101B-9F08-00AA002F954B} Count: 1", "Something19", 2, "Something19", "13452", "-", "winreg/userassist", new Dictionary<string, string>() { { "something19", "something19" } }, 19)),
            new EventViewModel(new EventModel(new DateOnly(2010, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, ".A..", "REG", "UNKNOWN", "Last Time Executed", "User", "Host", "UEME_RUNPATH:::{645FF040-5081-101B-9F08-00AA002F954D} Count: 1", "Something20", 2, "Something20", "13452", "-", "winreg/userassist", new Dictionary<string, string>() { { "something20", "something20" } }, 20)),
            new EventViewModel(new EventModel(new DateOnly(2011, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "META", "System", "Creation Time", "User", "Host", "Short Description", "Full Description21", 2, "testas.docx", "13452", "-", "Format", new Dictionary<string, string>() { { "number_of_paragraphs", "3" }, { "total_time", "1" } }, 21)),
            new EventViewModel(new EventModel(new DateOnly(2012, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "PE", "PE Compilation time", "Creation Time", "User", "Host", "pe_type", "Something22", 2, "test.txt", "13452", "-", "pe", new Dictionary<string, string>(), 22)),
            new EventViewModel(new EventModel(new DateOnly(2012, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "PE", "PE Compilation time", "Creation Time", "User", "Host", "pe_type", "Something23", 2, "test.txt", "13452", "-", "pe", new Dictionary<string, string>(), 23)),
            new EventViewModel(new EventModel(new DateOnly(2013, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "PE", "PE Compilation time", "Creation Time", "User", "Host", "pe_type", "Something24", 2, "test.exe", "13452", "-", "pe", new Dictionary<string, string>(), 24)),
            new EventViewModel(new EventModel(new DateOnly(2013, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "PE", "PE Compilation time", "Creation Time", "User", "Host", "Something", "Something25", 2, "test.txt", "13452", "-", "pe", new Dictionary<string, string>(), 25)),
            new EventViewModel(new EventModel(new DateOnly(2014, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "META", "System", "Creation Time", "User", "Host", "Short Description", "Full Description26", 2, "testas.docx", "13452", "-", "Format", new Dictionary<string, string>() { { "number_of_paragraphs", "3" }, { "total_time", "1" } }, 26)),
            new EventViewModel(new EventModel(new DateOnly(2014, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "META", "System", "Creation Time", "User", "Host", "Short Description", "Full Description27", 2, "testas.docx", "13452", "-", "Format", new Dictionary<string, string>() { { "number_of_paragraphs", "3" }, { "total_time", "1" } }, 27)),
            new EventViewModel(new EventModel(new DateOnly(2015, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "EVT", "PE Compilation time", "Creation Time", "User", "Host", "pe_type", "Something something", 2, "test.exe", "13452", "-", "pe", new Dictionary<string, string>(), 28)),
            new EventViewModel(new EventModel(new DateOnly(2015, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B..M", "OLECF", "OLECF Item", "Creation Time", "User", "Host", "Name: view", "Something something29", 2, "TSK:/WINDOWS/system32/wmimgmt.msc", "13451", "-", "olecf/olecf_default", new Dictionary<string, string>(), 29)),
            new EventViewModel(new EventModel(new DateOnly(2015, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "OLECF", "OLECF Item", "Creation Time", "User", "Host", "Name: data", "Something something something something something something something something 30", 2, "TSK:/WINDOWS/system32/wmimgmt.msc", "13452", "-", "olecf/olecf_default", new Dictionary<string, string>(), 30)),
            new EventViewModel(new EventModel(new DateOnly(2016, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B..M", "LNK", "Windows Shortcut", "Creation Time", "User", "Host", "[Empty description] C:\\Program Files\\Mozilla Firefox\\firefox.exe", "[Empty description] C:\\Program Files\\Mozilla Firefox\\firefox.exe", 2, "TSK:/Documents and Settings/All Users/Start Menu/Programs/Mozilla Firefox.lnk", "13451", "-", "lnk", new Dictionary<string, string>(), 31)),
            new EventViewModel(new EventModel(new DateOnly(2016, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B..M", "LNK", "Windows Shortcut", "Creation Time", "User", "Host", "[Empty description] C:\\Program Files\\Mozilla Firefox\\firefox.exe", "[Empty description] C:\\Program Files\\Mozilla Firefox\\firefox.exe", 2, "TSK:/Documents and Settings/All Users/Start Menu/Programs/Mozilla Firefox.lnk", "13451", "-", "lnk", new Dictionary<string, string>(), 32)),
            new EventViewModel(new EventModel(new DateOnly(2017, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "FILE", "NTFS Creation Time", "Creation Time", "User", "Host", "C:\\Users\\User\\testas33.txt", "C:\\Users\\User\\testas33.txt Type: file", 2, "TSK:\\Users\\User\\testas33.txt", "13452", "-", "filestat", new Dictionary<string, string>() { { "something33", "something33" } }, 33)),
            new EventViewModel(new EventModel(new DateOnly(2017, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "FILE", "NTFS Creation Time", "Creation Time", "User", "Host", "C:\\Users\\User\\testas34.txt", "C:\\Users\\User\\testas34.txt Type: file", 2, "TSK:\\Users\\User\\testas34.txt", "13452", "-", "filestat", new Dictionary<string, string>() { { "something34", "something34" } }, 34)),
            new EventViewModel(new EventModel(new DateOnly(2017, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "FILE", "NTFS Creation Time", "Creation Time", "User", "Host", "C:\\Users\\User\\testas35.txt", "C:\\Users\\User\\testas35.txt Type: file", 2, "TSK:\\Users\\User\\testas35.txt", "13452", "-", "filestat", new Dictionary<string, string>() { { "something35", "something35" } }, 35)),
            new EventViewModel(new EventModel(new DateOnly(2018, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, ".A..", "REG", "Registry Key: UserAssist", "Last Time Executed", "User", "Host", "UEME_RUNPATH:::{645FF040-5081-101B-9F08-00AA002F954G} Count: 1", "[HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\UserAssist\\{75048700-EF1F-11D0-9888-006097DEACF9}\\Count] Value name: UEME_RUNPATH:::{645FF040-5081-101B-9F08-00AA002F954E}\\test36.txt Count: 1", 2, "TSK:/Documents and Settings/PC1/NTUSER.DAT", "13452", "-", "winreg/userassist", new Dictionary<string, string>() { { "something36", "something36" } }, 36)),
            new EventViewModel(new EventModel(new DateOnly(2018, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "FILE", "NTFS Creation Time", "Creation Time", "User", "Host", "C:\\Users\\User\\testas37.txt", "C:\\Users\\User\\testas37.txt Type: file", 2, "TSK:\\Users\\User\\testas37.txt", "13452", "-", "filestat", new Dictionary<string, string>() { { "something37", "something37" } }, 37)),
        };
    }

    private List<HighLevelArtefactViewModel> GetExpectedEvents()
    {
        return new List<HighLevelArtefactViewModel>()
        {
            new HighLevelArtefactViewModel() { Date = new DateOnly(2000, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "FILE", Short = "testas0.txt", Reference = 0, Extra = "something0: somehing0", Macb = "B...", SourceType = "NTFS Creation Time", Description = "C:\\Users\\User\\testas0.txt Type: file" },
            new HighLevelArtefactViewModel() { Date = new DateOnly(2000, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "WEBHIST", Short = "C:\\Documents and Settings\\PC1\\My Documents\\Downloads\\Timeline2GUI A Log2Timeline CSV parser and training scenarios.pdf", Reference = 3, Visit = "Download", Extra = "https://www.google.com/search?client=firefox-b-ab&q=ekiga&oq=ekiga&aqs=heirloom-srp..0l5", Macb = "B...", SourceType = "Firefox History", Description = "https://mail-attachment.googleusercontent.com/attachment/... (C:\\Documents and Settings\\PC1\\My Documents\\Downloads\\Timeline2GUI A Log2Timeline CSV parser and training scenarios.pdf). Received: 1116192 bytes out of: 1116192 bytes." },
            new HighLevelArtefactViewModel() { Date = new DateOnly(2002, 1, 1), Time = new TimeOnly(12, 0, 1), Source = "WEBHIST", Short = "https://mail.google.com", Reference = 4, Visit = "Mail", Extra = "testas@gmail.com", Macb = "B...", SourceType = "Google History", Description = "https://mail.google.com/mail/u/0/#inbox (Gautieji (170) - testas@gmail.com - Gmail) [count: 0] Visit from: https://mail.google.com/mail/u/0/ (Gmail) Type: [LINK - User clicked a link] (URL not typed directly - no typed count)" },
            new HighLevelArtefactViewModel() { Date = new DateOnly(2003, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "WEBHIST", Short = "https://www.google.com/mail/", Reference = 6, Visit = "LINK", Macb = "B...", Extra = "visit_type: 2", SourceType = "Google History", Description = "https://google.com/mail/u/0/?tab=rm&ogbl (Gmail) [count: 0] Visit from: https://google.com/mail/?tab=rm&ogbl (Gmail) Type: [LINK - User clicked a link] (URL not typed directly - no typed count)" },
            new HighLevelArtefactViewModel() { Date = new DateOnly(2003, 2, 1), Time = new TimeOnly(12, 0, 0), Source = "LOG", Short = "Prefetch [SPOOLSV0.EXE] was executed - run count 1 path: \\WINDOWS\\SYSTEM32\\SPOOLSV0.EXE hash: 0x282F76A7 volume: 1 [serial number: 0x6C91EF9F  device path: \\DEVICE\\HARDDISKVOLUME0]", Reference = 7, Macb = "B...", SourceType = "System", Description = "TSK:/WINDOWS/Prefetch/SPOOLSV0.EXE-282F76A7.pf" },
            new HighLevelArtefactViewModel() { Date = new DateOnly(2004, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "LOG", Short = "very secret.lnk", Reference = 8, Macb = "B...", SourceType = "System", Description = "TSK:/Documents and Settings/PC1/Recent/very secret.lnk" },
            new HighLevelArtefactViewModel() { Date = new DateOnly(2005, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "LOG", Short = "Prefetch [SPOOLSV1.EXE] was executed - run count 1 path: \\WINDOWS\\SYSTEM32\\SPOOLSV1.EXE hash: 0x282F76A7 volume: 1 [serial number: 0x6C91EF9F  device path: \\DEVICE\\HARDDISKVOLUME1]", Reference = 9, Macb = "B...", SourceType = "System", Description = "TSK:/WINDOWS/Prefetch/SPOOLSV1.EXE-282F76A7.pf" },
            new HighLevelArtefactViewModel() { Date = new DateOnly(2006, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "LOG", Short = "Prefetch [SPOOLSV3.EXE] was executed - run count 1 path: \\WINDOWS\\SYSTEM32\\SPOOLSV3.EXE hash: 0x282F76A7 volume: 1 [serial number: 0x6C91EF9F  device path: \\DEVICE\\HARDDISKVOLUME3]", Reference = 11, Macb = "B...", SourceType = "System", Description = "TSK:/WINDOWS/Prefetch/SPOOLSV3.EXE-282F76A7.pf" },
            new HighLevelArtefactViewModel() { Date = new DateOnly(2007, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "LOG", Short = "C:\\december\\testas12.txt", Reference = 12, Macb = "B..M", SourceType = "System", Description = "" },
            new HighLevelArtefactViewModel() { Date = new DateOnly(2008, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "REG", Short = "{645FF040-5081-101B-9F08-00AA002F954E}", Reference = 14, Extra = "something14: something14", Macb = ".A..", SourceType = "Registry Key: UserAssist", Description = "test14.exe" },
            new HighLevelArtefactViewModel() { Date = new DateOnly(2010, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "REG", Short = "{645FF040-5081-101B-9F08-00AA002F954A}", Reference = 17, Extra = "something17: something17", Macb = ".A..", SourceType = "Registry Key : Run Key", Description = "Something17" },
            new HighLevelArtefactViewModel() { Date = new DateOnly(2010, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "REG", Short = "{645FF040-5081-101B-9F08-00AA002F954B}", Reference = 19, Extra = "something19: something19", Macb = ".A..", SourceType = "UNKNOWN", Description = "" },
            new HighLevelArtefactViewModel() { Date = new DateOnly(2011, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "META", Short = "testas.docx", Reference = 21, Extra = "number_of_paragraphs: 3 total_time: 1", Macb = "B...", SourceType = "System", Description = "Full Description21" },
            new HighLevelArtefactViewModel() { Date = new DateOnly(2012, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "PE", Short = "test.txt", Reference = 22, Macb = "B...", SourceType = "PE Compilation time", Description = "Something22" },
            new HighLevelArtefactViewModel() { Date = new DateOnly(2013, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "PE", Short = "test.exe", Reference = 24, Macb = "B...", SourceType = "PE Compilation time", Description = "Something24" },
            new HighLevelArtefactViewModel() { Date = new DateOnly(2014, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "META", Short = "testas.docx", Reference = 26, Extra = "number_of_paragraphs: 3 total_time: 1", Macb = "B...", SourceType = "System", Description = "Full Description26" },
            new HighLevelArtefactViewModel() { Date = new DateOnly(2015, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "OLECF", Short = "TSK:/WINDOWS/system32/wmimgmt.msc", Reference = 30, Extra = "Name: data", Macb = "B...", SourceType = "OLECF Item", Description = "Something something something something something something something something " },
            new HighLevelArtefactViewModel() { Date = new DateOnly(2016, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "LNK", Short = "C:\\Program Files\\Mozilla Firefox\\firefox.exe", Reference = 31, Macb = "B..M", SourceType = "Windows Shortcut" },
            new HighLevelArtefactViewModel() { Date = new DateOnly(2017, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "FILE", Short = "testas34.txt", Reference = 34, Extra = "something34: somehing34", Macb = "B...", SourceType = "NTFS Creation Time", Description = "C:\\Users\\User\\testas34.txt Type: file" },
            new HighLevelArtefactViewModel() { Date = new DateOnly(2018, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "FILE", Short = "testas37.txt", Reference = 37, Extra = "something37: somehing37", Macb = "B...", SourceType = "NTFS Creation Time", Description = "C:\\Users\\User\\testas37.txt Type: file" }
        };
    }

    [TestMethod]
    public void FormHighLevelArtefacts_ShouldReturnHighLevelArtefacts_WhenMethodIsCalled()
    {
        // Arrange
        List<HighLevelArtefactViewModel> expected = GetExpectedEvents();

        // Act
        List<HighLevelArtefactViewModel> actual = _abstractor.FormHighLevelArtefacts(_events).Cast<HighLevelArtefactViewModel>().ToList();

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
            Assert.AreEqual(expected[i].Extra, actual[i].Extra);
            Assert.AreEqual(expected[i].Macb, actual[i].Macb);
            Assert.AreEqual(expected[i].SourceType, actual[i].SourceType);
            Assert.AreEqual(expected[i].Description, actual[i].Description);
        }
    }

    [TestMethod]
    public void FormHighLevelArtefacts_ShouldReturnFirstOlecfSourceEvent_WhenMethodIsCalled()
    {
        // Arrange
        List<EventViewModel> events = new()
        {
            new EventViewModel(new EventModel(new DateOnly(2015, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B..M", "OLECF", "OLECF Item", "Creation Time", "User", "Host", "Name: view", "Something something0", 2, "TSK:/WINDOWS/system32/wmimgmt.msc", "13451", "-", "olecf/olecf_default", new Dictionary<string, string>(), 0)),
            new EventViewModel(new EventModel(new DateOnly(2015, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "OLECF", "OLECF Item", "Creation Time", "User", "Host", "Name: data", "Something1", 2, "TSK:/WINDOWS/system32/wmimgmt.msc", "13452", "-", "olecf/olecf_default", new Dictionary<string, string>(), 1))
        };
        List<HighLevelArtefactViewModel> expected = new()
        {
            new HighLevelArtefactViewModel() { Date = new DateOnly(2015, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "OLECF", Short = "TSK:/WINDOWS/system32/wmimgmt.msc", Reference = 1, Extra = "Name: data", Macb = "B...", SourceType = "OLECF Item", Description = "Something1" }
        };

        // Act
        List<HighLevelArtefactViewModel> actual = _abstractor.FormHighLevelArtefacts(events).Cast<HighLevelArtefactViewModel>().ToList();

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
            Assert.AreEqual(expected[i].Extra, actual[i].Extra);
            Assert.AreEqual(expected[i].Macb, actual[i].Macb);
            Assert.AreEqual(expected[i].SourceType, actual[i].SourceType);
            Assert.AreEqual(expected[i].Description, actual[i].Description);
        }
    }

    [TestMethod]
    public void FormHighLevelArtefacts_ShouldReturnOnlyLogSourceEvent_WhenMethodIsCalled()
    {
        // Arrange
        List<EventViewModel> events = new()
        {
            new EventViewModel(new EventModel(new DateOnly(2003, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "EVT", "PE Compilation time", "Creation Time", "User", "Host", "pe_type", "Something something", 2, "test.exe", "13452", "-", "pe", new Dictionary<string, string>(), 0)),
            new EventViewModel(new EventModel(new DateOnly(2003, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "LOG", "System", "Creation Time", "User", "Host", "SPOOLSV.EXE was run 1 time(s)", "Prefetch [SPOOLSV0.EXE] was executed - run count 1 path: \\WINDOWS\\SYSTEM32\\SPOOLSV0.EXE hash: 0x282F76A7 volume: 1 [serial number: 0x6C91EF9F  device path: \\DEVICE\\HARDDISKVOLUME-0]", 2, "TSK:/WINDOWS/Prefetch/SPOOLSV-0.EXE-282F76A7.pf", "13452", "-", "prefetch", new Dictionary<string, string>(), 1))
        };

        _lowLevelEventsAbstractorUtils.Setup(u => u.GetShort(events[1].Description)).Returns("Prefetch [SPOOLSV0.EXE] was executed - run count 1 path: \\WINDOWS\\SYSTEM32\\SPOOLSV0.EXE hash: 0x282F76A7 volume: 1 [serial number: 0x6C91EF9F  device path: \\DEVICE\\HARDDISKVOLUME-0]");
        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetDescriptionFromLogSource(events[1])).Returns("TSK:/WINDOWS/Prefetch/SPOOLSV-0.EXE-282F76A7.pf");

        List<HighLevelArtefactViewModel> expected = new()
        {
            new HighLevelArtefactViewModel() { Date = new DateOnly(2003, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "LOG", Short = "Prefetch [SPOOLSV0.EXE] was executed - run count 1 path: \\WINDOWS\\SYSTEM32\\SPOOLSV0.EXE hash: 0x282F76A7 volume: 1 [serial number: 0x6C91EF9F  device path: \\DEVICE\\HARDDISKVOLUME-0]", Reference = 1, Macb = "B...", SourceType = "System", Description = "TSK:/WINDOWS/Prefetch/SPOOLSV-0.EXE-282F76A7.pf" }
        };

        // Act
        List<HighLevelArtefactViewModel> actual = _abstractor.FormHighLevelArtefacts(events).Cast<HighLevelArtefactViewModel>().ToList();

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
            Assert.AreEqual(expected[i].Extra, actual[i].Extra);
            Assert.AreEqual(expected[i].Macb, actual[i].Macb);
            Assert.AreEqual(expected[i].SourceType, actual[i].SourceType);
            Assert.AreEqual(expected[i].Description, actual[i].Description);
        }
    }

    [TestMethod]
    public void FormHighLevelArtefacts_ShouldReturnOnlyPeSourceEvent_WhenMethodIsCalled()
    {
        // Arrange
        List<EventViewModel> events = new()
        {
            new EventViewModel(new EventModel(new DateOnly(2003, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "FILE", "NTFS Creation Time", "Creation Time", "User", "Host", "C:\\Users\\User\\testas-0.txt", "C:\\Users\\User\\testas-0.txt Type: file", 2, "TSK:\\Users\\User\\testas-0.txt", "13452", "-", "filestat", new Dictionary<string, string>() { { "something-0", "something-0" } }, 0)),
            new EventViewModel(new EventModel(new DateOnly(2003, 1, 1), new TimeOnly(12, 0, 0), TimeZoneInfo.Utc, "B...", "PE", "PE Compilation time", "Creation Time", "User", "Host", "pe_type", "Something-1", 2, "test.txt", "13452", "-", "pe", new Dictionary<string, string>(), 1))
        };

        _highLevelArtefactsAbstractorUtils.Setup(u => u.GetFilename(events[0].Description)).Returns("testas-0.txt");
        _lowLevelEventsAbstractorUtils.Setup(u => u.GetExtraTillSha256(events[0].Extra)).Returns("something-0: somehing-0");

        _highLevelArtefactsAbstractorUtils.Setup(u => u.IsPeLineExecutable(events[1])).Returns(false);
        _highLevelArtefactsAbstractorUtils.Setup(u => u.IsPeLineValid(events[1])).Returns(true);

        List<HighLevelArtefactViewModel> expected = new()
        {
            new HighLevelArtefactViewModel() { Date = new DateOnly(2003, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "FILE", Short = "testas-0.txt", Reference = 0, Extra = "something-0: somehing-0", Macb = "B...", SourceType = "NTFS Creation Time", Description = "C:\\Users\\User\\testas-0.txt Type: file" },
            new HighLevelArtefactViewModel() { Date = new DateOnly(2003, 1, 1), Time = new TimeOnly(12, 0, 0), Source = "PE", Short = "test.txt", Reference = 1, Macb = "B...", SourceType = "PE Compilation time", Description = "Something-1" }
        };

        // Act
        List<HighLevelArtefactViewModel> actual = _abstractor.FormHighLevelArtefacts(events).Cast<HighLevelArtefactViewModel>().ToList();

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
            Assert.AreEqual(expected[i].Extra, actual[i].Extra);
            Assert.AreEqual(expected[i].Macb, actual[i].Macb);
            Assert.AreEqual(expected[i].SourceType, actual[i].SourceType);
            Assert.AreEqual(expected[i].Description, actual[i].Description);
        }
    }
}
