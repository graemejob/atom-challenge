using ImageServiceCore.ImageServiceRequestConverter;
using System.IO;

namespace ImageServiceCore.Interfaces
{
    public interface IImageService
    {
        public byte[] Get(ImageTransformationRequest request);
    }
}
