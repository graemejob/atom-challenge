using ImageServiceCore.ImageServiceRequestConverter;
using ImageServiceCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.IO;
using System.Threading.Tasks;

namespace ProofOfConcept.Handlers
{
    public class ImageServiceMiddleware
    {
        internal const string RV_NAME = "name";
        private readonly RequestDelegate next;
        private readonly IImageService imageService;
        private readonly IEncodedStringImageTransformationRequestConverter imageRequestConverter;

        public ImageServiceMiddleware(RequestDelegate next, IImageService imageService, IEncodedStringImageTransformationRequestConverter imageRequestConverter)
        {
            this.next = next;
            this.imageService = imageService;
            this.imageRequestConverter = imageRequestConverter;
        }

        public async Task Invoke(HttpContext context)
        {
            // Retrieve the RouteData, and access the route values
            var routeValues = context.GetRouteData().Values;

            // Extract the name
            var imageName = routeValues[RV_NAME] as string;
            var imageRequest = imageRequestConverter.ConvertFrom(imageName);
            var imageBytes = imageService.Get(imageRequest);

            if (imageBytes != null)
            {
                var contentType = $"image/{imageRequest.Format ?? Path.GetExtension(imageRequest.Name)}";
                context.Response.StatusCode = 200;
                context.Response.ContentType = contentType;
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
