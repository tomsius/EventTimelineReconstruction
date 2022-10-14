using EventTimelineReconstruction.Models;

namespace EventTimelineReconstruction.Tests.UnitTests.Models;

[TestClass]
public class EventModelTests
{
    private static IEnumerable<object[]> DifferentObjects
    {
        get
        {
            return new[]
            {
                new object[]
                {
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
                    ),
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
                },
                new object[]
                {
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
                    ),
                    new EventModel(
                        new DateOnly(2022, 11, 14),
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
                },
                new object[]
                {
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
                    ),
                    new EventModel(
                        new DateOnly(2022, 10, 15),
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
                },
                new object[]
                {
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
                    ),
                    new EventModel(
                        new DateOnly(2022, 10, 14),
                        new TimeOnly(20, 52),
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
                },
                new object[]
                {
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
                    ),
                    new EventModel(
                        new DateOnly(2022, 10, 14),
                        new TimeOnly(10, 15),
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
                },
                new object[]
                {
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
                    ),
                    new EventModel(
                        new DateOnly(2022, 10, 14),
                        new TimeOnly(10, 52),
                        TimeZoneInfo.Utc,
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
                },
                new object[]
                {
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
                    ),
                    new EventModel(
                        new DateOnly(2022, 10, 14),
                        new TimeOnly(10, 52),
                        TimeZoneInfo.Local,
                        "BCAM",
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
                },
                new object[]
                {
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
                    ),
                    new EventModel(
                        new DateOnly(2022, 10, 14),
                        new TimeOnly(10, 52),
                        TimeZoneInfo.Local,
                        "MACB",
                        "Source2",
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
                },
                new object[]
                {
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
                    ),
                    new EventModel(
                        new DateOnly(2022, 10, 14),
                        new TimeOnly(10, 52),
                        TimeZoneInfo.Local,
                        "MACB",
                        "Source",
                        "Source Type2",
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
                },
                new object[]
                {
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
                    ),
                    new EventModel(
                        new DateOnly(2022, 10, 14),
                        new TimeOnly(10, 52),
                        TimeZoneInfo.Local,
                        "MACB",
                        "Source",
                        "Source Type",
                        "Type2",
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
                },
                new object[]
                {
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
                    ),
                    new EventModel(
                        new DateOnly(2022, 10, 14),
                        new TimeOnly(10, 52),
                        TimeZoneInfo.Local,
                        "MACB",
                        "Source",
                        "Source Type",
                        "Type",
                        "Username2",
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
                },
                new object[]
                {
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
                    ),
                    new EventModel(
                        new DateOnly(2022, 10, 14),
                        new TimeOnly(10, 52),
                        TimeZoneInfo.Local,
                        "MACB",
                        "Source",
                        "Source Type",
                        "Type",
                        "Username",
                        "Hostname2",
                        "Short Description",
                        "Full Description",
                        2.5,
                        "Filename",
                        "iNode number",
                        "Notes",
                        "Format",
                        new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }
                    )
                },
                new object[]
                {
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
                    ),
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
                        "Short Description2",
                        "Full Description",
                        2.5,
                        "Filename",
                        "iNode number",
                        "Notes",
                        "Format",
                        new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }
                    )
                },
                new object[]
                {
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
                    ),
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
                        "Full Description2",
                        2.5,
                        "Filename",
                        "iNode number",
                        "Notes",
                        "Format",
                        new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }
                    )
                },
                new object[]
                {
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
                    ),
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
                        2.0,
                        "Filename",
                        "iNode number",
                        "Notes",
                        "Format",
                        new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }
                    )
                },
                new object[]
                {
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
                    ),
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
                },
                new object[]
                {
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
                    ),
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
                        "iNode number2",
                        "Notes",
                        "Format",
                        new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }
                    )
                },
                new object[]
                {
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
                    ),
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
                        "Notes2",
                        "Format",
                        new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }
                    )
                },
                new object[]
                {
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
                    ),
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
                        "Format2",
                        new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }
                    )
                },
                new object[]
                {
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
                    ),
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
                        new Dictionary<string, string>() { { "Key1", "Value1" } }
                    )
                },
                new object[]
                {
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
                    ),
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
                        new Dictionary<string, string>() { { "Key3", "Value3" }, { "Key2", "Value2" } }
                    )
                },
                new object[]
                {
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
                    ),
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
                        new Dictionary<string, string>() { { "Key1", "Value3" }, { "Key2", "Value2" } }
                    )
                }
            };
        }
    }

    [TestMethod]
    public void Equals_ShouldReturnTrue_WhenObjectsAreEqual()
    {
        // Arange
        EventModel firstObject = new(
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

        EventModel secondObject = new(
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

        // Act
        bool result = firstObject.Equals(secondObject);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    [DynamicData(nameof(DifferentObjects))]
    public void Equals_ShouldReturnFalse_WhenObjectsAreNotEqual(EventModel firstObject, EventModel secondObject)
    {
        // Act
        bool result = firstObject.Equals(secondObject);

        // Assert
        Assert.IsFalse(result);
    }

    [DataTestMethod]
    [DataRow(1)]
    [DataRow(2.3)]
    [DataRow("string")]
    [DataRow('c')]
    [DataRow(true)]
    public void Equals_ShouldReturnFalse_WhenObjectsAreDifferentTypes(object secondObject)
    {
        // Arange
        EventModel firstObject = new(
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

        // Act
        bool result = firstObject.Equals(secondObject);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void GetHashCode_ShouldReturnIntegerCode_WhenMethodIsCalledOnObject()
    {
        // Arange
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

        // Act
        int firstCode = eventModel.GetHashCode();
        int secondCode = eventModel.GetHashCode();

        // Assert
        Assert.AreEqual(firstCode, secondCode);
    }
}