using ImageServiceCore.Extensions;
using ImageServiceCore.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProofOfConcept
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddImageService();
            services.AddImageServiceLocalFileStorage();
            services.AddBitmapImageTransformer();

            services.AddRazorPages();
            services.AddResponseCaching(options =>
            {
                options.MaximumBodySize = 10_000_000; // Max cached image size: 10MB
                options.SizeLimit = 10_000_000_000; // Max total cache size: 10GB
                options.UseCaseSensitivePaths = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseRouting();
            app.UseResponseCaching();

            app.Use(async (context, next) => {
                context.Response.GetTypedHeaders().CacheControl =
                    new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromHours(24)
                    };
                await next();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapGet("/images/{name}", ImageRequestHandler);
            });
        }

        private static async Task ImageRequestHandler(HttpContext context)
        {
            var name = context.Request.RouteValues["name"] as string;
            var maxWidth = context.Request.Query["w"].Select(s => (int?)int.Parse(s)).FirstOrDefault();
            var maxHeight = context.Request.Query["h"].Select(s => (int?)int.Parse(s)).FirstOrDefault();
            var colour = context.Request.Query["c"].FirstOrDefault();
            var watermark = context.Request.Query["t"].FirstOrDefault();
            var format = context.Request.Query["f"].FirstOrDefault();

            var imageService = context.RequestServices.GetRequiredService<IImageService>();
            var imageBytes = imageService.Get(name, format, (maxWidth, maxHeight), colour, watermark);

            if (imageBytes != null)
            {
                var contentType = $"image/{format ?? Path.GetExtension(name)}";
                context.Response.StatusCode = 200;
                context.Response.Headers.Add("Content-Type", new StringValues(contentType));
                await context.Response.BodyWriter.WriteAsync(imageBytes);
            }
            else
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("Image does not exist");
            }
        }
    }
}
