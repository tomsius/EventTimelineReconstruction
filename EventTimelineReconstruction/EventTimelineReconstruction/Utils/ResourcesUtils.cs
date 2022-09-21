using System;
using System.Windows;

namespace EventTimelineReconstruction.Utils;

public static class ResourcesUtils
{
    public static string GetCurrentLanguage()
    {
        Uri resourceSource = App.Current.Resources.MergedDictionaries[0].Source;
        string[] segments = resourceSource.ToString().Split('/');
        string[] parts = segments[^1].Split('.');
        string language = parts[1];

        return language;
    }

    internal static void ChangeLanguage(string language)
    {
        ResourceDictionary dictionary = new();
        string dictionaryPath = $@"/Resources/Localizations/Resource.{language}.xaml";
        Uri uri = new(dictionaryPath, UriKind.Relative);
        dictionary.Source = uri;

        App.Current.Resources.MergedDictionaries.Clear();
        App.Current.Resources.MergedDictionaries.Add(dictionary);
    }
}
