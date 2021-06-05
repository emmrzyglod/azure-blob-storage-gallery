using System;
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
            var cacheFileName = $"{container}_{filePath}";
            
            if (_cache.CacheExist(cacheFileName))
            {
                return Task.FromResult(_cache.RemoveCacheFile(cacheFileName));
            }
            return _wrappeeStorage.Delete(container, filePath);
        }

        public Task<List<string>> List(string container)
        {
            return _wrappeeStorage.List(container);
        }

        public async Task<(Stream file, string contentType)?> Get(string container, string filePath)
        {
            var cacheFileName = $"{container}_{filePath}";
            
            if (_cache.CacheExist(cacheFileName))
            {
                Console.WriteLine("--------------------------");
                Console.WriteLine($"Get file from cache {filePath}");
                var bytes = _cache.GetFileFromCache(cacheFileName);
                Stream memoryStream = new MemoryStream(bytes);
                (Stream file, string contentType)? result = (memoryStream, "image/jpeg");
                return result;
            }
            
            var resultFromStorage = await _wrappeeStorage.Get(container, filePath);
            if (resultFromStorage != null && resultFromStorage.Value.file != null)
            {
                await _cache.EnsureCacheFileAsync(cacheFileName, ReadFully(resultFromStorage.Value.file));
            }

            return resultFromStorage;
        }
        
        private static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16*1024];
            using MemoryStream ms = new MemoryStream();
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            return ms.ToArray();
        }
    }
}