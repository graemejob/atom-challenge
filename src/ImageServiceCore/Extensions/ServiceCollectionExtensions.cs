using ImageServiceCore.Interfaces;
using ImageServiceCore.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ImageServiceCore.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFileStorage(this IServiceCollection services)
        {
            return services
                .AddTransient<IImageBlobStorage>(o => 
                new ImageFileStorage(GetOptions<FileStorage.Options>(o, "ImageFileStorage")))
                .AddTransient<ICacheBlobStorage>(o => 
                new CacheFileStorage(GetOptions<FileStorage.Options>(o, "CacheFileStorage")));
        }

        private static T GetOptions<T>(IServiceProvider serviceProvider, string key)
        {
            return serviceProvider.GetRequiredService<IConfiguration>().GetSection(key).Get<T>();
        }
    }
}
