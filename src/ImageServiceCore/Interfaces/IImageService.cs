using System.IO;

namespace ImageServiceCore.Interfaces
{
    public interface IImageService
    {
        public Stream Get(string name, string format, int? maxWidth, int? maxHeight, string watermark);
    }
}
