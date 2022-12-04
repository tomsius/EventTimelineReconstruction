using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Tests.UnitTests.Utils;

[TestClass]
public class HighLevelArtefactsAbstractorUtilsTests
{
    private static IEnumerable<object[]> LogDescription
    {
        get
        {
            return new[]
            {
                new object[]
                {
                    new EventViewModel(new EventModel(new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "LOG", "SourceType", "Type", "Username", "Hostname", "Short Description", "Entry: number Pin status: unpinned Path: Something/Test.txt", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }, "1")),
                    "Something/Test.txt"
                },
                new object[]
                {
                    new EventViewModel(new EventModel(new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "LOG", "SourceType", "Type", "Username", "Hostname", "Short Description", "Something something", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }, "1")),
                    ""
                }
            };
        }
    }
    private static IEnumerable<object[]> WebhistExtra
    {
        get
        {
            return new[]
            {
                new object[]
                {
                    new Dictionary<string, string>() { { "page_transition_type", "1" }, { "schema_match", "False" }, { "sha256_hash", "a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741" }, { "visit_type", "5" } },
                    "page_transition_type: 1"
                },
                new object[]
                {
                    new Dictionary<string, string>() { { "page_transition_type", "2" }, { "sha256_hash", "a229a3e8240d2ab8a90deabe1600728a8859e6e895a4139824bc1c9862a8b741" }, { "visit_type", "5" } },
                    "page_transition_type: 2"
                }
            };
        }
    }
    private static IEnumerable<object[]> FileEvents
    {
        get
        {
            return new[]
            {
                new object[]
                {
                    new List<EventViewModel>() 
                    {
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "1")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "2")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "3")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "4")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "5")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "6"))
                    },
                    0,
                    6
                },
                new object[]
                {
                    new List<EventViewModel>()
                    {
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "1")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "2")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "3")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "4")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "5")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "REG", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "6"))
                    },
                    0,
                    5
                },
                new object[]
                {
                    new List<EventViewModel>()
                    {
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "1")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "REG", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "2")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "REG", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "3")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "REG", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "4")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "REG", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "5")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "REG", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "6"))
                    },
                    0,
                    1
                },
                new object[]
                {
                    new List<EventViewModel>()
                    {
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "REG", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "1")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "REG", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "2")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "REG", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "3")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "REG", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "4")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "REG", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "5")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "6"))
                    },
                    5,
                    1
                },
                new object[]
                {
                    new List<EventViewModel>()
                    {
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "REG", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "1")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "REG", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "2")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "REG", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "3")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "REG", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "4")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "5")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "6"))
                    },
                    4,
                    2
                },
                new object[]
                {
                    new List<EventViewModel>()
                    {
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "REG", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "1")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "REG", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "2")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "REG", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "3")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "REG", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "4")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "REG", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "5")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "REG", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "6"))
                    },
                    0,
                    0
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
                    new List<EventViewModel>()
                    {
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Filename.exe", 2.5, "Filename.exe", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "1")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Filename.exe", 2.5, "Filename.exe", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "2")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Filename.exe", 2.5, "Filename.exe", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "3")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Filename.exe", 2.5, "Filename.exe", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "4")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Filename.exe", 2.5, "Filename.exe", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "5")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Filename.exe", 2.5, "Filename.exe", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "6"))
                    },
                    0,
                    5,
                    new List<int> { 0, 1, 2, 3, 4, 5 }
                },
                new object[]
                {
                    new List<EventViewModel>()
                    {
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "1")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Filename.exe", 2.5, "Filename.exe", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "2")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Filename.exe", 2.5, "Filename.exe", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "3")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Filename.exe", 2.5, "Filename.exe", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "4")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Filename.exe", 2.5, "Filename.exe", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "5")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Filename.exe", 2.5, "Filename.exe", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "6"))
                    },
                    0,
                    5,
                    new List<int> { 0, 1, 2, 3, 4, 5 }
                },
                new object[]
                {
                    new List<EventViewModel>()
                    {
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Filename.exe", 2.5, "Filename.exe", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "1")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Filename.exe", 2.5, "Filename.exe", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "2")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Filename.exe", 2.5, "Filename.exe", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "3")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Filename.exe", 2.5, "Filename.exe", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "4")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Filename.exe", 2.5, "Filename.exe", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "5")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Filename.exe", 2.5, "Filename.exe", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "6"))
                    },
                    3,
                    5,
                    new List<int> { 3, 4, 5 }
                },
                new object[]
                {
                    new List<EventViewModel>()
                    {
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Filename.exe", 2.5, "Filename.exe", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "1")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Filename.exe", 2.5, "Filename.exe", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "2")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Filename.exe", 2.5, "Filename.exe", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "3")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "4")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Filename.exe", 2.5, "Filename.exe", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "5")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Filename.exe", 2.5, "Filename.exe", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "6"))
                    },
                    3,
                    5,
                    new List<int> { 3, 4, 5 }
                },
                new object[]
                {
                    new List<EventViewModel>()
                    {
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Filename.exe", 2.5, "Filename.exe", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "1")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Filename.exe", 2.5, "Filename.exe", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "2")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "3")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "4")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Filename.exe", 2.5, "Filename.exe", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "5")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "6"))
                    },
                    0,
                    5,
                    new List<int> { 0, 1, 4 }
                },
                new object[]
                {
                    new List<EventViewModel>()
                    {
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "1")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "2")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "3")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "4")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "5")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "6"))
                    },
                    0,
                    6,
                    new List<int> { 0 }
                }
            };
        }
    }
    private static IEnumerable<object[]> DuplicateEvents
    {
        get
        {
            return new[]
            {
                new object[]
                {
                    new List<EventViewModel>()
                    {
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52, 0), TimeZoneInfo.Local, "MACB", "LNK", "Source Type", "Type", "Username", "Hostname", "Short Description", "Filename.lnk", 2.5, "Filename.lnk", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "1")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52, 0), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Something/Filename.lnk", 2.5, "Filename.lnk", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "2")),
                    },
                    0
                },
                new object[]
                {
                    new List<EventViewModel>()
                    {
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52, 10), TimeZoneInfo.Local, "MACB", "LNK", "Source Type", "Type", "Username", "Hostname", "Short Description", "Filename.lnk", 2.5, "Filename.lnk", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "1")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52, 12), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Something/Filename.lnk", 2.5, "Filename.lnk", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "2")),
                    },
                    0
                }
            };
        }
    }
    private static IEnumerable<object[]> NotDuplicateEvents
    {
        get
        {
            return new[]
            {
                new object[]
                {
                    new List<EventViewModel>()
                    {
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "LNK", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename.lnk", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "1")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Something/Filename.exe", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "2")),
                    },
                    0
                },
                new object[]
                {
                    new List<EventViewModel>()
                    {
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "REG", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename.lnk", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "1")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Something/Filename.lnk", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "2")),
                    },
                    0
                },
                new object[]
                {
                    new List<EventViewModel>()
                    {
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52, 10), TimeZoneInfo.Local, "MACB", "LNK", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename.lnk", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "1")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52, 13), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Something/Filename.lnk", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "2")),
                    },
                    0
                },
                new object[]
                {
                    new List<EventViewModel>()
                    {
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "LNK", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename1.lnk", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "1")),
                        new EventViewModel(new EventModel( new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "FILE", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Something/Filename2.lnk", "iNode number", "Notes", "Format", new Dictionary<string, string>(), "2")),
                    },
                    0
                }
            };
        }
    }

    [TestMethod]
    public void GetDescriptionFromLogSource_ShouldReturnFilename_WhenFormatIsLnk()
    {
        // Arrange
        string expected = "Test.txt";
        EventViewModel eventViewModel = new(new EventModel(new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "LOG", "SourceType", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, expected, "iNode number", "Notes", "lnk", new Dictionary<string, string>(), "1"));
        HighLevelArtefactsAbstractorUtils utils = new();

        // Act
        string actual = utils.GetDescriptionFromLogSource(eventViewModel);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DynamicData(nameof(LogDescription))]
    public void GetDescriptionFromLogSource_ShouldReturnFilenameFromDescription_WhenFormatIsNotLnk(EventViewModel eventViewModel, string expected)
    {
        // Arrange
        HighLevelArtefactsAbstractorUtils utils = new();

        // Act
        string actual = utils.GetDescriptionFromLogSource(eventViewModel);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void GetDescriptionFromMetaSource_ShouldReturnFormatteddDescription_WhenMethodIsCalled()
    {
        // Arrange
        string description = "Creating App: Microsoft Office Word App version: 16.0000 Last saved by: Anonymous Author: ssssss Revision number: 130 Template: Normal.dotm Number of words: 806 Number of characters: 4597 Number of characters with spaces: 5393 Hyperlinks changed: false Links up to date: false Scale crop: false Number of lines: 38";
        string expected = "Author: ssssss Last saved by: Anonymous Number of words: 806 Number of characters: 4597 Number of characters with spaces: 5393 Number of lines: 38";
        HighLevelArtefactsAbstractorUtils utils = new();

        // Act
        string actual = utils.GetDescriptionFromMetaSource(description);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [DataTestMethod]
    [DataRow("Registry Key: UserAssist", "[HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\UserAssist\\{CEBFF5CD-ACE2-4F4F-9178-9926F41749EA}\\Count] UserAssist entry: 23 Value name: {D65231B0-B2F1-4857-A4CE-A8E7C6EA7D27}\\NOTEPAD.EXE Count: 2 Application focus count: 2 Application focus duration: 173333", "NOTEPAD.EXE")]
    [DataRow("Registry Key: UserAssist", "[HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\UserAssist\\{CEBFF5CD-ACE2-4F4F-9178-9926F41749EA}\\Count] UserAssist entry: 23 Value name: NOTEPAD.EXE Count: 2 Application focus count: 2 Application focus duration: 173333", "NOTEPAD.EXE")]
    [DataRow("Registry Key : MRU List", "[HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.doc\\OpenWithList] Index: 1 [MRU Value b]: WORDPAD.EXE Index: 2 [MRU Value a]: firefox.exe", "WORDPAD.EXE; firefox.exe")]
    [DataRow("Registry Key : MRUListEx", "[HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\RecentDocs] Index: 1 [MRU Value 1]: Path: Downloads  Shell item: [Downloads.lnk] Index: 2 [MRU Value 6]: Path: BetterTogetherWinXPProOffice03Pro.doc  Shell item: [BetterTogetherWinXPProOffice03Pro.lnk] Index: 3 [MRU Value 5]: Path: very secret.txt  Shell item: [very secret.lnk] Index: 4 [MRU Value 4]: Path: secret.txt  Shell item: [secret.lnk] Index: 5 [MRU Value 3]: Path: Timeline2GUI A Log2Timeline CSV parser and training scenarios.pdf  Shell item: [Timeline2GUI A Log2Timeline CSV parser and training scenarios.lnk] Index: 6 [MRU Value 2]: Path: Timeline2GUI A Log2Timeline CSV parser and training scenarios(2).pdf  Shell item: [Timeline2GUI A Log2Timeline CSV parser and training scenarios(2).lnk] Index: 7 [MRU Value 0]: Path: Timeline2GUI A Log2Timeline CSV parser and training scenarios(1).pdf  Shell item: [Timeline2GUI A Log2Timeline CSV parser and training scenarios(1).lnk]", "Downloads; BetterTogetherWinXPProOffice03Pro.doc; very secret.txt; secret.txt; Timeline2GUI A Log2Timeline CSV parser and training scenarios.pdf; Timeline2GUI A Log2Timeline CSV parser and training scenarios(2).pdf; Timeline2GUI A Log2Timeline CSV parser and training scenarios(1).pdf")]
    [DataRow("Registry Key : MRUListEx", "[HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\RecentDocs\\.txt] Index: 1 [MRU Value 1]: very secret.txt Index: 2 [MRU Value 0]: secret.txt", "very secret.txt; secret.txt")]
    [DataRow("Registry Key : MRUListEx", "[HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\RecentDocs] Index: 1 [MRU Value 1]: Path: Downloads  Shell item: [Downloads.lnk] Index: 2 [MRU Value 6]: Path: BetterTogetherWinXPProOffice03Pro.doc  Shell item: [BetterTogetherWinXPProOffice03Pro.lnk] Index: 3 [MRU Value 5]: Path: very secret.txt  Shell item: [very secret.lnk] Index: 4 [MRU Value 4]: Path: secret.txt  Shell item: [secret.lnk] Index: 5 [MRU Value 3]: Path: Timeline2GUI A Log2Timeline CSV parser and training scenarios.pdf  Shell item: [Timeline2GUI A Log2Timeline CSV parser and training scenarios.lnk] Index: 6 [MRU Value 2]: Path: Timeline2GUI A Log2Timeline CSV parser and training scenarios(2).pdf  Shell item: [Timeline2GUI A Log2Timeline CSV parser and training scenarios(2).lnk] Index: 7 [MRU Value 0]: Path: Timeline2GUI A Log2Timeline CSV parser and training scenarios(1).pdf", "Downloads; BetterTogetherWinXPProOffice03Pro.doc; very secret.txt; secret.txt; Timeline2GUI A Log2Timeline CSV parser and training scenarios.pdf; Timeline2GUI A Log2Timeline CSV parser and training scenarios(2).pdf; Timeline2GUI A Log2Timeline CSV parser and training scenarios(1).pdf")]
    [DataRow("Registry Key : Typed URLs", "[HKEY_CURRENT_USER\\Software\\Microsoft\\Internet Explorer\\TypedURLs] url1: http://auto.search.msn.com/response.asp?MT=google+mail&srch=5&prov=&utf8 url2: http://mail.google.com/ url3: http://google.com/ url4: http://reddit.com/ url5: http://www.microsoft.com/isapi/redir.dll?prd=ie&pver=6&ar=msnhome", "http://auto.search.msn.com/response.asp?MT=google+mail&srch=5&prov=&utf8 http://mail.google.com/ http://google.com/ http://reddit.com/ http://www.microsoft.com/isapi/redir.dll?prd=ie&pver=6&ar=msnhome")]
    [DataRow("Registry Key : BagMRU", "[HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\ShellNoRoam\\BagMRU\\0] Index: 1 [MRU Value 3]: Shell item path: <My Computer> C:\\ Index: 2 [MRU Value 1]: Shell item path: <My Computer> <UNKNOWN: 0x00> Index: 3 [MRU Value 2]: Shell item path: <My Computer> <UNKNOWN: 0x00> Index: 4 [MRU Value 0]: Shell item path: <My Computer> {21ec2020-3aea-1069-a2dd-08002b30309d}", "<My Computer> C:\\; <My Computer> <UNKNOWN: 0x00>; <My Computer> <UNKNOWN: 0x00>; <My Computer> {21ec2020-3aea-1069-a2dd-08002b30309d}")]
    [DataRow("Registry Key : Run Key", "[HKEY_LOCAL_MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Run] VBoxTray: %SystemRoot%\\system32\\VBoxTray.exe", "[HKEY_LOCAL_MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Run] VBoxTray: %SystemRoot%\\system32\\VBoxTray.exe")]
    [DataRow("UNKNOWN", "[HKEY_CURRENT_USER\\Software\\Microsoft\\Office\\15.0\\Word\\Reading Locations\\Document 2] Datetime: [REG_SZ] 2017-11-07T09:41 File Path: [REG_SZ] D:\\My Documents\\Biudzet\\prof\\test.docx Position: [REG_SZ] 0 0, (Vac_lapkrgruC)", "test.docx")]
    [DataRow("UNKNOWN", "[HKEY_CURRENT_USER\\Software\\Microsoft\\Office\\15.0\\Word\\Reading Locations\\Document 2] Datetime: [REG_SZ] 2017-11-07T09:41 File Path: [REG_SZ] test.docx Position: [REG_SZ] 0 0, (Vac_lapkrgruC)", "test.docx")]
    [DataRow("UNKNOWN", "Something else", "")]
    [DataRow("Something something", "Some description", "")]
    public void GetDescriptionFromRegSource_ShouldReturnFilenameFromDescription_WhenMetodsIsCalled(string sourceType, string description, string expected)
    {
        // Arrange
        HighLevelArtefactsAbstractorUtils utils = new();

        // Act
        string actual = utils.GetDescriptionFromRegSource(sourceType, description);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [DataTestMethod]
    [DataRow("TSK:/WINDOWS/system32/usrshuta.exe Type: file", "usrshuta.exe")]
    [DataRow("usrshuta.exe Type: file", "usrshuta.exe")]
    public void GetFilename_ShouldReturnFilenameFromDescription_WhenFormatIsCalled(string description, string expected)
    {
        // Arrange
        HighLevelArtefactsAbstractorUtils utils = new();

        // Act
        string actual = utils.GetFilename(description);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DynamicData(nameof(WebhistExtra))]
    public void GetOtherBrowserExtraFromWebhistSource_ShouldReturnFormattedExtraValues_WhenMethodIsCalled(Dictionary<string, string> extra, string expected)
    {
        // Arrange
        HighLevelArtefactsAbstractorUtils utils = new();

        // Act
        string actual = utils.GetOtherBrowserExtraFromWebhistSource(extra);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DynamicData(nameof(FileEvents))]
    public void GetFileCountInRowAtSameMinute_ShouldReturnFileEventsCountInARow_WhenMethodIsCalled(List<EventViewModel> events, int startIndex, int expected)
    {
        // Arrange
        HighLevelArtefactsAbstractorUtils utils = new();

        // Act
        int actual = utils.GetFileCountInRowAtSameMinute(events, startIndex);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DynamicData(nameof(ValidFileEvents))]
    public void GetValidFileEventIndices_ShouldReturnIndexList_WhenMethodIsCalled(List<EventViewModel> events, int startIndex, int endIndex, List<int> expected)
    {
        // Arrange
        HighLevelArtefactsAbstractorUtils utils = new();

        // Act
        List<int> actual = utils.GetValidFileEventIndices(events, startIndex, endIndex);

        // Assert
        Assert.AreEqual(expected.Count, actual.Count);

        for (int i = 0; i < expected.Count; i++)
        {
            Assert.AreEqual(expected[i], actual[i]);
        }
    }

    [TestMethod]
    [DynamicData(nameof(DuplicateEvents))]
    public void IsFileDuplicateOfLnk_ShouldReturnTrue_WhenFileEventIsDuplicateOfLnkEvent(List<EventViewModel> events, int startIndex)
    {
        // Arrange
        EventViewModel current = events[^1];
        HighLevelArtefactsAbstractorUtils utils = new();

        // Act
        bool actual = utils.IsFileDuplicateOfLnk(events, startIndex, current);

        // Assert
        Assert.IsTrue(actual);
    }

    [TestMethod]
    [DynamicData(nameof(NotDuplicateEvents))]
    public void IsFileDuplicateOfLnk_ShouldReturnFalse_WhenFileEventIsNotDuplicateOfLnkEvent(List<EventViewModel> events, int startIndex)
    {
        // Arrange
        EventViewModel current = events[^1];
        HighLevelArtefactsAbstractorUtils utils = new();

        // Act
        bool actual = utils.IsFileDuplicateOfLnk(events, startIndex, current);

        // Assert
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void IsPeLineExecutable_ShouldReturnTrue_WhenFileIsExecutable()
    {
        // Arrange
        EventViewModel eventViewModel = new(new EventModel(new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "LOG", "SourceType", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename.exe", "iNode number", "Notes", "lnk", new Dictionary<string, string>(), "1"));
        HighLevelArtefactsAbstractorUtils utils = new();

        // Act
        bool actual = utils.IsPeLineExecutable(eventViewModel);

        // Assert
        Assert.IsTrue(actual);
    }

    [TestMethod]
    public void IsPeLineExecutable_ShouldReturnFalse_WhenFileIsNotExecutable()
    {
        // Arrange
        EventViewModel eventViewModel = new(new EventModel(new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "LOG", "SourceType", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename.txt", "iNode number", "Notes", "lnk", new Dictionary<string, string>(), "1"));
        HighLevelArtefactsAbstractorUtils utils = new();

        // Act
        bool actual = utils.IsPeLineExecutable(eventViewModel);

        // Assert
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void IsPeLineValid_ShouldReturnTrue_WhenShortFieldHasKey()
    {
        // Arrange
        EventViewModel eventViewModel = new(new EventModel(new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "LOG", "SourceType", "Type", "Username", "Hostname", "pe_type", "Full Description", 2.5, "Filename", "iNode number", "Notes", "lnk", new Dictionary<string, string>(), "1"));
        HighLevelArtefactsAbstractorUtils utils = new();

        // Act
        bool actual = utils.IsPeLineValid(eventViewModel);

        // Assert
        Assert.IsTrue(actual);
    }

    [TestMethod]
    public void IsPeLineValid_ShouldReturnFalse_WhenShortFieldDoesNotHaveKey()
    {
        // Arrange
        EventViewModel eventViewModel = new(new EventModel(new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Local, "MACB", "LOG", "SourceType", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "lnk", new Dictionary<string, string>(), "1"));
        HighLevelArtefactsAbstractorUtils utils = new();

        // Act
        bool actual = utils.IsPeLineValid(eventViewModel);

        // Assert
        Assert.IsFalse(actual);
    }
}
