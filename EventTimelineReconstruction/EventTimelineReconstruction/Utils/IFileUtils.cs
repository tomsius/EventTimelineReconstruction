namespace EventTimelineReconstruction.Utils;

public interface IFileUtils
{
    public (string, string) GetLocale(string path);
    public string[] GetResourcesPaths();
}