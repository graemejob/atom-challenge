using ImageServiceCore.Interfaces;
using ImageServiceCore.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace ImageServiceCore.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register Image Service
        /// </summary>
        public static IServiceCollection AddImageService(this IServiceCollection services)
        {
            return services
                .AddTransient<IImageService, ImageService>()
                .AddTransient<ITransformedImageCache, TransformedImageCache>();
        }

        /// <summary>
        /// Register local file storage for images and cache
        /// </summary>
        public static IServiceCollection AddImageServiceLocalFileStorage(this IServiceCollection services)
        {
            FileStorage.Options GetConfig(IServiceProvider serviceProvider, string key) => 
                serviceProvider.GetRequiredService<IConfiguration>().GetSection(key).Get<FileStorage.Options>();

            ILogger<T> GetLogger<T>(IServiceProvider serviceProvider) => 
                serviceProvider.GetRequiredService<ILogger<T>>();

            return services
                .AddTransient<IImageBlobStorage>(o => 
                    new ImageFileStorage(GetConfig(o, "ImageFileStorage"), GetLogger<ImageFileStorage>(o)))
                .AddTransient<ICacheBlobStorage>(o => 
                    new CacheFileStorage(GetConfig(o, "CacheFileStorage"), GetLogger<CacheFileStorage>(o)));
        }

        /// <summary>
        /// Register BitmapImageTransformer as the transform strategy
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddBitmapImageTransformer(this IServiceCollection services)
        {
            return services
                .AddTransient<IImageTransformer, BitmapImageTransformer>();
        }
    }
}
