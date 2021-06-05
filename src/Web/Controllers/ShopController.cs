using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public class ShopController : ControllerBase
    {

        private readonly IStorageService _storageService;
        private readonly IOptions<StorageConfig> _storageConfig;
        private readonly ApplicationDbContext _context;

        public ShopController(
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
        public async Task AddProduct([FromForm] AddProductRequest request)
        {
            var file = request.PhotoFile;
            Photo photoEntity = null;
            if (file != null && file.Length > 0)
            {
                var imageName = $"{Guid.NewGuid().ToString()}.{file.ContentType.GetExtension()}";
                var fileAsBytes = await file.GetBytesAsync();
                var fileName = await _storageService.Add(fileAsBytes, _storageConfig.Value.PublicContainerName, imageName, file.ContentType);
                photoEntity = new Photo(fileName);
                await _context.Photo.AddAsync(photoEntity);
            }

            var product = new Product(request.Name, request.Description, request.Price, photoEntity);
            await _context.Products.AddAsync(product);

            await _context.SaveChangesAsync();
        }
        
        [HttpGet]
        public async Task<BrowseProductsResponse> BrowseProducts()
        {
            var products = await _context
                .Products
                .Include(m => m.Photo)
                .ToListAsync();

            var responseProducts = new List<ProductDto>();
            
            foreach (var product in products)
            {
                var dto = new ProductDto
                {
                    Description = product.Description,
                    Name = product.Name,
                    Price = product.Price
                };

                if (product.Photo != null)
                {
                    dto.PhotoUrl = $"{_storageConfig.Value.Url}/{_storageConfig.Value.PublicContainerName}";
                }
            }

            return new BrowseProductsResponse
            {
                Products = responseProducts
            };
        }
    }
}