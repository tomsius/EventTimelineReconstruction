using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Tests.IntegrationTests.Utils;

[TestClass]
public class EventSorterTests
{
    private readonly EventSorter _eventSorter;

    public EventSorterTests()
    {
        _eventSorter = new();
    }

    private static IEnumerable<object?[]> DifferentObjects
    {
        get
        {
            return new[]
            {
                new object?[]
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
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            1
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
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            2
                        )
                    ),
                    1
                },
                new object?[]
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
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            1
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
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            2
                        )
                    ),
                    -1
                },
                new object?[]
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
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            1
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
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            2
                        )
                    ),
                    0
                },
                new object?[]
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
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            1
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
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            2
                        )
                    ),
                    -1
                },
                new object?[]
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
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            1
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
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            2
                        )
                    ),
                    1
                },
                new object?[]
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
                            new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } },
                            1
                        )
                    ),
                    null,
                    1
                }
            };
        }
    }

    [TestMethod]
    [DynamicData(nameof(DifferentObjects))]
    public void Compare_ShouldReturnInteger_WhenObjectsAreEventViewModelType(EventViewModel first, EventViewModel second, int expected)
    {
        // Act
        int actual = _eventSorter.Compare(first, second);

        // Assert
        Assert.AreEqual(expected, actual);
    }
}
