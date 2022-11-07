using System.Windows;

namespace EventTimelineReconstruction.Tests;

[TestClass]
public class AssemblyInitializationAndCleanup
{
    private static Application _app;

    [AssemblyInitialize]
    public static void AssemblyInit(TestContext context)
    {
        if (Application.Current == null)
        {
            _app = new Application();
        }
    }

    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
        _app.Shutdown();
    }
}
