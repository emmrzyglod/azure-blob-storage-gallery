using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Web.Extensions;

namespace Web.Services
{
    public class StorageService : IStorageService
    {

        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<StorageService> _logger;

        public StorageService(BlobServiceClient blobServiceClient, ILogger<StorageService> logger)
        {
            _blobServiceClient = blobServiceClient;
            _logger = logger;
        }

        public async Task<string> Add(byte[] file, string container, string filePath, string contentType = null)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(container);
            
            BlobClient blobClient = containerClient.GetBlobClient(filePath);
            
            if (await blobClient.ExistsAsync())
            {
                var properties = await blobClient.GetPropertiesAsync();
                if (properties.GetHashCode() == filePath.GetHashCode())
                {
                    return blobClient.Name;
                }
            }
                
            _logger.LogInformation($"Uploading to Blob Storage:\n\t {blobClient.Uri}\n");

            contentType ??= filePath.GetContentType();
            
            var memoryStream  = new MemoryStream(file);
            await blobClient.UploadAsync(memoryStream, new BlobHttpHeaders {ContentType = contentType});

            _logger.LogInformation(blobClient.Name);
            
            return filePath;
        }
        
        public async Task<bool> Delete(string container, string filePath)
        {
            _logger.LogInformation($"Remove file from Blob Storage: {filePath}");

            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(container);
            var blobClient = containerClient.GetBlobClient(filePath);
            var response = await blobClient.DeleteIfExistsAsync();
            _logger.LogInformation("Delete result: " + response.Value);
            return response.Value;
        }

        public async Task<List<string>> List(string container)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(container);
            List<string> result = new List<string>();
            
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                result.Add(blobItem.Name);
            }

            return result;
        }
        
        public async Task<(Stream file, string contentType)?> Get(string container, string filePath)
        {
            _logger.LogInformation($"Get file from Blob Storage:\n\t {filePath}\n");

            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(container);
            
            BlobClient blobClient = containerClient.GetBlobClient(filePath);
            
            if (! await blobClient.ExistsAsync())
            {
                _logger.LogInformation($"File {filePath} not exist in storage");
                return null;
            }

            var downloadedFile = await blobClient.DownloadAsync();
            
            return (downloadedFile.Value.Content, downloadedFile.Value.ContentType);
        }
    }
}