using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace EventTimelineReconstruction.Services;

public sealed class SHA256Calculator : IHashCalculator
{
    public async Task<byte[]> Calculate(string filePath)
    {
        using SHA256 hashFunction = SHA256.Create();
        using FileStream fileStream = File.OpenRead(filePath);
        fileStream.Position = 0;

        byte[] hashValue = await hashFunction.ComputeHashAsync(fileStream);
        return hashValue;
    }
}
