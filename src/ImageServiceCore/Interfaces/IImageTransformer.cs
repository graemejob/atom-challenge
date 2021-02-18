using ImageServiceCore.ImageServiceRequestConverter;
using System.IO;

namespace ImageServiceCore.Interfaces
{
    public interface IImageTransformer
    {
        public byte[] Transform(byte[] bytes, ImageTransformationRequest request);
    }
}
