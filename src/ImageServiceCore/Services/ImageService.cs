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
        /// <param name="watermark">Optional text to draw onto the image</param>
        /// <returns>Bytes represenging the requested image file</returns>
        public byte[] Get(string name, string format, (int? Width, int? Height) maxSize, string watermark)
        {
            if (string.IsNullOrEmpty(format) && !maxSize.Width.HasValue && !maxSize.Height.HasValue && string.IsNullOrWhiteSpace(watermark))
            {
                // No transformation requested. Return original image, and don't cache.
                if (!imageBlobStorage.Exists(name))
                {
                    logger.LogInformation($"{name} does not exist");
                    return null;
                }
                logger.LogInformation($"{name} requested without transformation. Serving original image");
                return imageBlobStorage.Get(name);
            }
            if (imageCache.Exists(name, format, maxSize, watermark))
            {
                logger.LogInformation($"{name} {format} {maxSize.Width}x{maxSize.Height} exists in cache. Serving from cache");
                // Transformation exists in cache storage. Return image
                return imageCache.Get(name, format, maxSize, watermark);
            }
            else
            {
                if (!imageBlobStorage.Exists(name))
                {
                    logger.LogInformation($"{name} does not exist");
                    return null;
                }
                // Image exists, but not the requested transform. Transform image.
                var imageBytes = imageBlobStorage.Get(name);

                var transformedImageBytes = imageTransformer.Transform(imageBytes, format, maxSize, watermark);

                Task.Run(() =>
                {
                    // We shouldn't wait for the image to be saved to cache before returning the image to the requester
                    imageCache.Set(transformedImageBytes, name, format, maxSize, watermark);
                    logger.LogInformation($"Saving {name} {format} {maxSize.Width}x{maxSize.Height} in cache");

                }).ConfigureAwait(false);

                logger.LogInformation($"{name} {format} {maxSize.Width}x{maxSize.Height} has been processed. Served from transformer");

                return transformedImageBytes;
            }
        }
    }
}
