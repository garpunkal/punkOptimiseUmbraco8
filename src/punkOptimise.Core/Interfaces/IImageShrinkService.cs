using System.IO;

namespace punkOptimise.Interfaces
{
    public interface IImageShrinkService
    {
        byte[] ShrinkImage(byte[] data);
        byte[] ShrinkImage(Stream inStream);
        bool IsEnabled();
    }
}
