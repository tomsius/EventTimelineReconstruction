using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace EventTimelineReconstruction.ViewModels;

public class ErrorsViewModel : IErrorsViewModel
{
    private readonly ObservableCollection<string> _errors;

    public ObservableCollection<string> Errors
    {
        get
        {
            return _errors;
        }
    }

    private readonly Dictionary<string, List<string>> _propertyToErrors;

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    public bool HasErrors
    {
        get
        {
            return _propertyToErrors.Any();
        }
    }

    public ErrorsViewModel()
    {
        _propertyToErrors = new();
        _errors = new();
    }

    public IEnumerable GetErrors(string propertyName)
    {
        return _propertyToErrors.GetValueOrDefault(propertyName, new List<string>());
    }

    public void ClearErrors(string propertyName)
    {
        if (_propertyToErrors.Remove(propertyName, out List<string> errors))
        {
            this.ClearErrorMessages(errors);
            this.OnErrorsChanged(propertyName);
        }
    }

    private void ClearErrorMessages(List<string> errors)
    {
        foreach (string error in errors)
        {
            _errors.Remove(error);
        }
    }

    public void AddError(string propertyName, string errorMessage)
    {
        if (!_propertyToErrors.ContainsKey(propertyName))
        {
            _propertyToErrors.Add(propertyName, new List<string>());
        }

        _propertyToErrors[propertyName].Add(errorMessage);
        _errors.Add(errorMessage);
        this.OnErrorsChanged(propertyName);
    }

    private void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    public void UpdateErrorsLanguage(string oldLanguage)
    {
        string dictionaryPath = $@"{Directory.GetCurrentDirectory()}/../../../Resources/Localizations/Resource.{oldLanguage}.xaml";
        ResourceDictionary dictionary = new();
        Uri uri = new(dictionaryPath, UriKind.Absolute);
        dictionary.Source = uri;

        foreach (string item in dictionary.Keys)
        {
            if (_errors.Contains(dictionary[item]))
            {
                _errors.Remove((string)dictionary[item]);
                _errors.Add((string)App.Current.Resources[item]);
            }
        }
    }
}
