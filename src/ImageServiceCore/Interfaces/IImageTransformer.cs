using System.IO;

namespace ImageServiceCore.Interfaces
{
    public interface IImageTransformer
    {
        public byte[] Transform(byte[] bytes, string format, (int? Width, int? Height) maxSize, string colour, string watermark);
    }
}
