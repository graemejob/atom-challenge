using ImageServiceCore.Interfaces;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace ImageServiceCore.Services
{
    public class BitmapImageTransformer : IImageTransformer
    {
        private readonly Font font;
        private readonly Brush brush;
        private readonly ILogger<BitmapImageTransformer> logger;

        public BitmapImageTransformer(ILogger<BitmapImageTransformer> logger)
        {
            this.font = new Font(FontFamily.GenericSansSerif, 16F, FontStyle.Bold | FontStyle.Italic);
            this.brush = new SolidBrush(Color.FromArgb(32, Color.Black));
            this.logger = logger;
        }

        /// <summary>
        /// Transform image bytes from one format or resolution to another, optionally with a watermark
        /// </summary>
        /// <param name="bytes">Byte array containing the image file data</param>
        /// <param name="format">extension of the desired output format (eg "png", "jpeg", etc)</param>
        /// <param name="maxSize">Maximum width and/or height the returned image can have, or null if there is no width and/or height constraint</param>
        /// <param name="watermark">Optional text to draw onto the image</param>
        /// <returns>Array of bytes representing the transformed image file</returns>
        public byte[] Transform(byte[] bytes, string format, (int? Width, int? Height) maxSize, string watermark)
        {
            var stopwatch = Stopwatch.StartNew();

            byte[] returnBytes = null;

            using (var image = LoadImage(bytes))
            {
                var currentImageFormat = image.RawFormat;
                var newImageFormat = ParseImageFormatOrDefault(format) ?? currentImageFormat;
                var currentDims = (image.Width, image.Height);
                var newDims = CalculateNewDimensions((image.Width, image.Height), maxSize);

                if (string.IsNullOrWhiteSpace(watermark) && newDims == currentDims && newImageFormat == currentImageFormat)
                {
                    logger.LogTrace("No transformation required. Returning original image");
                    // No transformation required
                    returnBytes = bytes;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(watermark))
                    {
                        logger.LogTrace("Drawing watermark");
                        DrawWatermark(watermark, image);
                    }

                    if (newDims != currentDims)
                    {
                        logger.LogTrace("Resizing image");
                        using (var resizedImage = ResizeImage(image, newDims))
                        {
                            returnBytes = SaveImageToByteArray(resizedImage, newImageFormat);
                        }
                    }
                    else
                    {
                        returnBytes = SaveImageToByteArray(image, newImageFormat);
                    }
                }
            }
            logger.LogInformation($"Transformed image in {stopwatch.ElapsedMilliseconds} ms");
            
            return returnBytes;
        }

        // Create image from byte array
        private Image LoadImage(byte[] bytes)
        {
            using (MemoryStream inputStream = new MemoryStream(bytes))
            {
                var bitmap = Bitmap.FromStream(inputStream);
                return bitmap;
            }
        }

        // Draw watermark text on image
        private void DrawWatermark(string watermark, Image image)
        {
            using (var graphics = CreateGraphics(image))
            {
                // Calculate position of text if bottom-right of image
                var strBounds = graphics.MeasureString(watermark, font, new SizeF(image.Width, image.Height), new StringFormat());
                var strLayoutRect = new RectangleF(image.Width - strBounds.Width, image.Height - strBounds.Height, strBounds.Width, strBounds.Height);
                
                graphics.DrawString(watermark, font, brush, strLayoutRect);
            }
        }

        // Create new image and copy resized image onto it
        private Image ResizeImage(Image image, (int Width, int Height) newDims)
        {
            var resizedBitmap = new Bitmap(newDims.Width, newDims.Height, image.PixelFormat);
            
            using (var graphics = CreateGraphics(resizedBitmap))
            {
                graphics.DrawImage(image, 0, 0, newDims.Width, newDims.Height);
            }
            return resizedBitmap;
            
        }

        // Save image object to byte array
        private byte[] SaveImageToByteArray(Image image, ImageFormat imageFormat)
        {
            using (var outputStream = new MemoryStream())
            {
                image.Save(outputStream, imageFormat);
                return outputStream.ToArray();
            }
        }

        // Take format string and return a suitable ImageFormat object, or null
        private ImageFormat ParseImageFormatOrDefault(string format)
        {
            return format switch
            {
                "gif" => ImageFormat.Gif,
                "jpeg" => ImageFormat.Jpeg,
                "jpg" => ImageFormat.Jpeg,
                "png" => ImageFormat.Png,
                _ => null
            };
        }

        // Create a Graphics instance with high quality options set
        private Graphics CreateGraphics(Image image)
        {
            var graphics = Graphics.FromImage(image);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.CompositingMode = CompositingMode.SourceOver;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            return graphics;
        }

        // Take current image width & height and generate new dimensions given the maximum width and height
        // constraints, if specified. Aspect ratio is conserved.
        private (int Width, int Height) CalculateNewDimensions((int Width, int Height) imageSize, (int? Width, int? Height) maxSize)
        {
            if (imageSize.Width <= 0 || imageSize.Height <= 0) return (0, 0);

            // If no resize info has been provided, return the original dimensions
            if (!maxSize.Height.HasValue && !maxSize.Width.HasValue) return imageSize;

            if (maxSize.Width.HasValue && maxSize.Width <= 0 || maxSize.Height.HasValue && maxSize.Height <= 0) return (0, 0);

            // Calculate the width and height counterparts for maxWidth and maxHeight, with respect to the aspect ratio.
            var scaledHeight = imageSize.Height * maxSize.Width / imageSize.Width;
            var scaledWidth = imageSize.Width * maxSize.Height / imageSize.Height;
            if (!maxSize.Height.HasValue)
            {
                // Only maxWidth has been specified. scaledHeight is unbound
                return (maxSize.Width.Value, scaledHeight.Value);
            }
            else if (!maxSize.Width.HasValue)
            {
                // Only maxHeight has been specified. scaledWidth is unbound
                return (scaledWidth.Value, maxSize.Height.Value);
            }
            else
            {
                // Both maxWidth and maxHeight have been specified. Choose whichever
                if (scaledWidth <= maxSize.Width) return (scaledWidth.Value, maxSize.Height.Value);
                else return (maxSize.Width.Value, scaledHeight.Value);
            }
        }
    }
}
