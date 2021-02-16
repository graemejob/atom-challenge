using ImageServiceCore.Interfaces;
using System;
using System.IO;
using System.Text;

namespace ImageServiceCore.Services
{
    public class TransformedImageCache : ITransformedImageCache
    {
        private readonly ICacheBlobStorage blobStorage;

        public TransformedImageCache(ICacheBlobStorage blobStorage)
        {
            this.blobStorage = blobStorage;
        }

        /// <summary>
        /// Reads bytes from cache, indexed to name and transform requirements
        /// </summary>
        /// <param name="name">Name of the original image</param>
        /// <param name="format">Format of the transformed image</param>
        /// <param name="maxSize">Max width and/or height of the transformed image</param>
        /// <param name="watermark">Overlay text</param>
        public byte[] Get(string name, string format, (int? Width, int? Height) maxSize, string watermark)
        {
            var filename = GenerateFileName(name, format, maxSize, watermark);
            return blobStorage.Get(filename);
        }

        /// <summary>
        /// Writes bytes to cache, indexed to name and transform requirements
        /// </summary>
        /// <param name="name">Name of the original image</param>
        /// <param name="format">Format of the transformed image</param>
        /// <param name="maxSize">Max width and/or height of the transformed image</param>
        /// <param name="watermark">Overlay text</param>
        public void Set(byte[] bytes, string name, string format, (int? Width, int? Height) maxSize, string watermark)
        {
            var filename = GenerateFileName(name, format, maxSize, watermark);
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
        public bool Exists(string name, string format, (int? Width, int? Height) maxSize, string watermark)
        {
            var filename = GenerateFileName(name, format, maxSize, watermark);
            return blobStorage.Exists(filename);
        }

        // Generate a filename which is unique to the original image, and the requested transform
        private string GenerateFileName(string name, string format, (int? Width, int? Height) maxSize, string watermark)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(name);
            if (!string.IsNullOrEmpty(format) || maxSize.Width.HasValue || maxSize.Height.HasValue || !string.IsNullOrWhiteSpace(watermark))
            {
                sb.Append("!");
                if (maxSize.Width.HasValue) sb.Append($"w{maxSize.Width.Value}");
                if (maxSize.Height.HasValue) sb.Append($"h{maxSize.Height.Value}");
                if (!string.IsNullOrWhiteSpace(watermark)) sb.Append($"t{EncodeWatermark(watermark)}");
                if (!string.IsNullOrEmpty(format)) sb.Append($".{format}");
                else sb.Append(Path.GetExtension(name));
            }
            return sb.ToString();
        }

        // Encode UTF8 watermark into filename-friendly characters
        private string EncodeWatermark(string watermark)
        {
            // Base64 is case-sensitive, which some filesystems aren't. So the choice is Hex.
            return Convert.ToHexString(Encoding.UTF8.GetBytes(watermark));
        }
    }
}
