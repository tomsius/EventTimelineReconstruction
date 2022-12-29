using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Tests.IntegrationTests.Services;

[TestClass]
public class FileWorkLoaderTests
{
    private const string _file = @"WorkLoader.csv";

    [ClassInitialize]
    public static void Initialize(TestContext context)
    {
        using StreamWriter writeStream = File.CreateText(_file);
        writeStream.WriteLine("2020,4,6,5,23,0,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,MACB1,Source1,Source Type1,Type1,Username1,Hostname1,Short Description1,Full Description1,2.5,Filename1,iNode number1,Notes1,Format1,Key11:Value11;Key12:Value12,1,True,#FF000000");
        writeStream.WriteLine("\t2021,12,1,17,53,0,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,MACB3,Source3,Source Type3,Type3,Username3,Hostname3,Short Description3,Full Description3,2.5,Filename3,iNode number3,Notes3,Format3,Key31:Value31;Key32:Value32,3,True,#FF000000");
        writeStream.WriteLine("\t\t2021,12,1,17,53,0,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,MACB4,Source4,Source Type4,Type4,Username4,Hostname4,Short Description4,Full Description4,2.5,Filename4,iNode number4,Notes4,Format4,Key41:Value41;Key42:Value42,4,True,#FF000000");
        writeStream.WriteLine("\t2021,12,1,17,53,0,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,MACB5,Source5,Source Type5,Type5,Username5,Hostname5,Short Description5,Full Description5,2.5,Filename5,iNode number5,Notes5,Format5,Key51:Value51;Key52:Value52,5,True,#FF000000");
        writeStream.WriteLine("\t2021,12,1,17,53,0,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,MACB6,Source6,Source Type6,Type6,Username6,Hostname6,Short Description6,Full Description6,2.5,Filename6,iNode number6,Notes6,Format6,Key61:Value61;Key62:Value62,6,True,#FF000000");
        writeStream.WriteLine("2022,10,14,10,52,0,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,MACB2,Source2,Source Type2,Type2,Username2,Hostname2,Short Description2,Full Description2,2.5,Filename2,iNode number2,Notes2,Format2,Key21:Value21;Key22:Value22,2,True,#FF000000");
        writeStream.WriteLine();
        writeStream.WriteLine("2020,1,1,4,20,54,Source4,Short4,Visit4,4");
        writeStream.WriteLine("2020,1,2,4,21,4,Source5,Short5,Visit5,5");
        writeStream.WriteLine();
        writeStream.WriteLine("2020,1,3,4,22,54,Source6,Short6,Visit6,Extra6,6");
        writeStream.WriteLine("2020,1,4,4,23,4,Source7,Short7,Visit7,Extra7,7");
        writeStream.WriteLine();
        writeStream.WriteLine("2020,1,5,4,22,54,Source8,Short8,Visit8,Extra8,8,MACB8,SourceType8,Desc8");
        writeStream.WriteLine("2020,1,6,4,23,4,Source9,Short9,Visit9,Extra9,9,MACB9,SourceType9,Desc9");
        writeStream.WriteLine();
        writeStream.WriteLine("2020,1,7,4,22,54,Vilnius,MACB10,Source10,SourceType10,Type10,User10,Host10,Short10,Desc10,2,Filename10,Inode10,Notes10,Format10,Extra10,10");
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        File.Delete(_file);
    }

    [TestMethod]
    public void LoadWork_ShouldLoadAllWork_WhenMethodIsCalled()
    {
        // Arrange
        List<EventViewModel> expectedEvents = new()
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
                    "1"
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
                    "2"
                ))
        };
        EventViewModel firstChild = new(
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
                    "3"
                    ));
        EventViewModel secondChild = new(
                new EventModel(
                    new DateOnly(2021, 12, 1),
                    new TimeOnly(17, 53),
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
                    "4"
                    ));
        EventViewModel thirdChild = new(
                new EventModel(
                    new DateOnly(2021, 12, 1),
                    new TimeOnly(17, 53),
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
                    "5"
                    ));
        EventViewModel fourthChild = new(
                new EventModel(
                    new DateOnly(2021, 12, 1),
                    new TimeOnly(17, 53),
                    TimeZoneInfo.Utc,
                    "MACB6",
                    "Source6",
                    "Source Type6",
                    "Type6",
                    "Username6",
                    "Hostname6",
                    "Short Description6",
                    "Full Description6",
                    2.5,
                    "Filename6",
                    "iNode number6",
                    "Notes6",
                    "Format6",
                    new Dictionary<string, string>() { { "Key61", "Value61" }, { "Key62", "Value62" } },
                    "6"
                    ));
        expectedEvents[0].AddChild(firstChild);
        expectedEvents[0].Children[0].AddChild(secondChild);
        expectedEvents[0].AddChild(thirdChild);
        expectedEvents[0].AddChild(fourthChild);
        List<HighLevelEventViewModel> expectedHighLevelEvents = new()
        {
            new HighLevelEventViewModel(new DateOnly(2020, 1, 1), new TimeOnly(4, 20, 54), "Source4", "Short4", "Visit4", "4"),
            new HighLevelEventViewModel(new DateOnly(2020, 1, 2), new TimeOnly(4, 21, 4), "Source5", "Short5", "Visit5", "5")
        };
        List<LowLevelEventViewModel> expectedLowLevelEvents = new()
        {
            new LowLevelEventViewModel(new DateOnly(2020, 1, 3), new TimeOnly(4, 22, 54), "Source6", "Short6", "Visit6", "Extra6", "6"),
            new LowLevelEventViewModel(new DateOnly(2020, 1, 4), new TimeOnly(4, 23, 4), "Source7", "Short7", "Visit7", "Extra7", "7")
        };
        List<HighLevelArtefactViewModel> expectedHighLevelArtefacts = new()
        {
            new HighLevelArtefactViewModel(new DateOnly(2020, 1, 5), new TimeOnly(4, 22, 54), "Source8", "Short8", "Visit8", "Extra8", "8", "MACB8", "SourceType8", "Desc8"),
            new HighLevelArtefactViewModel(new DateOnly(2020, 1, 6), new TimeOnly(4, 23, 4), "Source9", "Short9", "Visit9", "Extra9", "9", "MACB9", "SourceType9", "Desc9")
        };
        List<LowLevelArtefactViewModel> expectedLowLevelArtefacts = new()
        {
            new LowLevelArtefactViewModel(new DateOnly(2020, 1, 7), new TimeOnly(4, 22, 54), "Vilnius", "MACB10", "Source10", "SourceType10", "Type10", "User10", "Host10", "Short10", "Desc10", "2", "Filename10", "Inode10", "Notes10", "Format10", "Extra10", "10")
        };
        FileWorkLoader loader = new();

        // Act
        LoadedWork loadedWork = loader.LoadWork(_file).Result;
        List<EventViewModel> actualEvents = loadedWork.Events;
        List<HighLevelEventViewModel> actualHighLevelEvents = loadedWork.HighLevelEvents;
        List<LowLevelEventViewModel> actualLowLevelEvents = loadedWork.LowLevelEvents;
        List<HighLevelArtefactViewModel> actualHighLevelArtefacts = loadedWork.HighLevelArtefacts;
        List<LowLevelArtefactViewModel> actualLowLevelArtefacts = loadedWork.LowLevelArtefacts;

        // Assert
        Assert.AreEqual(expectedEvents.Count, actualEvents.Count);

        Queue<EventViewModel> expectedQueue = new();
        Queue<EventViewModel> actualQueue = new();
        for (int i = 0; i < expectedEvents.Count; i++)
        {
            expectedQueue.Enqueue(expectedEvents[i]);
            actualQueue.Enqueue(actualEvents[i]);
        }

        while (expectedQueue.Count > 0)
        {
            EventViewModel currentExpected = expectedQueue.Dequeue();
            EventViewModel currentActual = actualQueue.Dequeue();

            Assert.AreEqual(currentExpected.Children.Count, currentActual.Children.Count);
            Assert.AreEqual(currentExpected.FullDate, currentActual.FullDate);
            Assert.AreEqual(currentExpected.Timezone, currentActual.Timezone);
            Assert.AreEqual(currentExpected.MACB, currentActual.MACB);
            Assert.AreEqual(currentExpected.Source, currentActual.Source);
            Assert.AreEqual(currentExpected.SourceType, currentActual.SourceType);
            Assert.AreEqual(currentExpected.Type, currentActual.Type);
            Assert.AreEqual(currentExpected.User, currentActual.User);
            Assert.AreEqual(currentExpected.Host, currentActual.Host);
            Assert.AreEqual(currentExpected.Short, currentActual.Short);
            Assert.AreEqual(currentExpected.Description, currentActual.Description);
            Assert.AreEqual(currentExpected.Version, currentActual.Version);
            Assert.AreEqual(currentExpected.Filename, currentActual.Filename);
            Assert.AreEqual(currentExpected.INode, currentActual.INode);
            Assert.AreEqual(currentExpected.Notes, currentActual.Notes);
            Assert.AreEqual(currentExpected.Format, currentActual.Format);
            Assert.AreEqual(currentExpected.Extra.Count, currentActual.Extra.Count);

            foreach (KeyValuePair<string, string> kvp in currentExpected.Extra)
            {
                string expectedKey = kvp.Key;
                string expectedValue = kvp.Value;

                Assert.IsTrue(currentActual.Extra.ContainsKey(expectedKey));
                Assert.AreEqual(expectedValue, currentActual.Extra[expectedKey]);
            }

            Assert.AreEqual(currentExpected.IsVisible, currentActual.IsVisible);
            Assert.AreEqual(currentExpected.Colour.ToString(), currentActual.Colour.ToString());
            Assert.AreEqual(currentExpected.SourceLine, currentActual.SourceLine);

            for (int i = 0; i < currentExpected.Children.Count; i++)
            {
                expectedQueue.Enqueue(currentExpected.Children[i]);
                actualQueue.Enqueue(currentActual.Children[i]);
            }
        }

        Assert.AreEqual(expectedHighLevelEvents.Count, actualHighLevelEvents.Count);

        for (int i = 0; i < expectedHighLevelEvents.Count; i++)
        {
            Assert.AreEqual(expectedHighLevelEvents[i].Date, actualHighLevelEvents[i].Date);
            Assert.AreEqual(expectedHighLevelEvents[i].Time, actualHighLevelEvents[i].Time);
            Assert.AreEqual(expectedHighLevelEvents[i].Short, actualHighLevelEvents[i].Short);
            Assert.AreEqual(expectedHighLevelEvents[i].Source, actualHighLevelEvents[i].Source);
            Assert.AreEqual(expectedHighLevelEvents[i].Visit, actualHighLevelEvents[i].Visit);
            Assert.AreEqual(expectedHighLevelEvents[i].Reference, actualHighLevelEvents[i].Reference);
        }

        Assert.AreEqual(expectedLowLevelEvents.Count, actualLowLevelEvents.Count);

        for (int i = 0; i < expectedLowLevelEvents.Count; i++)
        {
            Assert.AreEqual(expectedLowLevelEvents[i].Date, actualLowLevelEvents[i].Date);
            Assert.AreEqual(expectedLowLevelEvents[i].Time, actualLowLevelEvents[i].Time);
            Assert.AreEqual(expectedLowLevelEvents[i].Short, actualLowLevelEvents[i].Short);
            Assert.AreEqual(expectedLowLevelEvents[i].Source, actualLowLevelEvents[i].Source);
            Assert.AreEqual(expectedLowLevelEvents[i].Visit, actualLowLevelEvents[i].Visit);
            Assert.AreEqual(expectedLowLevelEvents[i].Extra, actualLowLevelEvents[i].Extra);
            Assert.AreEqual(expectedLowLevelEvents[i].Reference, actualLowLevelEvents[i].Reference);
        }

        Assert.AreEqual(expectedHighLevelArtefacts.Count, actualHighLevelArtefacts.Count);

        for (int i = 0; i < expectedHighLevelArtefacts.Count; i++)
        {
            Assert.AreEqual(expectedHighLevelArtefacts[i].Date, actualHighLevelArtefacts[i].Date);
            Assert.AreEqual(expectedHighLevelArtefacts[i].Time, actualHighLevelArtefacts[i].Time);
            Assert.AreEqual(expectedHighLevelArtefacts[i].Short, actualHighLevelArtefacts[i].Short);
            Assert.AreEqual(expectedHighLevelArtefacts[i].Source, actualHighLevelArtefacts[i].Source);
            Assert.AreEqual(expectedHighLevelArtefacts[i].Visit, actualHighLevelArtefacts[i].Visit);
            Assert.AreEqual(expectedHighLevelArtefacts[i].Extra, actualHighLevelArtefacts[i].Extra);
            Assert.AreEqual(expectedHighLevelArtefacts[i].Reference, actualHighLevelArtefacts[i].Reference);
            Assert.AreEqual(expectedHighLevelArtefacts[i].Macb, actualHighLevelArtefacts[i].Macb);
            Assert.AreEqual(expectedHighLevelArtefacts[i].SourceType, actualHighLevelArtefacts[i].SourceType);
            Assert.AreEqual(expectedHighLevelArtefacts[i].Description, actualHighLevelArtefacts[i].Description);
        }

        Assert.AreEqual(expectedLowLevelArtefacts.Count, actualLowLevelArtefacts.Count);

        for (int i = 0; i < expectedLowLevelArtefacts.Count; i++)
        {
            Assert.AreEqual(expectedLowLevelArtefacts[i].Date, actualLowLevelArtefacts[i].Date);
            Assert.AreEqual(expectedLowLevelArtefacts[i].Time, actualLowLevelArtefacts[i].Time);
            Assert.AreEqual(expectedLowLevelArtefacts[i].Timezone, actualLowLevelArtefacts[i].Timezone);
            Assert.AreEqual(expectedLowLevelArtefacts[i].Macb, actualLowLevelArtefacts[i].Macb);
            Assert.AreEqual(expectedLowLevelArtefacts[i].Source, actualLowLevelArtefacts[i].Source);
            Assert.AreEqual(expectedLowLevelArtefacts[i].SourceType, actualLowLevelArtefacts[i].SourceType);
            Assert.AreEqual(expectedLowLevelArtefacts[i].Type, actualLowLevelArtefacts[i].Type);
            Assert.AreEqual(expectedLowLevelArtefacts[i].User, actualLowLevelArtefacts[i].User);
            Assert.AreEqual(expectedLowLevelArtefacts[i].Host, actualLowLevelArtefacts[i].Host);
            Assert.AreEqual(expectedLowLevelArtefacts[i].Short, actualLowLevelArtefacts[i].Short);
            Assert.AreEqual(expectedLowLevelArtefacts[i].Description, actualLowLevelArtefacts[i].Description);
            Assert.AreEqual(expectedLowLevelArtefacts[i].Version, actualLowLevelArtefacts[i].Version);
            Assert.AreEqual(expectedLowLevelArtefacts[i].Filename, actualLowLevelArtefacts[i].Filename);
            Assert.AreEqual(expectedLowLevelArtefacts[i].Inode, actualLowLevelArtefacts[i].Inode);
            Assert.AreEqual(expectedLowLevelArtefacts[i].Notes, actualLowLevelArtefacts[i].Notes);
            Assert.AreEqual(expectedLowLevelArtefacts[i].Format, actualLowLevelArtefacts[i].Format);
            Assert.AreEqual(expectedLowLevelArtefacts[i].Extra, actualLowLevelArtefacts[i].Extra);
            Assert.AreEqual(expectedLowLevelArtefacts[i].Reference, actualLowLevelArtefacts[i].Reference);
        }
    }
}
