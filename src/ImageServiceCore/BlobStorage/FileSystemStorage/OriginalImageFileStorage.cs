using ImageServiceCore.BlobStorage;
using Microsoft.Extensions.Logging;

namespace ImageServiceCore.BlobStorage.FileSystemStorage
{
    public class OriginalImageFileStorage : FileStorage, IOriginalImageBlobStorage
    {
        public OriginalImageFileStorage(Options options, ILogger<OriginalImageFileStorage> logger) : base(options, logger)
        {
        }
    }
}
