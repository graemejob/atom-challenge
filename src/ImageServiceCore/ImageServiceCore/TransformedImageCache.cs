using ImageServiceCore.BlobStorage;
using ImageServiceCore.ImageServiceRequestConverter;

namespace ImageServiceCore.ImageServiceCore
{
    public class TransformedImageCache : ITransformedImageCache
    {
        private readonly ICachedTransformBlobStorage cacheStorage;
        private readonly IEncodedStringImageTransformationRequestConverter requestConverter;

        public TransformedImageCache(ICachedTransformBlobStorage cacheStorage, IEncodedStringImageTransformationRequestConverter requestConverter)
        {
            this.cacheStorage = cacheStorage;
            this.requestConverter = requestConverter;
        }

        /// <summary>
        /// Reads bytes from cache, indexed to name and transform requirements
        /// </summary>
        /// <param name="request">Descriptor of the requested transformed image</param>
        public byte[] Get(ImageTransformationModel request)
        {
            var filename = requestConverter.ConvertTo(request);
            return cacheStorage.Get(filename);
        }

        /// <summary>
        /// Writes bytes to cache, indexed to name and transform requirements
        /// </summary>
        /// <param name="bytes">byte array containing the transformed image file data</param>
        /// <param name="request">Descriptor of the requested transformed image</param>
        public void Set(byte[] bytes, ImageTransformationModel request)
        {
            var filename = requestConverter.ConvertTo(request);
            cacheStorage.Set(filename, bytes);
        }

        /// <summary>
        /// Checks to see whether a transformed image exists in cache that satisfies the transform requirements
        /// </summary>
        /// <param name="request">Descriptor of the requested transformed image</param>
        /// <returns>true if transformed image exists in cache</returns>
        public bool Exists(ImageTransformationModel request)
        {
            var filename = requestConverter.ConvertTo(request);
            return cacheStorage.Exists(filename);
        }
    }
}
