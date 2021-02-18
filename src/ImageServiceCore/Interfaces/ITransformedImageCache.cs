using ImageServiceCore.ImageServiceRequestConverter;
using System.IO;

namespace ImageServiceCore.Interfaces
{
    public interface ITransformedImageCache
    {
        public bool Exists(ImageTransformationRequest request);
        public byte[] Get(ImageTransformationRequest request);
        public void Set(byte[] bytes, ImageTransformationRequest request);
    }
}
