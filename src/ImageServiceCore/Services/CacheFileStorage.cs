using ImageServiceCore.Interfaces;
using Microsoft.Extensions.Logging;

namespace ImageServiceCore.Services
{
    public class CacheFileStorage : FileStorage, ICacheBlobStorage
    {
        public CacheFileStorage(Options options, ILogger<CacheFileStorage> logger) : base(options, logger)
        {
        }
    }
}
