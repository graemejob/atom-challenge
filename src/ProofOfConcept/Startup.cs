using ImageServiceCore.Extensions;
using ImageServiceCore.ImageServiceRequestConverter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProofOfConcept.Extensions;
using System;

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
            
            services.AddTransient<IEncodedStringImageTransformationRequestConverter, EncodedStringImageTransformationRequestConverterV1>();
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
                endpoints.MapImageServiceEndpoint("/images/");
            });
        }

    }
}
