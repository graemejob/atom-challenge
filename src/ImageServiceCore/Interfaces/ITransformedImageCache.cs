using System.IO;

namespace ImageServiceCore.Interfaces
{
    public interface ITransformedImageCache
    {
        public bool Exists(string name, string format, (int? Width, int? Height) maxSize, string watermark);
        public byte[] Get(string name, string format, (int? Width, int? Height) maxSize, string watermark);
        public void Set(byte[] bytes, string name, string format, (int? Width, int? Height) maxSize, string watermark);
    }
}
