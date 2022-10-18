using EventTimelineReconstruction.Models;
using EventTimelineReconstruction.Services;

namespace EventTimelineReconstruction.Tests.IntegrationTests.Services;

[TestClass]
public class L2tCSVEventsImporterTests
{
    [ClassInitialize]
    public static void Initialize(TestContext context)
    {
        using FileStream emptyStream = File.Create(@"EventsImporterEmpty.csv");

        using StreamWriter writeStream = File.CreateText(@"EventsImporter.csv");
        writeStream.WriteLine(@"date,time,timezone,MACB,source,sourcetype,type,user,host,short,desc,version,filename,inode,notes,format,extra");
        writeStream.WriteLine(@"01/01/1970,00:00:00,UTC,....,WEBHIST,MSIE Cache File URL record,Expiration Time,-,PC1-5DFC89FB1E0,Location: Visited: PC1@about:Home,Location: Visited: PC1@about:Home Number of hits: 2 Cached file size: 0,2,TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/index.dat,10536,-,msiecf,cache_directory_index: -2; recovered: False; sha256_hash: 243645de118fab85ae3e5f4f820ee50717ddf478f17f7a678c88aa5d437a7e70");
        writeStream.WriteLine(@"01/01/2003,00:00:00,UTC,....,WEBHIST,MSIE Cache File URL record,Expiration Time,-,PC1-5DFC89FB1E0,Location: Visited: PC1@about:Home,Location: Visited: PC1@about:Home Number of hits: 2 Cached file size: 0,2,TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/index.dat,10536,-,msiecf,cache_directory_index: -2; recovered: False; sha256_hash: 243645de118fab85ae3e5f4f820ee50717ddf478f17f7a678c88aa5d437a7e70");
        writeStream.WriteLine(@"01/01/2020,15:25:55,UTC,....,REG,AppCompatCache Registry Entry,File Last Modification Time,-,PC1-5DFC89FB1E0,[HKEY_LOCAL_MACHINE\System\ControlSet001\Control\Session Manager\AppCompatibi...,[HKEY_LOCAL_MACHINE\System\ControlSet001\Control\Session Manager\AppCompatibility] Cached entry: 28,2,TSK:/System Volume Information/_restore{5D162324-8035-4BDB-B6BA-8D2C3C5FBFF0}/RP2/snapshot/_REGISTRY_MACHINE_SYSTEM,13932,-,winreg/appcompatcache,sha256_hash: c822dbf91f7d96c0fcea412ed5ad22d8b1b0b7047357153d631940ac89042e38");
        writeStream.WriteLine(@"00:00:00,UTC,....,REG,AppCompatCache Registry Entry,File Last Modification Time,-,PC1-5DFC89FB1E0,[HKEY_LOCAL_MACHINE\System\ControlSet002\Control\Session Manager\AppCompatibi...,[HKEY_LOCAL_MACHINE\System\ControlSet002\Control\Session Manager\AppCompatibility] Cached entry: 28,2,TSK:/System Volume Information/_restore{5D162324-8035-4BDB-B6BA-8D2C3C5FBFF0}/RP2/snapshot/_REGISTRY_MACHINE_SYSTEM,13932,-,winreg/appcompatcache,sha256_hash: c822dbf91f7d96c0fcea412ed5ad22d8b1b0b7047357153d631940ac89042e38");
        writeStream.WriteLine(@"abc,00:00:00,UTC,....,REG,AppCompatCache Registry Entry,File Last Modification Time,-,PC1-5DFC89FB1E0,[HKEY_LOCAL_MACHINE\System\ControlSet002\Control\Session Manager\AppCompatibi...,[HKEY_LOCAL_MACHINE\System\ControlSet002\Control\Session Manager\AppCompatibility] Cached entry: 28,2,TSK:/WINDOWS/system32/config/system,7608,-,winreg/appcompatcache,sha256_hash: 9e3e9f916979ebaee33b80d75d7dc2b9e58fed306a69286b75cf3e14ade38d77");
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        File.Delete(@"EventsImporterEmpty.csv");
        File.Delete(@"EventsImporter.csv");
    }

    [TestMethod]
    public void Import_ShouldReturnEmptyList_WhenFileIsEmpty()
    {
        // Arrange
        L2tCSVEventsImporter importer = new();
        int expected = 0;

        // Test for threading issues
        for (int k = 0; k < 1000; k++)
        {
            // Act
            int actual = importer.Import("EventsImporterEmpty.csv", DateTime.MinValue, DateTime.MaxValue).Count;

            // Assert
            Assert.AreEqual(expected, actual); 
        }
    }

    [TestMethod]
    public void Import_ShouldReturnEventsInDateRange_WhenFileHasEvents()
    {
        // Arrange
        L2tCSVEventsImporter importer = new();
        List<EventModel> expected = new()
        {
            new EventModel(
                new DateOnly(2003, 1, 1),
                new TimeOnly(0, 0, 0),
                TimeZoneInfo.Utc,
                "....",
                "WEBHIST",
                "MSIE Cache File URL record",
                "Expiration Time",
                "-",
                "PC1-5DFC89FB1E0",
                "Location: Visited: PC1@about:Home",
                "Location: Visited: PC1@about:Home Number of hits: 2 Cached file size: 0",
                2,
                "TSK:/Documents and Settings/PC1/Local Settings/History/History.IE5/index.dat",
                "10536",
                "-",
                "msiecf",
                new Dictionary<string, string>() { { "cache_directory_index", "-2" }, { "recovered", "False" }, { "sha256_hash", "243645de118fab85ae3e5f4f820ee50717ddf478f17f7a678c88aa5d437a7e70" } }
                ),
            new EventModel(
                new DateOnly(2020, 1, 1),
                new TimeOnly(15, 25, 55),
                TimeZoneInfo.Utc,
                "....",
                "REG",
                "AppCompatCache Registry Entry",
                "File Last Modification Time",
                "-",
                "PC1-5DFC89FB1E0",
                "[HKEY_LOCAL_MACHINE\\System\\ControlSet001\\Control\\Session Manager\\AppCompatibi...",
                "[HKEY_LOCAL_MACHINE\\System\\ControlSet001\\Control\\Session Manager\\AppCompatibility] Cached entry: 28",
                2,
                "TSK:/System Volume Information/_restore{5D162324-8035-4BDB-B6BA-8D2C3C5FBFF0}/RP2/snapshot/_REGISTRY_MACHINE_SYSTEM",
                "13932",
                "-",
                "winreg/appcompatcache",
                new Dictionary<string, string>() { { "sha256_hash", "c822dbf91f7d96c0fcea412ed5ad22d8b1b0b7047357153d631940ac89042e38" } }
                )
        };

        // Test for threading issues
        for (int k = 0; k < 1000; k++)
        {
            // Act
            List<EventModel> actual = importer.Import("EventsImporter.csv", new DateTime(2000, 1, 1), DateTime.MaxValue).OrderBy(e => e.Date).ThenBy(e => e.Time).ThenBy(e => e.Filename).ToList();

            // Assert
            Assert.AreEqual(expected.Count, actual.Count);

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i].Date, actual[i].Date);
                Assert.AreEqual(expected[i].Time, actual[i].Time);
                Assert.AreEqual(expected[i].Timezone, actual[i].Timezone);
                Assert.AreEqual(expected[i].MACB, actual[i].MACB);
                Assert.AreEqual(expected[i].Source, actual[i].Source);
                Assert.AreEqual(expected[i].SourceType, actual[i].SourceType);
                Assert.AreEqual(expected[i].Type, actual[i].Type);
                Assert.AreEqual(expected[i].User, actual[i].User);
                Assert.AreEqual(expected[i].Host, actual[i].Host);
                Assert.AreEqual(expected[i].Short, actual[i].Short);
                Assert.AreEqual(expected[i].Description, actual[i].Description);
                Assert.AreEqual(expected[i].Version, actual[i].Version);
                Assert.AreEqual(expected[i].Filename, actual[i].Filename);
                Assert.AreEqual(expected[i].INode, actual[i].INode);
                Assert.AreEqual(expected[i].Notes, actual[i].Notes);
                Assert.AreEqual(expected[i].Format, actual[i].Format);
                Assert.AreEqual(expected[i].Extra.Count, actual[i].Extra.Count);

                foreach (KeyValuePair<string, string> kvp in expected[i].Extra)
                {
                    string expectedKey = kvp.Key;
                    string expectedValue = kvp.Value;

                    Assert.IsTrue(actual[i].Extra.ContainsKey(expectedKey));
                    Assert.AreEqual(expectedValue, actual[i].Extra[expectedKey]);
                }
            } 
        }
    }
}
