using System.IO;

namespace punkOptimise.Interfaces
{
    public interface IImageReductionService
    {
        byte[] ReduceQuality(byte[] bytes);
        byte[] ReduceQuality(Stream inStream);
    }
}
