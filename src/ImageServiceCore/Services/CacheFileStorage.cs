using ImageServiceCore.Interfaces;

namespace ImageServiceCore.Services
{
    public class CacheFileStorage : FileStorage, ICacheBlobStorage
    {
        public CacheFileStorage(Options options) : base(options)
        {
        }
    }
}
