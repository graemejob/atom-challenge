using ImageServiceCore.BlobStorage;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProofOfConcept.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IOriginalImageBlobStorage imageBlobStorage;

        public IndexModel(IOriginalImageBlobStorage imageBlobStorage)
        {
            this.imageBlobStorage = imageBlobStorage;
        }

        public string[] Items { get; set; }
        public void OnGet()
        {
            Items = imageBlobStorage.List();
        }
    }
}
