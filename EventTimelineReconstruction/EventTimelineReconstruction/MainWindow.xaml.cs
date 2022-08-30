using System.Windows;
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

    public MainWindow(
        ImportView view, 
        SaveWorkView saveWorkView, 
        LoadWorkView loadWorkView, 
        HiddenEventsView hiddenEventsView, 
        FilterView filterView,
        ChangeColourView changeColourView,
        ColourView colourView)
    {
        this.InitializeComponent();
        _importView = view;
        _saveWorkView = saveWorkView;
        _loadWorkView = loadWorkView;
        _hiddenEventsView = hiddenEventsView;
        _filterView = filterView;
        _changeColourView = changeColourView;
        _colourView = colourView;
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
}
