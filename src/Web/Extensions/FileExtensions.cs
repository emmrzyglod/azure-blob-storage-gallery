using HeyRed.Mime;
using Microsoft.AspNetCore.StaticFiles;

namespace Web.Extensions
{
    public static class FileExtensions
    {
        private static readonly FileExtensionContentTypeProvider Provider = new FileExtensionContentTypeProvider();
        
        public static string GetContentType(this string fileName)
        {
            if(!Provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
        
        public static string GetExtension(this string contentType)
        {
            return MimeTypesMap.GetExtension(contentType);
        }
    }
}