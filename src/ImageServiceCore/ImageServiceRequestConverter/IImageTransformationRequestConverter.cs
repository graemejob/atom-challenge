using ImageServiceCore.ImageServiceCore;

namespace ImageServiceCore.ImageServiceRequestConverter
{
    public interface IImageTransformationRequestConverter<T>
    {
        public ImageTransformationModel ConvertFrom(T source);
        public T ConvertTo(ImageTransformationModel source);
    }
}
