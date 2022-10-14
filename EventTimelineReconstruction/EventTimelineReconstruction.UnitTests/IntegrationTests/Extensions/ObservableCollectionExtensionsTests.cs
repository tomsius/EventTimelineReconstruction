using System.Collections.ObjectModel;
using EventTimelineReconstruction.Extensions;
using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.IntegrationTests.Extensions;

[TestClass]
public class ObservableCollectionExtensionsTests
{
    private static IEnumerable<object[]> Collections
    {
        get
        {
            return new[]
            {
                new object[]
                {
                    new ObservableCollection<EventViewModel>(
                        new List<EventViewModel>()
                        {
                            new EventViewModel(
                                new EventModel(
                                new DateOnly(2000, 1, 1),
                                new TimeOnly(0, 0),
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
                                new Dictionary<string, string>())
                            ),
                        new EventViewModel(
                            new EventModel(
                                new DateOnly(2010, 1, 1),
                                new TimeOnly(0, 0),
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
                                new Dictionary<string, string>())
                            ),
                        new EventViewModel(
                            new EventModel(
                                new DateOnly(2010, 1, 1),
                                new TimeOnly(0, 0),
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
                                "Filename3",
                                "iNode number",
                                "Notes",
                                "Format",
                                new Dictionary<string, string>())
                            ),
                        new EventViewModel(
                            new EventModel(
                                new DateOnly(2010, 1, 1),
                                new TimeOnly(12, 0),
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
                                "Filename4",
                                "iNode number",
                                "Notes",
                                "Format",
                                new Dictionary<string, string>())
                            )
                        })
                },
                new object[]
                {
                    new ObservableCollection<EventViewModel>(
                        new List<EventViewModel>()
                        {
                            new EventViewModel(
                                new EventModel(
                                    new DateOnly(2010, 1, 1),
                                    new TimeOnly(12, 0),
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
                                    "Filename4",
                                    "iNode number",
                                    "Notes",
                                    "Format",
                                    new Dictionary<string, string>())
                                ),
                                new EventViewModel(
                                new EventModel(
                                    new DateOnly(2010, 1, 1),
                                    new TimeOnly(0, 0),
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
                                    "Filename3",
                                    "iNode number",
                                    "Notes",
                                    "Format",
                                    new Dictionary<string, string>())
                                ),
                                new EventViewModel(
                                new EventModel(
                                    new DateOnly(2010, 1, 1),
                                    new TimeOnly(0, 0),
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
                                    new Dictionary<string, string>())
                                ),
                                new EventViewModel(
                                    new EventModel(
                                    new DateOnly(2000, 1, 1),
                                    new TimeOnly(0, 0),
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
                                    new Dictionary<string, string>())
                                    )
                        })
                }
            };
        }
    }

    [TestMethod]
    [DynamicData(nameof(Collections))]
    public void Sort_ShouldSortElementsAscending_WhenMethodIsCalled(ObservableCollection<EventViewModel> collection)
    {
        // Arrange
        ObservableCollection<EventViewModel> expected = new(
            new List<EventViewModel>()
            {
                new EventViewModel(
                    new EventModel(
                    new DateOnly(2000, 1, 1),
                    new TimeOnly(0, 0),
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
                    new Dictionary<string, string>())
                ),
            new EventViewModel(
                new EventModel(
                    new DateOnly(2010, 1, 1),
                    new TimeOnly(0, 0),
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
                    new Dictionary<string, string>())
                ),
            new EventViewModel(
                new EventModel(
                    new DateOnly(2010, 1, 1),
                    new TimeOnly(0, 0),
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
                    "Filename3",
                    "iNode number",
                    "Notes",
                    "Format",
                    new Dictionary<string, string>())
                ),
            new EventViewModel(
                new EventModel(
                    new DateOnly(2010, 1, 1),
                    new TimeOnly(12, 0),
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
                    "Filename4",
                    "iNode number",
                    "Notes",
                    "Format",
                    new Dictionary<string, string>())
                )
            });

        // Act
        ObservableCollectionExtensions.Sort(collection);

        // Assert
        for (int i = 0; i < expected.Count; i++)
        {
            Assert.AreEqual(expected[i].Children.Count, collection[i].Children.Count);
            Assert.AreEqual(expected[i].FullDate, collection[i].FullDate);
            Assert.AreEqual(expected[i].Timezone, collection[i].Timezone);
            Assert.AreEqual(expected[i].MACB, collection[i].MACB);
            Assert.AreEqual(expected[i].Source, collection[i].Source);
            Assert.AreEqual(expected[i].SourceType, collection[i].SourceType);
            Assert.AreEqual(expected[i].Type, collection[i].Type);
            Assert.AreEqual(expected[i].User, collection[i].User);
            Assert.AreEqual(expected[i].Host, collection[i].Host);
            Assert.AreEqual(expected[i].Short, collection[i].Short);
            Assert.AreEqual(expected[i].Description, collection[i].Description);
            Assert.AreEqual(expected[i].Version, collection[i].Version);
            Assert.AreEqual(expected[i].Filename, collection[i].Filename);
            Assert.AreEqual(expected[i].INode, collection[i].INode);
            Assert.AreEqual(expected[i].Notes, collection[i].Notes);
            Assert.AreEqual(expected[i].Format, collection[i].Format);
            Assert.AreEqual(expected[i].Extra.Count, collection[i].Extra.Count);

            foreach (KeyValuePair<string, string> kvp in collection[i].Extra)
            {
                string expectedKey = kvp.Key;
                string expectedValue = kvp.Value;

                Assert.IsTrue(collection[i].Extra.ContainsKey(expectedKey));
                Assert.AreEqual(expectedValue, collection[i].Extra[expectedKey]);
            }

            Assert.AreEqual(expected[i].IsVisible, collection[i].IsVisible);
            Assert.AreEqual(expected[i].Colour, collection[i].Colour);
        }
    }
}
