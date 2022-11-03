using System.Text;
using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Tests.UnitTests.ViewModels;

[TestClass]
public class EventViewModelTests
{
    private static IEnumerable<object[]> DifferentObjects
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
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }
                        )
                    ),
                    new EventViewModel(
                        new EventModel(
                            new DateOnly(2021, 10, 14),
                            new TimeOnly(10, 52),
                            TimeZoneInfo.Local,
                            "MACB",
                            "Source",
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
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }
                        )
                    ),
                    1
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
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }
                        )
                    ),
                    new EventViewModel(
                        new EventModel(
                            new DateOnly(2022, 10, 14),
                            new TimeOnly(10, 52),
                            TimeZoneInfo.Local,
                            "MACB",
                            "Source",
                            "Source Type",
                            "Type",
                            "Username",
                            "Hostname",
                            "Short Description",
                            "Full Description",
                            2.5,
                            "Filename2",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }
                        )
                    ),
                    -1
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
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }
                        )
                    ),
                    new EventViewModel(
                        new EventModel(
                            new DateOnly(2022, 10, 14),
                            new TimeOnly(10, 52),
                            TimeZoneInfo.Local,
                            "MACB",
                            "Source",
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
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }
                        )
                    ),
                    0
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
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }
                        )
                    ),
                    new EventViewModel(
                        new EventModel(
                            new DateOnly(2023, 10, 14),
                            new TimeOnly(10, 52),
                            TimeZoneInfo.Local,
                            "MACB",
                            "Source",
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
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }
                        )
                    ),
                    -1
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
                            "Source Type",
                            "Type",
                            "Username",
                            "Hostname",
                            "Short Description",
                            "Full Description",
                            2.5,
                            "Filename2",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }
                        )
                    ),
                    new EventViewModel(
                        new EventModel(
                            new DateOnly(2022, 10, 14),
                            new TimeOnly(10, 52),
                            TimeZoneInfo.Local,
                            "MACB",
                            "Source",
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
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }
                        )
                    ),
                    1
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
                            "Source Type",
                            "Type",
                            "Username",
                            "Hostname",
                            "Short Description",
                            "Full Description",
                            2.5,
                            "Filename2",
                            "iNode number",
                            "Notes",
                            "Format",
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }
                        )
                    ),
                    null,
                    1
                }
            };
        }
    }

    [TestMethod]
    public void Serialize_ShouldReturnSerializedString_WhenMethodIsCalled()
    {
        // Arrange
        EventModel eventModel = new(
            new DateOnly(2022, 10, 14),
            new TimeOnly(10, 52),
            TimeZoneInfo.Local,
            "MACB",
            "Source",
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
            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } });
        EventViewModel eventViewModel = new(eventModel);
        string expected = "2022,10,14,10,52,0,FLE Standard Time;120;(UTC+02:00) Helsinki, Kyiv, Riga, Sofia, Tallinn, Vilnius;FLE Standard Time;FLE Daylight Time;[01:01:0001;12:31:9999;60;[0;03:00:00;3;5;0;];[0;04:00:00;10;5;0;];];,MACB,Source,Source Type,Type,Username,Hostname,Short Description,Full Description,2.5,Filename,iNode number,Notes,Format,Key1:Value1;Key2:Value2,True,#FF000000";

        // Act
        string actual = eventViewModel.Serialize();

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void DisplayName_ShouldReturnFormattedName_WhenPropertyIsCalled()
    {
        // Arrange
        DateTime date = new(2022, 10, 14, 10, 52, 0);
        string file = "Filename";
        EventModel eventModel = new(
            new DateOnly(date.Year, date.Month, date.Day),
            new TimeOnly(date.Hour, date.Minute),
            TimeZoneInfo.Local,
            "MACB",
            "Source",
            "Source Type",
            "Type",
            "Username",
            "Hostname",
            "Short Description",
            "Full Description",
            2.5,
            file,
            "iNode number",
            "Notes",
            "Format",
            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } });
        EventViewModel eventViewModel = new(eventModel);
        string expected = string.Format("{0, 10} {1}", date, file);

        // Act
        string actual = eventViewModel.DisplayName;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void DisplayDate_ShouldReturnFormattedDate_WhenPropertyIsCalled()
    {
        // Arrange
        DateTime date = new(2022, 10, 14, 10, 52, 0);
        EventModel eventModel = new(
           new DateOnly(date.Year, date.Month, date.Day),
            new TimeOnly(date.Hour, date.Minute),
            TimeZoneInfo.Local,
            "MACB",
            "Source",
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
            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } });
        EventViewModel eventViewModel = new(eventModel);
        string expected = date.ToString("dd/MM/yyyy");

        // Act
        string actual = eventViewModel.DisplayDate;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void DisplayTime_ShouldReturnFormattedTime_WhenPropertyIsCalled()
    {
        // Arrange
        EventModel eventModel = new(
            new DateOnly(2022, 10, 14),
            new TimeOnly(10, 52),
            TimeZoneInfo.Local,
            "MACB",
            "Source",
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
            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } });
        EventViewModel eventViewModel = new(eventModel);
        string expected = "10:52:00";

        // Act
        string actual = eventViewModel.DisplayTime;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void DisplayDescription_ShouldReturnFormattedDescription_WhenPropertyIsCalled()
    {
        // Arrange
        EventModel eventModel = new(
            new DateOnly(2022, 10, 14),
            new TimeOnly(10, 52),
            TimeZoneInfo.Local,
            "MACB",
            "Source",
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
            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } });
        EventViewModel eventViewModel = new(eventModel);
        string expected = "Full\r\nDescription";

        // Act
        string actual = eventViewModel.DisplayDescription;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void DisplayExtra_ShouldReturnFormattedExtra_WhenPropertyIsCalled()
    {
        // Arrange
        EventModel eventModel = new(
            new DateOnly(2022, 10, 14),
            new TimeOnly(10, 52),
            TimeZoneInfo.Local,
            "MACB",
            "Source",
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
            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } });
        EventViewModel eventViewModel = new(eventModel);
        string expected = "Key1=Value1\r\nKey2=Value2";

        // Act
        string actual = eventViewModel.DisplayExtra;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void AddChild_ShouldAddNewChild_WhenMethodIsCalled()
    {
        // Arrange
        EventModel parentEventModel = new(
            new DateOnly(2022, 10, 14),
            new TimeOnly(10, 52),
            TimeZoneInfo.Local,
            "MACB",
            "Source",
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
            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } });
        EventModel childEventModel = new(
            new DateOnly(2020, 11, 5),
            new TimeOnly(7, 14),
            TimeZoneInfo.Local,
            "MACB2",
            "Source2",
            "Source Type2",
            "Type2",
            "Username2",
            "Hostname2",
            "Short Description2",
            "Full Description2",
            2.1,
            "Filename2",
            "iNode number2",
            "Notes2",
            "Format2",
            new Dictionary<string, string>() { { "Key3", "Value3" }, { "Key4", "Value4" } });
        EventViewModel parentEventViewModel = new(parentEventModel);
        EventViewModel childEventViewModel = new(childEventModel);
        int expectedCount = 1;

        // Act
        parentEventViewModel.AddChild(childEventViewModel);
        int actualCount = parentEventViewModel.Children.Count;
        EventViewModel actualValue = parentEventViewModel.Children[0];

        // Assert
        Assert.AreEqual(expectedCount, actualCount);
        Assert.AreEqual(childEventViewModel.Children.Count, actualValue.Children.Count);
        Assert.AreEqual(childEventViewModel.FullDate, actualValue.FullDate);
        Assert.AreEqual(childEventViewModel.Timezone, actualValue.Timezone);
        Assert.AreEqual(childEventViewModel.MACB, actualValue.MACB);
        Assert.AreEqual(childEventViewModel.Source, actualValue.Source);
        Assert.AreEqual(childEventViewModel.SourceType, actualValue.SourceType);
        Assert.AreEqual(childEventViewModel.Type, actualValue.Type);
        Assert.AreEqual(childEventViewModel.User, actualValue.User);
        Assert.AreEqual(childEventViewModel.Host, actualValue.Host);
        Assert.AreEqual(childEventViewModel.Short, actualValue.Short);
        Assert.AreEqual(childEventViewModel.Description, actualValue.Description);
        Assert.AreEqual(childEventViewModel.Version, actualValue.Version);
        Assert.AreEqual(childEventViewModel.Filename, actualValue.Filename);
        Assert.AreEqual(childEventViewModel.INode, actualValue.INode);
        Assert.AreEqual(childEventViewModel.Notes, actualValue.Notes);
        Assert.AreEqual(childEventViewModel.Format, actualValue.Format);
        Assert.AreEqual(childEventViewModel.Extra.Count, actualValue.Extra.Count);

        foreach (KeyValuePair<string, string> kvp in childEventViewModel.Extra)
        {
            string expectedKey = kvp.Key;
            string expectedValue = kvp.Value;

            Assert.IsTrue(actualValue.Extra.ContainsKey(expectedKey));
            Assert.AreEqual(expectedValue, actualValue.Extra[expectedKey]);
        }

        Assert.AreEqual(childEventViewModel.IsVisible, actualValue.IsVisible);
        Assert.AreEqual(childEventViewModel.Colour, actualValue.Colour);
    }

    [TestMethod]
    public void RemoveChild_ShouldRemoveChildFromList_WhenChildExists()
    {
        // Arrange
        EventModel parentEventModel = new(
            new DateOnly(2022, 10, 14),
            new TimeOnly(10, 52),
            TimeZoneInfo.Local,
            "MACB",
            "Source",
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
            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } });
        EventModel childEventModel = new(
            new DateOnly(2020, 11, 5),
            new TimeOnly(7, 14),
            TimeZoneInfo.Local,
            "MACB2",
            "Source2",
            "Source Type2",
            "Type2",
            "Username2",
            "Hostname2",
            "Short Description2",
            "Full Description2",
            2.1,
            "Filename2",
            "iNode number2",
            "Notes2",
            "Format2",
            new Dictionary<string, string>() { { "Key3", "Value3" }, { "Key4", "Value4" } });
        EventViewModel parentEventViewModel = new(parentEventModel);
        EventViewModel childEventViewModel = new(childEventModel);
        parentEventViewModel.AddChild(childEventViewModel);
        int expectedCount = 0;

        // Act
        parentEventViewModel.RemoveChild(childEventViewModel);
        int actualCount = parentEventViewModel.Children.Count;

        // Assert
        Assert.AreEqual(expectedCount, actualCount);
    }

    [TestMethod]
    public void RemoveChild_ShouldDoNothing_WhenChildDoesNotExist()
    {
        // Arrange
        EventModel parentEventModel = new(
            new DateOnly(2022, 10, 14),
            new TimeOnly(10, 52),
            TimeZoneInfo.Local,
            "MACB",
            "Source",
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
            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } });
        EventModel childEventModel = new(
            new DateOnly(2020, 11, 5),
            new TimeOnly(7, 14),
            TimeZoneInfo.Local,
            "MACB2",
            "Source2",
            "Source Type2",
            "Type2",
            "Username2",
            "Hostname2",
            "Short Description2",
            "Full Description2",
            2.1,
            "Filename2",
            "iNode number2",
            "Notes2",
            "Format2",
            new Dictionary<string, string>() { { "Key3", "Value3" }, { "Key4", "Value4" } });
        EventViewModel parentEventViewModel = new(parentEventModel);
        EventViewModel childEventViewModel = new(childEventModel);
        parentEventViewModel.AddChild(childEventViewModel);
        int expectedCount = 1;

        // Act
        parentEventViewModel.RemoveChild(parentEventViewModel);
        int actualCount = parentEventViewModel.Children.Count;

        // Assert
        Assert.AreEqual(expectedCount, actualCount);
    }

    [TestMethod]
    public void ContainsChild_ShouldReturnTrue_WhenChildExistsInList()
    {
        // Arrange
        EventModel parentEventModel = new(
            new DateOnly(2022, 10, 14),
            new TimeOnly(10, 52),
            TimeZoneInfo.Local,
            "MACB",
            "Source",
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
            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } });
        EventModel childEventModel = new(
            new DateOnly(2020, 11, 5),
            new TimeOnly(7, 14),
            TimeZoneInfo.Local,
            "MACB2",
            "Source2",
            "Source Type2",
            "Type2",
            "Username2",
            "Hostname2",
            "Short Description2",
            "Full Description2",
            2.1,
            "Filename2",
            "iNode number2",
            "Notes2",
            "Format2",
            new Dictionary<string, string>() { { "Key3", "Value3" }, { "Key4", "Value4" } });
        EventViewModel parentEventViewModel = new(parentEventModel);
        EventViewModel childEventViewModel = new(childEventModel);
        parentEventViewModel.AddChild(childEventViewModel);

        // Act
        bool actual = parentEventViewModel.ContainsChild(childEventViewModel);

        // Assert
        Assert.IsTrue(actual);
    }

    [TestMethod]
    public void ContainsChild_ShouldReturnFalse_WhenChildDoesNotExistInList()
    {
        // Arrange
        EventModel parentEventModel = new(
            new DateOnly(2022, 10, 14),
            new TimeOnly(10, 52),
            TimeZoneInfo.Local,
            "MACB",
            "Source",
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
            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } });
        EventModel childEventModel = new(
            new DateOnly(2020, 11, 5),
            new TimeOnly(7, 14),
            TimeZoneInfo.Local,
            "MACB2",
            "Source2",
            "Source Type2",
            "Type2",
            "Username2",
            "Hostname2",
            "Short Description2",
            "Full Description2",
            2.1,
            "Filename2",
            "iNode number2",
            "Notes2",
            "Format2",
            new Dictionary<string, string>() { { "Key3", "Value3" }, { "Key4", "Value4" } });
        EventViewModel parentEventViewModel = new(parentEventModel);
        EventViewModel childEventViewModel = new(childEventModel);

        // Act
        bool actual = parentEventViewModel.ContainsChild(childEventViewModel);

        // Assert
        Assert.IsFalse(actual);
    }

    [TestMethod]
    [DynamicData(nameof(DifferentObjects))]
    public void CompareTo_ShouldReturnInteger_WhenObjectsAreCompared(EventViewModel first, EventViewModel second, int expected)
    {
        // Act
        int actual = first.CompareTo(second);

        //Assert
        Assert.AreEqual(expected, actual);
    }
}
