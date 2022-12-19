using System;
using System.IO;
using System.Windows;

namespace EventTimelineReconstruction.Utils;

public class ResourcesUtils : IResourcesUtils
{
    public string GetCurrentLanguage()
    {
        Uri resourceSource = App.Current.Resources.MergedDictionaries[0].Source;
        string[] segments = resourceSource.ToString().Split('/');
        string[] parts = segments[^1].Split('.');
        string language = parts[1];

        return language;
    }

    public void ChangeLanguage(string language)
    {
        ResourceDictionary dictionary = new();
        string dictionaryPath = $@"{AppDomain.CurrentDomain.BaseDirectory}Resources/Localizations/Resource.{language}.xaml";
        Uri uri = new(dictionaryPath, UriKind.Absolute);
        dictionary.Source = uri;

        App.Current.Resources.MergedDictionaries.Clear();
        App.Current.Resources.MergedDictionaries.Add(dictionary);
    }
}
