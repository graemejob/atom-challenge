using ImageServiceCore.Interfaces;

namespace ImageServiceCore.Services
{
    public class ImageFileStorage : FileStorage, IImageBlobStorage
    {
        public ImageFileStorage(Options options) : base(options)
        {
        }
    }
}
