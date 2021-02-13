using System.IO;

namespace ImageServiceCore.Interfaces
{
    public interface ITransformedImageCache
    {
        Stream Get(string name, string format, int maxWidth, int maxHeight, string watermark);
        void Set(Stream stream, string name, string format, int maxWidth, int maxHeight, string watermark);
    }
}
