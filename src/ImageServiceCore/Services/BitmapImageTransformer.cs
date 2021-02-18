using ImageServiceCore.ImageServiceRequestConverter;
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
            this.font = new Font(FontFamily.GenericSansSerif, 64F, FontStyle.Regular, GraphicsUnit.Pixel);
            this.brush = new SolidBrush(Color.FromArgb(64, Color.Black));
            this.logger = logger;
        }

        /// <summary>
        /// Transform image bytes from one format or resolution to another, optionally with a watermark
        /// </summary>
        /// <param name="bytes">Byte array containing the image file data</param>
        /// <param name="request">Image transformation parameters</param>
        /// <returns>Array of bytes representing the transformed image file</returns>
        public byte[] Transform(byte[] bytes, ImageTransformationRequest request)
        {
            var stopwatch = Stopwatch.StartNew();

            byte[] returnBytes;

            var image = LoadImage(bytes);
            
            var currentImageFormat = image.RawFormat;
            var newImageFormat = ParseImageFormatOrDefault(request.Format) ?? currentImageFormat;
            var currentDims = (image.Width, image.Height);
            var maxDims = (request.MaxWidth, request.MaxHeight);
            var newDims = CalculateNewDimensions(currentDims, maxDims);

            if (string.IsNullOrWhiteSpace(request.Watermark) && string.IsNullOrWhiteSpace(request.Colour) && newDims == currentDims && newImageFormat == currentImageFormat)
            {
                logger.LogTrace("No transformation required. Returning original image");
                // No transformation required
                returnBytes = bytes;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(request.Colour))
                {
                    logger.LogTrace("Colouring background");
                    image = ColourBackground(request.Colour, image);
                }

                if (!string.IsNullOrWhiteSpace(request.Watermark))
                {
                    logger.LogTrace("Drawing watermark");
                    DrawWatermark(request.Watermark, image);
                }

                if (newDims != currentDims)
                {
                    logger.LogTrace("Resizing image");
                    image = ResizeImage(image, newDims);
                }

                returnBytes = SaveImageToByteArray(image, newImageFormat);
                
            }
            image.Dispose();

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

        // Colour background
        private Image ColourBackground(string colour, Image image)
        {
            var newImage = new Bitmap(image.Width, image.Height, image.PixelFormat);
            using (var g = CreateGraphics(newImage))
            {
                g.Clear(ColorTranslator.FromHtml(colour));

                g.DrawImage(image, 0, 0, image.Width, image.Height);
            }
            image.Dispose();
            return newImage;
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
            image.Dispose();
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
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
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
