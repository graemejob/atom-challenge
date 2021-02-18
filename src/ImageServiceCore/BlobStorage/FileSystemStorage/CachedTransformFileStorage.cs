using ImageServiceCore.BlobStorage;
using Microsoft.Extensions.Logging;

namespace ImageServiceCore.BlobStorage.FileSystemStorage
{
    public class CachedTransformFileStorage : FileStorage, ICachedTransformBlobStorage
    {
        public CachedTransformFileStorage(Options options, ILogger<CachedTransformFileStorage> logger) : base(options, logger)
        {
        }
    }
}
