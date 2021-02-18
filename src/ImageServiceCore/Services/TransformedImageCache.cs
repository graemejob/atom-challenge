using ImageServiceCore.ImageServiceRequestConverter;
using ImageServiceCore.Interfaces;
using System;
using System.Text;

namespace ImageServiceCore.Services
{
    public class TransformedImageCache : ITransformedImageCache
    {
        private readonly ICacheBlobStorage blobStorage;
        private readonly IEncodedStringImageTransformationRequestConverter requestConverter;

        public TransformedImageCache(ICacheBlobStorage blobStorage, IEncodedStringImageTransformationRequestConverter requestConverter)
        {
            this.blobStorage = blobStorage;
            this.requestConverter = requestConverter;
        }

        /// <summary>
        /// Reads bytes from cache, indexed to name and transform requirements
        /// </summary>
        /// <param name="name">Name of the original image</param>
        /// <param name="format">Format of the transformed image</param>
        /// <param name="maxSize">Max width and/or height of the transformed image</param>
        /// <param name="watermark">Overlay text</param>
        public byte[] Get(ImageTransformationRequest request)
        {
            var filename = requestConverter.ConvertTo(request);
            return blobStorage.Get(filename);
        }

        /// <summary>
        /// Writes bytes to cache, indexed to name and transform requirements
        /// </summary>
        /// <param name="name">Name of the original image</param>
        /// <param name="format">Format of the transformed image</param>
        /// <param name="maxSize">Max width and/or height of the transformed image</param>
        /// <param name="watermark">Overlay text</param>
        public void Set(byte[] bytes, ImageTransformationRequest request)
        {
            var filename = requestConverter.ConvertTo(request);
            blobStorage.Set(filename, bytes);
        }

        /// <summary>
        /// Checks to see whether a transformed image exists in cache that satisfies the transform requirements
        /// </summary>
        /// <param name="name">Name of the original image</param>
        /// <param name="format">Format of the transformed image</param>
        /// <param name="maxSize">Max width and/or height of the transformed image</param>
        /// <param name="watermark">Overlay text</param>
        /// <returns>true if transformed image exists in cache</returns>
        public bool Exists(ImageTransformationRequest request)
        {
            var filename = requestConverter.ConvertTo(request);
            return blobStorage.Exists(filename);
        }

        // Encode UTF8 watermark into filename-friendly characters
        private string EncodeWatermark(string watermark)
        {
            // Base64 is case-sensitive, which some filesystems aren't. So the choice is Hex.
            return Convert.ToHexString(Encoding.UTF8.GetBytes(watermark));
        }
    }
}
