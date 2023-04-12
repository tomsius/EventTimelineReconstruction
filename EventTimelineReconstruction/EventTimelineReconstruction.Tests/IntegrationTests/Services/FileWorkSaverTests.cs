using System;
using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.ViewModels;
using Microsoft.Extensions.Hosting;

namespace EventTimelineReconstruction.Tests.IntegrationTests.Services;

[TestClass]
public class FileWorkSaverTests
{
    private const string _file = @"WorkSaver.csv";

    [TestCleanup]
    public void Cleanup()
    {
        File.Delete(_file);
    }

    [TestMethod]
    public async Task SaveWork_ShouldWriteEventsToFile_WhenMethodIsCalled()
    {
        // Arrange
        List<EventViewModel> events = new()
        {
            new EventViewModel(
                new EventModel(
                    new DateOnly(2020, 4, 6),
                    new TimeOnly(5, 23),
                    TimeZoneInfo.Utc,
                    "MACB1",
                    "Source1",
                    "Source Type1",
                    "Type1",
                    "Username1",
                    "Hostname1",
                    "Short Description1",
                    "Full Description1",
                    2.5,
                    "Filename1",
                    "iNode number1",
                    "Notes1",
                    "Format1",
                    new Dictionary<string, string>() { { "Key11", "Value11" }, { "Key12", "Value12" } },
                    1
                    )),
            new EventViewModel(
                new EventModel(
                    new DateOnly(2022, 10, 14),
                    new TimeOnly(10, 52),
                    TimeZoneInfo.Utc,
                    "MACB2",
                    "Source2",
                    "Source Type2",
                    "Type2",
                    "Username2",
                    "Hostname2",
                    "Short Description2",
                    "Full Description2",
                    2.5,
                    "Filename2",
                    "iNode number2",
                    "Notes2",
                    "Format2",
                    new Dictionary<string, string>() { { "Key21", "Value21" }, { "Key22", "Value22" } },
                    2
                )),
            new EventViewModel(
                new EventModel(
                    new DateOnly(2022, 10, 14),
                    new TimeOnly(10, 52),
                    TimeZoneInfo.Utc,
                    "MACB4",
                    "Source4",
                    "Source Type4",
                    "Type4",
                    "Username4",
                    "Hostname4",
                    "Short Description4",
                    "Full Description4",
                    2.5,
                    "Filename4",
                    "iNode number4",
                    "Notes4",
                    "Format4",
                    new Dictionary<string, string>() { { "Key41", "Value41" }, { "Key42", "Value42" } },
                    4
                )),
            new EventViewModel(
                new EventModel(
                    new DateOnly(2022, 10, 14),
                    new TimeOnly(10, 52),
                    TimeZoneInfo.Utc,
                    "MACB5",
                    "Source5",
                    "Source Type5",
                    "Type5",
                    "Username5",
                    "Hostname5",
                    "Short Description5",
                    "Full Description5",
                    2.5,
                    "Filename5",
                    "iNode number5",
                    "Notes5",
                    "Format5",
                    new Dictionary<string, string>() { { "Key51", "Value51" }, { "Key52", "Value52" } },
                    5
                ))
        };
        EventViewModel child = new(
                new EventModel(
                    new DateOnly(2021, 12, 1),
                    new TimeOnly(17, 53),
                    TimeZoneInfo.Utc,
                    "MACB3",
                    "Source3",
                    "Source Type3",
                    "Type3",
                    "Username3",
                    "Hostname3",
                    "Short Description3",
                    "Full Description3",
                    2.5,
                    "Filename3",
                    "iNode number3",
                    "Notes3",
                    "Format3",
                    new Dictionary<string, string>() { { "Key31", "Value31" }, { "Key32", "Value32" } },
                    3
                    ));
        events[0].AddChild(child);
        List<HighLevelEventViewModel> highLevelEvents = new()
        {
            new HighLevelEventViewModel(new DateOnly(2020, 1, 1), new TimeOnly(4, 20, 54), "Source4", "Short4", "Visit4", 4),
            new HighLevelEventViewModel(new DateOnly(2020, 1, 2), new TimeOnly(4, 21, 4), "Source5", "Short5", "Visit5", 5)
        };
        List<LowLevelEventViewModel> lowLevelEvents = new()
        {
            new LowLevelEventViewModel(new DateOnly(2020, 1, 3), new TimeOnly(4, 22, 54), "Source6", "Short6", "Visit6", "Extra6", 6),
            new LowLevelEventViewModel(new DateOnly(2020, 1, 4), new TimeOnly(4, 23, 4), "Source7", "Short7", "Visit7", "Extra7", 7)
        };
        List<HighLevelArtefactViewModel> highLevelArtefacts = new()
        {
            new HighLevelArtefactViewModel(new DateOnly(2020, 1, 5), new TimeOnly(4, 22, 54), "Source8", "Short8", "Visit8", "Extra8", 8, "MACB8", "SourceType8", "Desc8"),
            new HighLevelArtefactViewModel(new DateOnly(2020, 1, 6), new TimeOnly(4, 23, 4), "Source9", "Short9", "Visit9", "Extra9", 9, "MACB9", "SourceType9", "Desc9")
        };
        List<LowLevelArtefactViewModel> lowLevelArtefacts = new()
        {
            new LowLevelArtefactViewModel(new DateOnly(2020, 1, 7), new TimeOnly(4, 22, 54), "Vilnius", "MACB10", "Source10", "SourceType10", "Type10", "User10", "Host10", "Short10", "Desc10", "2", "Filename10", "Inode10", "Notes10", "Format10", "Extra10", 10)
        };
        string[] expected = new string[]
        {
            "2020,4,6,5,23,0,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,MACB1,Source1,Source Type1,Type1,Username1,Hostname1,Short Description1,Full Description1,2.5,Filename1,iNode number1,Notes1,Format1,Key11:Value11;Key12:Value12,1,True,#FF000000",
            "\t2021,12,1,17,53,0,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,MACB3,Source3,Source Type3,Type3,Username3,Hostname3,Short Description3,Full Description3,2.5,Filename3,iNode number3,Notes3,Format3,Key31:Value31;Key32:Value32,3,True,#FF000000",
            "2022,10,14,10,52,0,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,MACB2,Source2,Source Type2,Type2,Username2,Hostname2,Short Description2,Full Description2,2.5,Filename2,iNode number2,Notes2,Format2,Key21:Value21;Key22:Value22,2,True,#FF000000",
            "2022,10,14,10,52,0,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,MACB4,Source4,Source Type4,Type4,Username4,Hostname4,Short Description4,Full Description4,2.5,Filename4,iNode number4,Notes4,Format4,Key41:Value41;Key42:Value42,4,True,#FF000000",
            "2022,10,14,10,52,0,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,MACB5,Source5,Source Type5,Type5,Username5,Hostname5,Short Description5,Full Description5,2.5,Filename5,iNode number5,Notes5,Format5,Key51:Value51;Key52:Value52,5,True,#FF000000",
            "",
            "2020,1,1,4,20,54,Source4,Short4,Visit4,4",
            "2020,1,2,4,21,4,Source5,Short5,Visit5,5",
            "",
            "2020,1,3,4,22,54,Source6,Short6,Visit6,Extra6,6",
            "2020,1,4,4,23,4,Source7,Short7,Visit7,Extra7,7",
            "",
            "2020,1,5,4,22,54,Source8,Short8,Visit8,Extra8,8,MACB8,SourceType8,Desc8",
            "2020,1,6,4,23,4,Source9,Short9,Visit9,Extra9,9,MACB9,SourceType9,Desc9",
            "",
            "2020,1,7,4,22,54,Vilnius,MACB10,Source10,SourceType10,Type10,User10,Host10,Short10,Desc10,2,Filename10,Inode10,Notes10,Format10,Extra10,10"
        };
        FileWorkSaver saver = new();

        // Act
        await saver.SaveWork(_file, events, highLevelEvents, lowLevelEvents, highLevelArtefacts, lowLevelArtefacts);
        string[] actual = File.ReadAllLines(_file);

        // Assert
        Assert.AreEqual(expected.Length, actual.Length);

        for (int i = 0; i < expected.Length; i++)
        {
            Assert.AreEqual(expected[i], actual[i]);
        }
    }
}
