using ImageServiceCore.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProofOfConcept.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IImageBlobStorage imageBlobStorage;

        public IndexModel(IImageBlobStorage imageBlobStorage)
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
