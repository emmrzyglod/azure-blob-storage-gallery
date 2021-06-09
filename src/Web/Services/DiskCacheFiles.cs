using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Web.Services
{

    [Serializable]
    internal class CacheFile
    {
        public DateTime ValidTo { get; set; }
        public object Data { get; set; }
    }
    
    public class DiskCacheFiles : ICacheFiles
    {
        private static readonly string storage = "data/cache/files";
        private readonly ILogger<DiskCacheFiles> _logger;

        public DiskCacheFiles(ILogger<DiskCacheFiles> logger)
        {
            this._logger = logger;
        }

        public bool CacheExists(string uniqueFileName, int? expiryTime = null)
        {
            var filePath = Path.Combine(storage, uniqueFileName);
            var fileExists = File.Exists(filePath);

            if (fileExists)
            {
                if (expiryTime != null)
                {
                    var createDate = File.GetCreationTime(filePath);
                    DateTime now = DateTime.UtcNow;
                    TimeSpan ts = now - createDate;
                    
                    if (ts.TotalMilliseconds >= expiryTime)
                    {
                        return false;
                    } 
                }

                return true;
            }

            return false;
        }

        public bool CacheExist(string uniqueFileName)
        {
            var filePath = Path.Combine(storage, uniqueFileName);
            var fileExists = File.Exists(filePath);

            Console.WriteLine($"File {filePath} is {fileExists}");
            
            return fileExists;
        }

        public async Task<bool> EnsureCacheFileAsync(string uniqueFileName, byte[] content)
        {
            if (!Directory.Exists(storage))
            {
                _logger.LogInformation($"Directory created: {storage}");
                Directory.CreateDirectory(storage);
            }
            var filePath = Path.Combine(storage, uniqueFileName);
            var fileExists = File.Exists(filePath);
            _logger.LogInformation($"File {uniqueFileName} exists? {fileExists}");
            
            if (!fileExists)
            {
                await File.WriteAllBytesAsync(filePath, content);
                _logger.LogInformation($"File {uniqueFileName} in {filePath} cache created");
            }
            
            return fileExists;
        }
        
        public async Task<bool> OverrideCacheFileAsync(string uniqueFileName, byte[] content)
        {
            var filePath = Path.Combine(storage, uniqueFileName);
            var fileExists = File.Exists(filePath);

            if (!fileExists)
            {
                return await EnsureCacheFileAsync(uniqueFileName, content);
            }
            
            await File.WriteAllBytesAsync(filePath, content);
            _logger.LogInformation($"Override existing file {uniqueFileName} in {filePath}");
            
            return true;
        }
        
        
        /* NEW TEST METHOD */
        public async Task<bool> OverrideCacheFileAsync(string uniqueFileName, object content, DateTime? validTo = null)
        {
            var filePath = Path.Combine(storage, uniqueFileName);
            var fileExists = File.Exists(filePath);
            
            // var validToTimestamp = new DateTimeOffset(validTo ?? DateTime.UtcNow.AddSeconds(10)).ToUnixTimeSeconds();

            var cacheFile = new CacheFile
            {
                Data = content,
                ValidTo = validTo ?? DateTime.UtcNow.AddSeconds(10)
            };
            
            BinaryFormatter bf = new BinaryFormatter();
            await using MemoryStream ms = new MemoryStream();
#pragma warning disable 618
            bf.Serialize(ms, cacheFile);
#pragma warning restore 618
            var contentBytes = ms.ToArray();

            if (!fileExists)
            {
                return await EnsureCacheFileAsync(uniqueFileName, contentBytes);
            }
            
            await File.WriteAllBytesAsync(filePath, contentBytes);
            _logger.LogInformation($"Override existing file {uniqueFileName} in {filePath}");
            
            return true;
        }

        public byte[] GetFileFromCache(string uniqueFileName)
        {
            var filePath = Path.Combine(storage, uniqueFileName);
            var fileExists = File.Exists(filePath);
            if (!fileExists)
            {
                throw new FileNotFoundException($"File {uniqueFileName} does not exists in path {filePath}!");
            }
            
            var content = File.ReadAllBytes(filePath);
            
            return content;
        }
        
        public T GetFileFromCache<T>(string uniqueFileName)
        {
            var filePath = Path.Combine(storage, uniqueFileName);
            var fileExists = File.Exists(filePath);
            if (!fileExists)
            {
                throw new FileNotFoundException($"File {uniqueFileName} does not exists in path {filePath}!");
            }
            
            var content = File.ReadAllBytes(filePath);
    
            CacheFile obj = (CacheFile) ByteArrayToObject(content);
            return (T)obj.Data;
        }

        public bool RemoveCacheFile(string uniqueFileName)
        {
            var filePath = Path.Combine(storage, uniqueFileName);
            var fileExists = File.Exists(filePath);
            if (!fileExists)
            {
                throw new FileNotFoundException($"File {uniqueFileName} does not exists in path {filePath}!");
            }
            
            File.Delete(filePath);
            
            return true;
        }
        
        private CacheFile GetCacheFileFromCache(string uniqueFileName)
        {
            var filePath = Path.Combine(storage, uniqueFileName);
            var fileExists = File.Exists(filePath);
            if (!fileExists)
            {
                throw new FileNotFoundException($"File {uniqueFileName} does not exists in path {filePath}!");
            }
            
            var content = File.ReadAllBytes(filePath);
    
            CacheFile obj = (CacheFile) ByteArrayToObject(content);
            return obj;
        }
        
        private Object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
#pragma warning disable 618
            Object obj = (Object) binForm.Deserialize(memStream);
#pragma warning restore 618
            return obj;
        }
    }
}
