using System.Windows;
using System.Windows.Controls;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Commands;

public class InitializeLanguagesCommand : CommandBase
{
    private readonly ImportViewModel _importViewModel;
    private readonly FilterViewModel _filterViewModel;
    private readonly IntegrityViewModel _integrityViewModel;
    private readonly IFileUtils _fileUtils;

    public InitializeLanguagesCommand(ImportViewModel importViewModel, FilterViewModel filterViewModel, IntegrityViewModel integrityViewModel, IFileUtils fileUtils)
    {
        _importViewModel = importViewModel;
        _filterViewModel = filterViewModel;
        _integrityViewModel = integrityViewModel;
        _fileUtils = fileUtils;
    }

    public override void Execute(object parameter)
    {
        RoutedEventArgs e = parameter as RoutedEventArgs;
        MenuItem languages = e.Source as MenuItem;
        string[] paths = _fileUtils.GetResourcesPaths();

        foreach (string path in paths)
        {
            (string code, string name) locale = _fileUtils.GetLocale(path);

            MenuItem menuItem = new() { Header = locale.name, Tag = locale.code, IsChecked = locale.code == "en" };
            menuItem.Click += this.MenuItem_Click;
            languages.Items.Add(menuItem);
        }

        ResourcesUtils.ChangeLanguage("en");
    }

    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
        MenuItem clickedMenuItem = (MenuItem)sender;
        MenuItem languages = clickedMenuItem.Parent as MenuItem;

        string newLanguage = (string)clickedMenuItem.Tag;
        string oldLanguage = ResourcesUtils.GetCurrentLanguage();

        foreach (MenuItem item in languages.Items)
        {
            item.IsChecked = false;
        }

        clickedMenuItem.IsChecked = true;
        ResourcesUtils.ChangeLanguage(newLanguage);

        _importViewModel.ErrorsViewModel.UpdateErrorsLanguage(oldLanguage);
        _filterViewModel.ErrorsViewModel.UpdateErrorsLanguage(oldLanguage);
        _integrityViewModel.ErrorsViewModel.UpdateErrorsLanguage(oldLanguage);
    }
}
