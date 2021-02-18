using ImageServiceCore.BlobStorage;
using ImageServiceCore.BlobStorage.FileSystemStorage;
using ImageServiceCore.ImageServiceCore;
using ImageServiceCore.ImageServiceRequestConverter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace ImageServiceCore.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register core Image Service
        /// </summary>
        public static IServiceCollection AddImageService(this IServiceCollection services)
        {
            return services
                .AddTransient<IImageService, ImageService>()
                .AddTransient<ITransformedImageCache, TransformedImageCache>();
        }

        /// <summary>
        /// Register file system storage for images and cache
        /// </summary>
        public static IServiceCollection AddImageServiceFileSystemStorage(this IServiceCollection services)
        {
            FileStorage.Options GetConfig(IServiceProvider serviceProvider, string key) => 
                serviceProvider.GetRequiredService<IConfiguration>().GetSection(key).Get<FileStorage.Options>();

            ILogger<T> GetLogger<T>(IServiceProvider serviceProvider) => 
                serviceProvider.GetRequiredService<ILogger<T>>();

            return services
                .AddTransient<IOriginalImageBlobStorage>(o => 
                    new OriginalImageFileStorage(GetConfig(o, "ImageFileStorage"), GetLogger<OriginalImageFileStorage>(o)))
                .AddTransient<ICachedTransformBlobStorage>(o => 
                    new CachedTransformFileStorage(GetConfig(o, "CacheFileStorage"), GetLogger<CachedTransformFileStorage>(o)));
        }

        /// <summary>
        /// Register image transformation strategy that uses System.Drawing.Bitmap and Graphics to perform image processing
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddBitmapImageTransformer(this IServiceCollection services)
        {
            return services
                .AddTransient<IImageTransformer, BitmapImageTransformer>();
        }

        /// <summary>
        /// Register the default transformation naming convention converter
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddImageTransformationNamingConvention(this IServiceCollection services)
        {
            return services
                .AddTransient<IEncodedStringImageTransformationRequestConverter, EncodedStringImageTransformationRequestConverterV1>();
        }
    }
}
