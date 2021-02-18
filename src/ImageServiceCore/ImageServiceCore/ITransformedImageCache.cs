using System.IO;

namespace ImageServiceCore.ImageServiceCore
{
    public interface ITransformedImageCache
    {
        public bool Exists(ImageTransformationModel request);
        public byte[] Get(ImageTransformationModel request);
        public void Set(byte[] bytes, ImageTransformationModel request);
    }
}
