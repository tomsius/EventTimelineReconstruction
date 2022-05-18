using Microsoft.Win32;
using System.Windows;

namespace EventTimelineReconstruction.Views;

/// <summary>
/// Interaction logic for ImportView.xaml
/// </summary>
public partial class ImportView : Window
{
    public ImportView()
    {
        InitializeComponent();
    }

    private void ChooseFileButton_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new();
        openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";

        if (openFileDialog.ShowDialog() == true)
        {
            FileNameTextBox.Text = openFileDialog.FileName;
        }
    }
}
