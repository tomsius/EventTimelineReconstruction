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
        writeStream.WriteLine("2020,4,6,5,23,0,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,MACB1,Source1,Source Type1,Type1,Username1,Hostname1,Short Description1,Full Description1,2.5,Filename1,iNode number1,Notes1,Format1,Key11:Value11;Key12:Value12,True,#FF000000");
        writeStream.WriteLine("\t2021,12,1,17,53,0,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,MACB3,Source3,Source Type3,Type3,Username3,Hostname3,Short Description3,Full Description3,2.5,Filename3,iNode number3,Notes3,Format3,Key31:Value31;Key32:Value32,True,#FF000000");
        writeStream.WriteLine("\t\t2021,12,1,17,53,0,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,MACB4,Source4,Source Type4,Type4,Username4,Hostname4,Short Description4,Full Description4,2.5,Filename4,iNode number4,Notes4,Format4,Key41:Value41;Key42:Value42,True,#FF000000");
        writeStream.WriteLine("\t2021,12,1,17,53,0,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,MACB5,Source5,Source Type5,Type5,Username5,Hostname5,Short Description5,Full Description5,2.5,Filename5,iNode number5,Notes5,Format5,Key51:Value51;Key52:Value52,True,#FF000000");
        writeStream.WriteLine("\t2021,12,1,17,53,0,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,MACB6,Source6,Source Type6,Type6,Username6,Hostname6,Short Description6,Full Description6,2.5,Filename6,iNode number6,Notes6,Format6,Key61:Value61;Key62:Value62,True,#FF000000");
        writeStream.WriteLine("2022,10,14,10,52,0,UTC;0;(UTC) Coordinated Universal Time;Coordinated Universal Time;Coordinated Universal Time;;,MACB2,Source2,Source Type2,Type2,Username2,Hostname2,Short Description2,Full Description2,2.5,Filename2,iNode number2,Notes2,Format2,Key21:Value21;Key22:Value22,True,#FF000000");
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
        List<EventViewModel> expected = new()
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
                    new Dictionary<string, string>() { { "Key31", "Value31" }, { "Key32", "Value32" } }
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
                    new Dictionary<string, string>() { { "Key41", "Value41" }, { "Key42", "Value42" } }
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
                    new Dictionary<string, string>() { { "Key51", "Value51" }, { "Key52", "Value52" } }
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
                    new Dictionary<string, string>() { { "Key61", "Value61" }, { "Key62", "Value62" } }
                    ));
        expected[0].AddChild(firstChild);
        expected[0].Children[0].AddChild(secondChild);
        expected[0].AddChild(thirdChild);
        expected[0].AddChild(fourthChild);
        FileWorkLoader loader = new();

        // Act
        List<EventViewModel> actual = loader.LoadWork(_file);

        // Assert
        Assert.AreEqual(expected.Count, actual.Count);

        Queue<EventViewModel> expectedQueue = new();
        Queue<EventViewModel> actualQueue = new();
        for (int i = 0; i < expected.Count; i++)
        {
            expectedQueue.Enqueue(expected[i]);
            actualQueue.Enqueue(actual[i]);
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

            for (int i = 0; i < currentExpected.Children.Count; i++)
            {
                expectedQueue.Enqueue(currentExpected.Children[i]);
                actualQueue.Enqueue(currentActual.Children[i]);
            }
        }
    }
}
