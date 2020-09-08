using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Web.Services
{
    public interface IStorageService
    {
        Task<string> Add(byte[] file, string containerName, string filePath, string fileContentType = null);
        Task<bool> Delete(string container, string filePath);
        Task<List<string>> List(string container);
        Task<(Stream file, string contentType)?> Get(string container, string filePath);
    }
}