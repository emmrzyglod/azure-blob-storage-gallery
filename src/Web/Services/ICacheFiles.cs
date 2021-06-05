using System;
using System.Threading.Tasks;

namespace Web.Services
{
    public interface ICacheFiles
    {
        Task<bool> EnsureCacheFileAsync(string uniqueFileName, byte[] content);
        
        bool CacheExists(string uniqueFileName, int? expiryTime = null);
        bool CacheExist(string uniqueFileName);
        
        byte[] GetFileFromCache(string uniqueFileName);
        T GetFileFromCache<T>(string uniqueFileName);
        
        Task<bool> OverrideCacheFileAsync(string uniqueFileName, object content, DateTime? validTo);

        bool RemoveCacheFile(string uniqueFileName);
    }
}