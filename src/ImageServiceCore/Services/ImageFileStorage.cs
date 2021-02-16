using ImageServiceCore.Interfaces;
using Microsoft.Extensions.Logging;

namespace ImageServiceCore.Services
{
    public class ImageFileStorage : FileStorage, IImageBlobStorage
    {
        public ImageFileStorage(Options options, ILogger<ImageFileStorage> logger) : base(options, logger)
        {
        }
    }
}
