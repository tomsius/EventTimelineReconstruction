using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.ViewModels;

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
    public void SaveWork_ShouldWriteEventsToFile_WhenMethodIsCalled()
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
                    new Dictionary<string, string>() { { "Key11", "Value11" }, { "Key12", "Value12" } }
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
                    new Dictionary<string, string>() { { "Key21", "Value21" }, { "Key22", "Value22" } }
                ))
        };
        EventViewModel child = new EventViewModel(
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
                    new Dictionary<string, string>() { { "Key31", "Value31" }, { "Key32", "Value32" } }
                    ));
        events[0].AddChild(child);
        string[] expected = new string[]
        {
            "2020,4,6,5,23,0,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,MACB1,Source1,Source Type1,Type1,Username1,Hostname1,Short Description1,Full Description1,2.5,Filename1,iNode number1,Notes1,Format1,Key11:Value11;Key12:Value12,True,#FF000000",
            "\t2021,12,1,17,53,0,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,MACB3,Source3,Source Type3,Type3,Username3,Hostname3,Short Description3,Full Description3,2.5,Filename3,iNode number3,Notes3,Format3,Key31:Value31;Key32:Value32,True,#FF000000",
            "2022,10,14,10,52,0,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,MACB2,Source2,Source Type2,Type2,Username2,Hostname2,Short Description2,Full Description2,2.5,Filename2,iNode number2,Notes2,Format2,Key21:Value21;Key22:Value22,True,#FF000000"
        };
        FileWorkSaver saver = new();

        // Act
        saver.SaveWork(_file, events);
        string[] actual = File.ReadAllLines(_file);

        // Assert
        Assert.AreEqual(expected.Length, actual.Length);

        for (int i = 0; i < expected.Length; i++)
        {
            Assert.AreEqual(expected[i], actual[i]);
        }
    }
}
