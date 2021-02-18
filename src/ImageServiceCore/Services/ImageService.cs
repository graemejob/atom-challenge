using ImageServiceCore.ImageServiceRequestConverter;
using ImageServiceCore.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ImageServiceCore.Services
{
    public class ImageService : IImageService
    {
        private readonly ITransformedImageCache imageCache;
        private readonly IImageBlobStorage imageBlobStorage;
        private readonly IImageTransformer imageTransformer;
        private readonly ILogger<ImageService> logger;

        public ImageService( ITransformedImageCache imageCache, IImageBlobStorage imageBlobStorage, IImageTransformer imageTransformer, ILogger<ImageService> logger)
        {
            this.imageCache = imageCache;
            this.imageBlobStorage = imageBlobStorage;
            this.imageTransformer = imageTransformer;
            this.logger = logger;
        }

        /// <summary>
        /// Attempt to get image from transformed image cache, or transform original image and store in cache, or return image if no transformation requested
        /// </summary>
        /// <param name="name">Name of the original image</param>
        /// <param name="format">extension of the desired output format (eg "png", "jpeg", etc)</param>
        /// <param name="maxSize">Maximum width and height the returned image can have. Either dimension can be null</param>
        /// <param name="colour">Optional background colour</param>
        /// <param name="watermark">Optional text to draw onto the image</param>
        /// <returns>Bytes represenging the requested image file</returns>
        public byte[] Get(ImageTransformationRequest request)
        {
            if (string.IsNullOrEmpty(request.Format) &&
                !request.MaxWidth.HasValue &&
                !request.MaxHeight.HasValue &&
                string.IsNullOrWhiteSpace(request.Watermark) &&
                string.IsNullOrWhiteSpace(request.Colour))
            {
                // No transformation requested. Return original image, and don't cache.
                if (!imageBlobStorage.Exists(request.Name))
                {
                    logger.LogInformation($"{request.Name} does not exist");
                    return null;
                }
                logger.LogInformation($"{request.Name} requested without transformation. Serving original image");
                return imageBlobStorage.Get(request.Name);
            }
            if (imageCache.Exists(request))
            {
                logger.LogInformation($"{request.Name} {request.Format} {request.MaxWidth}x{request.MaxHeight} exists in cache. Serving from cache");
                // Transformation exists in cache storage. Return image
                return imageCache.Get(request);
            }
            else
            {
                if (!imageBlobStorage.Exists(request.Name))
                {
                    logger.LogInformation($"{request.Name} does not exist");
                    return null;
                }
                // Image exists, but not the requested transform. Transform image.
                var imageBytes = imageBlobStorage.Get(request.Name);

                var transformedImageBytes = imageTransformer.Transform(imageBytes, request);

                Task.Run(() =>
                {
                    // We shouldn't wait for the image to be saved to cache before returning the image to the requester
                    imageCache.Set(transformedImageBytes, request);
                    logger.LogInformation($"Saving {request.Name} {request.Format} {request.MaxWidth}x{request.MaxHeight} in cache");

                }).ConfigureAwait(false);

                logger.LogInformation($"{request.Name} {request.Format} {request.MaxWidth}x{request.MaxHeight} has been processed. Served from transformer");

                return transformedImageBytes;
            }
        }
    }
}
