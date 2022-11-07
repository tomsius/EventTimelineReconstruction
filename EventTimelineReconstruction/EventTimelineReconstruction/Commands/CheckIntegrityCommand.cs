using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Commands;

public class CheckIntegrityCommand : AsyncCommandBase
{
    private readonly IntegrityViewModel _integrityViewModel;
    private readonly IEventsStore _eventsStore;
    private readonly IHashCalculator _hashCalculator;
    private readonly IEventsImporter _eventsImporter;

    public CheckIntegrityCommand(IntegrityViewModel integrityViewModel, IEventsStore eventsStore, IHashCalculator hashCalculator, IEventsImporter eventsImporter)
    {
        _integrityViewModel = integrityViewModel;
        _eventsStore = eventsStore;
        _hashCalculator = hashCalculator;
        _eventsImporter = eventsImporter;
        _integrityViewModel.PropertyChanged += this.OnViewModelPropertyChanged;
    }

    public override bool CanExecute(object parameter)
    {
        return !string.IsNullOrEmpty(_integrityViewModel.FileName) && !_integrityViewModel.HasErrors && base.CanExecute(parameter);
    }

    public override async Task ExecuteAsync(object parameter)
    {
        if (!File.Exists(_integrityViewModel.FileName))
        {
            string message = $"{(string)App.Current.Resources["FileNotFound_Message"]} {_integrityViewModel.FileName}";
            string caption = (string)App.Current.Resources["FileNotFound_Caption"];

            MessageBox.Show(message, caption, MessageBoxButton.OK);
            return;
        }

        _integrityViewModel.IsChecking = true;

        _integrityViewModel.FileOKVisibility = Visibility.Collapsed;
        _integrityViewModel.FileUnknownVisibility = Visibility.Collapsed;
        _integrityViewModel.FileCompromisedVisibility = Visibility.Collapsed;
        _integrityViewModel.EventsOKVisibility = Visibility.Collapsed;
        _integrityViewModel.EventsCompromisedVisibility = Visibility.Collapsed;

        byte[] calculatedHashValueBytes = await _hashCalculator.Calculate(_integrityViewModel.FileName);
        string calculatedHashHexadecimalValue = Convert.ToHexString(calculatedHashValueBytes);

        if (!string.IsNullOrEmpty(_integrityViewModel.HashValue))
        {
            if (AreHashValuesEqual(_integrityViewModel.HashValue, calculatedHashHexadecimalValue))
            {
                _integrityViewModel.FileOKVisibility = Visibility.Visible;
            }
            else
            {
                _integrityViewModel.FileCompromisedVisibility = Visibility.Visible;
            }
        }
        else
        {
            _integrityViewModel.FileUnknownVisibility = Visibility.Visible;
        }

        if (_eventsStore.Events.Any())
        {
            List<EventModel> fileEvents = await _eventsImporter.Import(_integrityViewModel.FileName, _integrityViewModel.FullFromDate, _integrityViewModel.FullToDate);
            fileEvents = fileEvents.OrderBy(e => e.Date).ThenBy(e => e.Time).ThenBy(e => e.Filename).ToList();

            List<EventModel> storedEvents = _eventsStore.GetStoredEventModels()
                .OrderBy(e => e.Date)
                .ThenBy(e => e.Time)
                .ThenBy(e => e.Filename)
                .ToList();

            if (AreEventsEqual(fileEvents, storedEvents))
            {
                _integrityViewModel.EventsOKVisibility = Visibility.Visible;
            }
            else
            {
                _integrityViewModel.EventsCompromisedVisibility = Visibility.Visible;
            }
        }

        _integrityViewModel.IsChecking = false;
    }

    private static bool AreHashValuesEqual(string hashValue, string calculatedHashHexadecimalValue)
    {
        hashValue = hashValue.Replace(" ", "");

        return hashValue.Equals(calculatedHashHexadecimalValue);
    }

    private static bool AreEventsEqual(List<EventModel> fileEvents, List<EventModel> storedEvents)
    {
        if (fileEvents.Count != storedEvents.Count)
        {
            return false;
        }

        for (int i = 0; i < fileEvents.Count; i++)
        {
            if (!fileEvents[i].Equals(storedEvents[i]))
            {
                return false;
            }
        }

        return true;
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IntegrityViewModel.FileName) || e.PropertyName == nameof(IntegrityViewModel.HasErrors))
        {
            this.OnCanExecuteChanged();
        }
    }
}
