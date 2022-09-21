using System;
using System.Globalization;
using System.IO;

namespace EventTimelineReconstruction.Utils;

public static class FileUtils
{
    private const string _languagesFolder = $@"Resources/Localizations";
    private const string _fileExtension = $@"*.xaml";

    public static string[] GetResourcesPaths()
    {
        string fullPath = @$"{Directory.GetCurrentDirectory()}/../../../{_languagesFolder}";
        string[] paths = Directory.GetFiles(fullPath, _fileExtension);

        return paths;
    }

    public static (string, string) GetLocale(string path)
    {
        string fileName = Path.GetFileName(path);
        string[] parts = fileName.Split('.');
        string locale = parts[1];
        string localeName = CultureInfo.GetCultureInfo(locale).NativeName;

        return (locale, localeName);
    }
}
