namespace EventTimelineReconstruction.Services;

public interface IHashCalculator
{
    public byte[] Calculate(string filePath);
}
