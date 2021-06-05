using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;

namespace Web.Services
{
    public class StorageServiceWithCaching : IStorageService
    {

        private readonly IStorageService _wrappeeStorage;
        private readonly ICacheFiles _cache;
        
        public StorageServiceWithCaching(
            BlobServiceClient blobServiceClient, 
            ILogger<StorageService> logger,
            ICacheFiles cache)
        {
            _wrappeeStorage = new StorageService(blobServiceClient, logger);
            _cache = cache;
        }
        
        public Task<string> Add(byte[] file, string containerName, string filePath, string fileContentType = null)
        {
            return _wrappeeStorage.Add(file, containerName, filePath, fileContentType);
        }

        public Task<bool> Delete(string container, string filePath)
        {
            if (_cache.CacheExist(filePath))
            {
                return Task.FromResult(_cache.RemoveCacheFile(filePath));
            }
            return _wrappeeStorage.Delete(container, filePath);
        }

        public Task<List<string>> List(string container)
        {
            return _wrappeeStorage.List(container);
        }

        public Task<(Stream file, string contentType)?> Get(string container, string filePath)
        {
            if (_cache.CacheExist(filePath))
            {
                var bytes = _cache.GetFileFromCache(filePath);
                Stream memoryStream = new MemoryStream(bytes);
                (Stream file, string contentType)? result = (memoryStream, "");
                return Task.FromResult(result);
            }
            
            return _wrappeeStorage.Get(container, filePath);
        }
    }
}