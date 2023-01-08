using EventTimelineReconstruction.Commands;
using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.Validators;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Tests.IntegrationTests.Commands;

[TestClass]
public class ApplyFilterOptionsCommandTests
{
    private readonly IFilteringStore _filteringStore;
    private readonly IErrorsViewModel _errorsViewModel;
    private readonly FilterViewModel _filterViewModel;
    private readonly EventTreeViewModel _eventTreeViewModel;
    private readonly ApplyFilterOptionsCommand _command;

    public ApplyFilterOptionsCommandTests()
    {
        ITimeValidator timeValidator = new TimeValidator();
        IFilteringUtils filteringUtils = new FilteringUtils();
        IDateTimeProvider dateTimeProvider = new DateTimeProvider();
        IDragDropUtils dragDropUtils = new DragDropUtils();
        EventDetailsViewModel eventDetailsViewModel = new();
        ChangeColourViewModel changeColourViewModel = new(eventDetailsViewModel);
        _errorsViewModel = new ErrorsViewModel();
        _filteringStore = new FilteringStore();
        _eventTreeViewModel = new(eventDetailsViewModel, _filteringStore, changeColourViewModel, dragDropUtils);
        _filterViewModel = new(_filteringStore, _eventTreeViewModel, timeValidator, filteringUtils, _errorsViewModel, dateTimeProvider);

        _command = new(_filterViewModel, _filteringStore, _eventTreeViewModel);
    }

    [TestMethod]
    public void CanExecute_ShouldReturnFalse_WhenThereAreErrors()
    {
        // Arrange
        _errorsViewModel.AddError("Test", "Error");
        bool expected = false;

        // Act
        bool actual = _command.CanExecute(null);

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void CanExecute_ShouldReturnTrue_WhenThereAreNoErrors()
    {
        // Arrange
        bool expected = true;

        // Act
        bool actual = _command.CanExecute(null);

        // Assert
        Assert.AreEqual(expected, actual);
        Assert.IsTrue(actual);
    }

    [TestMethod]
    public void Execute_ShouldFilterEvents_WhenFilteringIsEnabled()
    {
        // Arrange
        bool expectedAreAllFiltersApplied = true;
        string expectedKeyword = "description";
        DateTime expectedFromDate = new(2020, 1, 1);
        DateTime expectedToDate = new(2025, 1, 1);
        Dictionary<string, bool> expectedChosenEventTypes = new() { { "Type", true }, { "Tipas", true } };

        _filteringStore.IsEnabled = true;
        _filterViewModel.AreAllFiltersApplied = expectedAreAllFiltersApplied;
        _filterViewModel.Keyword = expectedKeyword;
        _filterViewModel.FromDate = expectedFromDate;
        _filterViewModel.ToDate = expectedToDate;
        foreach (KeyValuePair<string, bool> item in expectedChosenEventTypes)
        {
            _filterViewModel.UpdateChosenEventType(item.Key, item.Value);
        }
        
        EventViewModel expectedEvent = new(new EventModel(new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Utc, "MACB", "Source", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }, 1));
        EventViewModel other = new(new EventModel(new DateOnly(2002, 1, 2), new TimeOnly(6, 12), TimeZoneInfo.Utc, "BCAM", "Saltinis", "Saltinio Tipas", "Tipas", "Naudotojas", "Seimininkas", "Trumpas Aprasymas", "Aprasymas", 3, "Failo vardas", "iNode numeris", "Papildoma informacija", "Formatas", new Dictionary<string, string>() { { "Raktas1", "Reiksme1" }, { "Raktas2", "Reiksme2" } }, 2));
        _eventTreeViewModel.AddEvent(expectedEvent);
        _eventTreeViewModel.AddEvent(other);
        int expectedCount = 1;

        // Act
        _command.Execute(null);
        List<EventViewModel> actualValues = _eventTreeViewModel.EventsView.Cast<EventViewModel>().ToList();

        // Assert
        Assert.AreEqual(expectedAreAllFiltersApplied, _filteringStore.AreAllFiltersApplied);
        Assert.AreEqual(expectedKeyword, _filteringStore.Keyword);
        Assert.AreEqual(expectedFromDate, _filteringStore.FromDate);
        Assert.AreEqual(expectedToDate, _filteringStore.ToDate);

        foreach (KeyValuePair<string, bool> expectedItem in expectedChosenEventTypes)
        {
            Assert.IsTrue(_filteringStore.ChosenEventTypes.ContainsKey(expectedItem.Key));
            Assert.AreEqual(expectedItem.Value, _filteringStore.ChosenEventTypes[expectedItem.Key]);
        }

        Assert.AreEqual(expectedCount, actualValues.Count);

        Assert.AreEqual(expectedEvent.Children.Count, actualValues[0].Children.Count);
        Assert.AreEqual(expectedEvent.FullDate, actualValues[0].FullDate);
        Assert.AreEqual(expectedEvent.Timezone, actualValues[0].Timezone);
        Assert.AreEqual(expectedEvent.MACB, actualValues[0].MACB);
        Assert.AreEqual(expectedEvent.Source, actualValues[0].Source);
        Assert.AreEqual(expectedEvent.SourceType, actualValues[0].SourceType);
        Assert.AreEqual(expectedEvent.Type, actualValues[0].Type);
        Assert.AreEqual(expectedEvent.User, actualValues[0].User);
        Assert.AreEqual(expectedEvent.Host, actualValues[0].Host);
        Assert.AreEqual(expectedEvent.Short, actualValues[0].Short);
        Assert.AreEqual(expectedEvent.Description, actualValues[0].Description);
        Assert.AreEqual(expectedEvent.Version, actualValues[0].Version);
        Assert.AreEqual(expectedEvent.Filename, actualValues[0].Filename);
        Assert.AreEqual(expectedEvent.INode, actualValues[0].INode);
        Assert.AreEqual(expectedEvent.Notes, actualValues[0].Notes);
        Assert.AreEqual(expectedEvent.Format, actualValues[0].Format);
        Assert.AreEqual(expectedEvent.Extra.Count, actualValues[0].Extra.Count);

        foreach (KeyValuePair<string, string> kvp in expectedEvent.Extra)
        {
            string expectedKey = kvp.Key;
            string expectedValue = kvp.Value;

            Assert.IsTrue(actualValues[0].Extra.ContainsKey(expectedKey));
            Assert.AreEqual(expectedValue, actualValues[0].Extra[expectedKey]);
        }

        Assert.AreEqual(expectedEvent.IsVisible, actualValues[0].IsVisible);
        Assert.AreEqual(expectedEvent.Colour.ToString(), actualValues[0].Colour.ToString());
        Assert.AreEqual(expectedEvent.SourceLine, actualValues[0].SourceLine);
    }

    [TestMethod]
    public void Execute_ShouldNotFilterEvents_WhenFilteringIsDisabled()
    {
        // Arrange
        bool expectedAreAllFiltersApplied = true;
        string expectedKeyword = "description";
        DateTime expectedFromDate = new(2020, 1, 1);
        DateTime expectedToDate = new(2025, 1, 1);
        Dictionary<string, bool> expectedChosenEventTypes = new() { { "Type", true }, { "Tipas", true } };

        _filteringStore.IsEnabled = false;
        _filterViewModel.AreAllFiltersApplied = expectedAreAllFiltersApplied;
        _filterViewModel.Keyword = expectedKeyword;
        _filterViewModel.FromDate = expectedFromDate;
        _filterViewModel.ToDate = expectedToDate;
        foreach (KeyValuePair<string, bool> item in expectedChosenEventTypes)
        {
            _filterViewModel.UpdateChosenEventType(item.Key, item.Value);
        }

        EventViewModel firstEvent = new(new EventModel(new DateOnly(2022, 10, 14), new TimeOnly(10, 52), TimeZoneInfo.Utc, "MACB", "Source", "Source Type", "Type", "Username", "Hostname", "Short Description", "Full Description", 2.5, "Filename", "iNode number", "Notes", "Format", new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }, 1));
        EventViewModel secondEvent = new(new EventModel(new DateOnly(2002, 1, 2), new TimeOnly(6, 12), TimeZoneInfo.Utc, "BCAM", "Saltinis", "Saltinio Tipas", "Tipas", "Naudotojas", "Seimininkas", "Trumpas Aprasymas", "Aprasymas", 3, "Failo vardas", "iNode numeris", "Papildoma informacija", "Formatas", new Dictionary<string, string>() { { "Raktas1", "Reiksme1" }, { "Raktas2", "Reiksme2" } }, 2));
        _eventTreeViewModel.AddEvent(firstEvent);
        _eventTreeViewModel.AddEvent(secondEvent);
        List<EventViewModel> expectedValues = new() { firstEvent, secondEvent };
        expectedValues = expectedValues.OrderBy(e => e.FullDate).ThenBy(e => e.Filename).ToList();

        // Act
        _command.Execute(null);
        List<EventViewModel> actualValues = _eventTreeViewModel.EventsView.Cast<EventViewModel>().ToList();

        // Assert
        Assert.AreEqual(expectedAreAllFiltersApplied, _filteringStore.AreAllFiltersApplied);
        Assert.AreEqual(expectedKeyword, _filteringStore.Keyword);
        Assert.AreEqual(expectedFromDate, _filteringStore.FromDate);
        Assert.AreEqual(expectedToDate, _filteringStore.ToDate);

        foreach (KeyValuePair<string, bool> expectedItem in expectedChosenEventTypes)
        {
            Assert.IsTrue(_filteringStore.ChosenEventTypes.ContainsKey(expectedItem.Key));
            Assert.AreEqual(expectedItem.Value, _filteringStore.ChosenEventTypes[expectedItem.Key]);
        }

        Assert.AreEqual(expectedValues.Count, actualValues.Count);

        for (int i = 0; i < expectedValues.Count; i++)
        {
            Assert.AreEqual(expectedValues[i].Children.Count, actualValues[i].Children.Count);
            Assert.AreEqual(expectedValues[i].FullDate, actualValues[i].FullDate);
            Assert.AreEqual(expectedValues[i].Timezone, actualValues[i].Timezone);
            Assert.AreEqual(expectedValues[i].MACB, actualValues[i].MACB);
            Assert.AreEqual(expectedValues[i].Source, actualValues[i].Source);
            Assert.AreEqual(expectedValues[i].SourceType, actualValues[i].SourceType);
            Assert.AreEqual(expectedValues[i].Type, actualValues[i].Type);
            Assert.AreEqual(expectedValues[i].User, actualValues[i].User);
            Assert.AreEqual(expectedValues[i].Host, actualValues[i].Host);
            Assert.AreEqual(expectedValues[i].Short, actualValues[i].Short);
            Assert.AreEqual(expectedValues[i].Description, actualValues[i].Description);
            Assert.AreEqual(expectedValues[i].Version, actualValues[i].Version);
            Assert.AreEqual(expectedValues[i].Filename, actualValues[i].Filename);
            Assert.AreEqual(expectedValues[i].INode, actualValues[i].INode);
            Assert.AreEqual(expectedValues[i].Notes, actualValues[i].Notes);
            Assert.AreEqual(expectedValues[i].Format, actualValues[i].Format);
            Assert.AreEqual(expectedValues[i].Extra.Count, actualValues[i].Extra.Count);

            foreach (KeyValuePair<string, string> kvp in expectedValues[i].Extra)
            {
                string expectedKey = kvp.Key;
                string expectedValue = kvp.Value;

                Assert.IsTrue(actualValues[i].Extra.ContainsKey(expectedKey));
                Assert.AreEqual(expectedValue, actualValues[i].Extra[expectedKey]);
            }

            Assert.AreEqual(expectedValues[i].IsVisible, actualValues[i].IsVisible);
            Assert.AreEqual(expectedValues[i].Colour.ToString(), actualValues[i].Colour.ToString());
            Assert.AreEqual(expectedValues[i].SourceLine, actualValues[i].SourceLine);
        }
    }
}
