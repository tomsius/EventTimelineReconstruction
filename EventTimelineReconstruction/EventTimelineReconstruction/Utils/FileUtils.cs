using System;
using System.Globalization;
using System.IO;

namespace EventTimelineReconstruction.Utils;

public sealed class FileUtils : IFileUtils
{
    private const string _languagesFolder = $@"Resources/Localizations";
    private const string _fileExtension = $@"*.xaml";

    public string[] GetResourcesPaths()
    {
        string fullPath = @$"{AppDomain.CurrentDomain.BaseDirectory}{_languagesFolder}";
        string[] paths = Directory.GetFiles(fullPath, _fileExtension);

        return paths;
    }

    public (string, string) GetLocale(string path)
    {
        string fileName = Path.GetFileName(path);
        string[] parts = fileName.Split('.');
        string locale = parts[1];
        string localeName = CultureInfo.GetCultureInfo(locale).NativeName;

        return (locale, localeName);
    }
}
