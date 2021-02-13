using System.IO;

namespace ImageServiceCore.Interfaces
{
    public interface IImageTransformer
    {
        public Stream Transform(Stream stream, string format, int maxWidth, int maxHeight, string watermark);
    }
}
