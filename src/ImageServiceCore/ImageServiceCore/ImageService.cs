using ImageServiceCore.BlobStorage;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ImageServiceCore.ImageServiceCore
{
    public class ImageService : IImageService
    {
        private readonly ITransformedImageCache imageCacheHandler;
        private readonly IOriginalImageBlobStorage originalImageStorage;
        private readonly IImageTransformer imageTransformer;
        private readonly ILogger<ImageService> logger;

        public ImageService(
            ITransformedImageCache imageCacheHandler,
            IOriginalImageBlobStorage originalImageStorage,
            IImageTransformer imageTransformer,
            ILogger<ImageService> logger)
        {
            this.imageCacheHandler = imageCacheHandler;
            this.originalImageStorage = originalImageStorage;
            this.imageTransformer = imageTransformer;
            this.logger = logger;
        }

        /// <summary>
        /// Attempt to get image from transformed image cache, or transform original image and store in cache, or return image if no transformation requested
        /// </summary>
        /// <param name="request">Descriptor of the requested transformed image</param>
        /// <returns>Bytes represenging the requested image file</returns>
        public byte[] Get(ImageTransformationModel request)
        {
            if (request.IsOriginalImage)
            {
                // No transformation requested. Return original image, and don't cache.
                return OriginalImage(request);
            }
            if (imageCacheHandler.Exists(request))
            {
                // Transformation exists in cache storage. Return image
                return ImageFromCache(request);
            }
            else if (!originalImageStorage.Exists(request.Name))
            {
                // The original image doesn't exist.
                return ImageDoesNotExist(request);
            }
            else
            {
                // Image exists, but the requested transform doesn't exist in cache. Transform image, return and cache.
                return TransformExistingImageAndCache(request);
            }
        }

        private byte[] TransformExistingImageAndCache(ImageTransformationModel request)
        {
            var originalImageBytes = originalImageStorage.Get(request.Name);

            var transformedImageBytes = imageTransformer.Transform(originalImageBytes, request);

            CacheTransformedImage(request, transformedImageBytes);

            logger.LogInformation($"{request} has been processed. Served from transformer");

            return transformedImageBytes;
        }

        private void CacheTransformedImage(ImageTransformationModel request, byte[] transformedImageBytes)
        {
            Task.Run(() =>
            {
                // We shouldn't wait for the image to be saved to cache before returning the image to the requester
                imageCacheHandler.Set(transformedImageBytes, request);
                logger.LogInformation($"Saving {request} in cache");

            }).ConfigureAwait(false);
        }

        private byte[] ImageDoesNotExist(ImageTransformationModel request)
        {
            logger.LogInformation($"{request} does not exist");
            return null;
        }

        private byte[] ImageFromCache(ImageTransformationModel request)
        {
            logger.LogInformation($"{request} exists in cache. Serving from cache");
            return imageCacheHandler.Get(request);
        }

        private byte[] OriginalImage(ImageTransformationModel request)
        {
            if (!originalImageStorage.Exists(request.Name))
            {
                logger.LogInformation($"{request.Name} does not exist");
                return null;
            }
            logger.LogInformation($"{request.Name} requested without transformation. Serving original image");
            return originalImageStorage.Get(request.Name);
        }
    }
}
