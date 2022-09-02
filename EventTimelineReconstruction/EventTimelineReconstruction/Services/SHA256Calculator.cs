using System.IO;
using System.Security.Cryptography;

namespace EventTimelineReconstruction.Services;

public class SHA256Calculator : IHashCalculator
{
    public byte[] Calculate(string filePath)
    {
        using SHA256 hashFunction = SHA256.Create();
        using FileStream fileStream = File.OpenRead(filePath);
        fileStream.Position = 0;

        byte[] hashValue = hashFunction.ComputeHash(fileStream);
        return hashValue;
    }
}
