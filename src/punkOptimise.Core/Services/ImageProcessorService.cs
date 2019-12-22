using ImageProcessor;
using punkOptimise.Extensions;
using punkOptimise.Interfaces;
using System.Configuration;
using System.IO;

namespace punkOptimise.Services
{
    public class ImageProcessorService : IImageReductionService
    {
        private readonly int _defaultQuality;

        public ImageProcessorService()
        {
            _defaultQuality = int.Parse(ConfigurationManager.AppSettings["punkOptimise:DefaultQuality"]);
        }

        public byte[] ReduceQuality(byte[] bytes)
        {
            using (MemoryStream inStream = new MemoryStream(bytes))
                return ReduceQuality(inStream);
        }

        public byte[] ReduceQuality(Stream inStream)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                {
                    imageFactory.Load(inStream)
                                .Quality(_defaultQuality)
                                .Save(outStream);
                }
                return outStream.ReadAsBytes();
            }
        }
    }
}
