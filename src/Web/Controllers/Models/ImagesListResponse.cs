using System.Collections.Generic;

namespace Web.Controllers.Models
{
    public class ImagesListResponse
    {
        public string BaseUrl { get; set; }
        public List<string> ImagesPaths { get; set; }
    }
}