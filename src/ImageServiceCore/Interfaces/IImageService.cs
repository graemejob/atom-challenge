using System.IO;

namespace ImageServiceCore.Interfaces
{
    public interface IImageService
    {
        public byte[] Get(string name, string format, (int? Width, int? Height) maxSize, string colour, string watermark);
    }
}
