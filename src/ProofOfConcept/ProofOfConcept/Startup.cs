using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {   
                endpoints.MapGet("/{name}", async context =>
                {
                    var name = context.Request.RouteValues["name"];
                    var maxWidth = context.Request.Query["w"].FirstOrDefault();
                    var maxHeight = context.Request.Query["h"].FirstOrDefault();

                    context.Response.Headers.Add("Content-Type", new StringValues("image/png"));
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
