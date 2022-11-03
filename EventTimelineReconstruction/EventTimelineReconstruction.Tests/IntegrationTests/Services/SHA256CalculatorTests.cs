using EventTimelineReconstruction.Services;

namespace EventTimelineReconstruction.Tests.IntegrationTests.Services;

[TestClass]
public  class SHA256CalculatorTests
{
    private static IEnumerable<object[]> Files
    {
        get
        {
            return new[]
            {
                new object[]
                {
                    @"SHA256Empty.csv",
                    new byte[] { 227, 176, 196, 66, 152, 252, 28, 20, 154, 251, 244, 200, 153, 111, 185, 36, 39, 174, 65, 228, 100, 155, 147, 76, 164, 149, 153, 27, 120, 82, 184, 85 }
                },
                new object[]
                {
                    @"SHA256.csv",
                    new byte[] { 149, 66, 5, 158,120, 223, 210, 0, 227, 167, 252, 71, 207, 62, 207, 118, 83, 50, 213, 50, 110, 157, 165, 145, 247, 130, 47, 26, 137, 226, 92, 229 }
                }
            };
        }
    }

    [ClassInitialize]
    public static void Initialize(TestContext context)
    {
        using FileStream emptyStream = File.Create(@"SHA256Empty.csv");

        using StreamWriter writeStream = File.CreateText(@"SHA256.csv");
        writeStream.WriteLine("This is a file used to test SHA256 hash calculation.");
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        File.Delete(@"SHA256Empty.csv");
        File.Delete(@"SHA256.csv");
    }

    [DynamicData(nameof(Files))]
    [TestMethod]
    public async Task Calculate_ReturnByteArray_WhenMethodIsCalled(string path, byte[] expected)
    {
        // Arrange
        SHA256Calculator calculator = new();

        // Act 
        byte[] actual = await calculator.Calculate(path);

        // Assert
        Assert.AreEqual(expected.Length, actual.Length);

        for (int i = 0; i < expected.Length; i++)
        {
            Assert.AreEqual(expected[i], actual[i]);
        }
    }
}
