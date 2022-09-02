﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Services;
using EventTimelineReconstruction.Stores;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Commands;

public class CheckIntegrityCommand : CommandBase
{
    private readonly IntegrityViewModel _integrityViewModel;
    private readonly EventsStore _eventsStore;
    private readonly IHashCalculator _hashCalculator;
    private readonly IEventsImporter _eventsImporter;

    public CheckIntegrityCommand(IntegrityViewModel integrityViewModel, EventsStore eventsStore, IHashCalculator hashCalculator, IEventsImporter eventsImporter)
    {
        _integrityViewModel = integrityViewModel;
        _eventsStore = eventsStore;
        _hashCalculator = hashCalculator;
        _eventsImporter = eventsImporter;
        _integrityViewModel.PropertyChanged += this.OnViewModelPropertyChanged;
    }

    public override bool CanExecute(object parameter)
    {
        return !string.IsNullOrEmpty(_integrityViewModel.FileName) && base.CanExecute(parameter);
    }

    public override void Execute(object parameter)
    {
        object[] textBlocks = parameter as object[];
        foreach (TextBlock textBlock in textBlocks.Cast<TextBlock>())
        {
            textBlock.Visibility = Visibility.Collapsed;
        }

        TextBlock fileOKTextBlock = (TextBlock)textBlocks[0];
        TextBlock fileUnknownTextBlock = (TextBlock)textBlocks[1];
        TextBlock fileCompromisedTextBlock = (TextBlock)textBlocks[2];
        TextBlock eventsOKTextBlock = (TextBlock)textBlocks[3];
        TextBlock eventsCompromisedTextBlock = (TextBlock)textBlocks[4];

        byte[] calculatedHashValueBytes = _hashCalculator.Calculate(_integrityViewModel.FileName);
        string calculatedHashHexadecimalValue = Convert.ToHexString(calculatedHashValueBytes);

        if (!string.IsNullOrEmpty(_integrityViewModel.HashValue))
        {
            if (AreHashValuesEqual(_integrityViewModel.HashValue, calculatedHashHexadecimalValue))
            {
                fileOKTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                fileCompromisedTextBlock.Visibility = Visibility.Visible;
            }
        }
        else
        {
            fileUnknownTextBlock.Visibility = Visibility.Visible;
        }

        if (_eventsStore.Events.Any())
        {
            List<EventModel> fileEvents = _eventsImporter.Import(_integrityViewModel.FileName, _integrityViewModel.FullFromDate, _integrityViewModel.FullToDate)
                .OrderBy(e => e.Date)
                .ThenBy(e => e.Time)
                .ThenBy(e => e.Filename)
                .ToList();

            List<EventModel> storedEvents = _eventsStore.GetStoredEventModels()
                .OrderBy(e => e.Date)
                .ThenBy(e => e.Time)
                .ThenBy(e => e.Filename)
                .ToList();

            if (AreEventsEqual(fileEvents, storedEvents))
            {
                eventsOKTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                eventsCompromisedTextBlock.Visibility = Visibility.Visible;
            }
        }
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
        if (e.PropertyName == nameof(IntegrityViewModel.FileName))
        {
            this.OnCanExecuteChanged();
        }
    }
}