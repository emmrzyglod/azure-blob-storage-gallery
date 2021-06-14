using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Web.Config;
using Web.Controllers.Models;
using Web.Extensions;
using Web.Persistence;
using Web.Persistence.Entities;
using Web.Services;

namespace Web.Controllers
{
    
    [ApiController]
    [Route("[controller]/[action]")]
    public class GalleryController : ControllerBase
    {

        private readonly IStorageService _storageService;
        private readonly IOptions<StorageConfig> _storageConfig;
        private readonly ApplicationDbContext _context;

        public GalleryController(
            IStorageService storageService, 
            IOptions<StorageConfig> storageConfig, 
            ApplicationDbContext context
        ) 
        {
            _storageService = storageService;
            _storageConfig = storageConfig;
            _context = context;
        }

        [HttpPost]
        public async Task<string> AddPhoto([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0) throw new ArgumentNullException();

            var imageName = $"{Guid.NewGuid().ToString()}.{file.ContentType.GetExtension()}";

            var fileAsBytes = await file.GetBytesAsync();

            var storageContainerName = _storageConfig.Value.PublicContainerName;

            var fileName = await _storageService.Add(fileAsBytes, storageContainerName, imageName, file.ContentType);
            
            var photoEntity = new Photo(fileName);
            await _context.AddAsync(photoEntity);
            await _context.SaveChangesAsync();

            var fullPath = $"{_storageConfig.Value.Url}/{storageContainerName}/{fileName}";

            return fullPath;
        }
        
        [HttpGet]
        public async Task<ImagesListResponse> PhotosList()
        {
            var photos = await _context
                .Photo
                .Select(m => m.Path)
                .ToListAsync();
            
            var response = new ImagesListResponse
            {
                BaseUrl = $"{_storageConfig.Value.Url}/{_storageConfig.Value.PublicContainerName}",
                ImagesPaths = photos
            };

            return response;
        }

        [HttpGet]
        public async Task<List<string>> ListFromStorage() 
            => await _storageService.List(_storageConfig.Value.PublicContainerName);

        public async Task<FileStreamResult> Get(string photoName)
        {
            var fileStream = await _storageService.Get(_storageConfig.Value.PublicContainerName, photoName);

            if (fileStream != null)
            {
                var fileStreamResult = new FileStreamResult(fileStream.Value.file, fileStream.Value.contentType);
                return fileStreamResult;
            }
            
            return null;
        }

        [HttpDelete]
        public async Task DeletePhoto(string photoName)
        {
            var photoEntity = await _context.Photo.SingleOrDefaultAsync(m => m.Path == photoName);
            
            if (photoEntity == null) throw new Exception("Photo not found");

            _context.Remove(photoEntity);
            await _context.SaveChangesAsync();
            
            await _storageService.Delete(_storageConfig.Value.PublicContainerName, photoName);
        }
    }
}