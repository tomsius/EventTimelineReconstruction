using System.Threading.Tasks;

namespace EventTimelineReconstruction.Services;

public interface IHashCalculator
{
    public Task<byte[]> Calculate(string filePath);
}
