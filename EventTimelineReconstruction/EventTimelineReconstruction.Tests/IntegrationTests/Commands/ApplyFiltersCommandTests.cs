using System.Windows.Controls;
using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Tests.IntegrationTests.Commands;

[TestClass]
public class ApplyFiltersCommandTests
{
    private readonly IFilteringStore _filteringStore;
    private readonly EventTreeViewModel _eventTreeViewModel;
    private readonly ApplyFiltersCommand _command;

    public ApplyFiltersCommandTests()
    {
        IDragDropUtils dragDropUtils = new DragDropUtils();
        EventDetailsViewModel eventDetailsViewModel = new();
        ChangeColourViewModel changeColourViewModel = new(eventDetailsViewModel);
        _filteringStore = new FilteringStore();
        _eventTreeViewModel = new(eventDetailsViewModel, _filteringStore, changeColourViewModel, dragDropUtils);

        _command = new(_filteringStore, _eventTreeViewModel);
    }

    [STATestMethod]
    public void Execute_ShouldEnableFiltering_WhenFilteringWasDisabledBefore()
    {
        // Arrange
        Button enableButton = new() { IsEnabled = true };
        Button disableButton = new() { IsEnabled = false };
        object parameter = new[]
        {
            (object)enableButton,
            (object)disableButton
        };
        _filteringStore.AreAllFiltersApplied = false;
        _filteringStore.Keyword = "google";
        int expectedCount = 1;
        EventViewModel expected = new(new EventModel(
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
                "www.google.com",
                2.5,
                "Filename",
                "iNode number",
                "Notes",
                "Format",
                new Dictionary<string, string>(),
                1));
        EventViewModel otherEvent = new(new EventModel(
                new DateOnly(2018, 1, 5),
                new TimeOnly(15, 14),
                TimeZoneInfo.Local,
                "MACB2",
                "Source2",
                "Source Type2",
                "Type2",
                "Username2",
                "Hostname2",
                "Short Description2",
                "Full Description2",
                4.5,
                "Filename2",
                "iNode number2",
                "Notes2",
                "Format2",
                new Dictionary<string, string>(),
                2));
        _eventTreeViewModel.AddEvent(expected);
        _eventTreeViewModel.AddEvent(otherEvent);

        // Act
        _command.Execute(parameter);
        List<EventViewModel> actualValue = _eventTreeViewModel.EventsView.Cast<EventViewModel>().ToList();
        EventViewModel? actual = actualValue.FirstOrDefault();

        // Assert
        Assert.IsFalse(enableButton.IsEnabled);
        Assert.IsTrue(disableButton.IsEnabled);
        Assert.IsTrue(_filteringStore.IsEnabled);
        Assert.AreEqual(expectedCount, actualValue.Count);

        Assert.IsNotNull(actual);
        Assert.AreEqual(expected.Children.Count, actual.Children.Count);
        Assert.AreEqual(expected.FullDate, actual.FullDate);
        Assert.AreEqual(expected.Timezone, actual.Timezone);
        Assert.AreEqual(expected.MACB, actual.MACB);
        Assert.AreEqual(expected.Source, actual.Source);
        Assert.AreEqual(expected.SourceType, actual.SourceType);
        Assert.AreEqual(expected.Type, actual.Type);
        Assert.AreEqual(expected.User, actual.User);
        Assert.AreEqual(expected.Host, actual.Host);
        Assert.AreEqual(expected.Short, actual.Short);
        Assert.AreEqual(expected.Description, actual.Description);
        Assert.AreEqual(expected.Version, actual.Version);
        Assert.AreEqual(expected.Filename, actual.Filename);
        Assert.AreEqual(expected.INode, actual.INode);
        Assert.AreEqual(expected.Notes, actual.Notes);
        Assert.AreEqual(expected.Format, actual.Format);
        Assert.AreEqual(expected.Extra.Count, actual.Extra.Count);

        foreach (KeyValuePair<string, string> kvp in expected.Extra)
        {
            string expectedKey = kvp.Key;
            string expectedValue = kvp.Value;

            Assert.IsTrue(actual.Extra.ContainsKey(expectedKey));
            Assert.AreEqual(expectedValue, actual.Extra[expectedKey]);
        }

        Assert.AreEqual(expected.IsVisible, actual.IsVisible);
        Assert.AreEqual(expected.Colour.ToString(), actual.Colour.ToString());
        Assert.AreEqual(expected.SourceLine, actual.SourceLine);
    }

    [STATestMethod]
    public void Execute_ShouldDisableFiltering_WhenFilteringWasEnabledBefore()
    {
        // Arrange
        Button enableButton = new() { IsEnabled = false };
        Button disableButton = new() { IsEnabled = true };
        object parameter = new[]
        {
            (object)enableButton,
            (object)disableButton
        };
        _filteringStore.AreAllFiltersApplied = false;
        _filteringStore.Keyword = "google";
        EventViewModel firstEvent = new(new EventModel(
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
                "www.google.com",
                2.5,
                "Filename",
                "iNode number",
                "Notes",
                "Format",
                new Dictionary<string, string>(),
                1));
        EventViewModel secondEvent = new(new EventModel(
                new DateOnly(2018, 1, 5),
                new TimeOnly(15, 14),
                TimeZoneInfo.Local,
                "MACB2",
                "Source2",
                "Source Type2",
                "Type2",
                "Username2",
                "Hostname2",
                "Short Description2",
                "Full Description2",
                4.5,
                "Filename2",
                "iNode number2",
                "Notes2",
                "Format2",
                new Dictionary<string, string>(),
                2));
        List<EventViewModel> expected = new() { firstEvent, secondEvent };
        expected = expected.OrderBy(e => e.FullDate).ThenBy(e => e.Filename).ToList();
        _eventTreeViewModel.AddEvent(firstEvent);
        _eventTreeViewModel.AddEvent(secondEvent);

        // Act
        _command.Execute(parameter);
        List<EventViewModel> actual = _eventTreeViewModel.EventsView.Cast<EventViewModel>().ToList();

        // Assert
        Assert.IsTrue(enableButton.IsEnabled);
        Assert.IsFalse(disableButton.IsEnabled);
        Assert.IsFalse(_filteringStore.IsEnabled);
        Assert.AreEqual(expected.Count, actual.Count);

        for (int i = 0; i < expected.Count; i++)
        {
            Assert.AreEqual(expected[i].Children.Count, actual[i].Children.Count);
            Assert.AreEqual(expected[i].FullDate, actual[i].FullDate);
            Assert.AreEqual(expected[i].Timezone, actual[i].Timezone);
            Assert.AreEqual(expected[i].MACB, actual[i].MACB);
            Assert.AreEqual(expected[i].Source, actual[i].Source);
            Assert.AreEqual(expected[i].SourceType, actual[i].SourceType);
            Assert.AreEqual(expected[i].Type, actual[i].Type);
            Assert.AreEqual(expected[i].User, actual[i].User);
            Assert.AreEqual(expected[i].Host, actual[i].Host);
            Assert.AreEqual(expected[i].Short, actual[i].Short);
            Assert.AreEqual(expected[i].Description, actual[i].Description);
            Assert.AreEqual(expected[i].Version, actual[i].Version);
            Assert.AreEqual(expected[i].Filename, actual[i].Filename);
            Assert.AreEqual(expected[i].INode, actual[i].INode);
            Assert.AreEqual(expected[i].Notes, actual[i].Notes);
            Assert.AreEqual(expected[i].Format, actual[i].Format);
            Assert.AreEqual(expected[i].Extra.Count, actual[i].Extra.Count);

            foreach (KeyValuePair<string, string> kvp in expected[i].Extra)
            {
                string expectedKey = kvp.Key;
                string expectedValue = kvp.Value;

                Assert.IsTrue(actual[i].Extra.ContainsKey(expectedKey));
                Assert.AreEqual(expectedValue, actual[i].Extra[expectedKey]);
            }

            Assert.AreEqual(expected[i].IsVisible, actual[i].IsVisible);
            Assert.AreEqual(expected[i].Colour.ToString(), actual[i].Colour.ToString());
            Assert.AreEqual(expected[i].SourceLine, actual[i].SourceLine);
        }
    }
}
