using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using EventTimelineReconstruction.ViewModels;
using EventTimelineReconstruction.Views;

namespace EventTimelineReconstruction;

public partial class MainWindow : Window
{
    private readonly ImportView _importView;
    private readonly SaveWorkView _saveWorkView;
    private readonly LoadWorkView _loadWorkView;
    private readonly HiddenEventsView _hiddenEventsView;
    private readonly FilterView _filterView;
    private readonly ChangeColourView _changeColourView;
    private readonly ColourView _colourView;
    private readonly IntegrityView _integrityView;

    public MainWindow(
        ImportView view, 
        SaveWorkView saveWorkView, 
        LoadWorkView loadWorkView, 
        HiddenEventsView hiddenEventsView, 
        FilterView filterView,
        ChangeColourView changeColourView,
        ColourView colourView,
        IntegrityView integrityView)
    {
        this.InitializeComponent();
        _importView = view;
        _saveWorkView = saveWorkView;
        _loadWorkView = loadWorkView;
        _hiddenEventsView = hiddenEventsView;
        _filterView = filterView;
        _changeColourView = changeColourView;
        _colourView = colourView;
        _integrityView = integrityView;

        ChangeLanguage("en");
    }

    private void ImportButton_Click(object sender, RoutedEventArgs e)
    {
        _importView.Show();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        _saveWorkView.Show();
    }

    private void LoadButton_Click(object sender, RoutedEventArgs e)
    {
        _loadWorkView.Show();
    }

    private void HiddenEventsButton_Click(object sender, RoutedEventArgs e)
    {
        _hiddenEventsView.Show();
    }

    private void FilterButton_Click(object sender, RoutedEventArgs e)
    {
        _filterView.Show();
    }

    private void ChangeEventColourButton_Click(object sender, RoutedEventArgs e)
    {
        _changeColourView.Show();
    }

    private void ChangeColourByTypeButton_Click(object sender, RoutedEventArgs e)
    {
        _colourView.Show();
    }

    private void CheckIntegrityButton_Click(object sender, RoutedEventArgs e)
    {
        _integrityView.Show();
    }

    // TODO - move to command
    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
        MenuItem clickedMenuItem = (MenuItem)sender;
        string newLanguage = (string)clickedMenuItem.Tag;
        Uri resourceSource = App.Current.Resources.MergedDictionaries[0].Source;
        string[] segments = resourceSource.ToString().Split('/');
        string[] parts = segments[^1].Split('.');
        string oldLanguage = parts[1];

        foreach (MenuItem item in Languages.Items)
        {
            item.IsChecked = false;
        }

        clickedMenuItem.IsChecked = true;

        ChangeLanguage(newLanguage);

        ImportViewModel importViewModel = _importView.DataContext as ImportViewModel;
        importViewModel.ErrorsViewModel.UpdateErrorsLanguage(oldLanguage);

        FilterViewModel filterViewModel = _filterView.DataContext as FilterViewModel;
        filterViewModel.ErrorsViewModel.UpdateErrorsLanguage(oldLanguage);

        IntegrityViewModel integrityViewModel = _integrityView.DataContext as IntegrityViewModel;
        integrityViewModel.ErrorsViewModel.UpdateErrorsLanguage(oldLanguage);
    }

    private static void ChangeLanguage(string language)
    {
        ResourceDictionary dictionary = new();
        string dictionaryPath = $@"/Resources/Localizations/Resource.{language}.xaml";
        Uri uri = new(dictionaryPath, UriKind.Relative);
        dictionary.Source = uri;
        App.Current.Resources.MergedDictionaries.Clear();
        App.Current.Resources.MergedDictionaries.Add(dictionary);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        string folder = $@"Resources/Localizations";
        string fullPath = @$"{Directory.GetCurrentDirectory()}/../../../{folder}";
        string filter = "*.xaml";

        string[] paths = Directory.GetFiles(fullPath, filter);

        foreach (string path in paths)
        {
            string fileName = Path.GetFileName(path);
            string[] parts = fileName.Split('.');
            string locale = parts[1];
            string localeName = CultureInfo.GetCultureInfo(locale).NativeName;

            MenuItem menuItem = new() { Header = localeName, Tag = locale, IsChecked = locale == "en" };
            menuItem.Click += this.MenuItem_Click;
            Languages.Items.Add(menuItem);
        }
    }
}
