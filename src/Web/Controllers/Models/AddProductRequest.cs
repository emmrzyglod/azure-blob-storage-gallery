using Microsoft.AspNetCore.Http;

namespace Web.Controllers.Models
{
    public class AddProductRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public IFormFile PhotoFile { get; set; }
    }
}