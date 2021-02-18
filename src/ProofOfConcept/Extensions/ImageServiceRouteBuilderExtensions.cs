using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using ProofOfConcept.Handlers;
using System.Text;

namespace ProofOfConcept.Extensions
{
    public static class ImageServiceRouteBuilderExtensions
    {
        public static IEndpointConventionBuilder MapImageServiceEndpoint(this IEndpointRouteBuilder endpoints, string path)
        {
            var pipeline = endpoints.CreateApplicationBuilder()
                .UseMiddleware<ImageServiceMiddleware>()
                .Build();

            var pattern = GetPattern(path);

            return endpoints.Map(pattern, pipeline).WithDisplayName("Image Service");
        }

        private static string GetPattern(string path)
        {
            StringBuilder sb = new StringBuilder();
            var trimmedPath = path.Trim('/');
            sb.Append('/');
            sb.Append(trimmedPath);
            if (trimmedPath != string.Empty) sb.Append('/');
            sb.Append("{" + ImageServiceMiddleware.RV_NAME + "}");
            return sb.ToString();
        }
    }
}
